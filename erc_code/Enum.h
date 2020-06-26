#ifndef _SOME
int some = 0;
#define _SOME

//ERC
typedef enum SubSystem { RF, RSP, RDP, ERC, RCDS, ALL };

typedef enum  Command {
	EXEC_DISCRETE_CMD = 1, READ_SS_DATA, WRITE_SS_DATA, RAISE_SERVICE_RQST, RETURN_SERVICE_RSLT, CONGIF, INIT, SHUTDOWN, TEST, DECONFIG,
	CONFIG_ERC_SCHEDULE, MARK_FOR_DEBUG_CONSOLE, CLEAR_FOR_DEBUG_CONSOLE, DEBUG_DUMP, DEBUG_SEND, DATA_TRANSFER,
	CHECK_RCDS_RX_BUF, SEND_RSLT_TO_RCDS
};

typedef enum Address {
	START_SRC_READ_ADDR = 2455, END_SRC_READ_ADDE = 2555,
	START_SRC_WRITE_ADDR = 2556, END_SRC_WRITE_ADDR = 2655,
	START_DEST_READ_ADDR = 2656, END_DEST_READ_ADDR = 2755,
	START_DEST_WRITE_ADDR = 2756, END_DEST_WRITE_ADDR = 2855
};

//RCDS
typedef enum InterfaceIDs
{
	/*Used for identifying the packet transfer from where to where.
	Ranges[01 to 10]
	*/
	INTERFACE_ID_ETHERNET_PLUGIN_ERC = 01,
	INTERFACE_ID_ERC_ETHERNET_PLUGIN = 02,
};

typedef enum PacketIds
{
	//Request Packets. Ranges[1 to 10]
	REQ_LINK_STATUS = 01,
	REQ_RADAR_REDINESS = 02,
	REQ_ERC_DEVICE_DETAILS = 03,

	//Response packets. Ranges[11 to 20] 
	RESP_LINK_STATUS = 11,
	RESP_RADAR_REDINESS = 12,
	RESP_ERC_DEVICE_DETAILS = 13,
	RESP_BROADCAST_INFO = 14,

	//Command Packets. Ranges[21 to 30]
	CMD_SET_TIME_STAMP = 21,
	CMD_SET_RADAR_HEADER = 22,
	CMD_START_SCANNING = 23,
	CMD_START_VIDEO_STREAM = 24,
	CMD_STOP_SCANNING = 25,
	CMD_STOP_VIDEO_STREAM = 26,

	//Data packets. Ranges[31 to 40]
	DATA_SCANNED = 31,
	DATA_VIDEO = 32,

	//Notification packets. Ranges[41 to 50]
	NOTIFY_END_SCAN = 41,
	NOTIFY_APP_CLOSE = 42,
	NOTIFY_ERROR_INFO = 43,
};

typedef enum MsgVersion
{
	MSG_VERSION = 1,
};

typedef enum LinkStatuses{
	OK = 0,
	NOT_OK = 1
};

//RSP
typedef enum RADAR_TYP {
	SR_FSA = 0x01,
	MR_FSA = 0X02,
	SR_MIMO= 0X03,
	//Reserved
};

typedef enum FILT_TYP {
	 FIR = 0x01,
	 IIR = 0x02,
	 FIR_Butterworth = 0x03,
	 FIR_Chebyshev = 0x04,
	 FIR_Equiripple = 0x05,
	
	 //Reserved,
};

typedef enum FILT_LEN {
	FL_16 = 0x10,
	FL_32 = 0x20,
	FL_64 = 0x40,
	FL_128 = 0x80,
};

typedef enum FILT_COEFF {
	 FC_16_bytes = 0x01,
	 FC_32_bytes = 0x02,
	 Floating_point = 0x03,
	 Fixed_point = 0x04,
	 //Reserved
};

typedef enum LEN_FFT_R {
	RL_512	= 0x01,
	RL_1024	= 0x02,
	RL_2048	= 0x03,
	RL_4096	= 0x04,
	RL_8192	= 0x05,
	RL_16384 = 0x06,
};

typedef enum WINDOW_TY {
	Rectangular= 0x01,
	Hanning	= 0x02,
	Hamming	= 0x03,
	Blackman = 0x04,
};

typedef enum NOISE_CONTROL {
	Enable = 0x01,
	Disable = 0x02,
};

typedef enum LEN_FFT_D {
	LDF_128 = 0x01,
	LDF_256 = 0x02,
	LDF_512 = 0x03,
	LDF_1024 = 0x04,
};

typedef enum FILT_LEN_C {
	FL_6 = 0x05,
	FL_11 = 0x06,
	FL_21 = 0x07,
	//Reserved
};

typedef enum CELL_OMITTED{
	CO_2 = 0x01,
	CO_4 = 0x02,
	CO_6 = 0x03,
	CO_8 = 0x04,
};

 typedef enum NORM_COEFF {
	 NC_1div10 = 0x20,
	 NC_1div8 = 0x40,
	 NC_1div6 = 0x60,
	 NC_1div4 = 0x80,
 };

typedef enum  SIG_TYPE {
	Sine = 0x01,
	Cosine = 0x02,
	SineCosine = 0x03,
	Sweep = 0x04,
};

typedef enum NOISE_TYPE {
	PRNG = 0x01,
	WGN = 0x02,

};

typedef enum SNR {
	Signal_Amplitude_control = 0x01,
	Noise_Amplitude_control	= 0x02,
};

typedef enum HW_TEST {
	Complete_Test = 0x01,
	ADC_Test = 0x02,
	SRAM_Test = 0x03,
	SDRAM_Test = 0x04,
	PL_Test = 0x05,
	ADC_Interface_Test = 0x06,
	Memory_Interface_Test = 0x07,
	DDR_Data_Pass_Test = 0x08,
};

typedef enum SW_TEST {
	Data_Acquisition = 0x01,
	FIR_Module_Test = 0x02,
	FFT_Range = 0x03,
	FFT_Doppler = 0x04,
	Memory_Interface = 0x05,
	AXI_DMA_Test = 0x06,
	MIG_Test = 0x07,
	Cell_Averaging = 0x08,
};

typedef enum SUB_SYS_TEST {
	Complete_Internal_Test = 0x01,
	External_Test = 0x02,
};

typedef enum HW_TEST_ADC {
	ON = 0x01,
	OFF = 0x02,
};



//RDP
typedef enum DirectionScan {
	LeftToRight = 0,
	RightToLeft = 1,
};

typedef enum DirectionLan {
	East,
	West
};
typedef enum DirectionLat {
	North,
	South
};

typedef enum Operation { clear, create };



#endif
