
//RCDS
typedef struct Header {
	unsigned short InterfaceID;
	unsigned short PacketID;
	unsigned short PacketVersion;
	unsigned short PacketPayload;
};

typedef struct RcdsResp {
	unsigned short InterfaceID;
	unsigned short PacketID;
	unsigned short PacketVersion;
	unsigned short PacketPayload;
	unsigned char DataPayload[200];
};

typedef struct ResTestLink {
	Header header;
	unsigned short LinkStatus;
};

typedef struct ResDeviceDetails {
	Header header ;
	unsigned char DeviceName[31];
	unsigned char DeviceAlice[15];
	unsigned char DeviceVersion[15];
	unsigned char SerialNo[15];
	unsigned char ConnectionString[15];
};

typedef struct CommandResponseMessage {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x0009
	unsigned short  MSGID;
	//B4 val 0x07
	SubSystem Source;
	//B5 val 0x01
	SubSystem Destination;
	//B6 val 0x00 - 0xff
	unsigned char CMD_NUM;
	//B7 0x00, 0x01, 0x02
	unsigned char CMD_VLD;
	//TBD
	unsigned char CRB;
	//B19B18 val 0xFFFF
	unsigned char CHKSM;
};

typedef struct RDPStatusSummaryMessage {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000A
	unsigned short  MSGID;
	//B4 val 0x07
	unsigned char Source;
	//B5 val 0x01
	unsigned char Destination;
	//B7B6 val TBD
	unsigned char RDP_VN;
	//B9B8 0 - 99 decimal
	unsigned char CTC;
	//B11B10 val 0 - 150
	unsigned char TTC;
	//B12 val 0 - 99
	unsigned short DATC;
	//B13 val 0 - 99
	unsigned short DGTC;
	//TBD
	//B15 val 0 - 254 0xFF
	unsigned short SSRC;
	//B16 val 0-100 0xFF
	unsigned short STRC;
	//B17 0xF0, 0x0F
	unsigned short SCE;
	//B18 00,01,02,03
	unsigned short STE;
	//B19 0x00
	unsigned short RSD;
	//B21,B20 val tbd
	unsigned char SCN;
	//B25B24B23B22 val tbd
	unsigned char  RTC;
	//B19B18 val 0xFFFF
	unsigned char CHKSM;
};

//RDP
typedef struct EquipmentTypeSerialMessage {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000C
	unsigned short  TYSL;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x20/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x8C9F
	unsigned short SRSRorMRSR;
	//B10 val 0x01 - 0x0A
	unsigned char SLNo;
	//B13B12 val 0xFFFF
	unsigned short CHKSM;
};

typedef struct RadarStatusMessage {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000C
	unsigned short  MSGID;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x20/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x8C9F
	unsigned short OPST;
	//B10 val 0x01 - 0x0A
	unsigned char SSTAT1;
	//B13B12 val 0xFFFF
	unsigned char SSTAT2;
	//
	unsigned char NU;
	//
	unsigned short SBSP;
	//
	unsigned short SBEP;
	//
	unsigned char OPMD;
	//
	unsigned char RSD;
	//
	unsigned short CHKSM;
};

typedef struct LocationReadTimeMessage { //3.3.1.3
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000C
	unsigned short  MSGID;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x20/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x8C9F
	unsigned char RTC;
	//B10 val 0x01 - 0x0A
	unsigned short SEC;
	//B13B12 val 0xFFFF
	unsigned char MIN;
	//
	unsigned char HRS;
	//
	unsigned short YR : 4; //tbd
	//
	unsigned short LATS;
	//
	unsigned char LATM;
	//
	union {
		struct {
			//val 000 - 011
			unsigned char LAT : 1;
			//val 000 - 011
			DirectionLat LATD : 7;
		} LatitudeDirection;
		unsigned char LATITUDEDIRECTION;
	} Latitude;

	unsigned short LONS;
	//
	unsigned char LONM;
	//
	unsigned char LODG;
	//
	unsigned char LON;
	//
	unsigned char CC;
	//B19B18 val 0xFFFF
	unsigned short CHKSM;
};

