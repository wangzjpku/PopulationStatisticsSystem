#!/usr/bin/env python

import os
import datetime
import getopt
import math
import sys
import shutil
import time

from subprocess import call
from parse import *

usage_msg = """\

USAGE: 

python run.py [-f sample file path] [-o output file path] [-e executable file] [-h print this help message and exit]

count people from sample file.

Examples:

1. python run.py -f ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/1112/level1/APC_IN_1_10_150_20141112_141408.bin

indicate file path

2. python run.py -f ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/1112/level1

use 'sample_0008.exe' to process all sample file in subdir path

3. python run.py -e counter_example -f ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/1112/level1

use 'counter_example' to process all sample file in subdir path

"""

_file_idx = 1
_default_work_dir = "../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/"
_pass_arrays = []
_fail_arrays = []

def execute(cmd):
    return call(cmd, shell=True)

def do(in_path, output_path, executable):
    global _file_idx
    global _pass_count
    global _fail_count

    log_filepath = "APC_LOG_" + datetime.datetime.today().strftime("%Y%m%d") + ".log"

    if executable.find(".exe") == -1:
        today = datetime.datetime.today()
        execute("\"{}\" -l100 -f{} > log/APC_LOG_{}.log".format(executable, in_path, today.strftime("%Y%m%d")))
    else:
        execute("\"{}\" -f{}".format(executable, in_path))
    if output_path:
        execute("python apc_analyse.py -l 100 -f {} -o {}".format(in_path, output_path))
    sys.stdout.write(str(_file_idx)+",")
    if output_path:
        rc = execute("python log_parser.py -f {} -o {} -l log/{}".format(in_path, output_path, log_filepath))
    else:
        rc = execute("python log_parser.py -f {} -l log/{}".format(in_path, log_filepath))
    if rc == 0:
        _pass_arrays.append(_file_idx)
    else:
        _fail_arrays.append(_file_idx)
    _file_idx = _file_idx + 1

def do_dir(in_dir, output_path, executable):
    print("{}>change dir to {}".format(os.path.basename(__file__), in_dir))

    for item in os.listdir(in_dir):
        item = os.path.abspath(in_dir + "/" + item)
        if os.path.isdir(item):
            do_dir(item, output_path, executable)
        else:
            results = parse("APC_{}.bin", os.path.basename(item))
            if results:
                do(item, output_path, executable)               

def main(argv):
    global _pass_count
    global _fail_count

    try:
        opts, args = getopt.getopt(argv[1:], "o:f:e:h")

        in_path = None
        output_path = None
        executable = _default_work_dir + "/sample_0008.exe"
        for opt, arg in opts:
            if opt == '-f':
                in_path = arg
            elif opt == '-e':
                executable = "../../poc2/src/counter_example/" + arg
            elif opt == '-o':
                output_path = arg
            elif opt =='-h':
                print(usage_msg)
                sys.exit(1)
        print executable
        # in path
        if in_path is None:
            print(usage_msg)
            sys.exit(1)
        # mkdir
        if output_path:
            if not os.path.exists(output_path):
                os.makedirs(output_path)
        if not os.path.exists("log"):
            os.makedirs("log")

        t1 = time.time()
        if os.path.isdir(in_path):
            do_dir(in_path, output_path, executable)
        else:
            do(in_path, output_path, executable)
        t2 = time.time()

        _pass_count = len(_pass_arrays)
        _fail_count = len(_fail_arrays)
        rate = float("{0:.2f}".format(float(_pass_count) / float(_pass_count + _fail_count)))
        print("PASS={}, FAIL={}, RATE={}%").format(_pass_count, _fail_count, 100 * rate)
        print("FAIL={}".format(_fail_arrays))
        print("Total Consuming={}s".format(round(t2 - t1, 1)))
    except getopt.error, msg:
        sys.stderr.write("error: %s\n" % str(msg))
        sys.stderr.write(usage_msg)
        sys.exit(2)

if __name__ == '__main__':
    main(sys.argv)