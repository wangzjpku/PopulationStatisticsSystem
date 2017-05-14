#include <stdio.h>
#include "channel.h"
#include "log.h"

void channel_filtering(unsigned char *channels, 
                        unsigned int size,
                        struct active_channel *atv_channels,
                        unsigned int *atv_channels_size,
                        unsigned char filtered_width_threshold)
{
    unsigned int i, j, k, left, right;

    i = j = 0;
    while (i < size) {
        if (channels[i] == (unsigned char) DIRECTION_IN 
            || channels[i] == (unsigned char) DIRECTION_OUT) {
            atv_channels[j].direction = (enum people_direction) channels[i];
            atv_channels[j].begin = left = right = i;
            for (k = i + 1; k < size; k++) {
                if (channels[k] == channels[i]/* || channels[k] == 0*/)
                    right++;
                else
                    break;
            }
            atv_channels[j].width = right - left + 1;
            if (atv_channels[j].width >= filtered_width_threshold) {
                i = right + 1;
                j++;
            } else
                i++;
        } else
            i++;
    }
    *atv_channels_size = j;
}

static unsigned int _people_stuff(struct people *peoples,
                                    unsigned int number,
                                    struct active_channel *channel)
{
    unsigned char i, width;

    width = channel->width / number;
    if ((channel->width % number) != 0)
        width++;

    for (i = 0; i < number; i++) {
        peoples[i].direction = channel->direction;
        peoples[i].height = channel->height;
        /* split channel to people, adjust begin and width */
        peoples[i].begin = channel->begin + i * width;
        if (i == number - 1 && (channel->width % number) != 0)
            peoples[i].width = width - 1;
        else
            peoples[i].width = width;
    }
    return number;
}

unsigned int channel_analyze(struct active_channel *channels,
                            unsigned int size,
                            unsigned char one_people_width_threshold,
                            unsigned char two_people_width_threshold,
                            struct people *peoples,
                            unsigned char people_max_count)
{
    unsigned int i, j;

    for (i = j = 0; i < size && j < people_max_count; i++) {        
        if (channels[i].width <= one_people_width_threshold) {
            j += _people_stuff(&peoples[j], 1, &channels[i]);
        } else if (channels[i].width > two_people_width_threshold) {
            j += _people_stuff(&peoples[j], 2, &channels[i]);
        } else {
            /** 
             * FIXME: how to discern 1 or 2 people?
             * temperature variance upon T1?
             * current, I set to 1 people
             */
            j += _people_stuff(&peoples[j], 1, &channels[i]);
        }
    }
    return j;
}
