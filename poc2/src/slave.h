#ifndef __SLAVE_H__
#define __SLAVE_H__

#include "uart.h"
#include "system.h"

#ifdef __cplusplus
extern "C" {
#endif

#define slave_left  UART_PORT1
#define slave_right  UART_PORT2

/**
 * read slave board's sample data from UART, only master board can call.
 * @param  port         slave_left or slave_right
 * @param  sample       sample data buffer
 * @param  sample_size  sensor data length, POC2 must be 16*2=32
 * @return              0 is success, 
 *                      -1 is cannot read sample, 
 *                      -2 is max_sample_size less than real sample size
 */
int slave_read_sample(E_UART_PORT port, 
                        uint8_t *sample, 
                        uint32_t max_sample_size, 
                        uint32_t *sample_size);

/**
 * enable slave MCU to prepare Ultrasonic sample, only master board can call.
 * @param  port slave_left or slave_right
 * @return      0 is success, other is fail
 */
int slave_us_enable(E_UART_PORT port);

/**
 * disabe slave MCU to provide Ultrasonic sample, only master board can call.
 * @param  port slave_left or slave_right
 * @return      0 is success, other is fail
 */
int slave_us_disable(E_UART_PORT port);

/**
 * check board's us or else has been enabled
 * @param  type caller's board type, SLAVE_LEFT_BOARD or SLAVE_RIGHT_BOARD
 * @return      0 is not enabled, other is enabled
 */
int slave_us_is_enabled(enum board_type type);

#ifdef __cplusplus
#endif
#endif
