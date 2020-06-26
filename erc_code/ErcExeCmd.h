#define _DEBUG 1
#define MAX_BUFF 700
# define QMAX 20
#define NOT_DEMO 0


#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>


#include "Enum.h"
#include "Application.h"
#include "Erc.h"
#include "Rsp.h"
#include "Rdp.h"
#include "Rcds.h"
#include "SocketAPI.h"
#include "AxiAPI.h"
#include "QApi.h"


#ifndef _SOME
	int some = 0;
	#define _SOME
	typedef enum  Command {
		EXEC_DISCRETE_CMD=1, READ_SS_DATA, WRITE_SS_DATA, RAISE_SERVICE_RQST, RETURN_SERVICE_RSLT, CONGIF, INIT, SHUTDOWN, TEST, DECONFIG,
		CONFIG_ERC_SCHEDULE, MARK_FOR_DEBUG_CONSOLE, CLEAR_FOR_DEBUG_CONSOLE, DEBUG_DUMP, DEBUG_SEND, DATA_TRANSFER,
		CHECK_RCDS_RX_BUF
};
#endif

//globel variables
bool _RcvBufferStatusFlag = false, _writeSSFlag = false, _writeRdpFlag = false, _writeRspFlag = false, _readRdpFlag = false, _readRspFlag = false;
bool _ReadSSFlag = false, _writeRfFlag = false, AcceptStatusFlg = false, _she;
bool _init = true;
bool SocketStatusFlag = false, ConnectionStatusFlg = false;
char  _sendRspBuff[MAX_BUFF],_sendRdpBuff[MAX_BUFF], _sendRfBuff[MAX_BUFF];
int _rcdsFront = -1,_rspFront=-1,_rdpFront=-1;
int _rcdsRear = -1,_rspRear=-1, _rdpRear=-1;
char _rcdsMemBuff[QMAX][MAX_BUFF], _rspMemBuff[QMAX][MAX_BUFF], _rdpMemBuff[QMAX][MAX_BUFF];



void CloseSocketConnection() {
	EthernetClose();
}


void CreateSocketConnection() {
#if _LOCALMACHINE
	char ip[15] = "172.20.147.64";// "127.0.0.1";
#else
	char ip[10];//have to get IP address of ERC= "127.0.0.1";
#endif
	int port = 5002;
	int result;

	while (!SocketStatusFlag) {
		result = EthernetCreateSocket();
		if (result == 0) {
			result = EthernetListen(ip, port);
			if (result == 0) {
				SocketStatusFlag = true;
			}
			else {
				CloseSocketConnection();
				SocketStatusFlag = false;
			}
		}
		else {
			CloseSocketConnection();
			SocketStatusFlag = false;
		}
		printf("error no %d \n", result);
	}
	//return ConectionStatus;
}


void AcceptSocket() {
	bool result = false;
	if (SocketStatusFlag) {
		if (!AcceptStatusFlg) {
			result = EthernetConnect();
			if (result == 0) {
				//Receive();
				AcceptStatusFlg = true;
			}
			else
				AcceptStatusFlg = false;
		}
	}
}

void Erc_Rcds_Init() {
	CreateSocketConnection();
	AcceptSocket();
	//Receive();
}

