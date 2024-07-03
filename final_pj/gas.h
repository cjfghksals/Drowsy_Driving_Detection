#ifndef GAS_H
#define GAS_H
/*
////////////가상환경에서 나가는 방법////////////////////////////////////////////////////////////////////////
chmod +x /home/pi/.vscode-server/extensions/ms-python.python-2024.6.0/python_files/deactivate/bash/deactivate
deactivate

g++ -o mq135 mq135.cpp -lespeak -lcurl -lportaudio -lsndfile -ljsoncpp -I/usr/include/jsoncpp -lwiringPi
//////////////////////////////////////////////////////////////////////////////////////////////////////////
*/

#include <iostream> //표준입출력
#include <wiringPi.h> //Raspberry Pi GPIO 제어 
#include <wiringPiSPI.h> //SPI통신
#include <unistd.h> //sleep() 함수 
#include "lora.h"
#include "GAME.h"

using namespace std;

/*
매크로 정의
SPI 통신 채널과 속도를 정의
*/
#define SPI_CHANNEL 0
#define SPI_SPEED 1350000


//특정 채널에서 ADC값을 읽어오는 함수(SPI 통신을 통해 값을 읽어온다.)
int readADC(int channel) {
    uint8_t buffer[3] = {1};  // Start bit
    buffer[1] = (8 + channel) << 4;  // Single-ended mode, channel
    buffer[2] = 0;  // Placeholder for the result

    wiringPiSPIDataRW(SPI_CHANNEL, buffer, 3);

    int adcValue = ((buffer[1] & 3) << 8) + buffer[2];  // Combine the result
    return adcValue;
}

//ADC 값(0-1023)을 CO2 농도(0-2500PPM) 범위로 변환
int mapRange(int x, int in_min, int in_max, int out_min, int out_max) {
    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

//센서 값 평가 함수
void evaluateSensorValue() {
    int rawValue = readADC(0);
    int sensorValue = mapRange(rawValue, 0, 1023, 0, 2500) - 300;
    cerr << "CO2 Concentration: " << sensorValue << endl;
    if (sensorValue >= 2000)
    {
        lora();
        Game_start();
        // exit(0);
    }
}

int gas() {
    if (wiringPiSetup() == -1) {
        cerr << "Failed to initialize wiringPi" << endl;
        return 1;
    }

    if (wiringPiSPISetup(SPI_CHANNEL, SPI_SPEED) == -1) {
        cerr << "Failed to initialize SPI" << endl;
        return 1;
    }

    try {
        while (true) {
            evaluateSensorValue();
            sleep(1);
        }
    } catch (...) {
        cerr << "Program stopped by User" << endl;
    }

    return 0;
}

#endif // GAS_H
