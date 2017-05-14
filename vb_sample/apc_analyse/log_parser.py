#!/usr/bin/env python

import array
import codecs
import datetime
import os
import getopt
import glob
import sys
import shutil
import time

import xlrd
import xlwt

from xlutils.copy import copy
from copy import deepcopy
from xlrd import open_workbook
from xlwt import Workbook

from parse import *
from colorama import init, Fore, Back, Style

usage_msg = """\

USAGE: 

python log_parser.py [-f log file path] [-o output file path] [-d log file dir] [-h print this help message and exit]

read APC VB Sample software's log, take BFlag and IN/OUT information to XLS.

Examples:

1. python log_parser.py -f ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/log/APC_LOG_20141119.log

indicate file path

2. python log_parser.py -d ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/log

indicate file dir, name of default file path will same with current year/month/day

3. python log_parser.py

equal sample 2

"""

_default_dir = "../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/log"
_lane_states = {"flag_zero":0, "rose":2, "falled":1, "passed":-1}

def choose_file(in_dir):
    # today's timestamp
    today = datetime.datetime.today()
    match_string = "APC_LOG_" + today.strftime("%Y%m%d")

    # today's log
    match_files = []
    files = glob.glob(in_dir + "/APC_LOG_*.log")
    for file in files:
        if file.find(match_string) > -1:
            match_files.append(file)

        # last modify
        max_modify_time = 0.0
        max_modify_file = None
        for file in match_files:
            if os.path.getmtime(file) > max_modify_time:
                max_modify_time = os.path.getmtime(file)
                max_modify_file = file
    return max_modify_file

# APC_IN_1_6_train_20141112_165846.bin
# APC_IN_1_6_OUT_1_7_train_20141112_165846.bin
def take_test_result(in_path):
    targets = {"in":False, "out":False, "in_people":0, "out_people":0}
    basename = os.path.basename(in_path).lower()
    if len(basename.split("_")) == 7:
        results = parse("apc_{direction}_{mode}_{people}_{type}_{timestamp}", basename)
        if results['direction'] == 'in':
            targets["in"] = True
            targets["in_people"] = int(results['people'])
        elif results['direction'] == 'out':
            targets["out"] = True
            targets["out_people"] = int(results['people'])
    elif len(basename.split("_")) == 10:
        results = parse("apc_in_{mode1}_{in_people}_out_{mode2}_{out_people}_{type}_{timestamp}", basename)
        targets["in"] = True        
        targets["in_people"] = int(results['in_people'])
        targets["out"] = True
        targets["out_people"] = int(results['out_people'])
    return targets

def do(in_path, output_path, sample_path):
    # colorama
    init(autoreset=True)

    in_people = out_people = 0
    targets = take_test_result(sample_path)

    new_workbook = xlwt.Workbook()

    # XXX, backup sample sheet to Memory and write it to origin file
    if os.path.exists(output_path):
        # sample
        new_sample_sheet = new_workbook.add_sheet(u'sample', cell_overwrite_ok=True)
        workbook = xlrd.open_workbook(output_path, formatting_info=True)
        sample_sheet = workbook.sheet_by_name('sample')

        for row in xrange(sample_sheet.nrows):
            for col in xrange(sample_sheet.ncols):
                value = sample_sheet.cell(row, col).value
                if isinstance(value, int) or isinstance(value, float):
                    new_sample_sheet.write(row, col, int(value))
                else:
                    new_sample_sheet.write(row, col, value)

    # people
    bflag_sheet = new_workbook.add_sheet(u'BFlag1', cell_overwrite_ok=True)
    people_sheet = new_workbook.add_sheet(u'People', cell_overwrite_ok=True)
    bflag_count = people_count = 0
    sensors = array.array("i", [0] * 6) 

    fd = open(in_path, 'r')
    for line in fd:
        line = line.strip(" \t\n\r")
        line = line.replace("dbg>", "").replace("err>", "").replace("war>", "")
        results = parse("{}>{},{}", line)
        if not results:
            continue

        # three type
        # 2014/11/20 9:47:30>SetBFlag,sample=729,bFlag1(2,1,6) from PASSED->FLAG_ZERO
        # 2014/11/20 9:47:30>People,Sample=733,IN=1,height=36
        # 2014/11/21 10:49:58>Calibration,22.3166666,20.55,21.1,21.88,20.78,20.433,20.6,22.01
        line_type = results[1].lower()
        line_body = results[2].lower()
        if line_type == 'setbflag':
            results = parse("sample={sc},bFlag1({lane},{row},{idx}) " +
            "from {old_state}->{new_state}", line_body)

            # sensor
            sensors[int(results['idx']) - 1] = _lane_states[results['new_state']]
            bflag_sheet.write(bflag_count, 0, int(results['sc']))
            for i in range(0, len(sensors)):
                bflag_sheet.write(bflag_count, i+1, sensors[i])
            bflag_count = bflag_count + 1
        elif line_type == 'people':
            # 2014/11/20 16:32:56>People,Sample=1263,OUT=4,height=170,IN=1,height=170,begin=0,width=2
            results = parse("sample={sc},{direction}={people},height={height},begin={begin},width={width}", line_body)

            people_sheet.write(people_count, 0, int(results['sc']))
            people_sheet.write(people_count, 1, results['direction'])
            people_sheet.write(people_count, 2, int(results['height']))
            people_sheet.write(people_count, 3, int(results['people']))
            people_count = people_count + 1
            if results["direction"] == "in":
                in_people = int(results['people'])
            elif results["direction"] == "out":
                out_people = int(results['people'])
        elif line_type == 'calibration':
            results = parse("{},{},{},{},{},{},{},{}", line_body)
            for i in range(0, 8):
                people_sheet.write(0, 8+i, float(results[i]))
        else:
            None
    fd.close()
    if output_path:
        new_workbook.save(output_path)

    # stat
    PASS = False
    if in_people == targets['in_people'] and out_people == targets['out_people']:
        PASS = True
    print("{}{}>{}, IN={}, OUT={}, file={}".format("" if PASS else Fore.RED,
            os.path.basename(__file__), 
            "PASS" if PASS else "FAIL", in_people, out_people, 
            os.path.basename(sample_path)))
    return 0 if PASS else -1

def main(argv):
    try:
        opts, args = getopt.getopt(argv[1:], "d:f:l:o:h")

        in_path = None
        in_dir = None
        output_path = None
        sample_path = None
        for opt, arg in opts:
            if opt == '-d':
                in_dir = arg
            elif opt =='-f':
                sample_path = arg
            elif opt =='-o':
                output_path = arg            
            elif opt =='-l':
                in_path = arg
            elif opt =='-h':
                print(usage_msg)
                sys.exit(1)

        # in path
        if in_path is None and in_dir is None:
            in_dir = _default_dir
        if in_path is None:
            in_path = choose_file(in_dir)
        # mkdir
        if output_path and not os.path.exists(output_path):
            os.makedirs(output_path)
        
        basename = os.path.basename(sample_path).lower()
        basename = os.path.splitext(basename)[0]
        # output path
        if output_path:
            output_path = output_path + "/" + basename + ".xls"
        # do
        sys.exit(do(in_path, output_path, sample_path))
    except getopt.error, msg:
        sys.stderr.write("error: %s\n" % str(msg))
        sys.stderr.write(usage_msg)
        sys.exit(2)

if __name__ == '__main__':
    main(sys.argv)