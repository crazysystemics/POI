#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "IOTask.h"
#include "UDP.h"
#include <math.h>

#define _OPEN_SYS_ITOA_EXT

char* system_names[] = { "P", "Q", "MFWS", "COCKPIT" };

int system_ids[] = { 1, 2, 4, 3 };
unsigned int system_arinc_tx_labels[1];
unsigned int system_arinc_rx_labels[2];
//int system_milbus_in_index[] = { SYSTEM_P_DISCRETE_INDEX + MILBUS_IN_BASE , SYSTEM_Q_DISCRETE_INDEX + MILBUS_IN_BASE };
int analog_system_id[6];
int system_ain_index[6];

//int number_of_bits_received[] = { 1 , 1 };

//int mil_tx_systems[] = { BC_MC , RT_COCKPIT };
//int subAddr[] = {2, 1};
//int numOfWords[] = { 1, 1 };
//int startWord[] = { 0, 0 };
//int endWord[] = { 0, 0 };
//char* system_names[] = { "", "MC", "P", "Q", "MFWS", "COCKPIT"};

double referenceVoltage = 5.0;
int adcResolution = 16;	 
double voltageStep;		 
size_t totalBytes;
unsigned char* byteArray;
int number_of_bits_sent[] = { 1 };
float floatValues[10];
float floatValue[10];
U32 uintValues[10];
U32 uintValue[10];

U32 io_channel = UART;

U32 read_uart()
{

}

void input_task()
{
	// fill the global in by reading all the rx interfaces
	if (io_channel == NONE)
		io_channel = read_uart();
	else
	{
		if (io_channel == UART)
		{
			rx_discrete();
			rx_analog();
			rx_arinc();
			rx_milbus();
		}
		else
		{
			
		}
	}
}

//void input_task()
//{
//	// fill the global in by reading all the rx interfaces
//	if (io_channel == NONE)
//		io_channel = read_uart();
//	else
//	{
//		if (io_channel == UART)
//		{
//			while (all packets are not read)
//			{
//				Read next packet from uart
//					copy to corresponding global in memory
//			}
//		}
//		else
//		{
//			while (all subaddresses are not read)
//			{
//				read packet from next subadderss
//					copy to corresponding global in memory
//			}
//		}
//	}
//}

void output_task()
{
	if (io_channel == NONE)
		io_channel = read_uart();
	else
	{
		if (io_channel == UART)
		{
			tx_arinc();
			tx_discrete();
			tx_analog();
			tx_milbus();

			/*while (all packets are not written)
			{
				copy the corresponding global out memory
				Write next packet to uart

			}*/
		}
		else
		{
			/*while (all subaddresses are not read)
			{
				copy the corresponding global out memory
				Write packet to next subadderss
			}*/
		}
	}
}

void HW_init()
{
	
}

