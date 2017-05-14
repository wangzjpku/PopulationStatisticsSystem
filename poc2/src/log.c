#include <stdarg.h>
#include <stdio.h>
#include <string.h>

#include "log.h"

#ifdef NUC140
#include "pc.h"
#endif

#define BUFFER_SIZE 255
static char buffer[BUFFER_SIZE];

void printk(const char *level, const char *fmt, ...) {
    va_list ap;
    int nowlen, len;

    nowlen = sprintf(buffer, "%d>", (int) NOW());

    if (*level == 'd')
        memmove(buffer + nowlen, "dbg>", 4);
    else if (*level == 'e')
        memmove(buffer + nowlen, "err>", 4);
    else
        memmove(buffer + nowlen, "war>", 4);

    va_start(ap, fmt);
    len = vsnprintf(buffer + nowlen + 4, BUFFER_SIZE - nowlen - 4, fmt, ap);
    va_end(ap);

    if (len > 0) {
#ifdef NUC140
        pc_write_log_frame((uint8_t *) buffer, (uint8_t) (len + nowlen + 4));
#else
        printf("%s", buffer);
#endif
    }
}
