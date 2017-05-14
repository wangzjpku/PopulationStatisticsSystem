#ifndef __COMMON_H__
#define __COMMON_H__

#include <stdint.h>
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif

#define uint32_to_little_endian(val, buf) memmove(buf, (uint8_t *) &val, 4)
#define uint16_to_little_endian(val, buf) memmove(buf, (uint8_t *) &val, 2)

static void uint32_to_big_endian(uint32_t val, uint8_t buf[4]) {
    buf[0] = (val & 0xff000000) >> 24;
    buf[1] = (val & 0xff0000) >> 16;
    buf[2] = (val & 0xff00) >> 8;
    buf[3] = val & 0xff;
}

static void uint16_to_big_endian(uint16_t val, uint8_t buf[2]) {
    buf[0] = (val & 0xff00) >> 8;
    buf[1] = val & 0xff;
}

#ifdef __cplusplus
#endif
#endif
