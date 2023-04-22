using System;
using System.Net.Sockets;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace OO_OSI
{
    //public class Packet<HeadT, PayloadT, TailT>
    //    where HeadT : new()
    //    where TailT : new()
    //{
    //    public HeadT head;
    //    public PayloadT payload;
    //    public TailT tail;

    //    public HeadT ComputeHead()
    //    { return new HeadT(); }
    //    public TailT ComputeTail()
    //    { return new TailT(); }
    //    public Packet()
    //    { }
    //    public Packet(HeadT head, PayloadT payload, TailT tail)
    //    {
    //        this.head = head;
    //        this.payload = payload;
    //        this.tail = tail;
    //    }

    //    //public Queue<Packet<PayloadT>> receive_from_upper_q;
    //    //public Queue<Packet<Packet<PayloadT>>> send_to_lower_q;
    //    //Commented === raw mode
    //    //At Raw Bit Byte Level - No use is made of Type Information /// /except ToByteBuf
    //    //abstract public byte[] ToByteBuf();
    //    //abstract public Packet<PayloadT> FromByteBuf(byte[] byteBuf);
    //    //extract raw data from a raw buffer - probably from higher layer packet
    //    //public byte[] GetByteBuf(byte[] bytebuf, int from, int to)
    //    //{
    //    //    byte[] retByteBuf = new byte[to - from + 1];

    //    //    for (int i = from; i < to; i++)
    //    //    {
    //    //        retByteBuf.Append(bytebuf[i]);
    //    //        return retByteBuf;
    //    //    }

    //    //    return retByteBuf;
    //    //}
    //    //concat header, payload, tailer - form current layer packet
    //    //public byte[] ConcatByteBuf3(byte[] bytebuf1, byte[] bytebuf2, byte?
    //    //                            [] bytebuf3)
    //    //{
    //    //    byte[] retByteBuf = new byte[bytebuf1.Length + bytebuf2.Length + bytebuf3.Length];
    //    //    Array.Copy(bytebuf1, 0, retByteBuf, 0, bytebuf1.Length);
    //    //    Array.Copy(bytebuf2, 0, retByteBuf, bytebuf1.Length, bytebuf1.Length + bytebuf2.Length);
    //    //    Array.Copy(bytebuf3, 0, retByteBuf, bytebuf1.Length + bytebuf2.Length, bytebuf1.Length +
    //    //                bytebuf2.Length + bytebuf3.Length);
    //    //    return retByteBuf;
    //    //}


    //    //With Type Information (Engineering Data)


    //    //Forwarding from Upper to Lower Layer
    //    //Dequeue packet from upper layer
    //    //Upper Layer Packet is Current Layer Payload
    //    //abstract public PayloadT GetPayloadFromUpper();
    //    //Compute and Add Header and Tailer to Payload
    //    //to form Packet
    //    //abstract public Packet<PayloadT> GetPacketFromPayload(PayloadT payload);
    //    //enqueue packet to lower layer
    //    //abstract public void SendPacketToLower(Packet<PayloadT> packet);
    //    //Forward from lower to upper layer
    //    //Dequeue packet from lower layer
    //    //Payload of Lower Layer  is Current Layer Packet
    //    //Lower layer enqueues current layer Packet
    //    //abstract public Packet<PayloadT> GetPacketFromLower();
    //    //Strip Header and Tailer to Payload to form Packet
    //    //Current Layer Payload is Upper Layer Packet
    //    //abstract public PayloadT GetPayloadFromPacket(Packet<PayloadT> packet);
    //    //enqueue packet to upper layer
    //    //note that for upper layer, upper layer is the packet
    //    //abstract public void SendPacketToUpper(PayloadT packet);

    //}

    //Basically Layer specific Header/Tail is computed in this layer
    //and prepended. Header determines which is the layer.
    //Layer<Transport> determines Head and Tail of Transport Layer
    public enum Buffer { PING, PONG }

    public class PingPongQueue<T>:Queue<T>
    {
        public Queue<T> pingBuffer = new Queue<T>();
        public Queue<T> pongBuffer = new Queue<T>();
        public Buffer readBuffer, writeBuffer;
        public Buffer buffer;
        public PingPongQueue()
        {
            readBuffer  = Buffer.PING;
            writeBuffer = Buffer.PONG;
        } 
        
        public void SetReadBuffer(Buffer readBuffer)
        {
            this.readBuffer = readBuffer;
            writeBuffer = Toggle(readBuffer);
        }

        public new void Enqueue(T item)
        {
            if (buffer == Buffer.PING)
            {
                pingBuffer.Enqueue(item);
            }
            else
            {
                pongBuffer.Enqueue(item);
            }         
        }

        public new T Dequeue()
        {
            if (buffer == Buffer.PING)
            {
                return pingBuffer.Dequeue();
            }
            else
            {
                return pongBuffer.Dequeue();
            }
        }

        public Buffer Toggle(Buffer b)
        {
            if (b == Buffer.PING)
                return Buffer.PONG;
            else
                return Buffer.PING;
        }

    }
    
    public class Layer<PayloadT> where PayloadT : Payload 
    {
        //Current Layer Layer<PayLoadT>  packet<payloadT> = header + PayLoadT + tailer
        //On upward journey upper layer is enqueued with payLoadT
        //packet of upper layer is payload of current layer
        //By similar, logic Packet<PayloadT> would be payload of lower layer
        public PingPongQueue<Payload> fromUpperQ = new PingPongQueue<Payload>();
        public PingPongQueue<Payload> toUpperQ = new PingPongQueue<Payload>();

        public ref PingPongQueue<Payload> 
        GetPingPongQueueReference(ref PingPongQueue<Payload> ppQueue)
        {
            return ref ppQueue; 
        }

        //Queue<Transport> fromUpperQ, toUpperQ
        public class Head
        { }
        public class Tail
        { }
        public Head head = new Head();
        public Tail tail = new Tail();
        public Head ComputeHead(PayloadT upperPacket)
        { return new Head(); }

        public class Packet:Payload
        {
            public Head head;
            public PayloadT payload;
            public Tail tail;

            public Packet()
            { }

            public Packet(Head head, PayloadT payload, Tail tail)
            { this.head = head; this.payload = payload; this.tail = tail; }            
        }


        public Tail ComputeTail(PayloadT upperPacket)
        { return new Tail(); }

        public Packet layer_packet  = new Packet();

        //public PingPongQueue<Packet<Head, PayloadT, Tail>> toLowerQ
        //    = new PingPongQueue<Packet<Head, PayloadT, Tail>>();
        //public PingPongQueue<Packet<Head, PayloadT, Tail>> fromLowerQ
        //    = new PingPongQueue<Packet<Head, PayloadT, Tail>>();

        public PingPongQueue<Payload> toLowerQ
            = new PingPongQueue<Payload>();
        public PingPongQueue<Payload> fromLowerQ
            = new PingPongQueue<Payload>();

        //On deque from upper layer, PayloadT i.e., Payload of Transport come
        //On enque to   upper layer, Payloadt i.e.., Payload of Transform is sent upwards
        //Payload(Transform)=Session
        public void TransferFromUpperToLower()
        {
            //switch buffers of upper and lower queues
            Buffer toggledBuffer = fromUpperQ.Toggle(fromUpperQ.readBuffer);
            fromUpperQ.SetReadBuffer(toggledBuffer);

            toggledBuffer = toLowerQ.Toggle(toLowerQ.readBuffer);
            toLowerQ.SetReadBuffer(toggledBuffer);

            while (fromUpperQ.Count > 0)
            {
                PayloadT upper_packet = fromUpperQ.Dequeue();
                head = ComputeHead(upper_packet);
                tail = ComputeTail(upper_packet);
                Packet<Head, PayloadT, Tail> layer_packet =
                              new Packet<Head, PayloadT, Tail>(head, upper_packet, tail);
                toLowerQ.Enqueue(layer_packet);
            }
        }

        public void TransferFromLowerToUpper()
        {
            //switch buffers of upper and lower queues
            Buffer toggledBuffer = fromLowerQ.Toggle(fromLowerQ.readBuffer);
            fromLowerQ.SetReadBuffer(toggledBuffer);

            toggledBuffer = toUpperQ.Toggle(toUpperQ.readBuffer);
            toUpperQ.SetReadBuffer(toggledBuffer);

            while (fromLowerQ.Count > 0)
            {
                Packet lower_packet = (Packet)fromLowerQ.Dequeue();
                //We are interested only in Payload
                //Head and Tail can be stripped off
                toUpperQ.Enqueue(lower_packet.payload);
            }
        }

    }
    public abstract class Payload { public string data; }
    public class OSIStack
    {
        List<Layer<Payload>> OsiSevenLayers = new List<OO_OSI.Layer<Payload>>();
        
        class ApplicationPayload
        {  }

        class Presentation
        { }

        class Session
        { }
        public OSIStack()
        {            
            Layer<Application>  applicationLayer    = new Layer<Application>();
            Layer<Presentation> presentationLayer   = new Layer<Presentation>();
            Layer<Session>      sessionLayer        = new Layer<Session>();

            
            applicationLayer.toLowerQ = 
                presentationLayer.GetPingPongQueueReference(ref presentationLayer.fromUpperQ);

            applicationLayer.fromLowerQ =
                presentationLayer.GetPingPongQueueReference(ref presentationLayer.toUpperQ);

            presentationLayer.toLowerQ =
                sessionLayer.GetPingPongQueueReference(ref sessionLayer.fromUpperQ);

            sessionLayer.fromLowerQ =
                sessionLayer.GetPingPongQueueReference(ref sessionLayer.toUpperQ);
        }
    }

    public class Node
    {
        public OSIStack stack = new OSIStack();

        public void send(string s)
        {

        }

        public string receive()
        {
            return String.Empty;
        }
    }



    public class Hardware
    {
        Queue<byte> hwChannel;
        public int size;
        public Hardware(int size)
        {
            hwChannel = new Queue<byte>(size);
            this.size = size;
        }
    } 
   

    public class TestHarness
    {
        public bool test_case_01()
        {
            //Two point to point connected computers
            //exchanging a single packet
            //application-data link-physical
            Node n1 = new Node();
            Node n2 = new Node();
            int tick = 0;
            while (tick < 10)
            {
                if (tick % 10 == 0)
                {
                    n1.send("hello");
                }

                foreach (Layer<Payload> l in n1.stack.)
            }
            return false;
        }
    }

    public class Simulation
    {
        int node_one_period = 1;
        int node_two_period = 2;
        public void Run()
        {
            int tick = 0;
            while (true)
            {

                foreach(Layer<Payload> l in node1.layers)
                {
                    //dequeue phase
                    l.TransferFromUpperToLower();
                    l.TransferFromLowerToUpper();
                }

            }            
        }
    }

}