void Ethernet_init()
{
	UDP_ServerSocket(10);
	UDPSocketServer_Clock = UDPSocketServer;
	UDPClient_Clock = UDPClient;

	discrete_tx_udp.port_no = 102;
	UDP_ClientSocket(discrete_tx_udp.port_no);
	memcpy(&discrete_tx_udp.UDPSocketClient, &UDPSocketClient, sizeof(UDPSocketClient));
	memcpy(&discrete_tx_udp.UDPServer, &UDPServer, sizeof(UDPServer));

	discrete_rx_udp.port_no = 101;
	UDP_ServerSocket(discrete_rx_udp.port_no);
	memcpy(&discrete_rx_udp.UDPSocketServer, &UDPSocketServer, sizeof(UDPSocketServer));
	memcpy(&discrete_rx_udp.UDPClient, &UDPClient, sizeof(UDPClient));

	analog_tx_udp.port_no = 202;
	UDP_ClientSocket(analog_tx_udp.port_no);
	memcpy(&analog_tx_udp.UDPSocketClient, &UDPSocketClient, sizeof(UDPSocketClient));
	memcpy(&analog_tx_udp.UDPServer, &UDPServer, sizeof(UDPServer));

	analog_rx_udp.port_no = 201;
	UDP_ServerSocket(analog_rx_udp.port_no);
	memcpy(&analog_rx_udp.UDPSocketServer, &UDPSocketServer, sizeof(UDPSocketServer));
	memcpy(&analog_rx_udp.UDPClient, &UDPClient, sizeof(UDPClient));

	arinc_tx_udp.port_no = 302;
	UDP_ClientSocket(arinc_tx_udp.port_no);
	memcpy(&arinc_tx_udp.UDPSocketClient, &UDPSocketClient, sizeof(UDPSocketClient));
	memcpy(&arinc_tx_udp.UDPServer, &UDPServer, sizeof(UDPServer));

	arinc_rx_udp.port_no = 301;
	UDP_ServerSocket(arinc_rx_udp.port_no);
	memcpy(&arinc_rx_udp.UDPSocketServer, &UDPSocketServer, sizeof(UDPSocketServer));
	memcpy(&arinc_rx_udp.UDPClient, &UDPClient, sizeof(UDPClient));

	milbus_tx_udp.port_no = 402;
	UDP_ClientSocket(milbus_tx_udp.port_no);
	memcpy(&milbus_tx_udp.UDPSocketClient, &UDPSocketClient, sizeof(UDPSocketClient));
	memcpy(&milbus_tx_udp.UDPServer, &UDPServer, sizeof(UDPServer));

	milbus_rx_udp.port_no = 401;
	UDP_ServerSocket(milbus_rx_udp.port_no);
	memcpy(&milbus_rx_udp.UDPSocketServer, &UDPSocketServer, sizeof(UDPSocketServer));
	memcpy(&milbus_rx_udp.UDPClient, &UDPClient, sizeof(UDPClient));
}

int GetTick()
{
	if (UDP_Receive(UDPSocketServer_Clock, UDPClient_Clock) != -1)
	{
		int val;
		memcpy(&val, Buffer_receive, 4);

		return val;
	}
	else
		return 0;
}

BOOLEAN GetSlot(int offset)
{
	//Slot = (tick - offset) % (1) == 0 && tick != offset;
	Slot = tick % offset == 0 && tick != 0;
	return Slot;
}

void convertFloatArrayToBytes(float* floatValues, size_t numFloats, unsigned char* byteArray)
{
	for (size_t i = 0; i < numFloats; i++)
	{
		memcpy(byteArray + i * sizeof(float), &floatValues[i], sizeof(float));
	}
}

void convertBytesToFloat(const unsigned char* byteArray, size_t startIndex, float* floatValue)
{
	memcpy(floatValue, byteArray + startIndex, sizeof(float));
}

void convertBytesToUInts(const unsigned char* byteArray, size_t numBytesPerUint, size_t numUIints, unsigned int* uintValues)
{
	for (size_t i = 0; i < numUIints; i++)
	{
		memcpy(&uintValues[i], byteArray + i * numBytesPerUint, numBytesPerUint);
	}
}

void convertUIntsToBytes(unsigned int* uintValues, size_t numUiInts, unsigned char* byteArray)
{
	for (size_t i = 0; i < numUiInts; i++)
	{
		memcpy(byteArray + i * sizeof(U32), &uintValues[i], sizeof(U32));
	}
}

