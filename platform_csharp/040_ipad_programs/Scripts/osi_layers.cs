using System;
using System.Net.Sockets;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.PortableExecutable;
using static System.Collections.Specialized.BitVector32;
using static OO_OSI.OSI_Aggregation;
using static System.Net.WebRequestMethods;

namespace OO_OSI
{



    //		//public class Packet
    //		//{ { }
    //		// publicclass Header new Header( ) ;
    //		//public Header header = 
    //		// //public class Payload { new } Packet( );
    //		//publicPacket payload = 
    //		// public publicclassTailer Tailer tailer { = }new Tailer( );
    //		// to
    //		// public Packet()
    //		////tailer)

    //		//Packet(Header header, Packet payload, Tailer
    //		//public 
    //		// this. header = header;
    //		// payload;
    //		// // this. payload = tailer;
    //		//  this. tailer'/}
    //		//public class Application
    //		//} 38 //public class Session
    //		// public class ApplicationPacket : Packet
    //		// public new class Header : Packet.Header { } 43 public new class Tailer : Packet.Tailer { }

    //		//new Header header = new Header();.
    //		//new ApplicationPacket payload = new ApplicationPacket();
    //		//new Tailer tailer = new Tailer(); 48
    //		//public ApplicationPacket()

    //		/1 public ApplicationPacket(Header header, ApplicationPacket payload, Tailer tailer)
    //this. header = header; this. payload = payload; this. tailer = tailer;
    //// Application application = new Application( );
    //Presentation presenation = new Presentation();
    //		//Overloadedon ApplicationPacket
    //		//Sending to Downward i.e ., Session Layer public void SendToLowerLayer
    //		// (Packetp) presentation. enqueue (P);
    //        public void SendToUpperLayer(Packet p)
    //        {
    //            application.enqueue(p);
    //			// 
    //			//Overloaded on SessionPacket
    //			//receiving from Downward i.e ., Session Layer
    //			// //will be called by session. send
    //			// public void FlowFromUpperToDownward(Packet p)
    //            if (application.downq.count ? 0)
    //			{
    //                p = application.downq.dequeue();
    //				Packet down_packet; 
    //				down_packet.header = header; 
    //				down_packet.payload = p; 
    //				down_packet.tailer = tailer; 
    //				// presen . enqueue (down_packet) ;

    //            // public void FlowFromDownToUpward (Packet p)
    //11 {
    //					// if (presentation. upq. count > 0)
    //					// {
    //					// p= presentation.upq. dequeue( );
    //                    Packet up_packet; up_packet.header = header; up_packet.payload = p; up_packet.tailer = tailer;

    //                }

    //                application.enqueue(up_packet);
    //                //
    //                // public Packet ReceiveFromLowerLayer(Packet p) // if (from_lower_layer_q.count ? 0)
    //                p = from_lower_layer_q.dequeue(); Packet up_packet;
    //                to_lower_layer.enqueue(up_packet.payload);
    //                113
    ////public class Session
    //// Presentation presentation = new Presentation( );
    ////}
    ////class Presentation
    ////{
    //// public Transport transport = new Transport (32); public class Packet : Session. Packet
    //byte[] header; class Body { }
    //        Body body = new Body();
    //        //Internal Methods of Presentation Layer private class AuthenticationToken
    //        private AuthenticationToken authenticate(string userid, string password) return new AuthenticationToken();
    //11 private bool Authorize(Session session, AuthenticationToken auth_token)
    ////

    //return true;
    ////11 protected void send(string userid,stringpassword,Session
    //session_packet)
    ////AuthenticationToken auth_token = authenticate(userid,
    //1password)bool ; isAuthorized = Authorize(session_packet, auth_token);
    //if (isAuthorized)
    //// {
    //// transport. Send() ;
    ////
    //// 
    //1 protected void receive(string userid, string password, Session session)
    //11 AuthenticationToken auth_token = authenticate(userid, password); bool isAuthorized = Authorize(session, auth_token); if (isAuthorized)
    ////
    //11
    //11
    ////}
    ////public class ApplicationPacket : Packet //{ 

    ////
    //public new class Header : Packet.Header { }
    //11

    ////
    //public new class Tailer : Packet.Tailer { }
    //        new Header header = new Header(); new ApplicationPacket payload = new ApplicationPacket(); new Tailer tailer = new Tailer(); public ApplicationPacket()

    //
    //public ApplicationPacket(Header header, ApplicationPacket payload, ?
    //Tailer tailer)
    //this. header = header; this. payload = payload; this. tailer = tailer;

    //}
    public abstract class Packet<PayloadT>
    {
        public Head head;
        public PayloadT payload;
        public Tail tail;

        public class Head { }
        public class Tail { }
        public virtual Head ComputeHead()
        { return new Head(); }
        public virtual Tail ComputeTail()
        { return new Tail(); }     

        public Packet()
        { }
        public Packet(Head head, PayloadT payload, Tail tail)
        {
            this.head = head;
            this.payload = payload;
            this.tail = tail;
        }

        //public Queue<Packet<PayloadT>> receive_from_upper_q;
        //public Queue<Packet<Packet<PayloadT>>> send_to_lower_q;
        //Commented === raw mode
        //At Raw Bit Byte Level - No use is made of Type Information /// /except ToByteBuf
        //abstract public byte[] ToByteBuf();
        //abstract public Packet<PayloadT> FromByteBuf(byte[] byteBuf);
        //extract raw data from a raw buffer - probably from higher layer packet
        //public byte[] GetByteBuf(byte[] bytebuf, int from, int to)
        //{
        //    byte[] retByteBuf = new byte[to - from + 1];

