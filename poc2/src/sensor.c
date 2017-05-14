#include <stdint.h>
#include <string.h>

#include "Driver\DrvADC.h"
#include "Driver\DrvGPIO.h"
#include "Driver\DrvSys.h"

#include "system.h"
#include "sensor.h"
#include "slave.h"

#define sensor_counter_get()  _read_counter
#define us_delay              (12.5 * 1000)

extern uint32_t ZTP115[8];
extern uint32_t EZ4B40[8];
extern uint32_t ZTPINDEX[8];
extern uint32_t EZ4INDEX[8];
extern struct PinSet pupin[45];
extern struct PinSet rxpin[8];
extern volatile uint16_t values[32];

typedef int (*read_cb)(uint8_t *sample);

static uint32_t _read_counter, _us_r_counter;

/* 0x0128 / 2.56 - 98 = 17.625 */
static uint8_t _pir_default[2] = {0x00, 0x01};
/* 0x0096 = 150 * 1.27 = 190.5 */
static uint8_t _us_default[2] = {0x96, 0x00};

/**************************************************
 *º¯ÊýÃû£º ADC_average                *
 *¹¦ÄÜ£º ADC×ª»»ÖµÈ¡ÖÐ¼äÖµº¯Êý                 *
 *³ö¿Ú²ÎÊý£ºÖÐ¼äÖµ          *
 *Èë¿Ú²ÎÊý£º×ª»»ÖµÊý×éµØÖ·        *
 *************************************************/
static int32_t ADC_average (int32_t *ADC_value)   
{
 int32_t value[9] = {0};
 uint8_t i,j,k;
 int32_t value_max;
 for (i=0; i<9; i++)
 {
  value[i] = *ADC_value++;                    //»ñÈ¡½«Òª×ª»»µÄÖµ
 }
 
 //Ã°ÅÝ·¨´ÓÐ¡µ½´óÅÅÁÐ
 for (j=0; j<9; j++)                     //Ò»¹²Òª±È½Ï9´Î
 {
  for (k=0; k<9-j; k++)                  //µÚÒ»´Î¹ýºó×î´óµÄÖµ·ÅÔÚÁË×î¸ßµÄÎ»ÖÃ
  {                                      //¾Í²»ÓÃÔÙ±È½Ï×î´óµÄÄÇ¸öÔªËØÁË
   if(k != 8)                            //·ÀÖ¹kÎª8Ê±£¬value[8]Óëvalue[9]µÄ±È½Ï£¬ÒòÎªºóÕßÒÑ¾­³¬³öÁËÊý×éµÄ·¶Î§
   {
    if (value[k] > value[k+1])          //ËùÒÔÃ¿´Î±È½ÏÍêÏÂÒ»´ÎÉÙ±È½ÏÒ»´Î
    {
     value_max = value[k];              //½«½Ï´óµÄÖµ±£´æÆðÀ´
     value[k] = value[k+1];             //½ÏÐ¡µÄÖµ¸³¸øµÍÎ»ÔªËØ
     value[k+1] = value_max;            //½Ï´óµÄÖµ¸³¸ø¸ßÎ»ÔªËØ
    }
   }
  }
 }
 return (value_max);
} 

/**************************************************
 *º¯ÊýÃû£º get_ADC_value                *
 *¹¦ÄÜ£º ADC»ñÈ¡¶ÔÓ¦Í¨µÀ×ª»»Öµº¯Êý             *
 *³ö¿Ú²ÎÊý£º9´ÎA/DºóµÃµ½µÄÖÐÖµ       *
 *Èë¿Ú²ÎÊý£ºADCÍ¨µÀºÅÂë          *
 *************************************************/
 //#if 0
static int32_t get_ADC_value_1(uint8_t ADC_channel_number)
{
 uint8_t i;
    int32_t ADC_calibration[9]={0};                                    //ADCÇóÖÐ¼äÖµÊý¾Ý´¢´æÊý×é
 for (i=0; i<9; i++)                                                   //¶ÁÈ¡9´ÎµÄÖµ
 {
  DrvADC_StartConvert();                                               //Æô¶¯ADC×ª»»
  while(!DrvADC_IsConversionDone())                                    //µÈ´ýADC×ª»»Íê³É
  {
  }
  ADC_calibration[i] = DrvADC_GetConversionData(ADC_channel_number);   //¶ÁÈ¡¶ÔÓ¦ADCÍ¨µÀµÄÖµ
  DrvADC_StopConvert(); 
  DrvSYS_Delay( 10 * 1000 );                                               //Í£Ö¹×ª»»
 }
 return (ADC_average(ADC_calibration));                                //½«È¡µ½µÄÖµÇóÈ¡ÖÐ¼äÖµºó·µ»Ø
}
//#else
static int32_t get_ADC_value(uint8_t ADC_channel_number)
{
 uint8_t i;
 int32_t ADC_calibration;                                    //ADCÇóÖÐ¼äÖµÊý¾Ý´¢´æÊý×é

  DrvADC_StartConvert();                                               //Æô¶¯ADC×ª»»
  while(!DrvADC_IsConversionDone())                                    //µÈ´ýADC×ª»»Íê³É
  {
  }
  ADC_calibration = DrvADC_GetConversionData(ADC_channel_number);   //¶ÁÈ¡¶ÔÓ¦ADCÍ¨µÀµÄÖµ
  DrvADC_StopConvert();                                                //Í£Ö¹×ª»»

 return (ADC_calibration);                                //½«È¡µ½µÄÖµÇóÈ¡ÖÐ¼äÖµºó·µ»Ø
}
//#endif

