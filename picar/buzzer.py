import RPi.GPIO as GPIO
import time

class Buzzer:
    def __init__(self, buzzer_pin):
        self.buzzer_pin = buzzer_pin
        GPIO.setwarnings(False)
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(self.buzzer_pin, GPIO.OUT)

    def buzz(self, frequency, duration):
        period = 1 / frequency
        cycles = int(duration * frequency)
        for _ in range(cycles):
            GPIO.output(self.buzzer_pin, GPIO.HIGH)
            time.sleep(period / 2)
            GPIO.output(self.buzzer_pin, GPIO.LOW)
            time.sleep(period / 2)

    def cleanup(self):
        GPIO.cleanup()
