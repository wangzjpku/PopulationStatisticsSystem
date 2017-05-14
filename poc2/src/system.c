/*---------------------------------------------------------------------------------------------------------*/
/*                                                                                                         */
/* Copyright(c) 2009 Nuvoton Technology Corp. All rights reserved.                                         */
/*                                                                                                         */
/*---------------------------------------------------------------------------------------------------------*/
#include <stdio.h>
/*---------------------------------------------------------------------------------------------------------*/
/* MAIN function                                                                                          */
/*---------------------------------------------------------------------------------------------------------*/    

//---------------Include files-------------------------//
#include "NUC1xx.h"
#include "Driver\DrvSYS.h"
#include "Driver\DrvGPIO.h"
#include "Driver\DrvUART.h"
#include "Driver\DrvTIMER.h"
#include "Driver\DrvPDMA.h"
#include "Driver\DrvUSB.h"
#include "Driver\DrvPWM.h"
#include "Driver\DrvADC.h"
#include "Driver\DrvPDMA.h"

#include "system.h"

//---------------Defines-------------------------//
#define REMOTE_DEVICE_ID        0x12

#define TIMER0_FREQ         (1 * 1000)  //2500   1S ÖÐ¶Ï´¥·¢´ÎÊý
#define UART_BAUD          115200  

#define PWM_Frequence       1000
#define UART0_DMA_TX_MAX    300


//---------------Type define-------------------------//
typedef struct
{
    uint8_t txBuf[2][UART0_DMA_TX_MAX+4];
    uint8_t txSel;
    
    uint8_t index;
    uint8_t len;
} uartDMACfg_t;

uartDMACfg_t  midi_buf;
uint8_t  pdma_write_protect;

//---------------Variable-------------------------//
uint8_t  buffer[4] = {0X11,0x12,0x13,0x14};
uint8_t C_GPD = 0;
volatile uint16_t values[32];
uint32_t count = 0;

struct PinSet pupin[45]={
// Input GPIO  
{E_GPA,0},{E_GPA,3},{E_GPA,4},{E_GPA,6},{E_GPA,8},{E_GPA,9},{E_GPA,10},{E_GPA,11},{E_GPA,12},{E_GPA,13},{E_GPA,15},
{E_GPB,0},{E_GPB,1},{E_GPB,2},{E_GPB,4},{E_GPB,5},{E_GPB,6},{E_GPB,8},{E_GPB,13},{E_GPB,15},
{E_GPC,0},{E_GPC,1},{E_GPC,2},{E_GPC,3},{E_GPC,10},
{E_GPD,6},{E_GPD,7},{E_GPD,14},{E_GPD,15},

// Output GPIO
{E_GPA,1},{E_GPA,2},{E_GPC,8},{E_GPC,11},   //LED
{E_GPC,6},{E_GPC,7},{E_GPC,14},{E_GPC,15},   //AD µØÖ·
{E_GPA,7},{E_GPC,9},{E_GPB,3},{E_GPA,14},{E_GPB,12},{E_GPB,7},{E_GPB,14},{E_GPA,5}
};

static struct PinSet input_pins[] = {
    {E_GPA,0},{E_GPA,3},{E_GPA,4},{E_GPA,6},{E_GPA,9},{E_GPA,10},{E_GPA,11},{E_GPA,12},{E_GPA,13},{E_GPA,15},
    {E_GPB,0},{E_GPB,1},{E_GPB,2},{E_GPB,4},{E_GPB,5},{E_GPB,6},{E_GPB,8},{E_GPB,13},{E_GPB,15},
    {E_GPC,0},{E_GPC,1},{E_GPC,2},{E_GPC,3},{E_GPC,10},
    {E_GPD,6},{E_GPD,7},{E_GPD,14},{E_GPD,15},
};