void tx_arinc()
{
	size_t numUInts;

	memcpy(arinc_tx_udp.Buffer_send, buff_rec, sizeof(buff_rec));
	arinc_tx_udp.Buffer_send[0] = SYSTEM_COCKPIT;
	int index = 0;
	for (int i = ARINC_OUT_BASE; i < ARINC_OUT_OFFSET; i++)
	{
		uintValues[index++] = GLOBAL_OUTPUT_MEMORY[i];
	}
	numUInts = index; //sizeof(uintValues) / sizeof(float);
	totalBytes = (numUInts * sizeof(unsigned int)) + 1;
	byteArray = (unsigned char*)malloc(totalBytes);

	convertUIntsToBytes(uintValues, numUInts, byteArray);
	memcpy(&arinc_tx_udp.Buffer_send[1], byteArray, totalBytes);
	if (UDP_Send(arinc_tx_udp.UDPSocketClient, arinc_tx_udp.UDPServer, arinc_tx_udp.Buffer_send, sizeof(arinc_tx_udp.Buffer_send)) != -1)
	{
		for (int i = 0; i < index; i++)
		{
			if (uintValues[i] != 0)
				printf("\nTick %d  OFP.C : Data Sent ARINC \t\t : Label : %o Value : %x Word : %x", tick, (uintValues[i] & 0x000000ff), (uintValues[i] & 0xffffff00)>>8, uintValues[i]);
		}
	}
}

void rx_arinc()
{
	size_t numBytesPerUInt = sizeof(U32);
	size_t numUInts;
	int startIndex;
	int i, j, k, l;
	for (i = 0; i < sizeof(ARINCInMap) / sizeof(ARINCInMap[0]); i++)
	{
		if (UDP_Receive(arinc_rx_udp.UDPSocketServer, arinc_rx_udp.UDPClient) != -1)
		{
			k = 0;
			numUInts = (sizeof(Buffer_receive) - 1) / numBytesPerUInt;
			startIndex = 1;
			convertBytesToUInts(&Buffer_receive[startIndex], numBytesPerUInt, numUInts, &uintValue);

			for (j = 0; j < sizeof(system_arinc_rx_labels) / sizeof(system_arinc_rx_labels[0]); j++)
			{
				if ((uintValue[k] & 0x0000ff) == system_arinc_rx_labels[j])
				{
					GLOBAL_INPUT_MEMORY[j + ARINC_IN_BASE] = uintValue[k];
					printf("\nTick %d  OFP.C : Data Received ARINC \t\t : Label : %o Value : %x Word : %x", tick, (uintValue[k] & 0x000000ff), (uintValue[k] & 0xffffff00)>>8, uintValue[k]);
					k++;
				}
			}
		}
	}
}

void tx_analog()
{
	size_t numFloats = sizeof(floatValues) / sizeof(float);
	totalBytes = (numFloats * sizeof(float)) + 1;
	byteArray = (unsigned char*)malloc(totalBytes);

	memcpy(analog_tx_udp.Buffer_send, buff_rec, sizeof(buff_rec));
	analog_tx_udp.Buffer_send[0] = SYSTEM_COCKPIT;
	int index = 0;
	for (int i = AO_BASE; i < AO_OFFSET; i++)
	{
		floatValues[index++] = GLOBAL_OUTPUT_MEMORY[i];// *voltageStep;
	}

	convertFloatArrayToBytes(floatValues, numFloats, byteArray);
	memcpy(&analog_tx_udp.Buffer_send[1], byteArray, sizeof(byteArray));
	if (UDP_Send(analog_tx_udp.UDPSocketClient, analog_tx_udp.UDPServer, analog_tx_udp.Buffer_send, sizeof(analog_tx_udp.Buffer_send)) != -1)
	{
		for (int i = 0; i < sizeof(floatValues) / sizeof(floatValues[0]); i++)
		{
			if (floatValues[i] != 0)
				printf("\nTick %d  OFP.C : Data Sent Analog \t\t : %f ", tick, floatValues[i]);
		}
	}
}

