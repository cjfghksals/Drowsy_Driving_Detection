import time
import keyboard
import threading
import RPi.GPIO as GPIO
from gpiozero import PWMOutputDevice as PWM
from board import SCL, SDA
import busio
from adafruit_pca9685 import PCA9685
from adafruit_motor import motor, servo
import cv2

from sensor import IRSensor
from buzzer import Buzzer
from imu import IMUSensor

# IR 센서 및 부저 핀 번호
SENSOR_PIN = 23
BUZZER_PIN = 24

# 모터 핀 번호
MOTOR_M1_IN1 = 15
MOTOR_M1_IN2 = 14
MOTOR_M2_IN1 = 12
MOTOR_M2_IN2 = 13
MOTOR_M3_IN1 = 11
MOTOR_M3_IN2 = 10
MOTOR_M4_IN1 = 8
MOTOR_M4_IN2 = 9

# 방향 지시등 핀 번호
indicator_pin_left = 0
indicator_pin_right = 6

# I2C 및 PCA9685 초기화
i2c = busio.I2C(SCL, SDA)
pwm_motor = PCA9685(i2c, address=0x5f)
pwm_motor.frequency = 1000

# 모터 초기화
motor1 = motor.DCMotor(pwm_motor.channels[MOTOR_M1_IN1], pwm_motor.channels[MOTOR_M1_IN2])
motor2 = motor.DCMotor(pwm_motor.channels[MOTOR_M2_IN1], pwm_motor.channels[MOTOR_M2_IN2])
motor3 = motor.DCMotor(pwm_motor.channels[MOTOR_M3_IN1], pwm_motor.channels[MOTOR_M3_IN2])
motor4 = motor.DCMotor(pwm_motor.channels[MOTOR_M4_IN1], pwm_motor.channels[MOTOR_M4_IN2])

# 서보 모터 초기화
pwm_servo = PCA9685(i2c, address=0x5f)
pwm_servo.frequency = 50
servo0 = servo.Servo(pwm_servo.channels[0], min_pulse=580, max_pulse=2350)

# GPIO 초기화
GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)
GPIO.setup(indicator_pin_left, GPIO.OUT)
GPIO.setup(indicator_pin_right, GPIO.OUT)

pwm_left = PWM(pin=indicator_pin_left, initial_value=1.0, frequency=2000)
pwm_right = PWM(pin=indicator_pin_right, initial_value=1.0, frequency=2000)

stop_blink = threading.Event()
blink_thread = None

# IR 센서 및 부저 객체 생성
ir_sensor = IRSensor(SENSOR_PIN)
buzzer = Buzzer(BUZZER_PIN)
imu_sensor = IMUSensor()



# 맵핑 함수
def map(x, in_min, in_max, out_min, out_max):
    return (x - in_min) / (in_max - in_min) * (out_max - out_min) + out_min

# 모터 속도 및 방향 설정
def Motor(channel, direction, motor_speed):
    if motor_speed > 110:
        motor_speed = 110
    elif motor_speed < 0:
        motor_speed = 0
    speed = map(motor_speed, 0, 110, 0, 1.0)
    if direction == -1:
        speed = -speed

    if channel == 1:
        motor1.throttle = speed
    elif channel == 2:
        motor2.throttle = speed
    elif channel == 3:
        motor3.throttle = speed
    elif channel == 4:
        motor4.throttle = speed

# 모든 모터 정지
def motorStop():
    motor1.throttle = 0
    motor2.throttle = 0
    motor3.throttle = 0
    motor4.throttle = 0
    

# 자원 해제
def destroy():
    motorStop()
    pwm_motor.deinit()
    pwm_servo.deinit()
    try:
        pwm_left.close()
        pwm_right.close()
    except NameError:
        pass
    GPIO.cleanup()
    cv2.destroyAllWindows()
    ir_sensor.cleanup()
    buzzer.cleanup()

# 전진
def forward(speed_set):
    Motor(1, 1, speed_set)
    Motor(2, 1, speed_set)
    Motor(3, 1, speed_set)
    Motor(4, 1, speed_set)

# 후진
def backward(speed_set):
    Motor(1, -1, speed_set)
    Motor(2, -1, speed_set)
    Motor(3, -1, speed_set)
    Motor(4, -1, speed_set)

# 서보 각도 설정
def set_angle(ID, angle):
    servo_angle = servo.Servo(pwm_servo.channels[ID], min_pulse=500, max_pulse=2400, actuation_range=180)
    servo_angle.angle = angle

# 서보 각도 변경
def change_servo_angle(target_angle, current_angle, key):
    step = 15 if target_angle > current_angle else -15
    while current_angle != target_angle:
        if not keyboard.is_pressed(key):
            break
        current_angle += step
        if current_angle > 150:
            current_angle = 150
        elif current_angle < 30:
            current_angle = 30
        set_angle(0, current_angle)
        time.sleep(0.02)
    return current_angle

