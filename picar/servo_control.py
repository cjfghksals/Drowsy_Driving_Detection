import time
from adafruit_motor import servo
from adafruit_pca9685 import PCA9685
import keyboard

class ServoControl:
    def __init__(self, pwm_servo):
        self.servo_motor = servo.Servo(pwm_servo.channels[0], min_pulse=580, max_pulse=2350)
        self.current_angle = 90
        self.servo_motor.angle = self.current_angle

    def set_angle(self, angle):
        self.servo_motor.angle = angle

    def change_servo_angle(self, target_angle, key):
        step = 15 if target_angle > self.current_angle else -15
        while self.current_angle != target_angle:
            if not keyboard.is_pressed(key):
                break  # Stop if the key is released
            self.current_angle += step
            if self.current_angle > 150:
                self.current_angle = 150
            elif self.current_angle < 30:
                self.current_angle = 30
            self.set_angle(self.current_angle)
            time.sleep(0.02)  # Small delay for gradual movement
        return self.current_angle
