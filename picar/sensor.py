import RPi.GPIO as GPIO
import time

class IRSensor:
    def __init__(self, sensor_pin):
        self.sensor_pin = sensor_pin
        GPIO.setwarnings(False)
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(self.sensor_pin, GPIO.IN)

    def is_object_detected(self):
        return not GPIO.input(self.sensor_pin)  # 감지되면 True 반환

    def cleanup(self):
        GPIO.cleanup()
