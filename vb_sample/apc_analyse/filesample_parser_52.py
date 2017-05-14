#!/usr/bin/env python

import os
import math
import array
import sys
import shutil

def PIR_Conversion(value):
    return  80 * (5 * value / 1024 - 1) - 18

class FileSampleParser52:
    PIR_SENSOR = 0
    KHZ40_SENSOR = 1
    KHZ235_SENSOR = 2
    TIMESTAMP_SENSOR = 3

    def __init__(self, filepath):
        self.filepath = filepath
        self.fd = open(self.filepath, 'rb')
        self.next_idx = 0

    def next(self):
        pirs = array.array("f", [0.0] * 24)
        kh40s = array.array("f", [0.0] * 24)
        kh235s = array.array("f", [0.0] * 24)
        timestamps = array.array("i", [0] * 2)

        sample = bytearray(self.fd.read(100))
        if sample:
            pi = k40_i = ti = 0
            for i in range(3):
                for j in range(16):
                    value = (sample[i*32 + 1 + j*2] << 8) + sample[i*32 + j*2]
                    if i == 0 and value < 0x0130 and j == 0:
                        print "error>{}, pir({},{})={}".format(self.next_idx, i, j, hex(value))
                    # PIR
                    if j <= 7:
                        pirs[pi] = PIR_Conversion(float(value))
                        pi = pi + 1
                    else:
                        kh40s[k40_i] = value * 1.27
                        k40_i = k40_i + 1

            timestamps[0] = (sample[97] << 8) + sample[96]
            timestamps[1] = (sample[99] << 8) + sample[98]
            self.next_idx = self.next_idx + 1
            return self.next_idx, pirs, kh40s, kh235s, timestamps[0] + (timestamps[1] << 16)
        else:
            return -1, pirs, kh40s, kh235s, timestamps[0] + (timestamps[1] << 16)

    def sensor_means(self, sensor, max_rows):
        sums = array.array("f", [0.0] * 24)
        rows = 0

        while True:
            idx, pirs, kh40s, kh235s, timestamp = self.next()
            if idx < 1:
                break
            rows = idx

            sensor_datas = [pirs, kh40s, kh235s, timestamp]
            data = sensor_datas[sensor]
            for i in range(0, len(data)):
                sums[i]  = sums[i] + data[i]
            if max_rows > 0 and idx >= max_rows:
                break

        for i in range(0, len(sums)):
            sums[i] = sums[i] / rows
        return sums, rows

    def pir_means(self, max_rows):
        return self.sensor_means(self.PIR_SENSOR, max_rows)

    def khz40_means(self, max_rows):
        return self.sensor_means(self.KHZ40_SENSOR, max_rows)

    def khz235_means(self, max_rows):
        return self.sensor_means(self.KHZ235_SENSOR, max_rows)

    def timestamp_means(self, max_rows):
        return self.sensor_means(self.TIMESTAMP_SENSOR, max_rows)