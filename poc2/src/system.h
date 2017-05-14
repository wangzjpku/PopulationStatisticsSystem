#ifndef __SYSTEM_H__
#define __SYSTEM_H__

#include "Driver\DrvGPIO.h"

#ifdef __cplusplus
extern "C" {
#endif

enum {
    MODE_LIVE = 0,
    MODE_FILE,
    MODE_COLLECT,
    MODE_SLEEP,
};

#define work_mode_valid(wm)     (wm == MODE_LIVE \
                                || wm == MODE_FILE \
                                || wm == MODE_COLLECT)

#define threshold_valid(t1,t2)  (t1 > t2 \
                                && t1 > 20.0 && t1 < 36.0 \
                                && t2 > 20.0 && t2 < 36.0)

enum board_number {
    ONE_NUMBER = 0,
    TWO_LEFT_NUMBER,
    THREE_NUMBER,
};

enum board_type {
    MASTER_BOARD = 0,
    SLAVE_LEFT_BOARD,
    SLAVE_RIGHT_BOARD
};

struct PinSet
{ 
    E_DRVGPIO_PORT  Port;
    int32_t         Num;
};

void InitSystem(void);

void system_timer_start(void (*timer_callback)(void));
int8_t system_get_dip(void);

enum board_type system_get_board_type(int8_t dip);

#define system_is_master(dip)  (system_get_board_type(dip) == MASTER_BOARD)

#ifdef __cplusplus
#endif
#endif
