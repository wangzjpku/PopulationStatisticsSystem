#include <string.h>
#include "counter_people.h"
#include "channel.h"
#include "log.h"

#ifdef NUC140
#include "Driver\DrvSYS.h"
#else
#define DrvSYS_Delay(ms)   ;
#endif

enum {
    FLAG_ZERO = 0,
    ROSE,
    FALLED,
    PASSED,
};

enum {
    ULTRASONIC_235 = 0,
    ULTRASONIC_40
};

#define NUM_OF_PIR_SENSORS          (8)
#define NUM_OF_ULTRASONIC_SENSORS   (8)
#define NUM_OF_SENSORS              (NUM_OF_PIR_SENSORS + NUM_OF_ULTRASONIC_SENSORS)

#define MAX_BOARD_NUMBER            (3)

#define MAX_PIR_SENSORS             (NUM_OF_PIR_SENSORS * MAX_BOARD_NUMBER)
#define MAX_ULTRASONIC_SENSORS      (NUM_OF_ULTRASONIC_SENSORS * MAX_BOARD_NUMBER)
#define MAX_SENSORS                 (MAX_PIR_SENSORS + MAX_ULTRASONIC_SENSORS)

#define LANES_OF_BOARD              (NUM_OF_PIR_SENSORS / 2)
#define LANES                       (LANES_OF_BOARD * MAX_BOARD_NUMBER)
#define ROWS                        2
#define CHECKPOINTS                 (LANES * ROWS)

#define KEEPPERIOD                  50
#define CALIBRATION_MODE            1
#define COUNTING_MODE               2
#define CALIBRATION_CYCLES          60
#define BARHEIGHT                   190.0
#define KEEP_HEIGHT_LIFE_CNT        35
#define PIR_FILTER_LENGTH           2

#define CHECKDURATION               12

static unsigned char _board_number = 3;
static unsigned char _pir_number = 3 * 8;
static unsigned char _pir_lane = 3 * 8 / 2;
static unsigned char _us_number = 3 * 8;

static double _us_sensors[MAX_ULTRASONIC_SENSORS];
static int _us_max_heights[MAX_ULTRASONIC_SENSORS];
static int _us_keep_counters[MAX_ULTRASONIC_SENSORS] = {KEEP_HEIGHT_LIFE_CNT};

static double _pir_sensors[MAX_PIR_SENSORS];
static double _pir_means[MAX_PIR_SENSORS];
static double _pir_sums[MAX_PIR_SENSORS]; 
static double _pir_filter_buffers[MAX_PIR_SENSORS][PIR_FILTER_LENGTH];
static unsigned char _pir_filter_idxs[MAX_PIR_SENSORS];

static int iii = 0;
static int ii = 0;
static int _iiold = 0;
static double dT_pos[LANES][ROWS];
static double dT_neg[LANES][ROWS];
static int _iLastDetectioni = 0;

static double dThresh1 = 0;
static double dThresh2 = 0;

static double dT[CHECKPOINTS];
static double dTold[CHECKPOINTS];
static double dTmax[LANES][ROWS];
static int iTmax_time[LANES][ROWS];

static unsigned char bFlag1[LANES][ROWS];
static unsigned char _bState = 0;
static unsigned char _bLastState = 0;
static unsigned char bLane_State[LANES][KEEPPERIOD];
static int iDuration[LANES][KEEPPERIOD];
static unsigned char bLane_Tmax[LANES][KEEPPERIOD];

static unsigned char bMode = 0;
static unsigned char bCalibrationCnt = 0;

static int iHighDuration[LANES][ROWS];
static int iPassCount = 0;

