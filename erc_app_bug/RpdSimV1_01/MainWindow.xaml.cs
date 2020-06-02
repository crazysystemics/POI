using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static RpdSim.Protocols;

namespace RpdSim
{

    static class IRSParser
    {
        static public UInt32 getParameter(BitArray b, int fromByte, int toByte,
                                   int from_bit, int to_bit)
        {

            //Assumption from < to

            int[] byte_param = new int[toByte - fromByte + 1];
            int fromByteOffset = fromByte * 8;
            int toByteOffset = toByte * 8;
            UInt32 word = 0;

            for (int i = toByteOffset + to_bit;
                     i >= fromByteOffset + from_bit; i--)
            {
                //byte_param[to_bit-i] = b[i] ? 1 : 0;
                word <<= 1;
                word |= Convert.ToUInt32(b[i]);
            }

            return word;
        }
    }



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }

            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

		public bool _readFlag = false;
        public MainWindow()
        {
            InitializeComponent();
            //btnSend.IsEnabled = false;
        }

        private void btnReadData_Click(object sender, RoutedEventArgs e)
        {
            _readFlag = false;
            Thread thr = new Thread(RxData);

            thr.Start();
            Thread.Sleep(5);

        }
        
        private void RxData()
        {
            String ReadFilePath = @"E:\tcpip\WriteFlag.dat";
            byte[] Data = new byte[500];
            FileInfo fileInfo = new FileInfo(ReadFilePath);
            while (true)
            {

                bool result = IsFileLocked(fileInfo);
                if (!result)
                {
                    Data = File.ReadAllBytes(ReadFilePath);
                    PackerParser(Data, ReadFilePath);
                    break;
                }
            }
        }
        private void ClearFile()
        {
            byte[] hexstring = new byte[100];
            for (int i = 0; i < hexstring.Length; i++)
            {
                hexstring[1] = 0;
            }
            System.IO.File.WriteAllText(@"E:\tcpip\ReadFlag.dat", string.Empty);
            System.IO.File.WriteAllText(@"E:\tcpip\AXI.dat", string.Empty);
            System.IO.File.WriteAllText(@"E:\tcpip\WriteFlag.dat", string.Empty);
            //File.WriteAllBytes(@"E:\tcpip\ReadFlag.dat", hexstring);
            //File.WriteAllBytes(@"E:\tcpip\AXI.dat", hexstring);
        }

        private void PackerParser(byte[] data, string ReadfilePath)
        {
            int len = Buffer.ByteLength(data);

            //int len = data.Length;
            if (len > 0)
            {
                IntPtr ptr = Marshal.AllocHGlobal(8);
                Marshal.Copy(data, 0, ptr, 8);
                RDPHdr tic = new RDPHdr();
                tic = (RDPHdr)Marshal.PtrToStructure(ptr, tic.GetType());

                

                if (tic.MSGHDR == (ushort)0xFFFF)
                {
                    switch (tic.MSGID)
                    {
                        case (ushort)0x0010:
                            _readFlag = true;
                            
                            ClearFile();


                            break;
                        case (ushort)0x0008:
                            PeriodicTrackData ptd = new PeriodicTrackData();
                            IntPtr ptr1 = Marshal.AllocHGlobal(700);
                            Marshal.Copy(data, 0, ptr1, 8);
                            ptd = (PeriodicTrackData)Marshal.PtrToStructure(ptr1, ptd.GetType());
                            break;
                        //case (Command)Command.TEST:
                        //    HealthParam();


                        default:
                            break;
                    }
                }
            }
            DataParam();

        }
        private void DataParam()
        {
            if (_readFlag)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btnSend.IsEnabled = true;
                    labStatus.Content = "Send Target Data to ERC";
                    labStatus.FontWeight = FontWeights.Bold;
                    labStatus.Foreground = Brushes.Green;
                    //btnHealth. = true;
                    btnStart.IsEnabled = false;

                }));
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btnSend.IsEnabled = false;
                    labStatus.Content = "you have not receave any data \nPress Again to Start";
                    labStatus.FontWeight = FontWeights.Bold;
                    labStatus.Foreground = Brushes.Red;
                    //btnHealth. = true;
                    btnStart.IsEnabled = true;

                }));

            }



        }

        //public static PeriodicTrackData getRadarMsg()
        //{
        //    BinaryReader rcds_channel
        //       = new BinaryReader(File.Open("rcds.channel", FileMode.Open));

        //    List<byte> byteListPkt = new List<byte>();

        //    while (rcds_channel.BaseStream.Position < rcds_channel.BaseStream.Length)
        //    {
        //        byteListPkt.Add(rcds_channel.ReadByte());
        //    }
        //    BitArray b = new BitArray(byteListPkt.ToArray());

        //    PeriodicTrackData rmsg = new PeriodicTrackData();

        //    rmsg.MSGHDR = (ushort)IRSParser.getParameter(b, 0, 0, 0, 1);
        //    rmsg.MSGID = (ushort)IRSParser.getParameter(b, 0, 0, 2, 5);
        //    rmsg.Source = (ushort)IRSParser.getParameter(b, 0, 0, 7, 7);

        //    return rmsg;
        //}

        private void btnSendData_Click(object sender, RoutedEventArgs e)
        {

            BinaryReader rcds_channel
               = new BinaryReader(File.Open(@"E:\tcpip\WriteFlag.dat", FileMode.Open));

            List<byte> byteListPkt = new List<byte>();

            while (rcds_channel.BaseStream.Position < rcds_channel.BaseStream.Length)
            {
                byteListPkt.Add(rcds_channel.ReadByte());
            }
            BitArray b = new BitArray(byteListPkt.ToArray());

            rcds_channel.Close();
            PeriodicTrackData rmsg = new PeriodicTrackData();

            rmsg.MSGHDR = (ushort)IRSParser.getParameter(b, 0, 2, 0, 0);// 0, 15);
            rmsg.MSGID = (ushort)IRSParser.getParameter(b, 2, 4, 0, 0);// 16, 31);
            rmsg.Source = (ushort)IRSParser.getParameter(b, 4, 6, 0, 0);// 32, 47);
            rmsg.Destination = (ushort)IRSParser.getParameter(b, 6, 8, 0, 0);// 48, 63);
            rmsg.TrackName = (byte)IRSParser.getParameter(b, 8, 9, 0, 0);// 64, 71);

            rmsg.T_TYPE = (byte)IRSParser.getParameter(b, 9, 10, 0, 3);
            rmsg.T_STATUS = (byte)IRSParser.getParameter(b, 9, 10, 3, 6);

            rmsg.TBD = (byte)IRSParser.getParameter(b,10,12, 0, 1);// 78, 78);
            rmsg.TBD1 = (byte)IRSParser.getParameter(b,10,12, 1, 3);//, 79, 80);
            rmsg.GND_TGT = (byte)IRSParser.getParameter(b, 10, 12, 3, 6);// 81, 83);
            rmsg.CFN = (byte)IRSParser.getParameter(b, 10, 12, 6, 9);// 84, 86);


            rmsg.RSD = (byte)IRSParser.getParameter(b, 12, 13, 0, 0);//, 87, 94);
            rmsg.RNG = (ushort)IRSParser.getParameter(b, 13, 15, 0, 0);// 95, 110);
            rmsg.AZN = (ushort)IRSParser.getParameter(b, 15, 17, 0, 0);// 111, 126);
            rmsg.SPD = (ushort)IRSParser.getParameter(b, 17, 19, 0, 0);// 127, 142);
            rmsg.HDN = (ushort)IRSParser.getParameter(b, 19, 21, 0, 0);// 143, 158);

            rmsg.CHKSM = (ushort)IRSParser.getParameter(b, 21, 23, 0, 0);// 159, 174);


            byte[] Data = new byte[500];
            Data = File.ReadAllBytes(@"E:\tcpip\WriteFlag.dat");
            PeriodicTrackData rmsg1 = new PeriodicTrackData();

            IntPtr ptr = Marshal.AllocHGlobal(8);
            Marshal.Copy(Data, 0, ptr, 8);
            
            rmsg1 = (PeriodicTrackData)Marshal.PtrToStructure(ptr, rmsg1.GetType());







            //if ((String.IsNullOrEmpty(txtTrgID.Text)) || (String.IsNullOrEmpty(txtAzi.Text)) || (String.IsNullOrEmpty(txtTrgRng.Text)) || (String.IsNullOrEmpty(txtRTDC.Text)))
            //{
            //    MessageBox.Show("Please Enter The values");
            //}
            //else
            //{
            //    String filename = @"E:\tcpip\AXI.dat";
            //    TrackBeamRequest tb = new TrackBeamRequest();

            //    //tb.MSGHDR = (ushort)0xFFFF;
            //    //tb.MSGID = (ushort)0x0005;
            //    //tb.Source = (ushort)SubSystem.RDP;
            //    //tb.Destination = (ushort)SubSystem.RCDS;
            //    //tb.T_ID = Convert.ToUInt16(txtTrgID.Text);// (ushort)0x0003 ;
            //    //tb.TA = Convert.ToUInt16(txtAzi.Text);// (ushort)0x0045;
            //    //tb.TR = Convert.ToUInt16(txtTrgRng.Text);// (ushort)0x1238;
            //    //tb.RTCD = Convert.ToUInt16(txtRTDC.Text);// (ushort)0x3452;
            //    //tb.CHKSM = (ushort)0xFFFF;
            //    //int Size = Marshal.SizeOf(tb);


            //    TARGET_TYP tt = new TARGET_TYP();
            //    //tt.target = (ushort)207;
            //    tt.TBD=

            //    //pradic track data
            //    PeriodicTrackData ptd = new PeriodicTrackData();
            //    ptd.MSGHDR = (ushort)0xFFFF;
            //    ptd.MSGID = (ushort)0x0008;
            //    ptd.Source = (ushort)SubSystem.RDP;
            //    ptd.Destination = (ushort)SubSystem.RCDS;
            //    ptd.TrackName = (byte)0x02;
            //    ptd.TrkTyp.T_TYPE = (byte)000;
            //    ptd.TrkTyp.T_STATUS = (byte)001;
            //    //ptd.TrgTyp.TBD = (byte)0;
            //    //ptd.TrgTyp.TBD1 = (byte)02;
            //    //ptd.TrgTyp.GND_TGT = (byte)000;
            //    //ptd.TrgTyp.CFN = (byte)0x3;
            //    ptd.TrgTyp.target = (ushort)207;

            //    //ptd.TrkTyp.track = (char)1;

            //    ptd.RSD = (byte)0x00;
            //    ptd.RNG = (ushort)0x0012;
            //    ptd.AZN = (ushort)0x0021;
            //    ptd.SPD = (ushort)0x0001;
            //    ptd.HDN = (ushort)0x0001;
            //    ptd.CHKSM = (ushort)0xFFFF;
            //    int  Size = Marshal.SizeOf(ptd);

            //    //Structure to buffer
            //    byte[] _sendBuffer = new byte[Size]; // Builds byte array

            //    IntPtr memPtr = IntPtr.Zero;

            //    // Allocate some unmanaged memory
            //    memPtr = Marshal.AllocHGlobal(Size);

            //    // Copy struct to unmanaged memory
            //    //Marshal.StructureToPtr(tb, memPtr, true);

            //    Marshal.StructureToPtr(ptd, memPtr, true);

            //    // Copies to byte array
            //    Marshal.Copy(memPtr, _sendBuffer, 0, Size);

            //    //alternative for write

            //    //File.WriteAllBytes("output.dat", StringToByteArray(hexString));

            //    using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            //    {
            //        fs.Write(_sendBuffer, 0, _sendBuffer.Length);

            //    }
            //    byte[] hexstring = new byte[1];

            //    hexstring[0] = 1;
            //    File.WriteAllBytes(@"E:\tcpip\ReadFlag.dat", hexstring);


            //    //read byte from file
            //    //byte[] Data = new byte[1];
            //    //Data = File.ReadAllBytes(@"E:\tcpip\ReadFlag.dat");

            //    //read from file

            //    //byte[] Data = new byte[500];
            //    //Data = File.ReadAllBytes(filename);
            //    //int len = Data.Length;
            //    //IntPtr ptr = Marshal.AllocHGlobal(len + 20);
            //    //Marshal.Copy(Data, 0, ptr, 8);//copy only packet header to validate the content

            //    //tb1 = (TrackBeamRequest)Marshal.PtrToStructure(ptr, tb1.GetType());
            //}
        }

        private void btread_Click(object sender, RoutedEventArgs e)
        {

            BinaryReader rcds_channel
               = new BinaryReader(File.Open(@"E:\tcpip\WriteFlag.dat", FileMode.Open));

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