void SetADChannel( uint32_t channel )
{
    uint32_t dout_temp;
    //S0 S1 S2 S3   ==  C7 C6 C14 C15
    dout_temp = GPIOC->DOUT & 0xFFFF3F3F;
    dout_temp |=  ( ( ( channel  & 0x01 ) << 7 ) | ( ( (channel >> 1) & 0x1 ) << 6 ) | (((channel >> 2) & 0x03 ) << 14) );           

    GPIOC->DOUT = dout_temp;
}

uint32_t AnalongReadRF(uint32_t i)
{
    int32_t ADC_value, Last_ADC_value;  
    SetADChannel( ZTP115[i] );
    DrvSYS_Delay( 1 );
  ADC_value = get_ADC_value(0);                
    Last_ADC_value =  ( ADC_value /4  ); 
    return Last_ADC_value;
}

uint32_t AnalongReadEZ4(uint32_t i)
{
    int32_t ADC_value, Last_ADC_value;  
    SetADChannel( EZ4B40[i] );
    DrvSYS_Delay( 1000 );
  ADC_value = get_ADC_value(0); 
    Last_ADC_value = ( ADC_value /4  );  
    return Last_ADC_value;
        
}

static void _sensor_set_default(uint8_t *sample, uint8_t default_values[2])
{
    int8_t i;

    for (i = 0; i < 8; i++) {
        sample[i*2] = default_values[0];
        sample[i*2+1] = default_values[1];
    }
}

#define _pir_set_default(sample)  _sensor_set_default(sample, _pir_default)
#define _us_set_default(sample)   _sensor_set_default(sample, _us_default)

static void _set_default(uint8_t *sample)
{
    _pir_set_default(sample);
    _us_set_default(sample + 16);
}

static void pir_read(uint8_t *sample)
{
    uint8_t i;

    for (i = 0; i < 8; i++) {
        values[i] = AnalongReadRF(ZTPINDEX[i]);
        memmove(&sample[i*2], (const void *) &values[i], 2);
    }
}

static void ultrasonic_read_next(uint8_t *sample)
{
    uint8_t i, j;

    /* adjust j to 1, 2, 3, 4 */
    j = _us_r_counter % 8;
    if (j == 0)
        j = 8;

    /* read sensor i, equal 0, 1, 2, 3 */
    i = j - 1;		

    values[i+8] = AnalongReadEZ4(EZ4INDEX[i]);    
    /* enable sensor i */
    DrvGPIO_SetBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
    DrvSYS_Delay(0.1 * 1000);
    DrvGPIO_ClrBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
    /* FIXME:delay 12.5 maybe lead to PIR data error */
    DrvSYS_Delay(us_delay);

		
    /**
     * XXX:here read ultrasonic is safe, if move to after AnalongReadEZ4, 
     * sample variable is wrong. even if I set values to volatile but still so
     */
    for (i = 0; i < 8; i++) {
        memmove(&sample[i*2], (const void *) &values[i+8], 2);
    }
}

void sensor_counter_increment(void)
{
    ++_read_counter;
}

int sensor_read(uint8_t *sample, uint32_t max_size, enum board_type type)
{
    uint8_t us_read = 0;
    uint8_t *sl_sample = slave_left_sample(sample);

    if (max_size < SAMPLE_SIZE)
        return -1;
    sensor_counter_increment();

    pir_read(pir_sample(sl_sample));
    _us_set_default(ultrasonic_sample(sl_sample));
    /* enabled by Master? */
    if (slave_us_is_enabled(type)) {
        GPA_1 = GPC_11 = 0;
        if ((++_us_r_counter) <= 4)
            us_read = 1;
    } else {
        GPA_1 = GPC_11 = 1;
        _us_r_counter = 0;
    }
    if (us_read) {
        ultrasonic_read_next(ultrasonic_sample(sl_sample));
        /**
         * mark end of Ultrasonic sampling, Master will disable slave and 
         * reset sl_sample[SLAVE_US_COUNTER_OFFSET] to zero
         */
        sl_sample[SLAVE_US_COUNTER_OFFSET] = _us_r_counter;
    } else
        DrvSYS_Delay(us_delay);

    return 0;
}