static int Calibration(void)
{
    unsigned char i;

    for (i = 0; i < _pir_number; i++) {
        _pir_sums[i] += _pir_sensors[i];
    }

    if (bCalibrationCnt == CALIBRATION_CYCLES - 1) {
        for (i = 0; i < _pir_number; i++) {
            _pir_means[i] = _pir_sums[i] / CALIBRATION_CYCLES;
            if (_pir_means[i] < 0 || _pir_means[i] > 35) {
                printk(LEVEL_E, "pir_means(%d)=%f invalid\r\n", i, _pir_means[i]);
                return -1;
            }
        }
        /* for each per board */
        for (i = 0; i < _board_number; i++) {
            printk(LEVEL_D, "Calibration(%d),%f,%f,%f,%f,%f,%f,%f,%f\r\n",
                    i, 
                    _pir_means[i*8 + 0], _pir_means[i*8 + 1], 
                    _pir_means[i*8 + 2], _pir_means[i*8 + 3], 
                    _pir_means[i*8 + 4], _pir_means[i*8 + 5], 
                    _pir_means[i*8 + 6], _pir_means[i*8 + 7]);
            DrvSYS_Delay(10 * 1000);
        }
        bMode = COUNTING_MODE;
    } else
        bCalibrationCnt++;
    return 0;
}

static void SetBFlag1(unsigned int count, 
                    unsigned char lane, 
                    unsigned char row, 
                    unsigned char value)
{
#if 0
    static char *state_string[] = {
        "FLAG_ZERO",
        "ROSE",
        "FALLED",
        "PASSED",
        "UNKNOWN"
    };
    static int length = sizeof(state_string) / sizeof(char *);

    char *oldValue = state_string[bFlag1[lane][row]];
    char *newValue = state_string[value];

    printk(LEVEL_D, ">SetBFlag,sample=%d,bFlag1(%d,%d,%d),from %s->%s\r\n", 
        count, lane, row, (lane * 2 + row + 1), oldValue, newValue);
#endif
    bFlag1[lane][row] = value;
}

static void us_analyze(unsigned int count)
{
    int max_height;
    unsigned char us;

    for (us = 0; us < _us_number; us++) {
        /* reset max height once counter decrement to 0 */
        if (_us_keep_counters[us] > 0)
            _us_keep_counters[us]--;
        if (_us_keep_counters[us] == 0)
            _us_max_heights[us] = 0;

        /* max height */
        max_height = (int) (BARHEIGHT - _us_sensors[us]);
        if (max_height > _us_max_heights[us]) {
            _us_max_heights[us] = max_height;
            _us_keep_counters[us] = KEEP_HEIGHT_LIFE_CNT;
        }
    }
}

static int us_get_height(unsigned char begin, unsigned char width)
{
    int max_height = 0;
    unsigned char us;
    unsigned char board_idx = (begin / LANES_OF_BOARD);

    /**
     * assign begin and board index
     * begin=[0,3]=slave left, board_idx=0, us idx=[0,3]
     * begin=[4,7]=master, board_idx=1, us idx=[8, 11]
     * begin=[8,11]=master, board_idx=2, us idx=[16, 19]
     */
    begin = begin + board_idx * 4;
    for (us = begin; us < begin + width; us++) {
        if (_us_max_heights[us] > max_height)
            max_height = _us_max_heights[us];
    }
    return max_height < 120 ? 0 : max_height;
}

static void HistoryShift(void)
{
    unsigned char lane, i;

    // loop counter go back to 20
    ii = 20; // 20 just 20 Plextek one (can be other)

    //If _iiold >= (KEEPPERIOD - 20) Then
    _iiold = _iiold - (KEEPPERIOD - ii);
    //Else
    // what can I do ???
    //End If

    for (lane = 0; lane < _pir_lane; lane++) {
        for (i = 0; i < ii; i++) {
            bLane_State[lane][i] = bLane_State[lane][KEEPPERIOD - ii + i];
            iDuration[lane][i] = iDuration[lane][KEEPPERIOD - ii + i];
            bLane_Tmax[lane][i] = bLane_Tmax[lane][KEEPPERIOD - ii + i];
        }
        for (i = ii; i < KEEPPERIOD; i++) {
            bLane_State[lane][i] = 0;
            iDuration[lane][i] = 0;
            bLane_Tmax[lane][i] = 0;
        }
    }
}

