#ifndef __COUNTER_PEOPLE_H__
#define __COUNTER_PEOPLE_H__

#ifdef __cplusplus
extern "C" {
#endif

enum people_direction {
    DIRECTION_NONE = 0,
    DIRECTION_OUT = 1,
    DIRECTION_IN = 2,
    DIRECTION_OPPOSITE = 3,
};

struct people {
	enum people_direction direction;
    unsigned char height;
    unsigned char begin, width;
};

int counter_people_init(double t1, double t2, unsigned char board_number);
void counter_people_finalize(void);

/**
 * support single MCU, max output 2 people
 * @param [sample] sample data buffer
 * @param [size] sample size, 100 bytes, 96 sample + 4 timestamp
 * @param [count] sample sequence index
 * @param [peoples] counting result
 * @param [people_max_count] peoples's element count, this case must be equal 2
 * @return -1:fail, 0:success but no people, >0:people count, max value equal [people_max_count]
 */
int counter_people_process(const char *sample, 
                            unsigned char size,
                            unsigned int count,
                            struct people *peoples,
                            unsigned char people_max_count);

#ifdef __cplusplus
#endif
#endif