static struct PinSet output_pins[] = {
    {E_GPA,8}, /* GPIO0, master use it to write Ultrasonic Sample signle to left slave */
    {E_GPA,1},{E_GPA,2},{E_GPC,8},{E_GPC,11},   //LED
    {E_GPC,6},{E_GPC,7},{E_GPC,14},{E_GPC,15},   //AD µØÖ·
    {E_GPA,7},{E_GPC,9},{E_GPB,3},{E_GPA,14},{E_GPB,12},{E_GPB,7},{E_GPB,14},{E_GPA,5}
};

// IO Pin number         0   1  2  3   4   5   6  7        //Õý³£
uint32_t ZTP115[8] = {   2, 13, 3, 12,11 , 6,  9, 4 };
uint32_t EZ4B40[8] = {   0,  7, 8, 5, 14 ,10, 15, 1 };

struct PinSet rxpin[8]={    
//   RX0       RX1       RX2       RX3        RX4        RX5       RX6        RX7 
    {E_GPA,7},{E_GPC,9},{E_GPB,3},{E_GPA,14},{E_GPB,12},{E_GPB,7},{E_GPB,14},{E_GPA,5}
    };

struct PinSet pwpin[8]={    
//   PW0       PW1        PW2       PW3        PW4        PW5       PW6        PW7 
    {E_GPA,6},{E_GPC,10},{E_GPB,2},{E_GPA,15},{E_GPB,13},{E_GPB,6},{E_GPB,8},{E_GPA,4}
    };  
    
// Ö÷°å´«¸ÐÆ÷Âß¼­±àºÅ£¬Õý·½Ïò³¯ÉÏ£¬ÒÀ´ÎÅÅÐò¡£
// 0  2  4  6  
// 1  3  5  7  
// IO Pin number          0   1  2  3   4   5   6  7        
uint32_t ZTPINDEX[8] = {  1,  6, 3, 4,  2,  7,  0, 5 };

#if 0

// IO Pin number          0   1  2  3   4   5   6  7        
uint32_t EZ4INDEX[8] = {  6,  2, 4, 5,  7,  3,  0, 1 };
#else
// 0  2  4  6  
// 1  3  5  7
// IO Pin number          0   2  4  6   1   3   5  7        
uint32_t EZ4INDEX[8] = {  6,  4, 7, 0,  2,  5,  3, 1 };
#endif  

void delay_loop(void)
{
    uint32_t j;

    for(j=0;j<60;j++) {
        __NOP(); 
    };
}

void PWM_Configuration(int frequence , int pulseratio)
{
    S_DRVPWM_TIME_DATA_T sPt;
    /* PWM Timer property */ 
    sPt.u8Mode = DRVPWM_AUTO_RELOAD_MODE; 
    sPt.u32Frequency = frequence; 
    sPt.u8HighPulseRatio = pulseratio; 
    sPt.i32Inverter = 0; 
    /* Enable PWM clock */
    DrvPWM_Open(); 
    /* Select PWM engine clock */
    //DrvPWM_SelectClockSource(DRVPWM_TIMER0, DRVPWM_EXT_12M);
    DrvSYS_SelectIPClockSource(E_SYS_PWM01_CLKSRC,0); 
    /* Set PWM Timer0 Configuration */
    DrvPWM_SetTimerClk(DRVPWM_TIMER0, &sPt); 
    /* Enable Output for PWM Timer0 */
    DrvPWM_SetTimerIO(DRVPWM_TIMER0, 1); 
    /* Set PWM pins */
    DrvGPIO_InitFunction(E_FUNC_PWM01); 
    /* Enable the PWM Timer 0 */
    DrvPWM_Enable(DRVPWM_TIMER0, 1); 
}

/*  
 * === FUNCTION ========================================= 
 * Name         : TIMER_Configuration 
 * Description  : TIMER????o¡¥¨ºy
 * =====================================================                                                                                                                                                                                                                                            fdDDFFFFFFFS
 */