int Parsing(char _buff[MAX_BUFF],int _size) {
	RcdsReq resp;
	memcpy(&resp, _buff, sizeof(resp));
	if (resp.InterfaceID == INTERFACE_ID_ETHERNET_PLUGIN_ERC) {
		switch (resp.packetId)
		{
			char Data[200];
			int DataSize;
		case REQ_LINK_STATUS:
			ResTestLink _linkTest;
			_linkTest.header.InterfaceID = INTERFACE_ID_ERC_ETHERNET_PLUGIN;
			_linkTest.header.PacketID = RESP_LINK_STATUS;
			_linkTest.header.PacketVersion = MSG_VERSION;
			_linkTest.header.PacketPayload = 2;
			_linkTest.LinkStatus = OK;
			memcpy(Data, &_linkTest, sizeof(_linkTest));
			DataSize = sizeof(Data);
			Ethernetsend(Data, DataSize);

			break;

		case REQ_ERC_DEVICE_DETAILS:
			ResDeviceDetails _deviceDetails;
			_deviceDetails.header.InterfaceID = INTERFACE_ID_ERC_ETHERNET_PLUGIN;
			_deviceDetails.header.PacketID = RESP_ERC_DEVICE_DETAILS;
			_deviceDetails.header.PacketVersion = MSG_VERSION;
			_deviceDetails.header.PacketPayload = 40;
			memcpy(_deviceDetails.DeviceName, "RADAR 01", 31);
			memcpy(_deviceDetails.DeviceAlice, "RADAR", 15);
			memcpy(_deviceDetails.DeviceVersion, "ERC V1", 15);
			memcpy(_deviceDetails.SerialNo, "SL 01", 15);
			memcpy(_deviceDetails.ConnectionString, "127.0.0.1:5002", 15);
			memcpy(Data, &_deviceDetails, sizeof(_deviceDetails));
			DataSize = sizeof(Data);
			Ethernetsend(Data, DataSize);

			break;

		case REQ_RADAR_REDINESS:
			ErcCommand _testCommand;
			_testCommand = { 0xFFFF,TEST ,0x07 ,0x01 ,0xFFFF };
			memcpy(_sendRspBuff, &_testCommand, sizeof(_testCommand));
			_writeRspFlag = true;
			break;

		case CMD_START_SCANNING:
			RFDataDefination RfConfig;
			RfConfig = { 0xFFFF,0X0400,RCDS,ERC,RF,0x00,0x01,0x00,0x0,0x0,0xFFFF };
			memcpy(_sendRfBuff, &RfConfig, sizeof(RfConfig));
			_writeRfFlag = true;

			break;

		case CMD_STOP_SCANNING:
			RFDataDefination _RfConfig1;
			_RfConfig1 = { 0xFFFF,0X0400,RCDS,ERC,RF,0x00,0x00,0x01,0x0,0x0,0xFFFF };
			memcpy(_sendRfBuff, &_RfConfig1, sizeof(_RfConfig1));
			_writeRfFlag = true;

			break;


		default:
#if _DEBUG
			printf("\n\nDefault executed no commands match\n\n");
#endif

			break;

		}
	}
	else {
		SubSysHeader hdr1;
		memcpy(&hdr1, _buff, sizeof(hdr1));
		if (hdr1.MSGHDR == 0xFFFF) {
			switch (hdr1.MSGID) {
			case 0x000B:
				RadarOperationCommand _radarOperationCommand;
				//memcpy(&roc, RecvDataBuffer, sizeof(roc));
				memcpy(_sendRdpBuff, _buff, sizeof(_sendRdpBuff));
				memcpy(&_radarOperationCommand, _sendRdpBuff, sizeof(_radarOperationCommand));
				_writeRdpFlag = true;
				
				break;
			}
		}

	}


	//sub-system parcing
	SubSysHeader hdr;
	memcpy(&hdr, _buff, sizeof(hdr));
	if (hdr.MSGHDR == 0xFFFF) {
		switch (hdr.MSGID) {
		case 0xF0:
			BiteInformation bi;

			memcpy(&bi, _buff, sizeof(bi));
			Ethernetsend(_buff, _size);
			break;

		case 0x0008:

			RcdsResp _RCDSResp;
			_RCDSResp.InterfaceID = INTERFACE_ID_ERC_ETHERNET_PLUGIN;
			_RCDSResp.PacketID = DATA_SCANNED;
			_RCDSResp.PacketVersion = MSG_VERSION;
			_RCDSResp.PacketPayload = 24;
			_RCDSResp.DataPayload[200];

			memcpy(_RCDSResp.DataPayload, _buff, sizeof(_RCDSResp.DataPayload));


			//PeriodicTrackData ptd;

			memcpy(_buff, &_RCDSResp, sizeof(_RCDSResp));
			Ethernetsend(_buff, _size);
			break;

		default:
#if _DEBUG
			printf("\n\nDefault executed no commands match 1 1 1\n\n");
#endif
			break;

		}
	}
	return 1;
}


void ExeDiscrereCmd() {
#if _DEBUG
	printf("\nExeDiscrereCmd");
#endif
}

