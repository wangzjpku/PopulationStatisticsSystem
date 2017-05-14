#include "NUC1xx.h"
#if 0
#include "Driver\DrvGPIO.h"
#include "Driver\DrvSYS.h"
#endif

#include "uart.h"
#include "common.h"

#define RXBUF_MIN_SIZE 512
#define RXBUF_MAX_SIZE 2048

static volatile uint8_t _min_buffer[RXBUF_MIN_SIZE];
static volatile uint8_t _max_buffer[2][RXBUF_MAX_SIZE];
static volatile uint16_t _buffer_size[3];

static volatile uint8_t *comRbuf[3];
/* Available receiving bytes */
static volatile uint16_t comRbytes[3];
static volatile uint16_t comRhead[3];
static volatile uint16_t comRtail[3];

static UART_T *address_array[] = {UART0, UART1, UART2};
static volatile uint32_t _overflow_counter[3];

static int32_t _get_idx(E_UART_PORT port, UART_T **address)
{
    int32_t idx;

    if (port == UART_PORT0)
        idx = 0;
    else if (port == UART_PORT1)
        idx = 1;
    else
        idx = 2;
    *address = address_array[idx];
    return idx;
}

static void _UART_INT_HANDLE(E_UART_PORT port, uint32_t u32IntStatus)
{
    uint8_t bInChar[1] = {0xFF};
    UART_T *address;
    int32_t idx;

    if (u32IntStatus & DRVUART_RDAINT) {
        idx = _get_idx(port, &address);

        /* Get all the input characters */
        while (address->ISR.RDA_IF == 1) {
            /* Get the character from UART Buffer */
            if (DrvUART_Read(port, bInChar, 1) == E_SUCCESS) {
                /* Check if buffer full */
                if(comRbytes[idx] < _buffer_size[idx]) {
                    /* Enqueue the character */
                    comRbuf[idx][comRtail[idx]] = bInChar[0];
                    comRtail[idx] = (comRtail[idx] == (_buffer_size[idx]-1)) ? 0 : (comRtail[idx]+1);
                    comRbytes[idx]++;
                } else {
                    /* overflow */
                    comRhead[idx] = 0;
                    comRtail[idx] = 0;
                    comRbytes[idx] = 0;
                    _overflow_counter[idx]++;
                }
            }
        }
    }
}

static void _UART0_INT_HANDLE(uint32_t u32IntStatus)
{
    _UART_INT_HANDLE(UART_PORT0, u32IntStatus);
}

static void _UART1_INT_HANDLE(uint32_t u32IntStatus)
{
    _UART_INT_HANDLE(UART_PORT1, u32IntStatus);
}

static void _UART2_INT_HANDLE(uint32_t u32IntStatus)
{
    _UART_INT_HANDLE(UART_PORT2, u32IntStatus);
}

int uart_init(void)
{
    int i;

    /* initialize, ring buffer */
    comRbuf[0] = _min_buffer;
    comRbuf[1] = (volatile uint8_t *) &_max_buffer[0];
    comRbuf[2] = (volatile uint8_t *) &_max_buffer[1];
    _buffer_size[0] = RXBUF_MIN_SIZE;
    _buffer_size[1] = RXBUF_MAX_SIZE;
    _buffer_size[2] = RXBUF_MAX_SIZE;
    for (i = 0; i < sizeof(comRhead) / sizeof(uint16_t); i++) {        
        comRhead[i] = 0;
        comRtail[i] = 0;
        comRbytes[i] = 0;        
        _overflow_counter[i] = 0;
    }

    /* Enable Interrupt and install the call back function */
    DrvUART_EnableInt(UART_PORT0, 
                    (DRVUART_RLSINT | DRVUART_RDAINT), 
                    _UART0_INT_HANDLE);
    DrvUART_EnableInt(UART_PORT1, 
                    (DRVUART_RLSINT | DRVUART_RDAINT), 
                    _UART1_INT_HANDLE);
    DrvUART_EnableInt(UART_PORT2, 
                    (DRVUART_RLSINT | DRVUART_RDAINT), 
                    _UART2_INT_HANDLE);
    return 0;
}	

void uart_finalze(void)
{
}

static int _do_read(E_UART_PORT port, uint8_t *buffer, uint32_t size, int8_t try)
{
    UART_T *address;
    int32_t port_idx = _get_idx(port, &address);

    /* enough read */
    if (comRbytes[port_idx] >= size) {
        uint16_t read_idx = comRhead[port_idx];

        while (size-- > 0) {
            *buffer++ = comRbuf[port_idx][read_idx];
            read_idx = (read_idx == (_buffer_size[port_idx]-1)) ? 0 : (read_idx+1);
            if (try == 0) {
                comRhead[port_idx] = read_idx;
                comRbytes[port_idx]--;
            }
        }
        return 0;
    }
    return -1;
}

int uart_read(E_UART_PORT port, uint8_t *buffer, uint32_t size)
{
    return _do_read(port, buffer, size, 0);
}

int uart_read_try(E_UART_PORT port, uint8_t *buffer, uint32_t size)
{
    return _do_read(port, buffer, size, 1);
}

uint32_t uart_read_oversize(E_UART_PORT port)
{
    UART_T *address;
    int32_t port_idx = _get_idx(port, &address);

    return comRbytes[port_idx];
}

int uart_write(E_UART_PORT port, uint8_t *pu8TxBuf, uint32_t u32WriteBytes)
{
    return DrvUART_Write(port, pu8TxBuf, u32WriteBytes) == E_SUCCESS ? 0 : -1;
}

int uart_write_byte(E_UART_PORT port, uint8_t byte)
{
    uint8_t buffer[1];
    
    buffer[0] = byte;
    return uart_write(port, buffer, 1);
}

int uart_write_uint16(E_UART_PORT port, uint16_t integer)
{
    uint8_t buffer[2];

    uint16_to_big_endian(integer, buffer);
    return uart_write(port, buffer, sizeof(buffer));
}

int uart_write_uint32(E_UART_PORT port, uint32_t integer)
{
    uint8_t buffer[4];

    uint32_to_little_endian(integer, buffer);
    return uart_write(port, buffer, sizeof(buffer));
}

uint32_t *uart_get_overflow(void)
{
    return (uint32_t *) &_overflow_counter[0];
}
