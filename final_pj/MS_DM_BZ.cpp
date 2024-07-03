#include <iostream>
#include <opencv2/opencv.hpp>
#include <boost/asio.hpp>
#include <thread>
#include <pigpio.h>

// 빌드 명령어: g++ -o bzt bz_on.cpp `pkg-config --cflags --libs opencv4` -lboost_system -lpthread -lpigpio

#define VIDEO_SERVER_IP "192.168.0.110"
#define VIDEO_SERVER_PORT 12345
#define ALERT_SERVER_IP "192.168.0.81"
#define ALERT_SERVER_PORT 12345
#define BUZZER_PIN 18 // GPIO18
#define G4_FREQUENCY 392 // Frequency for the "sol" note in Hz

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

                // Send 'q' to the alert server
                boost::asio::write(alert_socket, boost::asio::buffer("q", 1));
            }
        }
    } catch (std::exception& e) {
        std::cerr << "Receive key exception: " << e.what() << std::endl;
    }
}

int main() {
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

        // Resolve the video server address and connect
        boost::asio::ip::tcp::resolver video_resolver(io_service);
        boost::asio::ip::tcp::resolver::query video_query(VIDEO_SERVER_IP, std::to_string(VIDEO_SERVER_PORT));
        boost::asio::ip::tcp::resolver::iterator video_endpoint_iterator = video_resolver.resolve(video_query);
        boost::asio::connect(video_socket, video_endpoint_iterator);

        // Resolve the alert server address and connect
        boost::asio::ip::tcp::resolver alert_resolver(io_service);
        boost::asio::ip::tcp::resolver::query alert_query(ALERT_SERVER_IP, std::to_string(ALERT_SERVER_PORT));
        boost::asio::ip::tcp::resolver::iterator alert_endpoint_iterator = alert_resolver.resolve(alert_query);
        boost::asio::connect(alert_socket, alert_endpoint_iterator);

        // Start the thread to receive key inputs from the video server and send alerts to the alert server
        std::thread receive_thread(receive_key, std::ref(video_socket), std::ref(alert_socket));

        // Open the default camera
        cv::VideoCapture cap(0);
        if (!cap.isOpened()) {
            std::cerr << "Error: Could not open camera" << std::endl;
            return -1;
        }

        while (true) {
            cv::Mat frame;
            // Capture a frame from the camera
            cap >> frame;
            if (frame.empty()) {
                std::cerr << "Error: Could not capture frame" << std::endl;
                break;
            }

            // Resize the image to reduce size
            cv::Mat resized_frame;
            cv::resize(frame, resized_frame, cv::Size(640, 480));

            // Encode the image
            std::vector<uchar> img_buffer;
            cv::imencode(".jpg", resized_frame, img_buffer);
            int img_size = img_buffer.size();

            // Send image size
            boost::asio::write(video_socket, boost::asio::buffer(&img_size, sizeof(int)));

            // Send image data
            boost::asio::write(video_socket, boost::asio::buffer(img_buffer.data(), img_buffer.size()));

            // Display the frame (optional)
            cv::imshow("Live Video", resized_frame);
            if (cv::waitKey(30) >= 0) {
                break;
            }
        }

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
