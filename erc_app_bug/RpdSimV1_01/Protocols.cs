using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RpdSim
{

    


    class PeriodicTrackData
    {
        //B1B0 val 0xFFFF
        public ushort MSGHDR;
        //B3B2 val 0x0008
        public ushort MSGID;
        //B4 val 0x07
        public ushort Source;
        //B5 val 0x01
        public ushort Destination;
        //B6 val 0x00 - 0x0F
        public byte TrackName;
        //B7 
        //public TRACK_TYP TrkTyp;



        //public TARGET_TYP TrgTyp;
        public byte T_TYPE;// : 3;
        //val 000 - 011
        public byte T_STATUS;//: 3;




        public byte TBD;//: 1;
        //val 00 - 11
        public byte TBD1;// : 2;
        //val 0000 - 111 
        public byte GND_TGT;// : 3;
        //val 0x0 - 0x3
        public byte CFN ;//: 3;

        //B9 val 0x00
        public byte RSD;
        //B11B10 val 0x0000 - 0xFFFF
        public ushort RNG;
        // B13B12 val 0x0000 - 0x8C9F
        public ushort AZN;
        // B15B14 val 0x0000 - 0xFFFF
        public ushort SPD;
        // B17B16 val 0x0001 - 0x8C9F
        public ushort HDN;
        //B19B18 val 0xFFFF
        public ushort CHKSM;

        public PeriodicTrackData()
        {

        }
    }







    class Protocols
    {




        public enum InterfaceIDs
        {
            /*Used for identifying the packet transfer from where to where.
            Ranges[01 to 10]
            */
            INTERFACE_ID_ETHERNET_PLUGIN_ERC = 10,
            INTERFACE_ID_ERC_ETHERNET_PLUGIN = 11,
            INTERFACE_ID_ERC_RDP = 20
        };
        public enum PacketIds
        {
            //Request Packets. Ranges[1 to 10]
            REQ_LINK_STATUS = 01,
            REQ_RADAR_REDINESS = 02,
            REQ_ERC_DEVICE_DETAILS = 03,
            REQ_BRDCST_DEVICE_DETAILS = 04,

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
        public enum MsgVersion
        {
            MSG_VERSION = 1,

        };

        public enum StatusValues
        {
            EMPTY = 0,
        };
        //------------------------- PACKET_PAYLOAD_STRUCTURES--------------------------------------
        //------------------------------------PacketHeader Payload-------------------------------------------

        public struct PacketHeader
        {
            public short aInterfaceId;
            public short bPktId;
            public short cPktVersion;
            public short dPayload;
        };//8 Bytes

        //------------------------------------Request packet Payloads----------------------------------------



        //------------------------------------Response packet Payloads----------------------------------------
        //-------------------------------ResponseHealthParameterPayload-------------------------------------
        public struct ResponseHealthParameterPayload
        {
            public float aTxPowerOutput;
            public float bExiterOutput;
            public float cPowerSupply;
        };//12 Bytes

        //-------------------------------ResponseBroadcastPayload-------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ResponseBroadcastPayload
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)] public string DeviceName;//[16];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)] public string DeviceAlias;//[16];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)] public string DeviceVersion;//[16];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)] public string Serail_No;//[16];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)] public string ConnectionString;//[16];
        };
        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 0)]
        public struct ResponseBroadcastOffset
        {
            public PacketHeader t_PacketHeader;
            public ResponseBroadcastPayload t_Responsebrdcst;
        };

        //------------------------------------ResponseHealthParameterOffset----------------------------------------
        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 0)]
        public struct ResponseHealthParameterOffset
        {
            public PacketHeader t_PacketHeader;
            public ResponseHealthParameterPayload t_ResponseHealthPayload;
        };

        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct DoplerData
        {
            public UInt16 BinNumber;
            public double Dopler;
            public Int16 Power;
        };
        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct RadarOneAzStepInfo
        {
            public UInt16 Elevation;
            public UInt16 RangeResolution;
            public Int16 RangeBin;
            public UInt16 TargetCount;
            public Int16 Azimuth;

        };
        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct RadarSignalPacket
        {
            public PacketHeader t_PacketHeader;
            public RadarOneAzStepInfo t_radarOneAzStepInfo;
            public DoplerData t_DoplerData;
        };

        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct TargetPosition
        {
            public double PosX;
            public double PosY;
        };

        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct DoplerDataDisplay
        {
            public UInt16 BinNumber;
            public double Dopler;
            public Int16 Power;
            public Int16 Azimuth;
        };
        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct TargetListTableItems
        {
            public DoplerDataDisplay t_DoplerData;
            public TargetPosition t_TargetPosition;
        };
        //hayath

        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 0)]

        //[StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct HHeader
        {
            public InterfaceIDs InterfaceID;
            public PacketIds PacketID;
            public MsgVersion PacketVersion;
            public short PacketPayload;
        };//8 Bytes
        public enum TestLinkStatusValues
        {
            OK = 0, NOT_OK = 1
        };
        public struct ResTestLink
        {
            public HHeader header;
            public TestLinkStatusValues LinkStatus;
        };

        public struct RcdsReq
        {
            public InterfaceIDs InterfaceID;
            public PacketIds packetId;
            public MsgVersion version;
            // public short PACKET_HEADER_PAYLOAD;
        };





        public enum Command
        {
            EXEC_DISCRETE_CMD, READ_SS_DATA, WRITE_SS_DATA, RAISE_SERVICE_RQST, RETURN_SERVICE_RSLT, CONGIF, INIT, SHUTDOWN, TEST, DECONFIG,
            CONFIG_ERC_SCHEDULE, MARK_FOR_DEBUG_CONSOLE, CLEAR_FOR_DEBUG_CONSOLE, DEBUG_DUMP, DEBUG_SEND, DATA_TRANSFER, TRACK_DATA
        };

        public enum SubSystem { RF = 1, RSP, RDP, ERC, RCDS };



        public enum Radars { radar };//tbd
        public enum Filters { filt, filter };//tbd
        public enum Windows { window };//tbd
        public enum Noises { noise };//tbd
        public enum Signals { signal };
        public enum DopplerWindows { dopplerwindow };

        public struct CmdUnit
        {
            public SubSystem Dest;
            public Command Cmd;
            public SubSystem VSource;
            public SubSystem Src;


            //PRE_CMD_UNIT_FUNC_PTR PrCmdUnitPtr;

            /*unsigned int NoOfParam;
            unsigned char Param[MAX_PARAM];*/
            //char Message[MAX_MSG];
            //memcpy((void*)Message,(void*)(&rspC), sizeof(rspCp));
            //void* memcpy(void *Message, void * &RSP_CP,sizeof(struct RSPControlParameters));

            //Sub-System Area

            //Address FromSrcReadAddr, ToSrcReadAddr;
            //Address FromSrcWriteAddr, ToSrcWriteAddr;

            //Address FromDestReadAddr, ToDestReadAddr;
            //Address FromDestWriteAddr, ToDestWriteAddr;
            //unsigned int srcReadLen, srcWriteLen, destReadLen, destWriteLen;

            //POST_CMD_RUN_FUNC_PTR PostCmdRunFuncPtr;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        [Serializable]
        public struct TrackBeamRequest
        {
            //B1B0 val 0xFFFF
            public ushort MSGHDR;
            //B3B2 val 0x000C
            public ushort MSGID;
            //B4 val 0x09/0x001
            public ushort Source;
            //B5 val 0x01/0x20/0x07
            public ushort Destination;
            //B7B6 val 0x0000 - 0x8C9F
            //track ID
            public ushort T_ID;//tbd conform
                               //B10 val 0x01 - 0x0A track range
            public ushort TR;
            //track azamith
            public ushort TA;
            //RTC for Beam Dwell
            public ushort RTCD;
            //B15B14 val 0xFFFF
            public ushort CHKSM;
        };

        public struct TrackInitiateCommand
        {
            //B1B0 val 0xFFFF
            public ushort MSGHDR;
            //B3B2 val 0x00011
            public ushort MSGID;
            //B4 val 0x09/0x01
            public ushort Source;
            //B5 val 0x01/0x07
            public ushort Destination;
            //B7B6 val 0x0000 - 0x03E7
            public ushort RP;
            //B9B8 val 0x0000 - 0x8C9F
            public ushort AP;
            //B15B14 val 0xFFFF
            public ushort CHKSM;


        };

        public struct RDPHdr
        {
            //B1B0 val 0xFFFF
            public ushort MSGHDR;
            //B3B2 val 0x00011
            public ushort MSGID;
            //B4 val 0x09/0x01
            public ushort Source;
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct TRACK_TYP
        {
            [FieldOffset(0)] 
            public byte T_TYPE;
            [FieldOffset(3)] 
            public byte T_STATUS;

            [FieldOffset(0)]
            public ushort target;
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct TARGET_TYP
        {
            [FieldOffset(0)] 
            public byte TBD;
            [FieldOffset(3)] 
            public byte TBD1;
            [FieldOffset(6)] 
            public byte GND_TGT;
            [FieldOffset(8)] 
            public byte CFN;

            [FieldOffset(0)]
            public ushort target;
        };


        //public struct PeriodicTrackData
        //{
        //    //B1B0 val 0xFFFF
        //    public ushort MSGHDR;
        //    //B3B2 val 0x0008
        //    public ushort MSGID;
        //    //B4 val 0x07
        //    public ushort Source;
        //    //B5 val 0x01
        //    public ushort Destination;
        //    //B6 val 0x00 - 0x0F
        //    public byte TrackName;
        //    //B7 
        //    //public TRACK_TYP TrkTyp;

        //    //public TARGET_TYP TrgTyp;

        //    public BitArray T_TYPE = new BitArray(8);
        //    BitArray T_STATUS = new BitArray(8);
        //    BitArray ba1 = new BitArray(8);


        //    //B9 val 0x00
        //    public byte RSD;
        //    //B11B10 val 0x0000 - 0xFFFF
        //    public ushort RNG;
        //    // B13B12 val 0x0000 - 0x8C9F
        //    public ushort AZN;
        //    // B15B14 val 0x0000 - 0xFFFF
        //    public ushort SPD;
        //    // B17B16 val 0x0001 - 0x8C9F
        //    public ushort HDN;
        //    //B19B18 val 0xFFFF
        //    public ushort CHKSM;
        //};

    }
}
