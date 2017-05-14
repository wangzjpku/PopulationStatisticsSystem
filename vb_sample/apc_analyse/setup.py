#!/usr/bin/env python

import os
import sys
from subprocess import call

def execute(cmd):
        rc = call(cmd, shell=True)
        if rc != 0:
                sys.exit(rc)

def install_lib(file):
	name = ('.').join(file.split('.')[:-1])

	if file.endswith(".tar.gz"):
		name = file.split(".tar.gz")[0]
		execute("tar xvf {}".format(file))
	elif file.endswith(".zip"):
		execute("unzip {}".format(file))
		name = file.split(".zip")[0]
	else:
		return
	execute("cd {} && python setup.py install".format(name))
	execute("rm -rf {}".format(name))

os.chdir("depend")

_first_lib = "setuptools-7.0.zip"
install_lib(_first_lib)

for file in os.listdir("."):
	if file != _first_lib:
		install_lib(file)

os.chdir("../")