void RcdsReceive() {
	AcceptSocket();
	int _result = 0; char _rcdsRecvbuff[MAX_BUFF];
	if (AcceptStatusFlg) {
		_result = EthernetReceive(_rcdsRecvbuff);
		if (_result == 0) {
			_result = QPush(_rcdsMemBuff, _rcdsRecvbuff, sizeof(_rcdsRecvbuff), _rcdsFront, _rcdsRear);
			if (_result == 0)
				memset(_rcdsRecvbuff, '\0', sizeof(_rcdsRecvbuff));
		}
#if _DEBUG
		printf("Receive Data from Ethernet");
#endif
	}
}

void ReadData() {
	char _rspRecvAxiBuff[MAX_BUFF], _rdpRecvAxiBuff[MAX_BUFF];
	//RCDS Read
	RcdsReceive();

	//Rsp Read
	if (_readRspFlag) {
		char path[100] = "E:\\tcpip\\AXI.dat";
		char data[3] = "12";
		AxiRead(path, _rspRecvAxiBuff, sizeof(_rspRecvAxiBuff));
		QPush(_rspMemBuff, _rspRecvAxiBuff, sizeof(_rspRecvAxiBuff), _rspFront, _rspRear);
		strcpy(path, "E:\\tcpip\\ReadRspFlag.dat");
		AxiWrite(path, data, sizeof(data));
		strcpy(path, "E:\\tcpip\\AXI.dat");
		AxiWrite(path, data, sizeof(data));
		_readRspFlag = false;
	}

	//Rdp Read
	if (_readRdpFlag) {
		char path[100] = "E:\\tcpip\\AXI.dat";
		char data[3] = "12";
		AxiRead(path, _rdpRecvAxiBuff, sizeof(_rdpRecvAxiBuff));
		QPush(_rdpMemBuff, _rdpRecvAxiBuff, sizeof(_rdpRecvAxiBuff), _rdpFront, _rdpRear);
		strcpy(path, "E:\\tcpip\\ReadRspFlag.dat");
		AxiWrite(path, data, sizeof(data));
		strcpy(path, "E:\\tcpip\\AXI.dat");
		AxiWrite(path, data, sizeof(data));
		_readRdpFlag = false;
	}

#if _DEBUG
	printf("\nReadData");
#endif

	
}

void WriteData() {
	if (_writeRspFlag) {
		char path[100] = "E:\\tcpip\\WriteRspFlag.dat";
		AxiWrite(path, _sendRspBuff, sizeof(_sendRspBuff));
		memset(_sendRspBuff, '\0', sizeof(_sendRspBuff));

		AxiRead(path, _sendRspBuff, sizeof(_sendRspBuff));
		ErcCommand _ercCmd;
		memcpy(&_ercCmd, _sendRspBuff, sizeof(_ercCmd));
		_writeRspFlag = false;
	}
	if (_writeRdpFlag) {
		char path[100] = "E:\\tcpip\\WriteRdpFlag.dat";
		int _buffsize = sizeof(_sendRspBuff);
		AxiWrite(path, _sendRdpBuff, _buffsize);
		memset(_sendRdpBuff, '\0', sizeof(_sendRdpBuff));

		_writeRdpFlag = false;
	}

	if (_writeRfFlag) {
		char path[100] = "E:\\tcpip\\WriteRfFlag.dat";
		int _buffsize = sizeof(_sendRfBuff);
		AxiWrite(path, _sendRfBuff, _buffsize);
		memset(_sendRfBuff, '\0', sizeof(_sendRfBuff));

		_writeRfFlag = false;
	}
}

void RaiseServiceRqst() {
#if _DEBUG
	printf("\nRaiseServiceRqst");
#endif	
}

void Config() {
	RSPControlParameters RspConfig;
	RspConfig = { 0xFFFF ,};




	char Data[50];


	//memcpy(&message, Data, sizeof(RSPControlParameters));
#if _DEBUG
	printf("\nConfig");
#endif
}

