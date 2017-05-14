#ifndef __LED_H__
#define __LED_H__

#include "Driver\DrvGPIO.h"

#ifdef __cplusplus
extern "C" {
#endif

static void led_toggle(void)
{
    static uint8_t _led_on;

    _led_on ^= 1;
    // GPC_11 = GPC_8 = GPA_2 = GPA_1 = _led_on;
}

#ifdef __cplusplus
#endif
#endif
