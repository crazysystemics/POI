typedef struct BiteInformation {
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x0009
	unsigned short  MSGID;
	//
	unsigned char OrginalSource;
	//B4 val 0x07
	unsigned char Source;
	//B5 val 0x01
	unsigned char Destination;
	//
	unsigned char ADC_STATUS;
	//
	unsigned char PL_STATUS;
	//
	unsigned char MEM_DEV_C;
	//
	unsigned char MOD_FUNC;
	//TBD
	//unsigned char CONGIF;
	unsigned short CHKSM;
};