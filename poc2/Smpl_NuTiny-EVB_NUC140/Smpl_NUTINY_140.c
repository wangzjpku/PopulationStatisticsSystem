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

//---------------Defines-------------------------//
#define REMOTE_DEVICE_ID		0x12

#define TIMER0_FREQ         10  //2500   1S 中断触发次数
#define UART_BAUD          115200  

#define PWM_Frequence       1000
#define UART0_DMA_TX_MAX    300


//---------------Type define-------------------------//
struct PinSet
{ 
    E_DRVGPIO_PORT  Port;
    int32_t         Num;
};

typedef struct
{
    uint8_t txBuf[2][UART0_DMA_TX_MAX+4];
    uint8_t txSel;
    
    uint8_t index;
    uint8_t len;
} uartDMACfg_t;

uartDMACfg_t  midi_buf;
uint8_t  pdma_write_protect;

//---------------Function Prototype-------------------------//

void InitSystem(void); 
void TIMER_Configuration(void);
void UART_Configuration(void);
void Timer0_Callback(void);
void ReadRTS0(int i);
void PWM_Configuration(int frequence , int pulseratio);
void ADC_Configuration(void);
void ADC_GetValue(void);
void SENSOR_Configuration(void);

void PDMA_Configuration(void);
void UART_DMA_Write(uint8_t *pu8TxBuf, uint8_t u32WriteBytes);
void PDMA_Callback(void);

//---------------Variable-------------------------//
uint8_t  buffer[4] = {0X11,0x12,0x13,0x14};
uint8_t C_GPD = 0;
uint16_t val[32];
uint32_t count = 0;
struct PinSet pupin[45]={
// Input GPIO  
{E_GPA,0},{E_GPA,3},{E_GPA,4},{E_GPA,6},{E_GPA,8},{E_GPA,9},{E_GPA,10},{E_GPA,11},{E_GPA,12},{E_GPA,13},{E_GPA,15},
{E_GPB,0},{E_GPB,1},{E_GPB,2},{E_GPB,4},{E_GPB,5},{E_GPB,6},{E_GPB,8},{E_GPB,13},{E_GPB,15},
{E_GPC,0},{E_GPC,1},{E_GPC,2},{E_GPC,3},{E_GPC,10},
{E_GPD,6},{E_GPD,7},{E_GPD,14},{E_GPD,15},

// Output GPIO
{E_GPA,1},{E_GPA,2},{E_GPC,8},{E_GPC,11},   //LED
{E_GPC,6},{E_GPC,7},{E_GPC,14},{E_GPC,15},   //AD 地址
{E_GPA,7},{E_GPC,9},{E_GPB,3},{E_GPA,14},{E_GPB,12},{E_GPB,7},{E_GPB,14},{E_GPA,5}
};


// IO Pin number         0   1  2  3   4   5   6  7        //正常
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
	
// 主板传感器逻辑编号，正方向朝上，依次排序。
// 0  2	 4  6  
// 1  3  5  7  
// IO Pin number          0   1  2  3   4   5   6  7        
uint32_t ZTPINDEX[8] = {  1,  6, 3, 4,  2,  7,  0, 5 };

#if 0

// IO Pin number          0   1  2  3   4   5   6  7        
uint32_t EZ4INDEX[8] = {  6,  2, 4, 5,  7,  3,  0, 1 };
#else
// 0  2	 4  6  
// 1  3  5  7
// IO Pin number          0   2  4  6   1   3   5  7        
uint32_t EZ4INDEX[8] = {  6,  4, 7, 0,  2,  5,  3, 1 };
#endif	

void delay_loop(void)
 {
    uint32_t j;
 		for(j=0;j<60;j++)
		{
		 __NOP(); 
		};	

 }
 
/*  
 * === FUNCTION ========================================= 
 * Name         : main 
 * Description  : ?μí3?÷oˉêy
 * ===================================================== 
 */
int main(void)
{ 

 	InitSystem();	
	while(1){
        Timer0_Callback();
    }	
}

/*  
 * === FUNCTION ========================================= 
 * Name         : InitSystem 
 * Description  : ?μí33?ê??ˉoˉêy
 * ===================================================== 
 */
