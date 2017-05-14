APC Analyse
=================================

read APC sample data per 52 bytes, write indicate position data to XLS file, for example PIR/40K/235K/Timestamp

Setup
=================================

### 1.Python2.7.5 or late

`//137.40.199.8/pubdir/008Software/Program%20Language/Python/python-2.7.5.msi`

### 2.Set Python.exe to Environment Variables

### 3.Click command window and enter `apc\tools\apc_analyse`

`python setup.py`

Usage
=================================

## Click command window and enter `apc\tools\apc_analyse`

## 1. write sample data to XLS
`python apc_analyse.py -f APC_IN1_150_20141106_112802.bin`

or

`python apc_analyse.py -f APC_IN1_150_20141106_112802.bin -b "0,7"`

### 40K
`python apc_analyse.py -f APC_IN1_150_20141106_112802.bin -b "8,15"`

### 235K
`python apc_analyse.py -f APC_IN1_150_20141106_112802.bin -b "16,23"`

### Timestamp
`python apc_analyse.py -f APC_IN1_150_20141106_112802.bin -b "24,25"`

### Output to `APC_IN1_150_20141106_112802.xls` 

## 2. calc variance from sample file

`apc_variance.py -f ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/1112/level2/APC_IN1_fast_20141112_163009.bin`

## 3. parser log of sample_008.exe

`log_parser.py -f ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/log/APC_LOG_20141119.log`

or

`log_parser.py`

## 4. run sample file

`run.py -f ../../demo_board_jp/src/vb_sample/sample_0008/bin/Debug/1112/level2/APC_IN1_fast_20141112_163009.bin`

## feature

* calc variance
* run sample_008.exe
* write sample data
* parser log