static double GetMax2(double Val1, double Val2)
{
    return Val1 >= Val2 ? Val1 : Val2;
}

static double GetMin2(double Val1, double Val2)
{
    return Val1 <= Val2 ? Val1 : Val2;
}

static unsigned char Search_LaneStateMax(unsigned char lane, 
                                        unsigned char t1, 
                                        unsigned char t2)
{
    unsigned char Max = 0, i;

    for (i = t1; i <= t2; i++) {
        /* XXX: ignore OUT=1 and IN=2 same time present in t1~t2 */
        if (bLane_State[lane][i] > Max)
            Max = bLane_State[lane][i];
    }
    return Max;
}

static void pir_mean(void)
{
    unsigned char i, j;
    double temp_sum;

    for (i = 0; i < _pir_number; i++) {
        _pir_sensors[i] = _pir_sensors[i] - _pir_means[i] + 20;

        /* add filtering (2014/08/26)  moving mean value */
        if (_pir_filter_idxs[i] >= PIR_FILTER_LENGTH)
            _pir_filter_idxs[i] = 0;
        _pir_filter_buffers[i][_pir_filter_idxs[i]] = _pir_sensors[i];
        _pir_filter_idxs[i]++;

        for (temp_sum = 0.0, j = 0; j < PIR_FILTER_LENGTH; j++) {
            temp_sum += _pir_filter_buffers[i][j];
        }
        _pir_sensors[i] = temp_sum / PIR_FILTER_LENGTH;

        /* save to lanes */
        dTold[i] = dT[i];
        dT[i] = _pir_sensors[i];
    }
}

