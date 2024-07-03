#include <Wire.h>
#include <U8g2lib.h>
#include <SoftwareSerial.h>

SoftwareSerial mySerial(10, 11); // RX, TX

U8G2_SSD1306_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0, U8X8_PIN_NONE);

void setup() {
  Serial.begin(9600);
  while (!Serial);

  mySerial.begin(9600);
  while (!mySerial);

  u8g2.begin();
  u8g2.enableUTF8Print();  // enable UTF8 support for the Arduino print() function
  u8g2.setFont(u8g2_font_unifont_t_korean1); // choose a suitable font for Korean
  u8g2.clearBuffer();
  u8g2.setCursor(0, 15);
  u8g2.print("LoRa Receiver Test");
  u8g2.sendBuffer();
  delay(2000); // Pause for 2 seconds
}

void loop() {
  if (mySerial.available()) {
    String receivedString = "";
    while (mySerial.available()) {
      char c = mySerial.read();
      receivedString += c;
      delay(10); // Small delay to allow full message to be received
    }

    // Parse the received message to extract the actual data
    int dataStartIndex = receivedString.indexOf(',') + 1;
    dataStartIndex = receivedString.indexOf(',', dataStartIndex) + 1;
    int dataEndIndex = receivedString.indexOf(',', dataStartIndex);

    if (dataStartIndex > 0 && dataEndIndex > dataStartIndex) {
      String extractedString = receivedString.substring(dataStartIndex, dataEndIndex);

      u8g2.clearBuffer();
      u8g2.setCursor(0, 15); // set the cursor to position (x, y)
      u8g2.print(extractedString + " 의 CO2");
      u8g2.setCursor(0, 35);
      u8g2.print("농도가 높습니다.");
      u8g2.setCursor(0, 55);
      u8g2.print("***주의하세요***");
      u8g2.sendBuffer();

      Serial.println(extractedString + " 의\n이산화탄소 농도가\n높습니다.\n환기가 필요합니다.\n");
    } else {
      Serial.println("Received packet is too short or malformed.");
    }
  }
}
