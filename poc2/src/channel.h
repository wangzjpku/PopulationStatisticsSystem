#ifndef __CHANNEL_H__
#define __CHANNEL_H__

#include "counter_people.h"

#define active_channel people

#ifdef __cplusplus
extern "C" {
#endif

void channel_filtering(unsigned char *channels, 
                        unsigned int size,
                        struct active_channel *atv_channels,
                        unsigned int *atv_channels_size,
                        unsigned char filtered_width_threshold);

unsigned int channel_analyze(struct active_channel *channels,
                            unsigned int size,
                            unsigned char one_people_width_threshold,
                            unsigned char two_people_width_threshold,
                            struct people *peoples,
                            unsigned char people_max_count);

#ifdef __cplusplus
#endif
#endif
