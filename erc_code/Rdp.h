
//MSGHDR, MSGID, SOURCE, DESTINATION
typedef struct SubSysHeader {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000B
	unsigned short  MSGID;
	//B4 val 0x07
	unsigned short Source;
	//B5 val 0x01
	unsigned short Destination;
};


typedef struct TrackBeamRequest {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000C
	unsigned short  TYSL;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x20/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x8C9F
	union {
		struct {
			unsigned char RSD : 3;
			unsigned char RTP : 3;
			unsigned char T_ID : 2;
		}Reservied;
	}UnionReservied;
	//
	unsigned char T_ID : 8;//tbd conform
	//B10 val 0x01 - 0x0A
	unsigned short TR;
	//
	unsigned short TA;
	//
	unsigned short RTCD;
	//B15B14 val 0xFFFF
	unsigned short CHKSM;
};

typedef struct TrackBeamRequestTest {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000C
	unsigned short  TYSL;
	//B4 val 0x09/0x001
	unsigned short Source;
	//B5 val 0x01/0x20/0x07
	unsigned short Destination;
	//B7B6 val 0x0000 - 0x8C9F
	
	//
	unsigned short T_ID ;//tbd conform
	//B10 val 0x01 - 0x0A
	unsigned short TR;
	//
	unsigned short TA;
	//
	unsigned short RTCD;
	//B15B14 val 0xFFFF
	unsigned short CHKSM;
};

//todo
//typedef struct PeriodicTrackData
//{
//	//B1B0 val 0xFFFF
//	unsigned short MSGHDR;
//	//B3B2 val 0x0008
//	unsigned short  MSGID;
//	//B4 val 0x07
//	SubSystem Source;
//	//B5 val 0x01
//	SubSystem Destination;
//	//B6 val 0x00 - 0x0F
//	unsigned char TrackName[16];
//	//B7 
//	union {
//		struct {
//			//val 000 - 011
//			unsigned char T_TYPE : 3;
//			//val 000 - 011
//			unsigned  char T_STATUS : 3;
//		} TYPE;
//		unsigned char word;
//	} TargetType;
//	//B8
//	union {
//		struct {
//			//val 0, 1
//			unsigned char TBD : 1;
//			//val 00 - 11
//			unsigned char TBD1 : 2;
//			//val 0000 - 111 
//			unsigned char GND_TGT : 3;
//			//val 0x0 - 0x3
//			unsigned char CFN : 3;
//		};
//	}	TGT_TYP;
//	//B9 val 0x00
//	unsigned char RSD;
//	//B11B10 val 0x0000 - 0xFFFF
//	unsigned short RNG;
//	// B13B12 val 0x0000 - 0x8C9F
//	unsigned short AZN;
//	// B15B14 val 0x0000 - 0xFFFF
//	unsigned short SPD;
//	// B17B16 val 0x0001 - 0x8C9F
//	unsigned short HDN;
//	//B19B18 val 0xFFFF
//	unsigned short CHKSM;
//};
typedef struct PeriodicTrackData
{
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x0008
	unsigned short  MSGID;
	//B4 val 0x07
	unsigned char Source;
	//B5 val 0x01
	unsigned char Destination;
	//B6 val 0x00 - 0x0F
	unsigned char TrackName;
	//B7 
	
			//val 000 - 011
			unsigned char T_TYPE;
			//val 000 - 011
			unsigned  char T_STATUS;
		
	//B8
	
			//val 0, 1
			unsigned char TBD;
			//val 00 - 11
			unsigned char TBD1;
			//val 0000 - 111 
			unsigned char GND_TGT;
			//val 0x0 - 0x3
			unsigned char CFN;
		
	//B9 val 0x00
	unsigned char RSD;
	//B11B10 val 0x0000 - 0xFFFF
	unsigned short RNG;
	// B13B12 val 0x0000 - 0x8C9F
	signed short AZN;
	// B15B14 val 0x0000 - 0xFFFF
	unsigned short SPD;
	// B17B16 val 0x0001 - 0x8C9F
	unsigned short HDN;
	//B19B18 val 0xFFFF
	unsigned short CHKSM;
};

