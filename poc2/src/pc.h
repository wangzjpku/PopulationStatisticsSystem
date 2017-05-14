#ifndef __PC_H__
#define __PC_H__

#include "uart.h"
#include "counter_people.h"
#include "system.h"

#ifdef __cplusplus
extern "C" {
#endif

/**
 * PC protocol define
 * 
 * frame start code 2Bytes, little endian
 * frame length     1Bytes
 * frame body       {
 *     for (frame length)
 *         1Bytes
 * }
 * frame end code   1Bytes
 */

#define PC_FRAME_MAX_LENGTH                 (2+1+255+1)

/* sample frame, board to pc and pc to board */
#define PC_SAMPLE_FRAME_STARTCODE           0x6868

/* sample frame ack, board to pc when receive a sample frame from pc */
#define PC_SAMPLE_ACK_FRAME_STARTCODE       0x6869

/* sample compress frame, slave to master, 163, 0xa3 */
#define PC_SAMPLE_COMMPRESS_FRAME_STARTCODE 0xa3a3

/* command frame, pc to board, PI, 31415 */
#define PC_COMMAND_FRAME_STARTCODE          0x7ab7

/* count people frame, board to pc, E, 27182 */
#define PC_PEOPLE_FRAME_STARTCODE           0x6a2e

/* log frame, board to pc, Golden Ratio, 16180 */
#define PC_LOG_FRAME_STARTCODE              0x3f34

/* handshake frame, board to pc and pc to board */
#define PC_HANDSHAKE_FRAME_STARTCODE        0x0815

#define PC_FRAME_ENDCODE                    0x00

#define pc_is_sample_frame(buffer)      \
        ((buffer[0] << 8) + buffer[1] == PC_SAMPLE_FRAME_STARTCODE)

#define pc_is_sample_compress_frame(buffer)      \
        ((buffer[0] << 8) + buffer[1] == PC_SAMPLE_COMMPRESS_FRAME_STARTCODE)

#define pc_is_cmd_frame(buffer)         \
        ((buffer[0] << 8) + buffer[1] == PC_COMMAND_FRAME_STARTCODE)

#define pc_is_handshake_frame(buffer)   \
        ((buffer[0] << 8) + buffer[1] == PC_HANDSHAKE_FRAME_STARTCODE)

#define PC_UART_PORT                    UART_PORT0

#define pc_read(buffer, size)           uart_read(PC_UART_PORT, buffer, size)
#define pc_read_try(buffer, size)       uart_read_try(PC_UART_PORT, buffer, size)
#define pc_read_oversize()              uart_read_oversize(PC_UART_PORT)
#define pc_write(buffer, size)          uart_write(PC_UART_PORT, buffer, size);
#define pc_write_byte(byte)             uart_write_byte(PC_UART_PORT, byte);
#define pc_write_uint16(integer)        uart_write_uint16(PC_UART_PORT, integer);
#define pc_write_uint32(integer)        uart_write_uint32(PC_UART_PORT, integer);

int pc_read_frame(uint8_t *buffer, uint32_t max_size);
void pc_write_sample_frame(uint8_t *sample, uint8_t size);
void pc_write_sample_ack_frame(uint32_t timestamp);
void pc_write_people_frame(uint32_t count, struct people *people);
void pc_write_log_frame(uint8_t *buffer, uint8_t size);
void pc_write_handshake_frame(enum board_number board_number, 
                            int32_t master_mode, 
                            double t1, 
                            double t2);

/**
 * command example
 * "number=1,mode=1,t1=2298,t2=2200"
 * warring:no allow space and tab
 */
void pc_parse_command(uint8_t *command, 
                    uint8_t length,
                    enum board_number *board_number,
                    int32_t *master_mode,
                    double *t1,
                    double *t2);


void pc_uart_test(void);

#ifdef __cplusplus
#endif
#endif
