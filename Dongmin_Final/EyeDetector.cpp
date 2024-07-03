#include "EyeDetector.h"        //EyeDetector 클래스

using boost::asio::ip::tcp;
using namespace std;
using namespace cv;

EyeDetector::EyeDetector(Inference& inference)
    : inf(inference), send_signal(false), db("test.db") {} // 데이터베이스 초기화하기

void EyeDetector::clearConsole() {
    cout << "\033[2J\033[1;1H"; // ANSI 이스케이프 시퀀스를 사용하여 콘솔 지우기
}

void EyeDetector::send_key(tcp::socket& socket) {
    try {
        while (true) {
            if (send_signal.load()) {
                std::lock_guard<std::mutex> lock(send_mutex);
                std::string input = "A";
                boost::asio::write(socket, boost::asio::buffer(input));
                send_signal.store(false);
            }
            std::this_thread::sleep_for(std::chrono::milliseconds(100));
        }
    }
    catch (std::exception& e) {
        std::cerr << "Send key exception: " << e.what() << std::endl;
    }
}

void EyeDetector::processFrames(tcp::socket& socket) {
    int open = 0;
    int frameCounter = 0;
    int Cnt = 0;
    bool recording = false;
    int video_Cnt = 1;
    VideoWriter videoWriter;

    while (true) {
        bool detected = false;
        uint32_t img_size;
        boost::asio::read(socket, boost::asio::buffer(&img_size, sizeof(img_size)));
        std::vector<unsigned char> buffer(img_size);
        boost::asio::read(socket, boost::asio::buffer(buffer.data(), buffer.size()));
        cv::Mat frame = imdecode(buffer, IMREAD_COLOR);
        if (frame.empty()) {
            cerr << "Failed to decode image" << endl;
            continue;
        }

        if (frameCounter % 10 == 0) {
            std::vector<Detection> output = inf.runInference(frame);
            for (const auto& detection : output) {
                Rect box = detection.box;
                Scalar color = detection.color;
                rectangle(frame, box, color, 2);
                std::string classString = detection.className + ' ' + to_string(detection.confidence).substr(0, 4);
                Size textSize = getTextSize(classString, FONT_HERSHEY_DUPLEX, 1, 2, 0);
                Rect textBox(box.x, box.y - 40, textSize.width + 10, textSize.height + 20);
                rectangle(frame, textBox, color, cv::FILLED);
                putText(frame, classString, Point(box.x + 5, box.y - 10), FONT_HERSHEY_DUPLEX, 1, Scalar(0, 0, 0), 2, 0);

                if (detection.className == "ClosedEyes" && detection.confidence > 0.5) {
                    Cnt++;
                    if (Cnt >= 4) {
                        Beep(1000, 500);
                        if (Cnt == 4) {
                            int lastId = db.getLastId();
                            int newId = lastId + 1;
                            db.insertData(newId, "78가1234");
                            detected = true;
                            if (detected) {
                                send_signal.store(true);
                                detected = false;
                            }
                            std::string filename = "C:/Users/blackmeta/Desktop/신동민/evidence_video/78라1234(" + to_string(video_Cnt) + ").avi";
                            Size frameSize(frame.cols, frame.rows);
                            int fourcc = VideoWriter::fourcc('M', 'J', 'P', 'G');
                            videoWriter.open(filename, fourcc, 10, frameSize);
                            if (videoWriter.isOpened()) {
                                clearConsole();
                                cout << "졸음이 감지되었습니다.!" << endl;
                                cout << "영상 녹화를 시작합니다." << endl;
                                recording = true;
                            }
                            else {
                                cerr << "영상 녹화를 시작할 수 없습니다." << endl;
                            }
                        }
                    }
                }
                else if (detection.className == "OpenEyes") {
                    Cnt = 0;
                    if (recording) {
                        clearConsole();
                        cout << "영상을 성공적으로 저장하였습니다." << endl;
                        video_Cnt++;
                        videoWriter.release();
                        recording = false;
                        db.printAllData();
                    }
                }
            }
        }

        if (recording) {
            videoWriter.write(frame);
        }

        imshow("Eye_Detector", frame);
        if (waitKey(1) == 27) {
            break;
        }

        frameCounter++;
    }
}

void EyeDetector::startProcessing(tcp::socket& socket) {
    try {
        std::thread processingThread(&EyeDetector::processFrames, this, std::ref(socket));
        std::thread sendingThread(&EyeDetector::send_key, this, std::ref(socket));
        processingThread.join();
        sendingThread.join();
    }
    catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
    }
}