void TIMER_Configuration(void)
{
    DrvTIMER_Init();  
    DrvSYS_SelectIPClockSource(E_SYS_TMR0_CLKSRC,0x00);  
    DrvTIMER_Open(E_TMR0,TIMER0_FREQ,E_PERIODIC_MODE);  
    //DrvTIMER_SetTimerEvent(E_TMR0, 1, (TIMER_CALLBACK)Timer0_Callback, 0);  
    DrvTIMER_EnableInt(E_TMR0);   
    DrvTIMER_Start(E_TMR0); 
}

/*  
 * === FUNCTION ========================================= 
 * Name         : UART_Configuration 
 * Description  : UART????o¡¥¨ºy
 * ===================================================== 
 */
void UART_Configuration(void)
{
    STR_UART_T param;
  
    /* Select UART Clock Source From 12MHz */
    DrvSYS_SelectIPClockSource(E_SYS_UART_CLKSRC, 0x03);

    param.u32BaudRate = UART_BAUD;
    param.u8cDataBits = DRVUART_DATABITS_8;
    param.u8cStopBits = DRVUART_STOPBITS_1;
    param.u8cParity = DRVUART_PARITY_NONE; 
    param.u8cRxTriggerLevel = DRVUART_FIFO_1BYTES;
    param.u8TimeOut = 0;
 
    DrvGPIO_InitFunction(E_FUNC_UART0_RX_TX);
    DrvUART_Open(UART_PORT0, &param);
    DrvGPIO_InitFunction(E_FUNC_UART1_RX_TX);
    DrvUART_Open(UART_PORT1, &param);
    DrvGPIO_InitFunction(E_FUNC_UART2);
    DrvUART_Open(UART_PORT2, &param);
}

void ADC_Configuration(void)
{
    DrvADC_Open (ADC_SINGLE_END,            //SingleÄ£Ê½
                ADC_SINGLE_CYCLE_OP,               //µ¥ÖÜÆÚÉ¨ÃèÄ£Ê½
                0x1,                               
                INTERNAL_RC22MHZ,                    
                9                               
                );
       
    ADC->ADCR.DMOF =0;
    delay_loop();                         //ÑÓ³ÙÊ¹Ê±ÖÓÎÈ¶¨  
}

void SENSOR_Configuration(void)
{
    uint8_t i = 0;
    for( i = 0 ; i < 4 ;  i++  )
    {   
        DrvSYS_Delay( 250*1000 );
    }
  
}

/*  
 * === FUNCTION ========================================= 
 * Name         : PDMA_Callback 
 * Description  : PDMA¨ª¨º3¨¦¡ä?¦ÌY2¨´¡Á¡Â¦Ì???¦Ì¡Âo¡¥¨ºy
 * ===================================================== 
 */
void PDMA_Callback(void)
{
    pdma_write_protect = FALSE;     //??D¡ä¡À¡ê?¡è
}

/*  
 * === FUNCTION ========================================= 
 * Name         : PDMA_Configuration 
 * Description  : PDMA????o¡¥¨ºy
 * ===================================================== 
 */
void PDMA_Configuration(void)
{
    STR_PDMA_T sPDMA;
    
    /* PDMA Init */
    DrvPDMA_Init();
    
    /* PDMA Setting */
    DrvPDMA_SetCHForAPBDevice(eDRVPDMA_CHANNEL_0,eDRVPDMA_UART0,eDRVPDMA_WRITE_APB);
        
    /* CH0 TX Setting */
    sPDMA.sSrcCtrl.u32Addr      = (uint32_t)NULL;
    sPDMA.sDestCtrl.u32Addr     = UART0_BASE;   
    sPDMA.u8TransWidth          = eDRVPDMA_WIDTH_8BITS;
    sPDMA.u8Mode            = eDRVPDMA_MODE_MEM2APB;
    sPDMA.sSrcCtrl.eAddrDirection   = eDRVPDMA_DIRECTION_INCREMENTED; 
    sPDMA.sDestCtrl.eAddrDirection  = eDRVPDMA_DIRECTION_FIXED;   
    sPDMA.i32ByteCnt                = 0;
    DrvPDMA_Open(eDRVPDMA_CHANNEL_0,&sPDMA);
    
    /* Enable INT */
    DrvPDMA_EnableInt(eDRVPDMA_CHANNEL_0, eDRVPDMA_BLKD );  //?¨ºD¨ªPDMA¡ä?¦ÌY¨ª¨º3¨¦?D??
    /* Install Callback function */
    DrvPDMA_InstallCallBack(eDRVPDMA_CHANNEL_0,eDRVPDMA_BLKD,(PFN_DRVPDMA_CALLBACK)PDMA_Callback); 
    
    /*  Enable UART0 TX-DMA */
    DrvUART_EnablePDMA(UART_PORT0);
}