void rx_analog()
{
	size_t startIndex;
	int i, j, k, l;
	k = 0;
	startIndex = 1;
	int min_volt = 0;
	int max_volt = 0;
	for (i = 0; i < sizeof(AnalogInMap) / sizeof(AnalogInMap[0]); i++)
	{
		if (UDP_Receive(analog_rx_udp.UDPSocketServer, analog_rx_udp.UDPClient) != -1)
		{
			//memcpy(&analog_rx_udp.Buffer_receive, &Buffer_receive, sizeof(Buffer_receive));
			for (j = 0; j < sizeof(AnalogInMap) / sizeof(AnalogInMap[0]); j++)
			{
				int start = AnalogInMap[j].GlobalMemAddr;
				if (Buffer_receive[0] == AnalogInMap[j].sourcesystemID)
				{
					k = 0;
					startIndex = 1;
					do
					{
						convertBytesToFloat(Buffer_receive, startIndex, &floatValue[k]);
						/*for (int iterm = 0; iterm < sizeof(ICD_Lines) / sizeof(ICD_Lines[0]); iterm++)
						{
							if (strcmp(ICD_Lines[iterm].destinationinterface, "AINPUT") == 0)
							{
								if (ICD_Lines[iterm].bitnumber == AnalogInMap[j].StartBit + k)
								{
									min_volt = ICD_Lines[iterm].minimum;
									max_volt = ICD_Lines[iterm].maximum;
									break;
								}
							}
						}*/
						//GLOBAL_INPUT_MEMORY[start] = round((floatValue[k] * pow(2, (adcResolution - 1))) / (max_volt - min_volt));
						GLOBAL_INPUT_MEMORY[start] = floatValue[k];
						startIndex = startIndex + 4;
						start++;
						printf("\nTick %d  OFP.C : Data Received Analog \t\t : %.4f ", tick, floatValue[k]);
						k++;
					} while (k != AnalogInMap[j].Width);
				}
			}
		}
	}
}

void tx_discrete()
{
	memcpy(discrete_tx_udp.Buffer_send, buff_rec, sizeof(buff_rec));
	discrete_tx_udp.Buffer_send[0] = SYSTEM_COCKPIT;
	for (int i = 0; i < sizeof(U32); i++)
	{
		if (GET_BIT(GLOBAL_OUTPUT_MEMORY[DO_BASE], i) == 1)
			discrete_tx_udp.Buffer_send[i + 1] = 1;
		else
			discrete_tx_udp.Buffer_send[i + 1] = 0;
	}
	if (UDP_Send(discrete_tx_udp.UDPSocketClient, discrete_tx_udp.UDPServer, discrete_tx_udp.Buffer_send, sizeof(discrete_tx_udp.Buffer_send)) != -1)
	{
		for (int i = 0; i < 32; i++)
		{
			if (i < number_of_bits_sent[0])
				printf("\nTick %d  OFP.C : Data Sent Discrete \t\t : %d ", tick, GET_BIT(GLOBAL_OUTPUT_MEMORY[0], i));
		}
	}
}

void rx_discrete()
{
	int i, j, k, l;
	int current_index = 1;
	unsigned int prev_width = 0;
	int prev_systemID = 0;
	for (i = 0; i < sizeof(DiscreteInMap) / sizeof(DiscreteInMap[0]); i++)
	{
		if (DiscreteInMap[i].sourcesystem != DiscreteInMap[i - 1].sourcesystem)
		{
			if (UDP_Receive(discrete_rx_udp.UDPSocketServer, discrete_rx_udp.UDPClient) != -1)
			{
				for (j = 0; j < sizeof(DiscreteInMap) / sizeof(DiscreteInMap[0]); j++)
				{
					if (Buffer_receive[0] == DiscreteInMap[j].sourcesystemID)
					{
						if (DiscreteInMap[j].sourcesystemID == prev_systemID)
							current_index = current_index + prev_width;
						else
							current_index = 1;
						k = DiscreteInMap[j].StartBit;
						int l = current_index;
						while ((l) != (current_index + DiscreteInMap[j].Width))
						{
							if (Buffer_receive[l] == 1)
								GLOBAL_INPUT_MEMORY[DiscreteInMap[j].GlobalMemAddr] = SET_BIT(GLOBAL_INPUT_MEMORY[DiscreteInMap[j].GlobalMemAddr], k);
							else
								GLOBAL_INPUT_MEMORY[DiscreteInMap[j].GlobalMemAddr] = CLR_BIT(GLOBAL_INPUT_MEMORY[DiscreteInMap[j].GlobalMemAddr], k);
							k++;

							printf("\nTick %d  OFP.C : Data Received Discrete \t\t : %d ", tick, Buffer_receive[l]);
							l++;
						}
						prev_width = DiscreteInMap[j].Width;
						prev_systemID = DiscreteInMap[j].sourcesystemID;
					}
				}
			}
		}
	}
}

