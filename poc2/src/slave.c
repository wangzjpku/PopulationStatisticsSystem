#include <stdint.h>
#include <string.h>

#include "pc.h"
#include "uart.h"
#include "slave.h"

#include "Driver\DrvGPIO.h"

int slave_read_sample(E_UART_PORT port, 
                        uint8_t *sample, 
                        uint32_t max_sample_size, 
                        uint32_t *sample_size)
{
    for (;;) {
        if (uart_read_try(port, sample, 3) == 0) {
            if (pc_is_sample_frame(sample) 
                || pc_is_sample_compress_frame(sample)) {
                if (uart_read_oversize(port) >= 3 + sample[2] + 1) {
                    /* skipe start code and length */
                    uart_read(port, sample, 3);
                    *sample_size = (uint32_t) sample[2];
                    uart_read(port, sample, *sample_size + 1);
                    return sample[*sample_size] == PC_FRAME_ENDCODE ? 0 : -1;
                } else
                    break;
            } else
                uart_read(port, sample, 1);
        } else
            break;
    }
    return -1;
}

int slave_us_enable(E_UART_PORT port)
{
    if (port == slave_left) {
        GPA_8 = 1;
        return 0;
        //return DrvGPIO_SetBit(E_GPA, 8);
    } else
        ;
    return -1;
}

int slave_us_disable(E_UART_PORT port)
{
    if (port == slave_left) {
        GPA_8 = 0;
        return 0;
        //return DrvGPIO_ClrBit(E_GPA, 8);
    } else
        ;
    return -1;
}

int slave_us_is_enabled(enum board_type type)
{
    if (type == SLAVE_LEFT_BOARD) {
        return GPA_11 == 1 ? 1 : 0;
        // return DrvGPIO_GetBit(E_GPA, 11) == 1;
    } else if (type == SLAVE_RIGHT_BOARD) {
        ;
    }
    return 0;
}
