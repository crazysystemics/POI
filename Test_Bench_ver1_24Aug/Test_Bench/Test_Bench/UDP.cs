using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Test_Bench
{
    public class UDP
    {
        public int port_no;
        public byte[] data_received = new byte[20];
        public UdpClient udpServer;
        public UdpClient client;

        public UDP(int pport_no)
        {
            port_no = pport_no;
        }

        public void UDP_Connect()
        {
            IPAddress iPAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(iPAddr, port_no);
            client = new UdpClient();
            client.Connect(localEndPoint);
        }

        public void UDP_Send(byte[] data_sent, int length)
        {
            client.Send(data_sent, length);
        }

        public void UDP_Bind()
        {
            udpServer = new UdpClient(port_no);
        }

        public int UDP_Receive()
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, port_no);
            if (udpServer.Available != 0)
            {
                data_received = udpServer.Receive(ref remoteEP);
                return 1;
            }
            else
                return -1;
        }
    }
}
