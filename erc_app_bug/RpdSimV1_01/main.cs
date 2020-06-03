using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace RpdSim
{
    class PacketHelper
    {

        public void interpretPkt()
        {

            BinaryReader rcds_channel
               = new BinaryReader(File.Open("WriteFlag.dat", FileMode.Open));

            List<byte> byteListPkt = new List<byte>();

            while (rcds_channel.BaseStream.Position < rcds_channel.BaseStream.Length)
            {
                byteListPkt.Add(rcds_channel.ReadByte());
            }
            BitArray b = new BitArray(byteListPkt.ToArray());

            rcds_channel.Close();
            PeriodicTrackData rmsg = new PeriodicTrackData();

            rmsg.MSGHDR = (ushort)IRSParser.getParameter(b, 0, 2, 0, 7);// 0, 15);
            rmsg.MSGID = (ushort)IRSParser.getParameter(b, 2, 4, 0, 0);// 16, 31);
            rmsg.Source = (ushort)IRSParser.getParameter(b, 4, 6, 0, 0);// 32, 47);
            rmsg.Destination = (ushort)IRSParser.getParameter(b, 6, 8, 0, 0);// 48, 63);
            rmsg.TrackName = (byte)IRSParser.getParameter(b, 8, 9, 0, 0);// 64, 71);

            rmsg.T_TYPE = (byte)IRSParser.getParameter(b, 0, 0, 72, 74);// 9, 10, 0, 3);
            rmsg.T_STATUS = (byte)IRSParser.getParameter(b, 0, 0, 75, 77);// 9, 10, 3, 6);

            rmsg.TBD = (byte)IRSParser.getParameter(b, 0, 0, 78, 79);// 10, 12, 0, 1);// 78, 78);
            rmsg.TBD1 = (byte)IRSParser.getParameter(b, 0, 0, 79, 80);// 10, 12, 1, 3);//, 79, 80);
            rmsg.GND_TGT = (byte)IRSParser.getParameter(b, 0, 0, 81, 83);// 10, 12, 3, 6);// 81, 83);
            rmsg.CFN = (byte)IRSParser.getParameter(b, 0, 0, 84, 86);// 10, 12, 6, 9);// 84, 86);


            rmsg.RSD = (byte)IRSParser.getParameter(b, 12, 12, 0, 7);//, 87, 94);
            rmsg.RNG = (ushort)IRSParser.getParameter(b, 13, 14, 0, 7);// 95, 110);
            rmsg.AZN = (ushort)IRSParser.getParameter(b, 15, 17, 0, 0);// 111, 126);
            rmsg.SPD = (ushort)IRSParser.getParameter(b, 17, 19, 0, 0);// 127, 142);
            rmsg.HDN = (ushort)IRSParser.getParameter(b, 19, 21, 0, 0);// 143, 158);

            rmsg.CHKSM = (ushort)IRSParser.getParameter(b, 21, 23, 0, 0);// 159, 174);
        }
    }
}