using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class UDPListener
{
    public const int listenPort = 20002;
    public List<string[]> emitter_records = new List<string[]>();

    public void StartListener(Aircraft aircraft)
    {
        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("192.168.43.91"), listenPort);
        Position tmp_Pos = aircraft.position;

        try
        {
            while (true)
            {
                Console.WriteLine("\nWaiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP);
                string[] cmd = Encoding.ASCII.GetString(bytes, 0, bytes.Length).Split(' ');
                Console.WriteLine($"Received broadcast from : {groupEP}\n");
                foreach (var word in cmd)
                {
                    Console.Write(word);
                    Console.Write(" ");
                }
                Console.WriteLine(" ");
                string return_data = "NA";
                if (cmd[0] == "get")
                {
                    return_data = tmp_Pos.x.ToString() + " " + tmp_Pos.y.ToString();
                }
                else if (cmd[0] == "set")
                {
                    string[] decode_data = cmd[1].Split('|');

                    foreach (var word in decode_data)
                    {
                        // Emitter Record fields:
                        // PW, PRI, Freq, AOA, Recieved Power in this order
                        emitter_records.Add(word.Split(','));
                    }
                    return_data = "set successful";

                    foreach (var words in emitter_records)
                    {
                        foreach (var word in words)
                        {
                            Console.Write(word);
                            Console.Write(",");
                        }
                        Console.WriteLine(" ");
                    }

                }
                byte[] sendbuf = Encoding.ASCII.GetBytes(return_data);
                listener.Send(sendbuf, sendbuf.Length, groupEP);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            listener.Close();
        }
    }
}