static void pir_parse(unsigned int count)
{
    unsigned char Point = 0;
    unsigned char row_other = 0;
    unsigned char lane, row;

    for (lane = 0; lane < _pir_lane; lane++) {
        for (row = 0; row < ROWS; row++) {
            Point = (lane * 2 + row); // 0-7

            // Check for Thermal trigger
            if (bFlag1[lane][row] == FLAG_ZERO) { // in case of new detection
                if (dT[Point] > dThresh1) {
                    SetBFlag1(count, lane, row, ROSE);
                    dTmax[lane][row] = dT[Point];
                    iTmax_time[lane][row] = iii;
                    dT_pos[lane][row] = iii; // i stamp of rising
                }
            }

            // check when temp falls below threshold
            if (bFlag1[lane][row] > 0) {
                if (dT[Point] > dTmax[lane][row]) {
                    dTmax[lane][row] = dT[Point];
                    iTmax_time[lane][row] = iii;
                }
                iHighDuration[lane][row]++;

                // keep the ultra sonic data at while ROSE state
                // put outside ROSE state
                if (dTold[Point] >= dThresh2 && dT[Point] < dThresh2) {
                    SetBFlag1(count, lane, row, FALLED);
                    // estimated (precise) i when drop below thresh2
                    dT_neg[lane][row] = iii - 1 
                        + (dThresh2 - dTold[Point]) / (dT[Point] - dTold[Point]);
                    iHighDuration[lane][row] = 0;
                }
            }

            // Get rid of detection if flag1=2 has been triggered for more than
            // a certain amount of i
            // think about this a bit more -
            // how to deal with it ?
            // if one rises then falls, and the other hasn't risen within x
            // amount of samples following th first ones demise, we reset to zero
            row_other = row == 0 ? 1 : 0;

            if (bFlag1[lane][row] == FALLED && bFlag1[lane][row_other] == FLAG_ZERO) {
                if (iii - 20 > dT_neg[lane][row])
                    SetBFlag1(count, lane, row, FLAG_ZERO);
            }

            // add T.Ito 2014/09/17  seen a bug of continued ROSE of one sensor
            if (bFlag1[lane][row] == ROSE && bFlag1[lane][row_other] == FLAG_ZERO) {
                if (iii - 100 > dT_pos[lane][row])
                    SetBFlag1(count, lane, row, FLAG_ZERO);
            }

            // add zheng 2014/11/03
            if (bFlag1[lane][row] == ROSE 
                && (bFlag1[lane][row_other] == ROSE || bFlag1[lane][row_other] == FALLED)) {
                if (iii - 100 > dT_pos[lane][row]) {
                    SetBFlag1(count, lane, row, FALLED);
                    SetBFlag1(count, lane, row_other, FALLED);

                    if (dT_pos[lane][row] < dT_pos[lane][row_other]) {
                        dT_neg[lane][row] = iii - 1;
                        dT_neg[lane][row_other] = iii;
                    } else {
                        dT_neg[lane][row] = iii;
                        dT_neg[lane][row_other] = iii - 1;
                    }
                    iHighDuration[lane][row] = 0;
                    iHighDuration[lane][row_other] = 0;
                }
            }
        } //row

        // find when both are below thermal threshold
        if (bFlag1[lane][0] == FALLED && bFlag1[lane][1] == FALLED) {
            if (dT[Point] < dThresh2 && dT[Point - 1] < dThresh2) {
                SetBFlag1(count, lane, 0, PASSED);
                SetBFlag1(count, lane, 1, PASSED);
            }
        }

        // now go for a detection along the lane
        if (bFlag1[lane][0] == PASSED && bFlag1[lane][1] == PASSED) {
            double dT_Tpos, dT_Tneg;
            int index1, index2;

            iPassCount = iPassCount + 1;
            dT_Tpos = dT_pos[lane][1] - dT_pos[lane][0];
            dT_Tneg = dT_neg[lane][1] - dT_neg[lane][0];
            SetBFlag1(count, lane, 0, FLAG_ZERO);
            SetBFlag1(count, lane, 1, FLAG_ZERO);

            index1 = GetMax2(dT_neg[lane][0], dT_neg[lane][1]);
            index2 = GetMin2(dT_pos[lane][0], dT_pos[lane][1]);
            bLane_Tmax[lane][ii] = GetMax2(dTmax[lane][0], dTmax[lane][1]);
            if (index1 >= index2) {
                iDuration[lane][ii] = index1 - index2;
            } else
                printk(LEVEL_D, ">error,sample=%d,sensor=%d,index1=%d<index2=%d\r\n", 
                                    count, (lane * 2 + row + 1), index1, index2);
#if 0
            if (dT_Tpos > 0) {
                if (dT_Tneg >= 0 || iTmax_time[lane][0] <= iTmax_time[lane][1])
                    bLane_State[lane][ii] = (int) DIRECTION_IN;
                else
                    bLane_State[lane][ii] = (int) DIRECTION_OUT;
            } else if (dT_Tpos < 0) {
                if (dT_Tneg <= 0 || iTmax_time[lane][0] >= iTmax_time[lane][1])
                    bLane_State[lane][ii] = (int) DIRECTION_OUT;
                else
                    bLane_State[lane][ii] = (int) DIRECTION_IN;
            } else {
                if (dT_Tneg < 0)
                    bLane_State[lane][ii] = (int) DIRECTION_OUT;
                else if (dT_Tneg > 0)
                    bLane_State[lane][ii] = (int) DIRECTION_IN;
                else {
                    if (iTmax_time[lane][0] > iTmax_time[lane][1])
                        bLane_State[lane][ii] = (int) DIRECTION_IN;
                    else if (iTmax_time[lane][0] < iTmax_time[lane][1])
                        bLane_State[lane][ii] = (int) DIRECTION_OUT;
                    else/* XXX: */
                        bLane_State[lane][ii] = (int) DIRECTION_NONE;
                }
            }
#else
            if (dT_Tpos >= 0 && dT_Tneg > 0) { // row1 -> row2 direction
                bLane_State[lane][ii] = 2;
                // IN
            } else if (dT_Tpos <= 0 && dT_Tneg < 0) { // row2 -> row1 direction
                bLane_State[lane][ii] = 1;
                // OUT
            } else {
                // dunno TODO
                double ind1 = iTmax_time[lane][0];
                double ind2 = iTmax_time[lane][1];

                if (ind1 > ind2)
                    bLane_State[lane][ii] = 1;
                else if (ind1 < ind2)
                    bLane_State[lane][ii] = 2;
                else
                    bLane_State[lane][ii] = 0;
            }
#endif
        }
    }
}

