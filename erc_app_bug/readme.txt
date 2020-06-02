this is c structure and below is the value values which have given

typedef struct PeriodicTrackData
{
	//B1B0 val 0xFFFF
	unsigned short MSGHDR;
	//B3B2 val 0x0008
	unsigned short  MSGID;
	//B4 val 0x07
	unsigned short Source;
	//B5 val 0x01
	unsigned short Destination;
	//B6 val 0x00 - 0x0F
	unsigned char TrackName;
	//B7 
	union {
		struct {
			//val 000 - 011
			unsigned char T_TYPE : 3;
			//val 000 - 011
			unsigned char T_STATUS : 3;
		} TRK_TYP_S;
		unsigned char TrackType;
	} TRK_TYP_U;
	//B8
	union {
		struct {
			//val 0, 1
			unsigned char TBD : 1;
			//val 00 - 11
			unsigned char TBD1 : 2;
			//val 0000 - 111 
			unsigned char GND_TGT : 3;
			//val 0x0 - 0x3
			unsigned char CFN : 3;
		}TRG_TYP_S;
		unsigned short TargetType;
	}TRG_TYP_U;
	//B9 val 0x00
	unsigned char RSD;
	//B11B10 val 0x0000 - 0xFFFF
	unsigned short RNG;
	// B13B12 val 0x0000 - 0x8C9F
	unsigned short AZN;
	// B15B14 val 0x0000 - 0xFFFF
	unsigned short SPD;
	// B17B16 val 0x0001 - 0x8C9F
	unsigned short HDN;
	//B19B18 val 0xFFFF
	unsigned short CHKSM;
};


these is value

PeriodicTrackData tbr;
			tbr.MSGHDR = 0xFFFF;
			tbr.MSGID = 0x0008;
			tbr.Source = 0x08;
			tbr.Destination =001;
			tbr.TrackName=0x0F;

			tbr.TRK_TYP_U.TRK_TYP_S.T_TYPE =0x1;
			tbr.TRK_TYP_U.TRK_TYP_S.T_STATUS = 0x2;
			
			tbr.TRG_TYP_U.TRG_TYP_S.TBD =0x0;
			tbr.TRG_TYP_U.TRG_TYP_S.TBD1 = 0x2;// 01;
			tbr.TRG_TYP_U.TRG_TYP_S.GND_TGT = 0x5;// 101;
			tbr.TRG_TYP_U.TRG_TYP_S.CFN = 0x3;

			tbr.RSD =0x01;
			tbr.RNG =0x8C9F;
			tbr.AZN =0x00FF;
			tbr.SPD= 0x8C9F;
			tbr.HDN= 0xFFF0;
			tbr.CHKSM = 0xFFFF;