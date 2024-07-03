int redLED = 13; // 빨강 LED 핀
int greenLED = 12; // 초록 LED 핀
int switchPin = 2; // 스위치 핀

int state = 0; // 현재 상태 (0: 초기, 1: 빨강, 2: 초록)
bool switchPressed = false; // 스위치 눌림 상태

void setup() {
  pinMode(redLED, OUTPUT); // 빨강 LED 핀을 출력으로 설정
  pinMode(greenLED, OUTPUT); // 초록 LED 핀을 출력으로 설정
  pinMode(switchPin, INPUT_PULLUP); // 스위치 핀을 입력으로 설정, 내부 풀업 저항 사용
}

void loop() {
  if (digitalRead(switchPin) == LOW) { // 스위치가 눌렸을 때
    if (!switchPressed) { // 스위치가 새로 눌렸을 때만 처리
      delay(150); // 스위치 바운스 방지를 위한 지연
      state = (state + 1) % 3; // 상태 변경 (0 -> 1 -> 2 -> 0)
      
      switch (state) {
        case 0: // 초기 상태
          digitalWrite(redLED, LOW);
          digitalWrite(greenLED, LOW);
          break;
        case 1: // 빨강 LED 켜짐
          digitalWrite(redLED, HIGH);
          digitalWrite(greenLED, LOW);
          break;
        case 2: // 초록 LED 켜짐
          digitalWrite(redLED, LOW);
          digitalWrite(greenLED, HIGH);
          break;
      }
      switchPressed = true; // 스위치 눌림 상태 변경
    }
  } else {
    switchPressed = false; // 스위치 해제 상태 변경
  }
}
