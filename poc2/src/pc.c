#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "pc.h"

int pc_read_frame(uint8_t *sample, uint32_t max_size)
{
    if (pc_read_try(sample, 3) == 0) {
        if (pc_is_sample_frame(sample)
            || pc_is_cmd_frame(sample)
            || pc_is_handshake_frame(sample)) {            
            //if (pc_read_try(sample, 3 + sample[2] + 1) == 0) {
            if (pc_read_oversize() >= 3 + sample[2] + 1) {
                pc_read(sample, 3 + sample[2] + 1);
                return sample[3+sample[2]] == PC_FRAME_ENDCODE ? 0 : -1;
            }
        } else
            pc_read(sample, 1);
    }
    return -1;
}

void pc_write_sample_frame(uint8_t *sample, uint8_t size)
{
    pc_write_uint16(PC_SAMPLE_FRAME_STARTCODE);
    pc_write_byte(size);
    pc_write(sample, size);
    pc_write_byte(PC_FRAME_ENDCODE);
}

void pc_write_sample_ack_frame(uint32_t timestamp)
{
    pc_write_uint16(PC_SAMPLE_ACK_FRAME_STARTCODE);
    pc_write_byte(sizeof(timestamp));
    pc_write_uint32(timestamp);
    pc_write_byte(PC_FRAME_ENDCODE);
}

void pc_write_people_frame(uint32_t count, struct people *people)
{
    pc_write_uint16(PC_PEOPLE_FRAME_STARTCODE);
    pc_write_byte(6);
    pc_write_uint32(count);
    pc_write_byte(people->direction);
    pc_write_byte(people->height);
    pc_write_byte(PC_FRAME_ENDCODE);
}

void pc_write_log_frame(uint8_t *buffer, uint8_t size)
{
    pc_write_uint16(PC_LOG_FRAME_STARTCODE);
    pc_write_byte(size);
    pc_write(buffer, size);
    pc_write_byte(PC_FRAME_ENDCODE);
}

void pc_write_handshake_frame(enum board_number board_number, 
                            int32_t master_mode, 
                            double t1, 
                            double t2)
{
    uint8_t buffer[64];

    int size = sprintf((char *) buffer, "number=%d,mode=%d,t1=%f,t2=%f", 
                        (int32_t) board_number, master_mode, t1*100.0, t2*100.0);

    pc_write_uint16(PC_HANDSHAKE_FRAME_STARTCODE);
    pc_write_byte((uint8_t) size);
    pc_write(buffer, (uint32_t) size);
    pc_write_byte(PC_FRAME_ENDCODE);
}

void pc_parse_command(uint8_t *command, 
                    uint8_t length,
                    enum board_number *board_number,
                    int32_t *master_mode,
                    double *t1,
                    double *t2)
{
    uint8_t i, k0, k1, v0, v1;
    char *key, *value;

    for (i = k0 = 0; i < length; i++) {
        switch (command[i]) {
        case '=':
            k1 = i-1;
            v0 = i+1;
            break;
        case ',':
        case PC_FRAME_ENDCODE:
            v1 = i-1;
            
            command[k1+1] = '\0';
            command[v1+1] = '\0';
            key = (char *) (command+k0);
            value = (char *) (command+v0);

            if (strcmp(key, "t1") == 0)
                *t1 = atoi(value) / 100.0;
            else if (strcmp(key, "t2") == 0)
                *t2 = atoi(value) / 100.0;
            else if (strcmp(key, "number") == 0)
                *board_number = (enum board_number) (atoi(value));
            else if (strcmp(key, "mode") == 0)
                *master_mode = atoi(value);

            k0 = i+1;
            break;
        default:
            break;
        }
    }
}

void pc_uart_test(void)
{
    uint8_t buffer[1];

    if (pc_read(buffer, 1) == 0)
        pc_write(buffer, 1);
}