void InitSystem(void)
{
    uint32_t i;	
    UNLOCKREG();
    SYSCLK->PWRCON.XTL12M_EN = 1;  
    DrvSYS_Delay(5000);            
    DrvSYS_SelectPLLSource(E_SYS_EXTERNAL_12M);   
    DrvSYS_Open(50000000);          
    LOCKREG();

    for(i=0;i<29;i++)
    {
        DrvGPIO_Open(pupin[i].Port, pupin[i].Num, E_IO_INPUT); 
			  DrvGPIO_ClrBit( pupin[i].Port, pupin[i].Num );
    }
		
    while(i<45)
    {
        DrvGPIO_Open(pupin[i].Port, pupin[i].Num, E_IO_OUTPUT);
			  DrvGPIO_ClrBit( pupin[i].Port, pupin[i].Num );
				i++;
		}			

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
 * Description  : TIMER????oˉêy
 * =====================================================                                                                                                                                                                                         													fdDDFFFFFFFS
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
 * Description  : UART????oˉêy
 * ===================================================== 
 */
void UART_Configuration(void)
{
    STR_UART_T param;
  
    /* Select UART Clock Source From 12MHz */
    DrvSYS_SelectIPClockSource(E_SYS_UART_CLKSRC, 0x00); 

    param.u32BaudRate = UART_BAUD; 
    param.u8cDataBits = DRVUART_DATABITS_8;
    param.u8cStopBits = DRVUART_STOPBITS_1; 
    param.u8cParity = DRVUART_PARITY_NONE;  
    param.u8cRxTriggerLevel = DRVUART_FIFO_1BYTES;  
    param.u8TimeOut = 0; 
 
    DrvGPIO_InitFunction(E_FUNC_UART0);  
    DrvUART_Open(UART_PORT0, &param);  
	
	  param.u32BaudRate = UART_BAUD; 
    param.u8cDataBits = DRVUART_DATABITS_8;
    param.u8cStopBits = DRVUART_STOPBITS_1; 
    param.u8cParity = DRVUART_PARITY_NONE;  
    param.u8cRxTriggerLevel = DRVUART_FIFO_1BYTES;  
    param.u8TimeOut = 0; 
		
		DrvGPIO_InitFunction(E_FUNC_UART1);  
		DrvUART_Open(UART_PORT1, &param); 
		
    param.u32BaudRate = UART_BAUD; 
    param.u8cDataBits = DRVUART_DATABITS_8;
    param.u8cStopBits = DRVUART_STOPBITS_1; 
    param.u8cParity = DRVUART_PARITY_NONE;  
    param.u8cRxTriggerLevel = DRVUART_FIFO_1BYTES;  
    param.u8TimeOut = 0; 
		
		DrvGPIO_InitFunction(E_FUNC_UART2);  
		DrvUART_Open(UART_PORT2, &param);  
}

void ADC_Configuration(void)
{
		DrvADC_Open (ADC_SINGLE_END,            //Single模式
				 ADC_SINGLE_CYCLE_OP,               //单周期扫描模式
				 0x1,                               
				 INTERNAL_RC22MHZ,                    
				 9                               
				);
	   
	   ADC->ADCR.DMOF =0;
		 delay_loop();                         //延迟使时钟稳定  
}

void SENSOR_Configuration(void)
{
		uint8_t i = 0;
	  for( i = 0 ; i < 4 ;  i++  )
		{	
			DrvSYS_Delay( 250*1000 );
	  }
		
	  for( i = 0 ; i < 8 ;  i++  )
		{
			DrvGPIO_SetBit( rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num );		
			DrvSYS_Delay(100*1000);
			DrvGPIO_ClrBit( rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num );		
			DrvSYS_Delay(100*1000);					
		}	
}

/*  
 * === FUNCTION ========================================= 
 * Name         : PDMA_Configuration 
 * Description  : PDMA????oˉêy
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
    sPDMA.sSrcCtrl.u32Addr 	    = (uint32_t)NULL;
    sPDMA.sDestCtrl.u32Addr	    = UART0_BASE;   
    sPDMA.u8TransWidth 		    = eDRVPDMA_WIDTH_8BITS;
    sPDMA.u8Mode 		    = eDRVPDMA_MODE_MEM2APB;
    sPDMA.sSrcCtrl.eAddrDirection   = eDRVPDMA_DIRECTION_INCREMENTED; 
    sPDMA.sDestCtrl.eAddrDirection  = eDRVPDMA_DIRECTION_FIXED;   
    sPDMA.i32ByteCnt                = 0;
    DrvPDMA_Open(eDRVPDMA_CHANNEL_0,&sPDMA);
    
    /* Enable INT */
    DrvPDMA_EnableInt(eDRVPDMA_CHANNEL_0, eDRVPDMA_BLKD );  //?êDíPDMA′?μYíê3é?D??
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
    DrvUART_Write(UART_PORT1, pu8TxBuf, u32WriteBytes);   //·?DMA·?ê?￡?μ÷ê?ó? 
*/  
    if(pdma_write_protect != TRUE)  //è?1?é?′?·￠?í??óDíê3é￡??±?óí?3?
    {  
        pdma_write_protect = TRUE;  //????D′±￡?¤
				PDMA0->SAR = (uint32_t)pu8TxBuf;
        PDMA0->u32BCR = u32WriteBytes;
        PDMA0->CSR.PDMACEN = 1;
        PDMA0->CSR.TRIG_EN = 1;         
    }
	 return;	
}

