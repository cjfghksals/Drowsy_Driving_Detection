#ifndef CAMERA_H
#define CAMERA_H

#include <iostream>
#include <opencv2/opencv.hpp>
#include <boost/asio.hpp>
#include <thread>
#include <pigpio.h>

// 빌드 명령어: g++ -o bzt main.cpp `pkg-config --cflags --libs opencv4` -lboost_system -lpthread -lpigpio

#define VIDEO_SERVER_IP "192.168.0.110"
#define VIDEO_SERVER_PORT 12345
#define ALERT_SERVER_IP "192.168.0.81"
#define ALERT_SERVER_PORT 12345
#define BUZZER_PIN 24 // GPIO18
#define G4_FREQUENCY 392 // Frequency for the "sol" note in Hz

void connect_to_server(boost::asio::io_service& io_service, boost::asio::ip::tcp::socket& socket, const std::string& server_ip, int server_port) {
    try {
        boost::asio::ip::tcp::resolver resolver(io_service);
        boost::asio::ip::tcp::resolver::query query(server_ip, std::to_string(server_port));
        boost::asio::ip::tcp::resolver::iterator endpoint_iterator = resolver.resolve(query);
        boost::asio::connect(socket, endpoint_iterator);
    } catch (std::exception& e) {
        std::cerr << "Connect to server exception: " << e.what() << std::endl;
    }
}

void receive_key(boost::asio::ip::tcp::socket& video_socket, boost::asio::ip::tcp::socket& alert_socket) {
    try {
        while (true) {
            char key;
            boost::asio::read(video_socket, boost::asio::buffer(&key, 1));
            std::cout << "Received from video server: " << key << std::endl;
            if (key == 'A') {
                std::cout << "졸음 감지 비이~~~~~상 " << std::endl;
                gpioSetPWMfrequency(BUZZER_PIN, G4_FREQUENCY); // Set frequency to G4 note
                gpioPWM(BUZZER_PIN, 128); // Set duty cycle to 50% (128/255)
                time_sleep(3); // Keep the buzzer on for 3 seconds
                gpioPWM(BUZZER_PIN, 0); // Turn the buzzer off

                boost::asio::write(alert_socket, boost::asio::buffer("q", 1));
            }
        }
    } catch (std::exception& e) {
        std::cerr << "Receive key exception: " << e.what() << std::endl;
    }
}

void capture_and_send_video(boost::asio::ip::tcp::socket& video_socket) {
    cv::VideoCapture cap(0);
    if (!cap.isOpened()) {
        std::cerr << "Error: Could not open camera" << std::endl;
        return;
    }

    while (true) {
        cv::Mat frame;
        cap >> frame;
        if (frame.empty()) {
            std::cerr << "Error: Could not capture frame" << std::endl;
            break;
        }

        cv::Mat resized_frame;
        cv::resize(frame, resized_frame, cv::Size(640, 480));

        std::vector<uchar> img_buffer;
        cv::imencode(".jpg", resized_frame, img_buffer);
        int img_size = img_buffer.size();

        boost::asio::write(video_socket, boost::asio::buffer(&img_size, sizeof(int)));
        boost::asio::write(video_socket, boost::asio::buffer(img_buffer.data(), img_buffer.size()));

        cv::imshow("Live Video", resized_frame);
        if (cv::waitKey(30) >= 0) {
            break;
        }
    }
}

int camera() {
    if (gpioInitialise() < 0) {
        std::cerr << "pigpio initialization failed" << std::endl;
        return -1;
    }

    gpioSetMode(BUZZER_PIN, PI_OUTPUT);
    gpioWrite(BUZZER_PIN, 0); // Ensure the buzzer is off initially

    try {
        boost::asio::io_service io_service;

        // Create sockets
        boost::asio::ip::tcp::socket video_socket(io_service);
        boost::asio::ip::tcp::socket alert_socket(io_service);

        // Connect to servers
        connect_to_server(io_service, video_socket, VIDEO_SERVER_IP, VIDEO_SERVER_PORT);
        connect_to_server(io_service, alert_socket, ALERT_SERVER_IP, ALERT_SERVER_PORT);

        // Start the thread to receive key inputs from the video server and send alerts to the alert server
        std::thread receive_thread(receive_key, std::ref(video_socket), std::ref(alert_socket));

        // Capture and send video frames
        capture_and_send_video(video_socket);

        // Close the receive thread
        if (receive_thread.joinable()) {
            receive_thread.join();
        }

    } catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    gpioTerminate(); // Terminate pigpio
    return 0;
}

#endif // CAMERA_H