typedef struct ScanDirectionChangeMessage {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000C
	unsigned short  MSGID;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x20/0x07
	unsigned char Destination;
	//
	unsigned char SDR;
	//
	unsigned char SFR;
	//B19B18 val 0xFFFF
	unsigned short CHKSM;
};

//RSP
typedef struct RSPControlParameters {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x0009
	unsigned short  MSGID;
	//B4 val 0x07
	unsigned char Source;
	//B5 val 0x01
	unsigned char Destination;
	//B6 val
	unsigned char RADAR_TYPE;
	//B7 
	unsigned char FILT_TYPE;
	//TBD
	unsigned int FILT_LEN;
	//TBD
	unsigned char FILT_COEFF;
	//TBD
	unsigned char LEN_FFT_R;
	//TBD
	unsigned char WINDOW_TYPE;
	//TBD
	unsigned char WINDOW_COEFF;
	//TBD
	unsigned char NOISE_CONTROL_R;
	//TBD
	unsigned char LEN_FFT_D;
	//TBD
	unsigned char WINDOW_TYP_D;
	//TBD
	unsigned char WINDOW_COEFF_D;
	//TBD
	unsigned char NOISE_CONTROL_D;
	//TBD
	unsigned char FILT_LEN_C;
	//TBD
	unsigned char CELL_OMITTED;
	//TBD
	unsigned char NORM_COEFF;
	//TBD
	unsigned char NOISE_CONTROL;
	//TBD
	unsigned char METHOD;
	//TBD
	unsigned short METHOD_COEFF;
	//TBD
	unsigned short LONG_TERM_AVG;
	//TBD
	unsigned char SIGN_TYPE;
	//TBD
	unsigned char NOISE_TYPE;
	//TBD
	unsigned char SNR;
	//TBD
	unsigned char HW_TEST;
	//TBD
	unsigned char SW_TEST;
	//TBD
	unsigned char SUB_SYS_TEST;
	//TBD
	unsigned char SAMP_FREQ;
	//TBD
	unsigned char LVDS_LEVEL;
	//TBD
	unsigned char OUTPUT_FORMAT;
	//TBD
	unsigned char ADC_IN_SELECT;
	//TBD
	unsigned char FUTURE_PARAM;
	//TBD
	unsigned char ADC_CONV_CONT_ROL;
	//TBD
	unsigned char HW_TEST_ADC;
	//TBD
	unsigned char RDP_READ_FLAG;
	//TBD
	unsigned char RDP_WRITE_FLAG;
	//TBD
	unsigned char ERC_READ_FLAG;
	//TBD
	unsigned char ERC_WRITE_FLAG;
};

typedef struct DataInput {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x0009
	unsigned short  MSGID;
	//B4 val 0x07
	unsigned char Source;
	//B5 val 0x01
	unsigned char Destination;
	//
	unsigned char INPUT;
	//
	unsigned char REAL_TIME_CLOCK;
	//
	unsigned char HW_LINE;
	//
	unsigned char START_STOP_DATA;
};

//RF
//Tbd todo name has to b conform
struct  RFDataDefination
{
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000B
	unsigned short  MSGID;

	unsigned char OrgSource;
	//B4 val 0x07
	unsigned char Source;
	//B5 val 0x01
	unsigned char Destination;
	//
	unsigned char ADF_PARAM;
	//
	unsigned char RF1_TX_ON;
	//
	unsigned char RF2_TX_OFF;
	//
	unsigned char RF1_SSPA;
	//
	unsigned char RF2_BITE;
	//
	unsigned short CHKSM;
};


//ERC commands Structures
//MSGHDR, CMD, Source, Destination, CHKSM
typedef struct ErcCommand {
	unsigned short MSGHDR;
	unsigned short CMD;
	unsigned char Source;
	unsigned char Destination;
	unsigned short CHKSM;
};
