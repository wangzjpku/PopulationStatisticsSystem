#ifndef __UART_H__
#define __UART_H__

#include "Driver\DrvUART.h"

#ifdef __cplusplus
extern "C" {
#endif

int uart_init(void);
void uart_finalze(void);

int uart_read(E_UART_PORT port, uint8_t *buffer, uint32_t size);
int uart_read_try(E_UART_PORT port, uint8_t *buffer, uint32_t size);
int uart_write(E_UART_PORT port, uint8_t *buffer, uint32_t size);
int uart_write_byte(E_UART_PORT port, uint8_t byte);
int uart_write_uint16(E_UART_PORT port, uint16_t integer);
uint32_t uart_read_oversize(E_UART_PORT port);

/**
 * write uint32_t, little endian
 * @param  port uart port
 * @param  integer value
 * @return         0 is success, other is fail
 */
int uart_write_uint32(E_UART_PORT port, uint32_t integer);

uint32_t *uart_get_overflow(void);

#ifdef __cplusplus
#endif
#endif