void tx_milbus()
{
	size_t numUInts;
	memcpy(milbus_tx_udp.Buffer_send, buff_rec, sizeof(buff_rec));

	int j;
	for (j = 0; j < sizeof(MILBUSOutMap) / sizeof(MILBUSOutMap[0]); j++)
	{
		milbus_tx_udp.Buffer_send[0] = MILBUSOutMap[j].sourceRTID;
		milbus_tx_udp.Buffer_send[1] = MILBUSOutMap[j].SubAddr;
		milbus_tx_udp.Buffer_send[2] = MILBUSOutMap[j].NumOfWords;
		milbus_tx_udp.Buffer_send[3] = MILBUSOutMap[j].destinationRTID;
		milbus_tx_udp.Buffer_send[4] = MILBUSOutMap[j].SubAddr;
		milbus_tx_udp.Buffer_send[5] = MILBUSOutMap[j].StartWord;
		milbus_tx_udp.Buffer_send[6] = MILBUSOutMap[j].EndWord;

		int index = 0;
		int count = MILBUSOutMap[j].GlobalMemAddr + MILBUSOutMap[j].NumOfWords;
		for (int i = MILBUSOutMap[j].GlobalMemAddr; i < count; i++)
		{
			uintValues[index++] = GLOBAL_OUTPUT_MEMORY[i];
		}
		numUInts = index;// sizeof(uintValues) / sizeof(float);
		totalBytes = (numUInts * sizeof(U32)) + 1;
		byteArray = (unsigned char*)malloc(totalBytes);

		convertUIntsToBytes(uintValues, numUInts, byteArray);
		memcpy(&milbus_tx_udp.Buffer_send[7], byteArray, totalBytes);
		if (UDP_Send(milbus_tx_udp.UDPSocketClient, milbus_tx_udp.UDPServer, milbus_tx_udp.Buffer_send, sizeof(milbus_tx_udp.Buffer_send)) != -1)
		{
			for (int i = 0; i < MILBUSOutMap[j].NumOfWords; i++)
			{
				printf("\nTick %d  OFP.C : Data Sent MILBUS \t\t : Source : %s Destination : %s SubAddr : %d Word : %d", tick, MILBUSOutMap[j].sourcesystem,
					MILBUSOutMap[j].destinationsystem, MILBUSOutMap[j].SubAddr, uintValues[i]);
			}
		}
	}
}

void rx_milbus()
{
	size_t numBytesPerUInt = sizeof(U32);
	size_t numUInts;
	int startIndex;
	int i, j, k, l;

	for (i = 0; i < sizeof(MILBUSInMap) / sizeof(MILBUSInMap[0]); i++)
	{
		if (UDP_Receive(milbus_rx_udp.UDPSocketServer, milbus_rx_udp.UDPClient) != -1)
		{
			//memcpy(&milbus_rx_udp.Buffer_receive, &Buffer_receive, sizeof(Buffer_receive));
			for (j = 0; j < sizeof(MILBUSInMap) / sizeof(MILBUSInMap[0]); j++)
			{
				int start = MILBUSInMap[j].GlobalMemAddr;
				if (Buffer_receive[0] == MILBUSInMap[j].sourceRTID
					&& Buffer_receive[3] == MILBUSInMap[j].destinationRTID)
				{
					k = 0;
					numUInts = (sizeof(Buffer_receive) - 1) / numBytesPerUInt;
					startIndex = 7;
					do
					{
						convertBytesToUInts(&Buffer_receive[startIndex], numBytesPerUInt, numUInts, &uintValue[k]);
						GLOBAL_INPUT_MEMORY[start] = uintValue[k];
						startIndex = startIndex + 4;
						start++;
						printf("\nTick %d  OFP.C : Data Received MILBUS \t\t : Source : %s Destination : %s SubAddr : %d Word : %d", tick, MILBUSInMap[j].sourcesystem,
							MILBUSInMap[j].destinationsystem, Buffer_receive[1], uintValue[k]);
						k++;
						//convertBytesToUInts(&Buffer_receive[startIndex], numBytesPerUInt, numUInts, &uintValue[k]);
					} while (k != MILBUSInMap[j].NumOfWords);
				}
			}
		}
	}
}