/*  
 * === FUNCTION ========================================= 
 * Name         : UART_DMA_Write 
 * Description  : 
 * ===================================================== 
 */
void UART_DMA_Write(uint8_t *pu8TxBuf, uint8_t u32WriteBytes)
{
/*  
    DrvUART_Write(UART_PORT1, pu8TxBuf, u32WriteBytes);   //¡¤?DMA¡¤?¨º?¡ê?¦Ì¡Â¨º?¨®? 
*/  
    if(pdma_write_protect != TRUE)  //¨¨?1?¨¦?¡ä?¡¤¡é?¨ª??¨®D¨ª¨º3¨¦¡ê??¡À?¨®¨ª?3?
    {  
        pdma_write_protect = TRUE;  //????D¡ä¡À¡ê?¡è
                PDMA0->SAR = (uint32_t)pu8TxBuf;
        PDMA0->u32BCR = u32WriteBytes;
        PDMA0->CSR.PDMACEN = 1;
        PDMA0->CSR.TRIG_EN = 1;         
    }
     return;    
}

static void _gpio_init(const struct PinSet *pins, uint8_t size, E_DRVGPIO_IO mode)
{
    uint32_t i;

    for (i = 0; i < size; i++) {
        DrvGPIO_Open(pins[i].Port, pins[i].Num, mode);
        DrvGPIO_ClrBit(pins[i].Port, pins[i].Num);
    }
}

void InitSystem(void)
{
    UNLOCKREG();
    SYSCLK->PWRCON.XTL12M_EN = 1;
    DrvSYS_Delay(5000);
    DrvSYS_SelectPLLSource(E_SYS_EXTERNAL_12M);
    DrvSYS_Open(50000000);
    LOCKREG();

    /* input */
    _gpio_init(input_pins, sizeof(input_pins) / sizeof(struct PinSet), E_IO_INPUT);
    /* output */
    _gpio_init(output_pins, sizeof(output_pins) / sizeof(struct PinSet), E_IO_OUTPUT);

    UART_Configuration();   
    TIMER_Configuration();  
    ADC_Configuration();
    SENSOR_Configuration();
#if 0
    PDMA_Configuration();       

    midi_buf.txSel = 0;
    midi_buf.index = 0;
    midi_buf.len = 0;
    pdma_write_protect = FALSE;     
#endif
}

void system_timer_start(void (*timer_callback)(void))
{
    if (timer_callback) {
        static int8_t timer_start_counter;

        if ((++timer_start_counter) > 1)
            DrvTIMER_ClearTimerEvent(E_TMR0, 0);
        DrvTIMER_SetTimerEvent(E_TMR0, 1, (TIMER_CALLBACK) timer_callback, 0);
    }
}

int8_t system_get_dip(void)
{
    int8_t i, value;

    for (i = value = 0; i < 4; i++) {
        value += DrvGPIO_GetBit(E_GPC, i) << (4-i-1);
    }
    return value;
}

enum board_type system_get_board_type(int8_t dip)
{
    dip &= 0x03;
    if (dip == 3)
        dip = 0;
    return (enum board_type) dip;
}
