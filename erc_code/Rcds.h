

typedef struct RcdsReq {
	unsigned short InterfaceID;
	unsigned short packetId;
	unsigned short version;
	//unsigned short PacketPayload;
};


typedef struct RadarOperationCommand {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000B
	unsigned short  MSGID;
	//B4 val 0x07
	unsigned char Source;
	//B5 val 0x01
	unsigned char Destination;
	//B9B8B7B6 val tbd
	unsigned int DSNA; //not sure
	//B11B10 val tbd
	unsigned short OMC;
	//B13B12 val tbd
	unsigned short SSL;
	//B15B14 val tbd
	unsigned short SEL;
	//B16 val tbd
	unsigned char SR;
	//B17 val tbd
	unsigned char CLC;
	//B13B12 val 0xFFFF
	unsigned short CHKSM;
};


typedef struct RadiationBlanckSector {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000C
	unsigned short  MSGID;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x20/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x8C9F
	unsigned short BSA;
	//B10 val 0x01 - 0x0A
	unsigned char BS;
	//B11 val 0x00, 0x01
	unsigned char OPN;//tbd clear and create is there we have to make enum?
	//B13B12 val 0xFFFF
	unsigned char CHKSM;
};


typedef struct ManualInitionZone {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000D
	unsigned short  MSGID;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x2710
	unsigned short MZSR;
	//B9B18 val 0x0000 - 0x8C9F 
	unsigned short MZSA;
	//B11B10 val 0x0000 - 0x2710
	unsigned short MZER;
	//B13B12 val 0x0000 - 0x8C9F 
	unsigned short  MZEA;
	//B14 val 0x01 - 0x0A 
	unsigned char MZSN;
	//B15 val 0x00 - 0x01 
	unsigned char OPN;
	//B17B16 val 0xFFFF
	unsigned short  CHKSM;
};


typedef struct RateZone {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000E
	unsigned short  MSGID;
	//B4 val 0x09/0x001
	unsigned char Source;
	//B5 val 0x01/0x07
	unsigned char Destination;
	//B8B7 val 0x0000 - 0x2710 
	unsigned short MZSR;
	//B9B8 val 0x0000 - 0x8C9F 
	unsigned short MZSA;
	//B11B10 val 0x0000 - 0x2710 
	unsigned short MZER;
	//B13B12 val 0x0000 - 0x8C9F
	unsigned short  MZEA;
	//B14 val 0x01 - 0x0A 
	unsigned char MZSN;
	//B15 val 0x00  - 0x01 
	unsigned char OPN;
	//B17B16 val 0xFFFF
	unsigned short  CHKSM;
};


typedef struct TrackCommunicationCommand {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x000F
	unsigned short  MSGID;
	//B4 val 0x09
	unsigned char Source;
	//B5 val 0x07
	unsigned char Destination;
	//B6 val 0x00 - 0x01
	unsigned char TBD;
	//B7 val tbd
	unsigned char TBD1;
	//B9B8 val 0x0000 - 0x8C9F
	unsigned short RSSA;
	//B13B12 val 0x0000 - 0x8C9F
	unsigned short RSEA;
	//B15B14 val 0x0000 - 0x8C9F
	unsigned short LSSA;
	//B17B16 val 0x0000 - 0x0BBB
	unsigned short LSEA;
	//B13B14 val 0x0000 - 0x8C9F
	unsigned short STSP;
	//B19B18 val 0x000A - 0x0BBB
	unsigned short EDSP;
	//B21B20 val TBD
	unsigned short TRNM;
	//B23B22 val 0xFFFF
	unsigned short CHKSM;
};

//MSGHDR, MSGID, Source, Destination, RP, AP, CHECKSM
typedef struct TrackInitiateCommand {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x00011
	unsigned short  MSGID;
	//B4 val 0x09/0x01
	unsigned short Source;
	//B5 val 0x01/0x07
	unsigned short Destination;
	//B7B6 val 0x0000 - 0x03E7
	unsigned short RP;
	//B9B8 val 0x0000 - 0x8C9F
	unsigned short AP;
	//B15B14 val 0xFFFF
	unsigned short CHKSM;
};


typedef struct TrackInitiateWithVectorCommand {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x00011
	unsigned short  MSGID;
	//B4 val 0x09/0x01
	unsigned char Source;
	//B5 val 0x01/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x03E7
	unsigned short RP;
	//B9B8 val 0x0000 - 0x8C9F
	unsigned short AP;
	//B11B10 val 0x0000 - 0x0CC8
	unsigned short SPD;
	//B13B12 val 0x0000 - 0x0167
	unsigned short DRN;
	//B15B14 val 0xFFFF
	unsigned short CHKSM;
};


typedef struct TrackDeleteCommand {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x00012
	unsigned short  MSGID;
	//B4 val 0x09/0x01
	unsigned char Source;
	//B5 val 0x01/0x07
	unsigned char Destination;
	//B7B6 val 0x0000 - 0x03E7
	unsigned short TN;
	//B13B12 val 0xFFFF
	unsigned short CHKSM;
};
