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
}