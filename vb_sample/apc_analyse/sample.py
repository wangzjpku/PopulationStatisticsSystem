#!/usr/bin/env python

import os
import getopt
import sys
import shutil

from parse import *

def sample_parser(basename):
    results = {}

    if len(basename.split("_")) == 7:
        results = parse("apc_{direction}_{mode}_{people}_{type}_{timestamp}", basename)
    elif len(basename.split("_")) == 10:
        results = parse("apc_in_{mode1}_{in_people}_out_{mode2}_{out_people}_{type}_{timestamp}", basename)
    return results

def sample_ymd(basename):
    results = sample_parser(basename)
    return results['timestamp'].split("_")[0]