static int pir_lane_analyze(unsigned int count,
                            struct people *peoples,
                            unsigned char people_max_count)
{
    struct active_channel atv_channels[LANES];
    unsigned int atv_channels_size = 0, people_count;
    unsigned char chan[LANES];
    unsigned char ActiveLaneCnt, lane, i;
    int iMaxLaneDuration = 0;
    int iPeopleCnt = 0;

    _bState = 0;
    /* take lane state, IN=2, OUT=1, NONE=0 */
    for (lane = ActiveLaneCnt = 0; lane < _pir_lane; lane++) {
        chan[lane] = Search_LaneStateMax(lane, ii - CHECKDURATION, ii);
    }

    /* filtering and analyze */
    memset(atv_channels, 0x00, sizeof(atv_channels));
    channel_filtering(chan, _pir_lane, atv_channels, &atv_channels_size, 1);
    people_count = channel_analyze(atv_channels, 
                                    atv_channels_size, 
                                    3, 
                                    4, 
                                    peoples, 
                                    people_max_count);
    if (people_count > 0) {
        iPeopleCnt = (int) people_count;
        for (i = 0; i < people_count; i++) {
            /* max height of [begin, begin+width] */
            peoples[i].height = us_get_height(peoples[i].begin, peoples[i].width);
            if (_bState == 0 || _bState == (int) atv_channels[i].direction)
                _bState = (int) atv_channels[i].direction;
            else {
                _bState = (int) DIRECTION_OPPOSITE;
                break;
            }
        }
        for (lane = 0; lane < _pir_lane; lane++) {
            if (iDuration[lane][ii - CHECKDURATION] > iMaxLaneDuration) {
                iMaxLaneDuration = iDuration[lane][ii - CHECKDURATION];
                ActiveLaneCnt = lane;
            }
        }
        if (count == 955)
            printk(LEVEL_D, "count=%d,iMaxLaneDuration=%d, " \
                            "iii=%d,_iLastDetectioni=%d," \
                            "iii - _iLastDetectioni=%d," \
                            "_bState=%d,_bLastState=%d\n",
                            count, iMaxLaneDuration, 
                            iii+CALIBRATION_CYCLES, _iLastDetectioni + CALIBRATION_CYCLES, 
                            iii - _iLastDetectioni, 
                            _bState, _bLastState);
        if (iMaxLaneDuration > 16) {
            /**
             * _bLastState is 3 indicate it includes IN and OUT, so _bState can be ignore
             * _bState is 3 indicate need to drop 1 or 2, because it maybe repeat _bLastState
             * ignore it when _bState equal _bLastState and duration more than last detection
             */
            if (iMaxLaneDuration > iii - _iLastDetectioni) {
                if (_bLastState == _bState || _bLastState == 3) {
                    iPeopleCnt = _bState = 0;
                    printk(LEVEL_D, "MaxDuration,sample=%d,lane=%d,duration=%d\r\n",
                                     count, ActiveLaneCnt, iMaxLaneDuration);
                } else if (_bState == 3) {
                    if (_bLastState == 1)
                        _bState = 2;
                    else if (_bLastState == 2)
                        _bState = 1;
                }
            }
        }
    }

    /* save */
    if (_bState > 0) {
        _bLastState = _bState;
        _iLastDetectioni = iii;
    }
    _iiold = ii + CHECKDURATION + 1;
    _bState = 0;
    return iPeopleCnt;
}