/*  
 * === FUNCTION ========================================= 
 * Name         : PDMA_Callback 
 * Description  : PDMAíê3é′?μY2ù×÷μ???μ÷oˉêy
 * ===================================================== 
 */
void PDMA_Callback(void)
{
    pdma_write_protect = FALSE;     //??D′±￡?¤
}


/**************************************************
 *函数名： ADC_average                *
 *功能： ADC转换值取中间值函数                 *
 *出口参数：中间值          *
 *入口参数：转换值数组地址        *
 *************************************************/
static int32_t ADC_average (int32_t *ADC_value)   
{
 int32_t value[9] = {0};
 uint8_t i,j,k;
 int32_t value_max;
 for (i=0; i<9; i++)
 {
  value[i] = *ADC_value++;                    //获取将要转换的值
 }
 
 //冒泡法从小到大排列
 for (j=0; j<9; j++)                     //一共要比较9次
 {
  for (k=0; k<9-j; k++)                  //第一次过后最大的值放在了最高的位置
  {                                      //就不用再比较最大的那个元素了
   if(k != 8)                            //防止k为8时，value[8]与value[9]的比较，因为后者已经超出了数组的范围
   {
    if (value[k] > value[k+1])          //所以每次比较完下一次少比较一次
    {
     value_max = value[k];              //将较大的值保存起来
     value[k] = value[k+1];             //较小的值赋给低位元素
     value[k+1] = value_max;            //较大的值赋给高位元素
    }
   }
  }
 }
 return (value_max);
} 

/**************************************************
 *函数名： get_ADC_value                *
 *功能： ADC获取对应通道转换值函数             *
 *出口参数：9次A/D后得到的中值       *
 *入口参数：ADC通道号码          *
 *************************************************/
 //#if 0
