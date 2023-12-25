#pragma once
#include<Windows.h>
#include "Port.h"
#include "UDP.h"

#define UART			0
#define MIL1553B		1
#define NONE			2

#define IOTASKIN_OFFSET  1
#define OFP_OFFSET		 1
#define IOTASKOUT_OFFSET 1

#define RT_P		2
#define RT_Q		3
#define RT_MFWS		4
#define RT_COCKPIT	5
#define BC_MC		1

#define SYSTEM_P		1
#define SYSTEM_Q		2
#define SYSTEM_MFWS		4
#define SYSTEM_COCKPIT	3

#define SYSTEM_P_DISCRETE_INDEX			0
#define SYSTEM_Q_DISCRETE_INDEX			1
#define SYSTEM_COCKPIT_DISCRETE_INDEX	0


#define U32 unsigned int

int tick;
float a, b;
BOOLEAN Slot;

// sglobals
struct UDP discrete_tx_udp;
struct UDP discrete_rx_udp;
struct UDP analog_tx_udp;
struct UDP analog_rx_udp;
struct UDP arinc_tx_udp;
struct UDP arinc_rx_udp;
struct UDP milbus_tx_udp;
struct UDP milbus_rx_udp;

#define DI_BASE				0
#define AI_BASE				20
#define ARINC_IN_BASE		40
#define MILBUS_IN_BASE		60

#define DO_BASE				0
#define AO_BASE				20
#define ARINC_OUT_BASE		40
#define MILBUS_OUT_BASE		60

#define DI_OFFSET			10
#define AI_OFFSET			30
#define ARINC_IN_OFFSET		1
#define MILBUS_IN_OFFSET	70

#define DO_OFFSET			1
#define AO_OFFSET			21
#define ARINC_OUT_OFFSET	41
#define MILBUS_OUT_OFFSET	61

#define SIZE_OF_GLOBAL_IN	100
#define SIZE_OF_GLOBAL_OUT	100

U32 GLOBAL_INPUT_MEMORY[SIZE_OF_GLOBAL_IN];
U32 GLOBAL_OUTPUT_MEMORY[SIZE_OF_GLOBAL_OUT];

float a;
float b;

U32 a1;
U32 b1;
U32 c1;

U32 a2;
U32 b2;
U32 c2;

float c;

typedef struct
{
	int id;
	char sourcesystem[30];
	int sourcesystemID;
	char sourceinterface[10];
	unsigned int GlobalMemAddr;
	unsigned int StartBit;
	unsigned int Width;
}discrete_analog_iotask_mapping;

discrete_analog_iotask_mapping DiscreteInMap[2];
discrete_analog_iotask_mapping AnalogInMap[2];

discrete_analog_iotask_mapping DiscreteOutMap[1];

typedef struct
{
	int id;
	char sourceinterface[10];
	unsigned int label;
	unsigned int GlobalMemAddr;
}arinc_iotask_mapping;

arinc_iotask_mapping ARINCOutMap[1];
arinc_iotask_mapping ARINCInMap[2];


typedef struct
{
	int id;
	char sourceinterface[10];
	int sourceRTID;
	char sourcesystem[30];
	int destinationRTID;
	char destinationsystem[30];
	unsigned int GlobalMemAddr;
	unsigned int SubAddr;
	unsigned int NumOfWords;
	unsigned int StartWord;
	unsigned int EndWord;
}mil_iotask_mapping;

mil_iotask_mapping MILBUSInMap[2];
mil_iotask_mapping MILBUSOutMap[1];


SOCKET UDPSocketServer_P;
struct sockaddr_in UDPClient_P;

SOCKET UDPSocketServer_Q;
struct sockaddr_in UDPClient_Q;

SOCKET UDPSocketServer_Clock;
struct sockaddr_in UDPClient_Clock;

void HW_init();

int GetTick();
BOOLEAN GetSlot(int offset);

void tx_arinc();
void rx_arinc();

void tx_analog();
void rx_analog();

void tx_discrete();
void rx_discrete();

void tx_milbus();
void rx_milbus();

U32 SET_BIT(U32 word, int bit);
U32 GET_BIT(U32 word, int bit);
U32 CLR_BIT(U32 word, int bit);