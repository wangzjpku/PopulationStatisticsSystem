#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "counter_people.h"
#include "common.h"
#include "log.h"
#include "master.h"
#include "sensor.h"
#include "slave.h"
#include "system.h"
#include "pc.h"
#include "led.h"

#include "DrvSYS.h"
#include "DrvTimer.h"

#define MAX_PEOPLE_COUNT    6

static const char *_version = "git develop branch, commit:7baaf271";
static uint8_t _sample[PC_FRAME_MAX_LENGTH];
static uint32_t _sample_count, _people_in_count, _people_out_count;
static int8_t _dip;
static struct people _peoples[MAX_PEOPLE_COUNT];
static uint8_t _people_max_count = 2;

/* PC can set them to Master through command frame */
static enum board_number _board_number = TWO_LEFT_NUMBER;
static int32_t _work_mode = MODE_LIVE;
static double _t1 = 21.54;
static double _t2 = 21.2;

static void _initialize(enum board_number board_number,
                        int32_t work_mode, 
                        double t1,
                        double t2)
{
    _board_number = board_number;
    _work_mode = work_mode;
    _t1 = t1;
    _t2 = t2;

    _sample_count = _people_in_count = _people_out_count = 0;
    _people_max_count = (_board_number == THREE_NUMBER) ? 3 : 2;
    _dip = system_get_dip();
    uart_init();
    counter_people_init(_t1, _t2, 1 + ((unsigned char) _board_number));
}

static void _do_slave(uint32_t timestamp)
{
    uint32_t compressed_size = 0;
    int8_t use_compress = 0;

    /* read self's sample and send to master */
    sensor_read(_sample, SAMPLE_SIZE, system_get_board_type(_dip));
    if (use_compress && sensor_compress(_sample, 
                                        SAMPLE_SIZE, 
                                        _sample + SAMPLE_SIZE, 
                                        &compressed_size) == 0) {
        master_write(_sample + SAMPLE_SIZE, 
                    compressed_size, 
                    PC_SAMPLE_COMMPRESS_FRAME_STARTCODE);
    } else {
        master_write(_sample, SAMPLE_SIZE, PC_SAMPLE_FRAME_STARTCODE);
    }
}

static int _do_pc_frame(uint8_t *sample, uint32_t max_size)
{
    int rc = pc_read_frame(sample, max_size);
    if (rc)
        return rc;

    if (pc_is_cmd_frame(sample)) {
        enum board_number bnumber = _board_number;
        int32_t work_mode = _work_mode;
        double t1 = _t1, t2 = _t2;

        printk(LEVEL_D, "pc, cmd='%s'\r\n", sample+3);
        pc_parse_command(sample+3, sample[2]+1, &bnumber, &work_mode, &t1, &t2);
        DrvSYS_Delay(5 * 1000);
        printk(LEVEL_D, "pc, board_number=%D, mode=%d, t1=%f, t2=%f\r\n", 
                        bnumber, work_mode, t1, t2);

        if (work_mode_valid(work_mode) && threshold_valid(t1, t2)) {
            _initialize(bnumber, work_mode, t1, t2);
        }
    } else if (pc_is_handshake_frame(sample))
        pc_write_handshake_frame(_board_number, _work_mode, _t1, _t2);
    return rc;
}

static int _do_counter(uint8_t *sample, uint32_t size, uint32_t timestamp)
{
    int i;
    int people_count = counter_people_process((const char *) sample, 
                                                size, 
                                                ++_sample_count, 
                                                _peoples, 
                                                _people_max_count);
    if (_sample_count % 1000 == 0) {
        uint32_t *uart_overflow_counters = uart_get_overflow();
        printk(LEVEL_D, "counting=%d, timestamp=%d, " \
                        "uart_overflow_counters=%d,%d,%d,version=%s\r\n", 
                _sample_count, timestamp, 
                uart_overflow_counters[0], 
                uart_overflow_counters[1],
                uart_overflow_counters[2],
                _version);
    }
    for (i = 0; i < people_count; i++) {
        pc_write_people_frame(_sample_count, &_peoples[i]);

        if (_peoples[i].direction == 1)
            _people_out_count++;
        else
            _people_in_count++;
        printk(LEVEL_D, "People,Sample=%d,%s=%d,height=%d,begin=%d,width=%d\r\n", 
                _sample_count,
                _peoples[i].direction == 1 ? "OUT" : "IN",
                _peoples[i].direction == 1 ? _people_out_count : _people_in_count,
                _peoples[i].height,
                _peoples[i].begin, 
                _peoples[i].width);
    }
    return people_count;
}

static void _do_master(uint32_t timestamp)
{
    /* read pc' frame, parse it when it is a command frame */
    int rc = _do_pc_frame(_sample, sizeof(_sample));

    if (_work_mode == MODE_LIVE) {
        sensor_read_all(_sample, THREE_BOARD_SAMPLE_SIZE, _board_number);
        uint32_to_little_endian(timestamp, &_sample[THREE_BOARD_SAMPLE_SIZE]);
        _do_counter(_sample, THREE_BOARD_SAMPLE_SIZE + 4, timestamp);
        pc_write_sample_frame(_sample, THREE_BOARD_SAMPLE_SIZE + 4);
    } else if (_work_mode == MODE_FILE) {
        if (rc == 0 && pc_is_sample_frame(_sample)) {
            if (_do_counter(&_sample[3], _sample[2], timestamp) <= 0) {
                DrvSYS_Delay(8 * 1000);
                /* ack frame */
                pc_write_sample_ack_frame(timestamp);
            } else
                DrvSYS_Delay(1 * 1000);
        }
    } else if (_work_mode == MODE_COLLECT) {
        sensor_read_all(_sample, THREE_BOARD_SAMPLE_SIZE, _board_number);
        uint32_to_little_endian(timestamp, &_sample[THREE_BOARD_SAMPLE_SIZE]);
        pc_write_sample_frame(_sample, THREE_BOARD_SAMPLE_SIZE + 4);
    } else {
        printk(LEVEL_E, "system mode=%d unknown\r\n", _work_mode);
        DrvSYS_Delay(1000 * 1000);
    }
}

static void _do(void)
{
    static uint32_t now, last;

    now = DrvTIMER_GetIntTicks(E_TMR0);
#if 1
    if (_board_number == ONE_NUMBER || system_is_master(_dip))
        _do_master(now);
    else {
        static uint32_t begin, end;

        begin = DrvTIMER_GetIntTicks(E_TMR0);
        _do_slave(now);
        end = DrvTIMER_GetIntTicks(E_TMR0);
        if ((end - begin) < 22)
            DrvSYS_Delay((22 - (end - begin)) * 1000);
    }
#else
    pc_uart_test();
#endif

    if (now - last >= 100) {
        last = now;
        led_toggle();
    }
}

int main(void)
{
    InitSystem();

    _initialize(_board_number, _work_mode, _t1, _t2);
    while(1) {
        _do();
    }
}