# 지시등 깜빡임
def blink(pwm):
    while not stop_blink.is_set():
        pwm.value = 0.5
        time.sleep(0.5)
        pwm.value = 1.0
        time.sleep(0.5)

def handle_blink(pwm):
    global blink_thread
    stop_blink.clear()
    blink_thread = threading.Thread(target=blink, args=(pwm,))
    blink_thread.start()

def stop_blinking():
    global blink_thread
    stop_blink.set()
    if blink_thread:
        blink_thread.join()
    stop_blink.clear()

def check_imu_for_lanes():
    while True:
        gyro_z = imu_sensor.get_gyro_z()
        if gyro_z > imu_sensor.gyro_threshold:
            print("급격한 좌측 차선 변경 감지됨!")
            buzzer.buzz(1000, 0.5)
        elif gyro_z < -imu_sensor.gyro_threshold:
            print("급격한 우측 차선 변경 감지됨!")
            buzzer.buzz(1000, 0.5)
        time.sleep(0.1)

if __name__ == '__main__':
    cap = cv2.VideoCapture(0)
    imu_thread = threading.Thread(target=check_imu_for_lanes)
    imu_thread.daemon = True
    imu_thread.start()
    
    try:
        base_speed = 30
        max_speed = 110
        acceleration_step = 5
        current_speed = base_speed
        direction = 0 #0 정지 1 후진 : -1 전진

        current_servo_angle = 90
        set_angle(0, current_servo_angle)
        print("서보 초기화 완료: 90도")

        left_blinking = False
        right_blinking = False

        while True:
            ret, frame = cap.read()
            if not ret:
                print("영상 캡처 실패")
                break

            cv2.imshow("camera", frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

            if ir_sensor.is_object_detected():
                print("물체 감지됨")
                #buzzer.buzz(1000, 1)
                motorStop()
                current_speed=0
                print("긴급 제동")
                time.sleep(1)  # 감지 시 1초 대기
            else:
                print("물체 없음")

            if keyboard.is_pressed('x'):
                forward(max_speed)
                print("최대 속도로 전진")
                time.sleep(0.1)
            elif keyboard.is_pressed('w'):
                backward(max_speed)
                print("최대 속도로 후진")
                time.sleep(0.1)
            elif keyboard.is_pressed('s'):
                motorStop()
                current_speed=0
                print("정지")
                time.sleep(0.1)
            elif keyboard.is_pressed('down'):
                direction = 1
                if current_speed < max_speed:
                    current_speed += acceleration_step
                forward(current_speed)
                print(f"서서히 전진, 속도: {current_speed}")
                time.sleep(0.1)
            elif keyboard.is_pressed('up'):
                direction = -1
                if current_speed < max_speed:
                    current_speed += acceleration_step
                backward(current_speed)
                print(f"서서히 후진, 속도: {current_speed}")
                time.sleep(0.1)
            elif keyboard.is_pressed('right'):
                stop_blinking()
                servo_target_angle = 0
                current_servo_angle = change_servo_angle(servo_target_angle, current_servo_angle, 'right')
                print(f"서보 각도 0도로 이동 중, 현재 각도: {current_servo_angle}")
            elif keyboard.is_pressed('0'):
                servo_target_angle = 90
                current_servo_angle = change_servo_angle(servo_target_angle, current_servo_angle, '0')
                print(f"Servo angle moving towards 0 degrees, current angle: {current_servo_angle}")
            elif keyboard.is_pressed('left'):
                stop_blinking()
                servo_target_angle = 180
                current_servo_angle = change_servo_angle(servo_target_angle, current_servo_angle, 'left')
                print(f"서보 각도 180도로 이동 중, 현재 각도: {current_servo_angle}")
            elif keyboard.is_pressed('a'):
                if not left_blinking:
                    handle_blink(pwm_left)
                    left_blinking = True
                    right_blinking = False
                    print("왼쪽 방향 지시등 깜빡임 시작")
                else:
                    stop_blinking()
                    left_blinking = False
                    print("왼쪽 방향 지시등 깜빡임 중지")
                time.sleep(0.1)
            elif keyboard.is_pressed('d'):
                if not right_blinking:
                    handle_blink(pwm_right)
                    right_blinking = True
                    left_blinking = False
                    print("오른쪽 방향 지시등 깜빡임 시작")
                else:
                    stop_blinking()
                    right_blinking = False
                    print("오른쪽 방향 지시등 깜빡임 중지")
                time.sleep(0.1)
            else:
                if current_speed > base_speed:
                    current_speed -= acceleration_step
                else:
                    current_speed = base_speed
                    direction = 0
                if direction == 1:
                    forward(current_speed)
                elif direction == -1:
                    backward(current_speed)
                else:
                    motorStop()
                print(f"속도 감소 중, 속도: {current_speed}")
                time.sleep(0.1)

    except KeyboardInterrupt:
        destroy()
    finally:
        stop_blinking()
        destroy()
        cap.release()
        cv2.destroyAllWindows()
