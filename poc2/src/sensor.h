#ifndef __SENSOR_H__
#define __SENSOR_H__

#include "system.h"

#ifdef __cplusplus
extern "C" {
#endif

#define SAMPLE_SIZE                 32
#define TWO_BOARD_SAMPLE_SIZE       2 * SAMPLE_SIZE
#define THREE_BOARD_SAMPLE_SIZE     3 * SAMPLE_SIZE

#define pir_sample(s)               (s)
#define ultrasonic_sample(s)        ((s) + 16)

#define slave_left_sample(s)        (s)
#define master_sample(s)            ((s) + 32)
#define slave_right_sample(s)       ((s) + 64)

/* first ultrasonic's high byte */
#define SLAVE_US_COUNTER_OFFSET     17

/**
 * read slef's sensor sample, 8 PIR and 4 40K UltraSonic, call by slave left or slave right
 * @param  sample       sample data buffer
 * @param  max_size     sensor data length, POC2 must be 16*2=32
 * @param  type         MASTER_BOARD/SLAVE_LEFT_BOARD/SLAVE_RIGHT_BOARD
 * @return              0 is success, other is fail
 */
int sensor_read(uint8_t *sample, uint32_t max_size, enum board_type type);

/**
 * read all board's sensor sample, it depend on board_number, call by master
 * @param  sample       sample data buffer
 * @param  max_size     sample length, 16*2*3=96 when POC2
 * @param  number       if it is ONE_NUMBER, master MCU's data will set to 0~31 as sample's head
 * @return              0 is success, other is fail
 */
int sensor_read_all(uint8_t *sample, uint32_t max_size, enum board_number number);

#define sensor_compress_size()   (1 + (SAMPLE_SIZE >> 1))

int sensor_compress(uint8_t *sample, 
                    uint32_t sample_size, 
                    uint8_t *compressed_sample, 
                    uint32_t *compressed_size);

void sensor_uncompress(uint8_t *compressed_sample, 
                    uint32_t compressed_size, 
                    uint8_t *sample);

#ifdef __cplusplus
#endif
#endif
