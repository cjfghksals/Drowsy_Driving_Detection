/*
가스 명령어: g++ -o InCar InCar_main.cpp `pkg-config --cflags --libs opencv4` -lboost_system -lpthread -lpigpio -lespeak -lcurl -lportaudio -lsndfile -ljsoncpp -I/usr/include/jsoncpp -lwiringPi

MS_DM_BZ빌드 명령어: g++ -o InCar InCar.cpp `pkg-config --cflags --libs opencv4` -lboost_system -lpthread -lpigpio




g++ -o InCar InCar_main.cpp `pkg-config --cflags --libs opencv4` -lboost_system -lpthread -lpigpio -lespeak -lcurl -lportaudio -lsndfile -ljsoncpp -I/usr/include/jsoncpp -lwiringPi

*/

#include <thread>

#include "gas.h"
#include "camera.h"

int main() {
    // thread camera_thread(camera); // 카메라 스레드 시작
    thread gas_thread(gas); // 가스 스레드 시작
    // camera();

    // 두 스레드가 종료될 때까지 대기
    // camera_thread.join();
    gas_thread.join();

    // camera();

    return 0;
}
