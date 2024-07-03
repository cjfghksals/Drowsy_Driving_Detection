#ifndef EYEDETECTOR_H
#define EYEDETECTOR_H

#include <boost/asio.hpp>
#include <opencv2/opencv.hpp>
#include "Inference.h"
#include "Database.h" // DataBase Ŭ���� ��� ����

class EyeDetector {
public:
    EyeDetector(Inference& inference);
    void startProcessing(boost::asio::ip::tcp::socket& socket);
    void clearConsole();

private:
    void send_key(boost::asio::ip::tcp::socket& socket);
    void processFrames(boost::asio::ip::tcp::socket& socket);

    Inference& inf;
    std::atomic<bool> send_signal;
    std::mutex send_mutex;
    DataBase db; // �����ͺ��̽� ��ü �߰�
};

#endif // EYEDETECTOR_H