U32 SET_BIT(U32 word, int bit)
{
	word = word | (1 << bit);
	return word;
}

U32 CLR_BIT(U32 word, int bit)
{
	word = word & ~(1 << bit);
	return word;
}

int GET_BIT(U32 word, int bit)
{
	return((word & (1 << bit)) >> bit);
}

void GetIOTaskMapping()
{
	FILE* Input_file = fopen("Example_IOTaskMapping.csv", "r");

	if (Input_file == NULL)
	{
		perror("unable to open the file");
	}
	char* newterms[10];
	char newline[50];

	char line[50];

	int new_id = 0;
	char* terms[10];
	int din_count = 0;
	int ain_count = 0;
	int arinc_out_count = 0;
	int arinc_in_count = 0;
	int mil_in_count = 0;
	int mil_out_count = 0;

	while (fgets(line, sizeof(line), Input_file))
	{
		char* token;
		token = strtok(line, ",");
		int i = 0;

		while (token != NULL)
		{
			terms[i++] = token;
			token = strtok(NULL, ",");
		}

		if (strcmp(terms[0],"DINPUT") == 0)
		{
			DiscreteInMap[din_count].id = din_count;
			strcpy(DiscreteInMap[din_count].sourceinterface, terms[0]);
			strcpy(DiscreteInMap[din_count].sourcesystem, terms[1]);
			DiscreteInMap[din_count].GlobalMemAddr = atoi(terms[2]);
			DiscreteInMap[din_count].StartBit = atoi(terms[3]);
			DiscreteInMap[din_count].Width = atoi(terms[4]);

			for (int i = 0; i < sizeof(system_names) / sizeof(system_names[0]); i++)
			{
				if (strcmp(DiscreteInMap[din_count].sourcesystem, system_names[i]) == 0)
				{
					DiscreteInMap[din_count].sourcesystemID = system_ids[i];
				}
			}
			din_count++;
		}

		if (strcmp(terms[0],"AINPUT") == 0)
		{
			AnalogInMap[ain_count].id = ain_count;
			strcpy(AnalogInMap[ain_count].sourceinterface, terms[0]);
			strcpy(AnalogInMap[ain_count].sourcesystem, terms[1]);
			AnalogInMap[ain_count].GlobalMemAddr = atoi(terms[2]);
			AnalogInMap[ain_count].StartBit = atoi(terms[3]);
			AnalogInMap[ain_count].Width = atoi(terms[4]);
			for (int i = 0; i < sizeof(system_names) / sizeof(system_names[0]); i++)
			{
				if (strcmp(AnalogInMap[ain_count].sourcesystem, system_names[i]) == 0)
				{
					AnalogInMap[ain_count].sourcesystemID = system_ids[i];
					analog_system_id[ain_count] = system_ids[i];
				}
			}
			system_ain_index[ain_count] = atoi(terms[3]);
			ain_count++;
		}

		if (strcmp(terms[0],"ARINC_TX") == 0)
		{
			unsigned int deciLabel = 0;
			unsigned int octaLabel = 0;
			unsigned int msbWeight = 1;
			unsigned int digit;
			deciLabel = atoi(terms[1]);
			while (deciLabel > 0)
			{
				digit = deciLabel % 10;
				octaLabel = digit * msbWeight + octaLabel;
				msbWeight = msbWeight * 8;
				deciLabel = deciLabel / 10;
			}
			strcpy(ARINCOutMap[arinc_out_count].sourceinterface, terms[0]);
			ARINCOutMap[arinc_out_count].id = arinc_out_count;
			ARINCOutMap[arinc_out_count].label = octaLabel;
			ARINCOutMap[arinc_out_count].GlobalMemAddr = atoi(terms[2]);
			system_arinc_tx_labels[arinc_out_count] = octaLabel;
			arinc_out_count++;
		}

		if (strcmp(terms[0],"ARINC_RX") == 0)
		{
			unsigned int deciLabel = 0;
			unsigned int octaLabel = 0;
			unsigned int msbWeight = 1;
			unsigned int digit;
			deciLabel = atoi(terms[1]);
			while (deciLabel > 0)
			{
				digit = deciLabel % 10;
				octaLabel = digit * msbWeight + octaLabel;
				msbWeight = msbWeight * 8;
				deciLabel = deciLabel / 10;
			}
			strcpy(ARINCInMap[arinc_in_count].sourceinterface, terms[0]);
			ARINCInMap[arinc_in_count].id = arinc_in_count;
			ARINCInMap[arinc_in_count].label = octaLabel;
			ARINCInMap[arinc_in_count].GlobalMemAddr = atoi(terms[2]);
			system_arinc_rx_labels[arinc_in_count] = octaLabel;
			arinc_in_count++;
		}

		if (strcmp(terms[0], "MILIN") == 0)
		{
			MILBUSInMap[mil_in_count].id = mil_in_count;
			strcpy(MILBUSInMap[mil_in_count].sourceinterface, terms[0]);
			MILBUSInMap[mil_in_count].sourceRTID = atoi(terms[2]);
			strcpy(MILBUSInMap[mil_out_count].sourcesystem, terms[1]);
			MILBUSInMap[mil_in_count].destinationRTID = atoi(terms[4]);
			strcpy(MILBUSInMap[mil_out_count].destinationsystem, terms[3]);
			MILBUSInMap[mil_in_count].GlobalMemAddr = atoi(terms[5]);
			MILBUSInMap[mil_in_count].SubAddr = atoi(terms[6]);
			MILBUSInMap[mil_in_count].NumOfWords = atoi(terms[7]);
			MILBUSInMap[mil_in_count].StartWord = atoi(terms[8]);
			MILBUSInMap[mil_in_count].EndWord = atoi(terms[9]);
			mil_in_count++;
		}

		if (strcmp(terms[0], "MILOUT") == 0)
		{
			MILBUSOutMap[mil_out_count].id = mil_out_count;
			strcpy(MILBUSOutMap[mil_out_count].sourceinterface, terms[0]);
			MILBUSOutMap[mil_out_count].sourceRTID = atoi(terms[2]);
			strcpy(MILBUSOutMap[mil_out_count].sourcesystem, terms[1]);
			MILBUSOutMap[mil_out_count].destinationRTID = atoi(terms[4]);
			strcpy(MILBUSOutMap[mil_out_count].destinationsystem, terms[3]);
			MILBUSOutMap[mil_out_count].GlobalMemAddr = atoi(terms[5]);
			MILBUSOutMap[mil_out_count].SubAddr = atoi(terms[6]);
			MILBUSOutMap[mil_out_count].NumOfWords = atoi(terms[7]);
			MILBUSOutMap[mil_out_count].StartWord = atoi(terms[8]);
			MILBUSOutMap[mil_out_count].EndWord = atoi(terms[9]);
			mil_out_count++;
		}
	}

}