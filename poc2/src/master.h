#ifndef __MASTER_H__
#define __MASTER_H__

#include "pc.h"
#include "uart.h"

#ifdef __cplusplus
extern "C" {
#endif

#define master_write(buffer, size, start_code) {            \
    uart_write_uint16(UART_PORT2, start_code);   \
    uart_write_byte(UART_PORT2, size);  \
    uart_write(UART_PORT2, buffer, size);   \
    uart_write_byte(UART_PORT2, PC_FRAME_ENDCODE);  \
}

#ifdef __cplusplus
#endif
#endif