static int32_t get_ADC_value_1(uint8_t ADC_channel_number)
{
 uint8_t i;
    int32_t ADC_calibration[9]={0};                                    //ADC求中间值数据储存数组
 for (i=0; i<9; i++)                                                   //读取9次的值
 {
  DrvADC_StartConvert();                                               //启动ADC转换
  while(!DrvADC_IsConversionDone())                                    //等待ADC转换完成
  {
  }
  ADC_calibration[i] = DrvADC_GetConversionData(ADC_channel_number);   //读取对应ADC通道的值
  DrvADC_StopConvert(); 
  DrvSYS_Delay( 10 * 1000 );                                               //停止转换
 }
 return (ADC_average(ADC_calibration));                                //将取到的值求取中间值后返回
}
//#else
static int32_t get_ADC_value(uint8_t ADC_channel_number)
{
 uint8_t i;
 int32_t ADC_calibration;                                    //ADC求中间值数据储存数组

  DrvADC_StartConvert();                                               //启动ADC转换
  while(!DrvADC_IsConversionDone())                                    //等待ADC转换完成
  {
  }
  ADC_calibration = DrvADC_GetConversionData(ADC_channel_number);   //读取对应ADC通道的值
  DrvADC_StopConvert();                                                //停止转换

 return (ADC_calibration);                                //将取到的值求取中间值后返回
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

/*  
 * === FUNCTION ========================================= 
 * Name         : Timer0_Callback 
 * Description  : Timer0??μ÷oˉêy
 * ===================================================== 
 */
void Timer0_Callback(void)
{
    uint8_t command[6]={0x0,0x0,0x0,0x0,0x0,0x0}; 
    uint32_t i, j;

    count = count + 1;

    /*PIR*/
    for (i = 0; i < 8; i++) {
        val[i] = AnalongReadRF(ZTPINDEX[i]);
    }

    /*UltraSonic 40KHZ*/
#if 1
    /*for (i = 0; i < 4; i++) {
        val[i + 8] = 0x0000;
    }*/

    /* adjust j to 1, 2, 3, 4 */
    j = count % 4;
    if (j == 0)
        j = 4;

    /* read sensor i */
    i = j - 1;
    val[i + 8] = AnalongReadEZ4(EZ4INDEX[i]);
    /* enable sensor i */
    DrvGPIO_SetBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
    DrvSYS_Delay(0.1 * 1000);
    DrvGPIO_ClrBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
    DrvSYS_Delay(12.5 * 1000);
#else
    if ((count%2) == 0) {
        for (i = 0; i < 2; i++) {            
            val[i + 8] = 0x00AA;
            DrvGPIO_SetBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
            DrvSYS_Delay(0.1 * 1000);
            DrvGPIO_ClrBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
            DrvSYS_Delay(12.5 * 1000);
        }
        DrvSYS_Delay(5*1000);
        for (i = 2; i < 4; i++) {
            val[i + 8] = AnalongReadEZ4(EZ4INDEX[i]);
        }
    } else {
        for (i = 2; i < 4; i++) {
            val[i + 8] = 0x00AA;
            DrvGPIO_SetBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
            DrvSYS_Delay(0.1 * 1000);
            DrvGPIO_ClrBit(rxpin[EZ4INDEX[i]].Port, rxpin[EZ4INDEX[i]].Num);
            DrvSYS_Delay(12.5 * 1000);
        }
        DrvSYS_Delay(5*1000);
        for (i = 0; i < 2; i++) {
            val[i + 8] = AnalongReadEZ4(EZ4INDEX[i]);
        }
    }
#endif
    
#if 0
	 DrvUART_Write(UART_PORT0,(uint8_t *)val,32);
#else 
    {
        static uint8_t sample[108];
#if 0
        uint32_t timestamp = count;
#else
        /*use tick will lead to crash*/
        uint32_t timestamp = DrvTIMER_GetIntTicks(E_TMR0);
#endif

        sample[0] = 0x68;
        sample[1] = 0x68;
        sample[2] = 96 + 4;
        memmove(sample+3, (uint8_t *) val, 48);
        memmove(sample+99, (uint8_t *) timestamp, 4);
        //memmove(sample+51, (uint8_t *) timestamp, 4);
        sample[103] = 0x00;

        DrvUART_Write(UART_PORT0, sample, 104);
    }
#endif

    //////////////////////////////////////////////
    //UART test
    //////////////////////////////////////////////
	command[0] = 0x02;
	command[1] = 0xff;
	DrvUART_Write(UART_PORT1,command,4);

	command[0] = 0x03;
	command[1] = 0xff;
	DrvUART_Write(UART_PORT2,command,4);

    //////////////////////////////////////////////    
    //LED test
    //////////////////////////////////////////////	
	if ( C_GPD == 1 )
	{
		GPC_11 = 0;
		GPC_8 = 0;
		
		GPA_2 = 0;
		GPA_1 = 0;
		
		C_GPD = 0;
	}
	else
	{
		GPC_11 = 1;
		GPC_8 = 1;
		
		GPA_2 = 1;
		GPA_1 = 1;
		
		C_GPD = 1;
	}	

}