        //    for (int i = from; i < to; i++)
        //    {
        //        retByteBuf.Append(bytebuf[i]);
        //        return retByteBuf;
        //    }

        //    return retByteBuf;
        //}
        //concat header, payload, tailer - form current layer packet
        //public byte[] ConcatByteBuf3(byte[] bytebuf1, byte[] bytebuf2, byte?
        //                            [] bytebuf3)
        //{
        //    byte[] retByteBuf = new byte[bytebuf1.Length + bytebuf2.Length + bytebuf3.Length];
        //    Array.Copy(bytebuf1, 0, retByteBuf, 0, bytebuf1.Length);
        //    Array.Copy(bytebuf2, 0, retByteBuf, bytebuf1.Length, bytebuf1.Length + bytebuf2.Length);
        //    Array.Copy(bytebuf3, 0, retByteBuf, bytebuf1.Length + bytebuf2.Length, bytebuf1.Length +
        //                bytebuf2.Length + bytebuf3.Length);
        //    return retByteBuf;
        //}


        //With Type Information (Engineering Data)


        //Forwarding from Upper to Lower Layer
        //Dequeue packet from upper layer
        //Upper Layer Packet is Current Layer Payload
        //abstract public PayloadT GetPayloadFromUpper();
        //Compute and Add Header and Tailer to Payload
        //to form Packet
        //abstract public Packet<PayloadT> GetPacketFromPayload(PayloadT payload);
        //enqueue packet to lower layer
        //abstract public void SendPacketToLower(Packet<PayloadT> packet);
        //Forward from lower to upper layer
        //Dequeue packet from lower layer
        //Payload of Lower Layer  is Current Layer Packet
        //Lower layer enqueues current layer Packet
        //abstract public Packet<PayloadT> GetPacketFromLower();
        //Strip Header and Tailer to Payload to form Packet
        //Current Layer Payload is Upper Layer Packet
        //abstract public PayloadT GetPayloadFromPacket(Packet<PayloadT> packet);
        //enqueue packet to upper layer
        //note that for upper layer, upper layer is the packet
        //abstract public void SendPacketToUpper(PayloadT packet);

    }



    //Basically Layer specific Header/Tail is computed in this layer
    //and prepended. Header determines which is the layer.
    public class Layer<PayloadT>
    {
        //Current Layer Layer<PayLoadT>  packet<payloadT> = header + PayLoadT + tailer
        //On downward journey upper layer enqueues payLoadT
        //On upward journey upper layer is enqueued with payLoadT
        //packet of upper layer is payload of current layer
        //By similar, logic Packet<PayloadT> would be payload of lower layer
        Queue<PayloadT> fromUpperQ = new Queue<PayloadT>();
        Queue<PayloadT> toUpperQ = new Queue<PayloadT>();

        class LayerPacket : Packet<PayloadT>
        {
            public class Head : Packet<PayloadT>.Head
            { }
            public class Tail : Packet<PayloadT>.Tail
            { }
        }
    


    Queue<Packet<Packet<PayloadT>>> toLowerQ;
    Queue<Packet<Packet<PayloadT>>> fromLowerQ;







    public void ForwardOutwards()
    {
        while (fromUpperQ.Count > 0)
        {
            PayloadT payload = fromUpperQ.Dequeue();
            Packet<PayloadT> packet = new Packet<PayloadT>();

        }
    }
}
}


    //public Layer(Layer lower, Layer upper)
    //{
    //    this.lower = lower; this.upper = upper; this.packet = null;
    //    //Overloaded on SessionPacket
    //    //receiving from Downward i.e ., Session Layer //will be called by session. send public void FlowFromUpperToDownward(Packet p) if (application.downq.count > 0)
    //    p = application.downq.dequeue(); Packet down_packet;
    //    down_packet.header = header; down_packet.payload = p; down_packet.tailer = tailer; presen.enqueue(down_packet);
    //    public void FlowFromDownToUpward(Packet p) if (presentation.upq.count ? 0)
    //        p = presentation.upq.dequeue(); Packet up_packet; up_packet.header = header; up_packet.payload = p; up_packet.tailer = tailer; application.enqueue(up_packet);
    //    public Packet ReceiveFromLowerLayer(Packet p)
    //    {
    //        if (from_lower_layer_q.count > 0)
    //            p = from_lower_layer_q.dequeue();
    //        Packet up_packet;
    //        to_lower_layer.enqueue(up_packet.payload);
    //class Transport
    //Network network = new Network(32); int nPacketSize; int referred_int = 0; public class Packet : Presentation.Packet
    //M
    //public Transport(int nPacketSize) ref int rx = ref referred_int;

    //this.nPacketSize = nPacketSize;
    //}
    //    protected void send(Transport tp)
    //M string stp = tp.ToString(); string[] astp = stp.Split('c');//'c' corresponds to nPacketSize; foreach (string packet in astp)
    //    (packet) ;
    //}
    //protected Transport receive()
    //class Network
    //DataLink dataLink = new DataLink(); protected void send()
    //get source_ip:port
    //get destination_ip:port //Determine next hop dataLink.
//    protected void receive()
//class DataLink
//M public Physical physical = new Physical(); protected void send(string packet)
//get source_ip:port
//get destination_ip:port
//Determine next hop
//physical.send(packet);
//protected void receive()
//class Physical
//public void hw_send(string byte_array_packet) { }
//public void send(string packet)
//get source_ip: port
//get destination_ip:port //Determine next hop hw_send (packet);
//protected void receive( )








