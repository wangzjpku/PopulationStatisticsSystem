#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "../counter_people.h"
#include "../log.h"
#include "counter_example.h"

static char _sample_filepath[256];
static uint16_t _sample_length = 52;

static uint8_t _sample[256+3];
static double _t1 = 21.54;
static double _t2 = 21.2;
static uint32_t _sample_count, _people_in_count, _people_out_count;

static void _usage(void)
{
    printf("Usage:\n");
    printf(" -f<sample file path>\n");
    printf("     must provide\n");
    printf(" -l<sample unit length>\n");
    printf("     52Bytes for POC1, 100Bytes for POC2(Winter Solstice)\n");
}

static int _parse_cmd_line(int argc, char *argv[])
{
    while ((argc > 1) && (argv[1][0] == '-')) {
        switch (argv[1][1]) {
        case 'f':
            strcpy(_sample_filepath, &argv[1][2]);
            break;
        case 'l':
            _sample_length = atoi(&argv[1][2]);
            if (_sample_length != 52 && _sample_length != 100)                
                return -1;
            break;
        default:
            return -1;
        }

        ++argv;
        --argc;
    }
    return _sample_filepath[0] == '\0' ? -1 : 0;
}

static void _do_counter(uint8_t *sample, uint32_t size)
{
    struct people peoples[2];
    int i;

    int people_count = counter_people_process((const char *) sample, 
                                                size, 
                                                ++_sample_count, 
                                                peoples, 
                                                sizeof(peoples)/sizeof(struct people));
    for (i = 0; i < people_count; i++) {
        if (peoples[i].direction == 1)
            _people_out_count++;
        else
            _people_in_count++;
        printk(LEVEL_D, "People,Sample=%d,%s=%d,height=%d\r\n", 
                _sample_count,
                peoples[i].direction == 1 ? "OUT" : "IN",
                peoples[i].direction == 1 ? _people_out_count : _people_in_count,
                peoples[i].height);
    }
}

static int _do(void)
{
    FILE *fd = fopen(_sample_filepath, "rb");

    if (fd) {
        size_t rsize;

        while (1) {
            rsize = fread(_sample, 1, _sample_length, fd);
            if (rsize == _sample_length) {
                _do_counter(_sample, _sample_length);
            } else
                break;
        }
        
        fclose(fd);
    } else {
        printf("cannot open file=%s\r\n", _sample_filepath);
        _usage();
        return -1;
    }
    return fd ? 0 : -1;
}

int main(int argc, char **argv)
{
#if 1
    _sample_length = 100;
    strcpy(_sample_filepath, "D:\\work\\apc\\demo_board_jp\\src\\vb_sample\\sample_0008\\bin\\Debug\\0122\\APC_IN_1_10_150_20150122_155547.bin");
#else
    if (_parse_cmd_line(argc, argv)) {
        _usage();
        return -1;
    }
#endif    
    counter_people_init(_t1, _t2, 2);
    return _do();
}
