import time
from adafruit_motor import motor
from adafruit_pca9685 import PCA9685
from gpiozero import PWMOutputDevice as PWM
import RPi.GPIO as GPIO
import threading

class MotorControl:
    def __init__(self, pwm_motor, motor_pins):
        self.motors = []
        for pins in motor_pins:
            motor_instance = motor.DCMotor(pwm_motor.channels[pins[0]], pwm_motor.channels[pins[1]])
            motor_instance.decay_mode = motor.SLOW_DECAY
            self.motors.append(motor_instance)

    def map_speed(self, x, in_min, in_max, out_min, out_max):
        return (x - in_min) / (in_max - in_min) * (out_max - out_min) + out_min

    def set_motor(self, channel, direction, motor_speed):
        if motor_speed > 110:
            motor_speed = 110
        elif motor_speed < 0:
            motor_speed = 0
        speed = self.map_speed(motor_speed, 0, 110, 0, 1.0)
        if direction == -1:
            speed = -speed

        self.motors[channel-1].throttle = speed

    def stop(self):
        for motor_instance in self.motors:
            motor_instance.throttle = 0

# class IndicatorControl:
#     def __init__(self, indicator_pins):
#         GPIO.setmode(GPIO.BCM)
#         GPIO.setwarnings(False)
#         GPIO.setup(indicator_pins['left'], GPIO.OUT)
#         GPIO.setup(indicator_pins['right'], GPIO.OUT)
#         self.pwm_left = PWM(pin=indicator_pins['left'], initial_value=1.0, frequency=2000)
#         self.pwm_right = PWM(pin=indicator_pins['right'], initial_value=1.0, frequency=2000)
#         self.stop_blink = threading.Event()
#         self.blink_thread = None

#     def blink(self, pwm):
#         while not self.stop_blink.is_set():
#             pwm.value = 0.5
#             time.sleep(0.5)
#             pwm.value = 1.0
#             time.sleep(0.5)

#     def handle_blink(self, pwm):
#         self.stop_blink.clear()
#         self.blink_thread = threading.Thread(target=self.blink, args=(pwm,))
#         self.blink_thread.start()

#     def stop_blinking(self):
#         self.stop_blink.set()
#         if self.blink_thread:
#             self.blink_thread.join()
#         self.stop_blink.clear()

#     def cleanup(self):
#         try:
#             self.pwm_left.close()
#             self.pwm_right.close()
#         except NameError:
#             pass
#         GPIO.cleanup()
class IndicatorControl:
    def __init__(self, indicator_pins):
        """
        방향 지시등을 위한 GPIO 핀 정의를 사용하여 지시등 제어 클래스를 초기화합니다.
        """
        GPIO.setmode(GPIO.BCM)
        GPIO.setwarnings(False)
        GPIO.setup(indicator_pins['left'], GPIO.OUT)
        GPIO.setup(indicator_pins['right'], GPIO.OUT)
        self.pwm_left = PWM(pin=indicator_pins['left'], initial_value=1.0, frequency=2000)
        self.pwm_right = PWM(pin=indicator_pins['right'], initial_value=1.0, frequency=2000)
        self.stop_blink_left = threading.Event()
        self.stop_blink_right = threading.Event()
        self.blink_thread_left = None
        self.blink_thread_right = None

    def blink(self, pwm, stop_event):
        """
        지시등을 깜빡이게 합니다.
        """
        while not stop_event.is_set():
            pwm.value = 0.5
            time.sleep(0.5)
            pwm.value = 1.0
            time.sleep(0.5)

    def handle_blink(self, pwm, side):
        """
        지시등을 별도의 스레드에서 깜빡이게 시작합니다.
        """
        if side == 'left':
            self.stop_blink_left.clear()
            self.blink_thread_left = threading.Thread(target=self.blink, args=(pwm, self.stop_blink_left))
            self.blink_thread_left.start()
        elif side == 'right':
            self.stop_blink_right.clear()
            self.blink_thread_right = threading.Thread(target=self.blink, args=(pwm, self.stop_blink_right))
            self.blink_thread_right.start()

    def stop_blinking(self, side):
        """
        특정 지시등의 깜빡임을 중지합니다.
        """
        if side == 'left':
            self.stop_blink_left.set()
            if self.blink_thread_left:
                self.blink_thread_left.join()
            self.stop_blink_left.clear()
        elif side == 'right':
            self.stop_blink_right.set()
            if self.blink_thread_right:
                self.blink_thread_right.join()
            self.stop_blink_right.clear()

    def cleanup(self):
        """
        지시등 핀을 정리합니다.
        """
        try:
            self.pwm_left.close()
            self.pwm_right.close()
        except NameError:
            pass
        GPIO.cleanup()
