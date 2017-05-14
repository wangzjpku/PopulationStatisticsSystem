#ifndef __LOG_H__
#define __LOG_H__

#ifdef __cplusplus
extern "C" {
#endif

#define LEVEL_D "d"
#define LEVEL_W "w"
#define LEVEL_E "e"

#ifdef NUC140
#include "Driver\DrvTIMER.h"
#define NOW()   DrvTIMER_GetIntTicks(E_TMR0)
#else
#include <time.h>
#define NOW()	time(NULL)
#endif

/**
 * printf log to UART1, only master board program can call it.
 * warring, internal buffer size is 251Bytes, your string must be less than its
 * @param  level    debug or warring or error
 * @param  fmt      string format
 * @param  ...      variable-length parameters
 */
void printk(const char *level, const char *fmt, ...);

#define printe(fmt, ...)  {             \
    printk(LEVEL_E, fmt, __VA_ARGS__);  \
}

#ifdef __cplusplus
#endif
#endif
