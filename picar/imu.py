import smbus2
import time

class IMUSensor:
    def __init__(self, address=0x68, gyro_threshold=7000):
        self.address = address
        self.gyro_threshold = gyro_threshold
        self.bus = smbus2.SMBus(1)
        self.setup()

    def setup(self):
        # MPU-6500 연결
        self.bus.write_byte_data(self.address, 0x6B, 0x00)
        # 자이로스코프 범위를 ±500 dps로 설정
        self.bus.write_byte_data(self.address, 0x1B, 0x08)

    def read_word_2c(self, addr):
        high = self.bus.read_byte_data(self.address, addr)
        low = self.bus.read_byte_data(self.address, addr + 1)
        val = (high << 8) + low
        if val >= 0x8000:
            return -((65535 - val) + 1)
        else:
            return val

    def get_gyro_z(self):
        return self.read_word_2c(0x47)
