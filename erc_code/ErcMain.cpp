#define _DEBUG 1
#define _LOCALMACHINE 1


#include <stdio.h>
#include <string.h>

#include "ErcExecutive.h"

void SchedChkSSReadFlgFun() {
	char _checkBuff[5];
	char path[100] = "E:\\tcpip\\ReadFlag.dat";
	int result=0;
	strcpy(path,"E:\\tcpip\\ReadRspFlag.dat");
	result = AxiRead(path, _checkBuff, sizeof(_checkBuff));
	if (result == 0) {
		if(_checkBuff[0]==1)
			_readRspFlag = true;
	}

	strcpy(path, "E:\\tcpip\\ReadRdpFlag.dat");
	result = AxiRead(path, _checkBuff, sizeof(_checkBuff));
	if (result == 0) {
		if (_checkBuff[0] == 1)
			_readRdpFlag = true;
	}
}


int main()
{
	printf("****************************************************************************\n");
	printf("<--------------------------- ERC Started ---------------------------------->\n");

	CmdUnit cmdUnitTable[5] =
	{
		{NULL,INIT,ERC,ALL,NULL },
		{NULL,CHECK_RCDS_RX_BUF,RCDS,RCDS,},
		{SchedChkSSReadFlgFun,READ_SS_DATA,RCDS,RCDS,},
		{NULL,DATA_TRANSFER,RCDS,ALL,NULL},
		{NULL,WRITE_SS_DATA,RCDS,RDP,NULL }
		
		
		//{ NULL,SEND_RSLT_TO_RCDS,RCDS,RCDS,NULL }
	};

	CmdBlock CmdBlockTable = {
	5,{cmdUnitTable[0],cmdUnitTable[1],cmdUnitTable[2],cmdUnitTable[3],cmdUnitTable[4]}
	};

	Schedule  s =
	{
		NULL,1,CmdBlockTable, 400,NULL
	};

	ErcExecutive(s);
}