static int _read_one_board(uint8_t *sample)
{
    pir_read(pir_sample(sample));
    /** 0xffffffff is not a multiple of 4 */
    if ((++_us_r_counter) > 8)
        _us_r_counter = 1;
    ultrasonic_read_next(ultrasonic_sample(sample));
    _set_default(master_sample(sample));
    _set_default(slave_right_sample(sample));
    return 0;
}

/**
 * select ultrasonic
 * [1,4]->slave left, [5,8]->master
 */
static int _read_two_left_board(uint8_t *sample)
{
    static int fail_counter;
    int rc;
    uint8_t *sl_sample = slave_left_sample(sample);
    uint8_t *master_sample = master_sample(sample);
    uint8_t slave_left_enabled = _us_r_counter < 5 ? 1 : 0;
    uint32_t sample_size = 0;
 
    /* trig */
    if (_us_r_counter < 5)
        slave_us_enable(slave_left);

    /* slave left */
    rc = slave_read_sample(slave_left, sl_sample, SAMPLE_SIZE, &sample_size);
    if (rc) {
        _set_default(sl_sample);
        /* jump to master */
        if (++fail_counter > 5 && _us_r_counter < 5)
            _us_r_counter = 5;
    } else {
        /* compress frame then borrow master_sample */
        if (sample_size == sensor_compress_size()) {
            sensor_uncompress(sl_sample, sample_size, master_sample);
            memmove(sl_sample, master_sample, SAMPLE_SIZE);
        }

        fail_counter = 0;
        if (slave_left_enabled) {
            /**
             * 4 indicate end of slave Ultrasonic sampling
             * 0 indicate slave left cannot enable Ultrasonic
             */
            if (sl_sample[SLAVE_US_COUNTER_OFFSET] == 4
                || sl_sample[SLAVE_US_COUNTER_OFFSET] == 0) {
                slave_us_disable(slave_left);
                /* next ultrasonic is master */
                _us_r_counter = 5;
            }
            /* reset to zero */
            sl_sample[SLAVE_US_COUNTER_OFFSET] = 0x00;
        }
    }

    /* master */
    pir_read(pir_sample(master_sample));
    if (slave_left_enabled) {
        _us_set_default(ultrasonic_sample(master_sample));
        DrvSYS_Delay(us_delay);
    } else {
        ultrasonic_read_next(ultrasonic_sample(master_sample));
        if ((++_us_r_counter) >= 9)
            _us_r_counter = 1;
    }
    /* slave right */
    _set_default(slave_right_sample(sample));
    return rc;
}

int sensor_read_all(uint8_t *sample, uint32_t max_size, enum board_number number)
{
    static read_cb callbacks[THREE_NUMBER - ONE_NUMBER + 1] = {
        _read_one_board,
        _read_two_left_board,
        _read_one_board,
    };

    if (max_size < THREE_BOARD_SAMPLE_SIZE)
        return -1;
    sensor_counter_increment();
    return callbacks[number - ONE_NUMBER](sample);
}

int sensor_compress(uint8_t *sample, 
                    uint32_t sample_size, 
                    uint8_t *compressed_sample, 
                    uint32_t *compressed_size)
{
    uint8_t i, j;
    uint8_t us_counter = sample[SLAVE_US_COUNTER_OFFSET];

    /* clear SLAVE_US_COUNTER_OFFSET */
    sample[SLAVE_US_COUNTER_OFFSET] = 0x00;

    /**
     * PIR's high byte is 0x01
     * Ultrasonic's high byte is 0x00
     */
    for (i = j = 0; i < sample_size; i+=2, j++) {
        if (i < (sample_size >> 1)) {
            if (sample[i+1] != 0x01)
                goto fail;
        } else {
            if (sample[i+1] != 0x00)
                goto fail;
        }
        compressed_sample[j] = sample[i];
    }
    compressed_sample[j++] = us_counter;
    *compressed_size = j;
    return 0;
fail:
    sample[SLAVE_US_COUNTER_OFFSET] = us_counter;
    return -1;
}

void sensor_uncompress(uint8_t *compressed_sample, 
                        uint32_t compressed_size, 
                        uint8_t *sample)
{
    uint8_t i;
    uint8_t us_counter = compressed_sample[compressed_size-1];

    /* skip last byte->us counter */
    compressed_size--;

    /**
     * PIR's high byte is 0x01
     * Ultrasonic's high byte is 0x00
     */
    for (i = 0; i < compressed_size; i++) {
        sample[i*2] = compressed_sample[i];
        sample[i*2+1] = i < (compressed_size >> 1) ? 0x01 : 0x00;
    }
    sample[SLAVE_US_COUNTER_OFFSET] = us_counter;
}