static int Counting(unsigned int count,
                    struct people *peoples,
                    unsigned char people_max_count)
{
    ii = ii + 1;
    iii = iii + 1;
    if (ii >= KEEPPERIOD)
        HistoryShift();

    /* ultrasonic */
    us_analyze(count);    

    /* pir */
    pir_mean();
    pir_parse(count);
    /* postponed to 'ii + CHECKDURATION' report when found people */
    if (ii >= _iiold) {
        unsigned char lane;

        for (lane = 0; lane < _pir_lane; lane++) {
            if (bLane_State[lane][ii - CHECKDURATION] > 0)
                return pir_lane_analyze(count, peoples, people_max_count);
        }
    }
    return 0;
}

int counter_people_init(double t1, double t2, unsigned char board_number)
{
    unsigned char lane, i;

    /* must be bigger than 20, and less than thresh 1 */
    if (t1 < 20.0 || t2 < 20.0 || t2 >= t1)
        return -1;
    dThresh1 = t1;
    dThresh2 = t2;

    /* pir/us number */
    if (board_number > MAX_BOARD_NUMBER)
        return -1;
    _board_number = board_number;
    _pir_number = _board_number * 8;
    _pir_lane = _pir_number / 2;
    _us_number = _board_number * 8;
 
    /* pir */
    memset(_pir_sensors, 0.0, sizeof(_pir_sensors));
    memset(_pir_sums, 0.0, sizeof(_pir_sums));
    memset(_pir_means, 0.0, sizeof(_pir_means));
    memset(_pir_filter_idxs, 0x00, sizeof(_pir_filter_idxs));
    memset(_pir_filter_buffers, 0.0, sizeof(_pir_filter_buffers));
    /* ultrasonic */
    memset(_us_sensors, 0.0, sizeof(_us_sensors));    
    memset(_us_max_heights, 0, sizeof(_us_max_heights));    
    memset(_us_keep_counters, KEEP_HEIGHT_LIFE_CNT, sizeof(_us_keep_counters));    

    iii = 0;
    ii = 0;
    //_iiold = KEEPPERIOD - 20
    _iiold = CHECKDURATION;

    _bState = _bLastState = 0;
    /* lane */
    for (lane = 0; lane < LANES; lane++) {
        bFlag1[lane][0] = 0;
        bFlag1[lane][1] = 0;
        iHighDuration[lane][0] = 0;
        iHighDuration[lane][1] = 0;
        for (i = 0; i < KEEPPERIOD; i++) {
            bLane_State[lane][i] = 0;
            iDuration[lane][i] = 0;
            bLane_Tmax[lane][i] = 0;
        }
    }

    bMode = CALIBRATION_MODE;
    bCalibrationCnt = 0;
    return 0;
}

void counter_people_finalize(void)
{
}

int counter_people_process(const char *sample, 
                            unsigned char size,
                            unsigned int count,
                            struct people *peoples,
                            unsigned char people_max_count)
{
    unsigned char i, j, pi, ui;
    unsigned short value;
    unsigned char *data;

    /* multi board */
    for (i = pi = ui = 0; i < _board_number; i++) {
        data = (unsigned char *) (sample + 32 * i);
        for (j = 0; j < NUM_OF_SENSORS; j++) {
            /* [0,7]->PIR, [8,15]->Ultrasonic */
            value = data[j * 2] + (data[j * 2 + 1] << 8);
            if (j < NUM_OF_PIR_SENSORS) {
                _pir_sensors[pi] = 80 * (5 * value / 1024.0 - 1) - 18;
                if ((value & 0x0100) != 0x0100) {
                    /* invalid temperature, set to 23 */
                    _pir_sensors[pi] = 23;
                    printk(LEVEL_E, "process(%d,%d)=0x%x invalid\r\n", i,j,value);
                    return -1;
                }
                pi++;
            } else {
                _us_sensors[ui] = 2.54 * value / 2;
                if (_us_sensors[ui] > BARHEIGHT)
                    _us_sensors[ui] = BARHEIGHT;
                ui++;
            }
        }
    }

    return bMode == COUNTING_MODE ? 
            Counting(count, peoples, people_max_count) : Calibration();
}
