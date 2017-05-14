#!/usr/bin/env python

import os
import getopt
import sys
import shutil

from copy import deepcopy

from filesample_parser_52 import *

usage_msg = """\

USAGE: 

python apc_analyse.py [-f sample file path] [-l sample length] [-o output file path] [-b board number, default is 3 for 100Bytes sample length] [-h print this help message and exit]

read 52 or 100 bytes from sample file once time, write PIR data to xls file.

Examples:

for 52Bytes sample file, save PIR to XLS.

python apc_analyse.py -f APC_IN1_170_20141106112023.bin -b 1

for 100Bytes sample file and 2 board, save PIR to XLS.

python apc_analyse.py -b 2 -l 100 -o . -f APC_IN_1_10_150_20141218_100159.bin

"""

# per board
# PIR: [0, 7], [0b, 15b]
# Timestamp: 96b - 99b
def do_sample_100B(in_path, output_path, board_number):
    count = 0
    workbook = xlwt.Workbook()
    sheet = workbook.add_sheet(u'sample')
    fd = open(in_path, 'rb')

    while True:
        sample = bytearray(fd.read(100))
        if sample.__len__() == 0:
            break
        col = timestamp_col = 0
        for i in range(board_number):
            # PIR, only [j, j+8]
            for j in range(16*i, 16*i+8):
                value = (sample[1+j*2] << 8) + sample[j*2]
                temperature = PIR_Conversion(value)
                sheet.write(count, col, value)
                col = col + 1

        def take_timestamp(i):
            return sample[i]+(sample[i+1]<<8)+(sample[i+2]<<16)+(sample[i+3]<<24)

        for i in range(board_number):
            if i == 0:      # slave left
                value = take_timestamp(24)
                timestamp_col = col + 1
            elif i == 1:    # master
                value = take_timestamp(96)
                timestamp_col = timestamp_col + 1
            else:           # slave right
                value = take_timestamp(92)
                timestamp_col = timestamp_col + 1
            sheet.write(count, timestamp_col, value)

        count = count + 1

    workbook.save(output_path)
    fd.close()

def do_sample_52B(in_path, output_path, start, end):
    count = 0

    # start - end
    # PIR: 0 - 7, 0b - 15b
    # UltraSonic 40K: 8 - 15, 16b - 31b
    # UltraSonic 235K: 16 - 23, 32b - 47b
    # Timestamp: 24 - 25, 48b - 51b

    workbook = xlwt.Workbook()
    sheet = workbook.add_sheet(u'sample')
    fd = open(in_path, 'rb')

    while True:
        sample = bytearray(fd.read(52))
        if sample:
            for i in range(start, end+1):
                value = float(sample[1+i*2] << 8) + sample[i*2]

                # PIR
                if start == 0:
                    # value = 80 * (5 * value / 1024 - 1) - 18
                    None
                # UltraSonic 40K
                elif start == 8:
                    value = value * 1.27
                # UltraSonic 235K
                elif start == 16:
                    value = value
                # Time
                elif start == 24:
                    value = value

                sheet.write(count, i, value)

            count = count + 1
        else:
            break

    fd.close()
    workbook.save(output_path)

def main(argv):
    try:
        opts, args = getopt.getopt(argv[1:], "o:b:f:l:h")

        # default
        in_path = None
        output_path = None
        sample_length = 52
        board_number = 3

        for opt, arg in opts:
            if opt == '-f':
                in_path = arg
            elif opt =='-b':
                board_number = int(arg)
            elif opt =='-o':
                output_path = arg
            elif opt == '-l':
                sample_length = int(arg)
            elif opt =='-h':
                print(usage_msg)
                sys.exit(1)
        # in path
        if in_path is None:
            print(usage_msg)
            sys.exit(1)
        # mkdir
        if not os.path.exists(output_path):
            os.makedirs(output_path)
        # do
        basename = os.path.basename(in_path).lower()
        basename = os.path.splitext(basename)[0]
        output_path = output_path + "/" + basename + ".xls"
        if sample_length == 52:
            do_sample_52B(in_path, output_path, 0, 7)
        elif sample_length == 100:
            do_sample_100B(in_path, output_path, board_number)
    except getopt.error, msg:
        sys.stderr.write("error: %s\n" % str(msg))
        sys.stderr.write(usage_msg)
        sys.exit(2)

if __name__ == '__main__':
    main(sys.argv)