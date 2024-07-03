#ifndef LORA_H
#define LORA_H
#include <wiringPi.h>
#include <wiringSerial.h>
#include <iostream>

#define BAUD_RATE 9600
#define SERIAL_PORT "/dev/ttyS0"

using namespace std;

int lora() {
 
    if (wiringPiSetup() == -1) {
        cerr << "Error setting up WiringPi." << endl;
        return 1;
    }

    int serial_port = serialOpen(SERIAL_PORT, BAUD_RATE);
    if (serial_port < 0) {
        cerr << "Error opening serial port." << endl;
        return 1;
    }

    // Set LoRa module parameters
    serialPrintf(serial_port, "AT+ADDRESS=0\r\n");
    delay(200);
    serialPrintf(serial_port, "AT+PARAMETER=12,7,1,4\r\n");
    delay(200);
    serialPrintf(serial_port, "AT+IPR=9600\r\n");
    delay(200);
    serialPrintf(serial_port, "AT+NETWORKID=0\r\n");
    delay(200);

    
    // Send data
    string message = "78가1234";
    serialPrintf(serial_port, "AT+SEND=1,%d,%s\r\n", message.length(), message.c_str());

    serialClose(serial_port);
    
    return 0;
}

#endif // LORA_H