void Init() {
	ErcCommand _ercCmd;
	if(_init){
		Erc_Rcds_Init();
#if NOT_DEMO
		
		Erc_Rdp_Init();
		Erc_Rsp_Init();
		Erc_Rf_Init();
		externalhost_Init();

		Erc_Rdp_Open();
		Erc_Rsp_Open();

		/*Erc_Rsp_Write(char* _buff, int _memAddr);
		Erc_Rdp_Write(char* _buff, int _memAddr);
		Erc_Rf_Write(char* _buff);*/


#else
		_ercCmd = { 0xFFFF,INIT ,ERC ,ALL ,0xFFFF };
		memcpy(_sendRspBuff, &_ercCmd, sizeof(_ercCmd));
		memcpy(_sendRdpBuff, &_ercCmd, sizeof(_ercCmd));
		memcpy(_sendRfBuff, &_ercCmd, sizeof(_ercCmd));
		_writeRspFlag = true;
		_writeRdpFlag = true;
		_writeRfFlag = true;
		WriteData();

#endif
		_init =false;
	}
}

void Shutdown() {
	ErcCommand _ercCmd;
	if (_init) {
		_ercCmd = { 0xFFFF,SHUTDOWN ,ERC ,ALL ,0xFFFF };
		memcpy(_sendRspBuff, &_ercCmd, sizeof(_ercCmd));
		_writeSSFlag = true;
		_writeRfFlag = true;
		WriteData();
		_init = false;
	}
#if _DEBUG
	printf("Shutdown");
#endif	
}

void DebugDump() {
#if _DEBUG
	printf("DebugDump");
#endif
}

void DebugSend() {
#if _DEBUG
	printf("DebugSend");
#endif	
}


void DataTransfer() {
	int _result = 1; 
	char _dataBuff[MAX_BUFF];
	//Rcds Data Transfer
	_result = QPop(_dataBuff,_rcdsMemBuff,_rcdsFront,_rcdsRear);
	if (_result == 0) {
		Parsing(_dataBuff,sizeof(_dataBuff));
	}

	//RSP Data Transfer
	_result = QPop(_dataBuff, _rspMemBuff, _rspFront, _rspRear);
	if (_result == 0) {
		Parsing(_dataBuff,sizeof(_dataBuff));
	}

	//RDP Data Transfer
	_result = QPop(_dataBuff, _rdpMemBuff, _rdpFront, _rdpRear);
	if (_result == 0) {
		Parsing(_dataBuff, sizeof(_dataBuff));
	}
#if _DEBUG
	printf("DataTransfer");
#endif	
}

void Test() {
	char Data[100];
	int DataSize=0;
	BiteInformation bi;
	bi.MSGHDR = 0xFFFF;
	bi.MSGID = 0xF0;
	bi.OrginalSource = RSP;
	bi.Source = ERC;
	bi.Destination = RCDS;
	bi.ADC_STATUS = '10';
	bi.PL_STATUS = '12';
	bi.MOD_FUNC = '13';
	bi.MEM_DEV_C = '13';
	memcpy(Data, &bi, sizeof(bi));
	DataSize = sizeof(Data);
	Ethernetsend(Data, DataSize);
}

void checkRCDSrxbuf() {
	int BuffSize = strlen(RecvDataBuffer);
	if ( BuffSize > 0) {
		_RcvBufferStatusFlag = true;	
	}
	else
	_RcvBufferStatusFlag = false;
#if _DEBUG
	printf("\nRCDS RX buffer\n");
#endif
}

void ExecuteErcCmd(enum Command ch) {
	switch (ch) {
	case  EXEC_DISCRETE_CMD: ExeDiscrereCmd();//todo;
		break;

	case READ_SS_DATA: ReadData(); //todo;
		break;

	case WRITE_SS_DATA:WriteData(); //todo;
		break;

	case RAISE_SERVICE_RQST:RaiseServiceRqst(); //todo;
		break;

	case CONGIF:Config(); //todo;
		break;

	case INIT:Init();
		break;

	case SHUTDOWN:Shutdown();
		break;

	case TEST:Test();
		break;

	case DEBUG_DUMP: DebugDump();//todo;
		break;

	case DEBUG_SEND: DebugSend(); //todo;
		break;

	case DATA_TRANSFER:DataTransfer();
		break;

	case CHECK_RCDS_RX_BUF:checkRCDSrxbuf();
		break;
	
	deafult:
#if _DEBUG
		printf("No Commands Matchs In ExecuteErcCmd");
#endif
		break;
	}
}