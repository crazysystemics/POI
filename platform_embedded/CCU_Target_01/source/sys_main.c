/** @file sys_main.c 
*   @brief Application main file
*   @date 11-Dec-2018
*   @version 04.07.01
*
*   This file contains an empty main function,
*   which can be used for the application.
*/

/* 
* Copyright (C) 2009-2018 Texas Instruments Incorporated - www.ti.com 
* 
* 
*  Redistribution and use in source and binary forms, with or without 
*  modification, are permitted provided that the following conditions 
*  are met:
*
*    Redistributions of source code must retain the above copyright 
*    notice, this list of conditions and the following disclaimer.
*
*    Redistributions in binary form must reproduce the above copyright
*    notice, this list of conditions and the following disclaimer in the 
*    documentation and/or other materials provided with the   
*    distribution.
*
*    Neither the name of Texas Instruments Incorporated nor the names of
*    its contributors may be used to endorse or promote products derived
*    from this software without specific prior written permission.
*
*  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
*  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
*  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
*  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
*  OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
*  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
*  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
*  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
*  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
*  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
*  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
*/


/* USER CODE BEGIN (0) */
/* USER CODE END */

/* Include Files */

#include "sys_common.h"
#include "het.h"
#include "gio.h"
#include "sci.h"
#include "spi.h"

/* USER CODE BEGIN (1) */
/* USER CODE END */

/** @fn void main(void)
*   @brief Application main function
*   @note This function is empty by default.
*
*   This function is called after startup.
*   The user can use this function to implement the application.
*/

/* USER CODE BEGIN (2) */
/* USER CODE END */

int main(void)
{
/* USER CODE BEGIN (3) */
    int i;
    static unsigned char command;
     /* spiDAT1_t dataconfig_t;
      uint16_t txBuffer[] = {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xAA, 0x55, 0x99};
      uint16_t rxBuffer[10];
      SpiDataStatus_t SpiStatus;
      */
   /*  mibspiInit();

     for(i=0;i<10;i++)
     {
         mibspiSetData(mibspiREG1, 0, (uint16 *)0x0001);//txBuffer[i]);
         mibspiTransfer(mibspiREG1, 0);
         while(!(mibspiIsTransferComplete(mibspiREG1, 0)));
         mibspiGetData(mibspiREG1, 0, (uint16 *) rxBuffer[i]);
     }
  */

      spiInit();

    /*  dataconfig_t.CS_HOLD = FALSE; // Specify if CS has to be held low for the full transmission.
      dataconfig_t.WDEL = FALSE;
      dataconfig_t.DFSEL = SPI_FMT_2; // Specify which data format to use. They are defined in Halcogen
      dataconfig_t.CSNR = 0xFE;
      spiTransmitAndReceiveData(spiREG1, &dataconfig_t, 10, txBuffer, rxBuffer);

      for(i=1;i<20;i++)
      {
          SpiStatus = SpiTxStatus(spiREG1);
          if(SpiStatus == SPI_READY)
              break;
      }

      */
      _enable_IRQ();
     sciInit();
     sciEnableNotification(scilinREG, SCI_RX_INT);
     sciSend(scilinREG, 5, (unsigned char *)"Hey\r\n");// Send data to USB UART
     sciReceive(scilinREG, 1, (unsigned char *) &command);
     sciReceive(scilinREG, 1, (unsigned char *) &command);

     sciEnableNotification(sciREG, SCI_RX_INT);
     sciSend(sciREG, 4, (unsigned char *)"Hi\r\n");// Send data to RS422 UART
     sciReceive(sciREG, 1, (unsigned char *) &command);
     sciReceive(sciREG, 1, (unsigned char *) &command);

    gioSetDirection(hetPORT1, 0xffffffff);

    gioSetBit(hetPORT1, 0, 1);

    gioSetBit(hetPORT1, 17, 1);
    gioSetBit(hetPORT1, 18, 1);
    gioSetBit(hetPORT1, 25, 1);
    gioSetBit(hetPORT1, 29, 1);
    gioSetBit(hetPORT1, 31, 1);
    gioSetBit(hetPORT1, 27, 1);
    gioSetBit(hetPORT1, 5, 1);
/* USER CODE END */

    return 0;
}


/* USER CODE BEGIN (4) */
/* USER CODE END */
