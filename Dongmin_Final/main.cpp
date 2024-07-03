#include <boost/asio.hpp>
#include "Inference.h"
#include "EyeDetector.h"

using boost::asio::ip::tcp;

int main(int argc, char** argv) {
    try {
        boost::asio::io_context io_context;
        tcp::acceptor acceptor(io_context, tcp::endpoint(tcp::v4(), 12345));
        tcp::socket socket(io_context);
        acceptor.accept(socket);
        bool runOnGPU = true;
        Inference inf("100_64.onnx", cv::Size(640, 640), "classes3.txt", false);
        EyeDetector eyeDetector(inf);

        eyeDetector.startProcessing(socket);
        eyeDetector.clearConsole();
    }
    catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
    }
    return 0;
}
