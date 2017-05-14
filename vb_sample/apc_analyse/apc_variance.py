#!/usr/bin/env python

import os
import getopt
import math
import array
import sys
import shutil

from filesample_parser_52 import *

usage_msg = """\

USAGE: 

python apc_variance.py [-f sample bin file path] [-m max of sample row] [-h print this help message and exit]

read 52 bytes from sample bin file once time, calc variance of PIR, max/default row default is file size / 52.

Examples:

python apc_variance.py -f APC_IN1_20141117_111948.bin -m 1000

Sample format

start - end

PIR: 0 - 7, 0b - 15b
UltraSonic 40K: 8 - 15, 16b - 31b
UltraSonic 235K: 16 - 23, 32b - 47b
Time:	24 - 25, 48b - 51b

"""

def variance_calc(items):
	rows = len(items)
	sums = 0.0
	sums_square = 0.0

	for i in range(0, rows):
		value = items[i]
		sums  = sums + value
		sums_square  = sums_square + value * value

	mean = sums / rows
	sums = sums_square - rows * mean * mean
	sums = math.sqrt(sums / rows)
	return sums

def variance(in_path, max_rows, pir_means, khz40_means):
	pir_sums = array.array("f", [0.0] * 24)
	us_sums = array.array("f", [0.0] * 24)
	rows = 0

	parser = FileSampleParser52(in_path)
	while True:
		idx, pirs, kh40s, kh235s, timestamp = parser.next()
		if idx < 1:
			break
		rows = idx
		for i in range(24):
			# PIR
			diff = pirs[i] - pir_means[i]
			pir_sums[i] = pir_sums[i] + diff * diff
			# Ultrasonic 40Khz
			diff = kh40s[i] - khz40_means[i]
			us_sums[i] = us_sums[i] + diff * diff
		if max_rows > 0 and idx >= max_rows:
			break

	for i in range(len(pir_sums)):
		pir_sums[i] = math.sqrt(pir_sums[i] / rows)
		us_sums[i] = math.sqrt(us_sums[i] / rows)
	return pir_sums, us_sums, rows

def do(in_path, max_rows):
	# O(2N)
	pir_means, rows = FileSampleParser52(in_path).pir_means(max_rows)
	khz40_means, rows = FileSampleParser52(in_path).khz40_means(max_rows)
	pir_variances, us_variances, rows = variance(in_path, max_rows, pir_means, khz40_means)

	print("------------------------------------------")
	print("APC Sample Variance, O(2N), {} samples".format(rows))
	print("------------------------------------------")
	for i in range(len(pir_variances)):
		print("PIR({:>2})->Variance={},\tMean={}".format(i+1, pir_variances[i], pir_means[i]))
	print("------------------------------------------")
	for i in range(len(us_variances)):
		print("Ultrasonic-40K({:>2})->Variance={},\tMean={}".format(i+1, us_variances[i], khz40_means[i]))

def main(argv):
	try:
		opts, args = getopt.getopt(argv[1:], "f:m:h")

		in_path = ""
		max_rows = 0

		for opt, arg in opts:
			if opt == '-f':
				in_path = arg
			elif opt =='-m':
				max_rows = int(arg)
			elif opt =='-h':
				print(usage_msg)
				sys.exit(1)

		if in_path == "":
			print(usage_msg)
			sys.exit(1)

		do(in_path, max_rows)
	except getopt.error, msg:
		sys.stderr.write("error: %s\n" % str(msg))
		sys.stderr.write(usage_msg)
		sys.exit(2)

if __name__ == '__main__':
    main(sys.argv)