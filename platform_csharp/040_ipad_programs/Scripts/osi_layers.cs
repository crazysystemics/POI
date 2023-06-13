using System;
using System.Net.Sockets;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata.Ecma335;


//TODO: Idea: Using IIT in recursive connected component algorithm (fitness of a cluster)
//TODO: Idea: Phi can be used as distance metric between components

//TODO: Idea: Tool: Probably scalable with recursive connected component algorithm??
//TODO: Idea: Tool: Connected Components in  Module(Class)wise
//TODO: Idea: Tool: Threadwise (Call Graph, Call Stack)
//TODO: Idea: Stack as well as (logic-data flow, flowchart)

//TODO: Idea: Tool: MBSE-Product Program (OFP.EXE) Flow Visualizer(UML Diagrams?? Flowcharts??)
//TODO: Idea: Tool: MBSE-Product Should link with Data Analysis and Visualization (.RECDATA, Plots and Charts)
//TODO: Idea: Tool: MBSE-Product Evolved Configuration: Resulting in Improved Plans,.Configs and Weights (.PFMG)
//TODO: Idea: Tool: MBSE-Product Evolved Product Feature: Resulting in improved Features, Programs
//TODO: Idea: Tool: MBSE-Product Warning/Health Monitoring Analysis MFWS-LSS-FLT (BIT of Systems)

//TODO: Idea: Tool: MBSE-Product Battle Management System Evaluator
//TODO: Idea: Tool: MBSE-Product Mission with multiple Systems
//TODO: Idea: Tool: MBSE-Product SACCIN kind of Battle Visualization
//TODO: Idea: Tool: MBSE-Product Multi-System Monitoring, Recording, Visualization (RED/BLUE, COPE INDIA)
//TODO: Idea: Tool: MBSE-Product MSDF-SSA


//TODO: Idea: Tool: MBSE-Process Engineering - Documentation, VnV 
//TODO: Idea: Tool: MBSE-Process SE-[SDLC]
//                  [SDLC-V, Integration, VnV(Review, Testing), CM, Documentation]
//                  Product Backlog-Model Repository or Knowledge Base 
//                  Dimensions        : Normalized Peformance Measure [SPEC-SET](a Product Feature)
//                                      Time/Spec, Cost/Spec, Defect/Spec                        
//                  Capabilit Maturity: Continuous Improvement

//TODO: Jupyter Notebook type in C#

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
    public enum BufferType { PING, PONG }
    public enum BufferCycle { PING_READ_PONG_WRITE, PONG_READ_PING_WRITE }
    public enum StackPosition { TOP, BOTTOM, MIDDLE }
    public static class sglobal
    {
        public static string data = String.Empty;
        private static int global_id = 0;
        public static BufferCycle OsiStackCycle;
        public static int GetGlobalId() { return global_id++; }
        public static void InitOsiCycle(BufferCycle OsiCycle =
                            BufferCycle.PING_READ_PONG_WRITE)
        {
            OsiStackCycle = OsiCycle;
        }

        public static void ToggleOsiCycle()
        {
            if (OsiStackCycle == BufferCycle.PING_READ_PONG_WRITE)
                OsiStackCycle = BufferCycle.PONG_READ_PING_WRITE;
            else
                OsiStackCycle = BufferCycle.PING_READ_PONG_WRITE;
        }



    }
    public abstract class Payload { public string data; }


    public class PingPongQueue<T>
    {
        public Queue<T> readQ
        {
            //readQ is readOnly. It does not have set operator
            //But dynamically it can be evaluated to ping or pong
            get
            {
                //TODO: Depending on global flag is not very modular
                //TODO: Should change to a flag within a limit
                return sglobal.OsiStackCycle == BufferCycle.PING_READ_PONG_WRITE ?
                         pingBuffer : pongBuffer;
            }
        }

        public class WriteQ<T1> : Queue<T1>
        {
            public new T1 Dequeue()
            {
                throw new Exception();
            }

            public WriteQ(Queue<T1> qt1)
            {

            }
        }

        public Queue<T> writeQ
        {
            //writeQ is writeOOnly. It does not have get  operator
            //But dynamically it can be evaluated to ping or pong
            //TODO: Dequeue should throw exception on WriteQ and vice versa
            //Check WriteQ1  implementation
            get
            {
                if (sglobal.OsiStackCycle == BufferCycle.PING_READ_PONG_WRITE)
                {
                    return pingBuffer;
                }
                else
                {
                    return pongBuffer;
                }
            }

        }

        public Queue<T> pingBuffer = new Queue<T>();
        public Queue<T> pongBuffer = new Queue<T>();

    }
    public class Layer
    {
        int id;
        private string header;
        private string trailer;

        public class Packet : Payload
        {
            public Head head;
            public Payload payload;
            public Tail tail;

            public Packet()
            { }

            public Packet(Head head, Payload payload, Tail tail)
            { this.head = head; this.payload = payload; this.tail = tail; }

            public override string ToString()
            {
                return head.data + payload.data + tail.data;
            }
        }
        public class Head
        {
            public string data;
            public Head(string header)
            {
                data = "<" + header + ">";
            }
        }
        public class Tail
        {
            public string data;
            public Tail(string trailer)
            {
                data = "<" + trailer + ">";
            }
        }

        //Properties, Ports
        public Queue<Payload> FromUpperQ
        {
            get
            {
                return fromUpperQ.readQ;
            }
        }
        public Queue<Payload> ToUpperQ
        {
            get
            {
                return toUpperQ.writeQ;
            }
        }
        public Queue<Payload> FromLowerQ
        {
            get
            {
                return fromLowerQ.readQ;
            }
        }
        public Queue<Payload> ToLowerQ
        {
            get
            {
                return toLowerQ.writeQ;
            }
        }

        //Members, Fields
        public string Name;
        public StackPosition position;

        private PingPongQueue<Payload> fromUpperQ = new PingPongQueue<Payload>();
        private PingPongQueue<Payload> toUpperQ = new PingPongQueue<Payload>();

        public Head head;
        public Tail tail;

        public PingPongQueue<Payload> toLowerQ
            = new PingPongQueue<Payload>();
        public PingPongQueue<Payload> fromLowerQ
            = new PingPongQueue<Payload>();
        public Packet layer_packet = new Packet();

        //Methods
        //Constructors
       public Layer( StackPosition position,
                      string Name    = "undefined",
                      string header  = "empty",
                      string trailer = "empty")
        {
            this.id = sglobal.GetGlobalId();
            this.Name       = Name;
            this.position   = position;
            this.header     = header;
            this.trailer    = trailer;

            head = new Head(header);
            tail = new Tail(trailer);
        }

        //TODO: To promote as an extension method of string library
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// https://stackoverflow.com/questions/8809354/replace-first-occurrence-of-pattern-in-a-string
        /// <returns>
        /// </returns>
        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public string ReplaceLast(string text, string search, string replace)
        {
            int pos = text.LastIndexOf(search);

            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public void Toggle()
        {
            sglobal.ToggleOsiCycle();
        }

        public void Ontick(int layerIndex, List<Layer> layers)
        {
            TransferFromUpperToLower(layerIndex, layers);
            //TransferFromLowerToUpper();
        }

        public Head ComputeHead(Payload upperPacket)
        { return new Head(); }
        public Tail ComputeTail(Payload upperPacket)
        { return new Tail(); }


        public void TransferFromUpperToLower(int layerIndex, List<Layer> layers)
        {
            //This entire operation will happen in one tick. 
            //If current layer reads from ping buffer of two lower and writes into
            //pong of write buffer, 
            //next layer-Transfer from toLowerQ of upperLayer to
            //fromUpperQ of currentLayer
            if (position == StackPosition.TOP)
            {
                //Transfer from Upper is relevant only for non-Top rows
                return;
            }
            else
            {
                
                while (layers[layerIndex - 1].ToLowerQ.Count > 0)
                {
                    Payload payloadFromUpperLayer = layers[layerIndex - 1].ToLowerQ.Dequeue();                   

                    head = ComputeHead(payloadFromUpperLayer);
                    tail = ComputeTail(payloadFromUpperLayer);
                    Packet layer_packet =
                           new Packet(head, payloadFromUpperLayer, tail);

                    if (position == StackPosition.BOTTOM)
                    {
                        Console.WriteLine(layer_packet);
                    }
                    else
                    {
                        ToLowerQ.Enqueue(layer_packet);
                    }
                }
            }
        }

        //TODO: This is brute-force. Equivalent of bytecopy/memcpy. Low level manipulation
        //TODO:Better approach would be recursive definition of Payload/Packet.  
        //TODO:Given that Payload can be converted to Packet, if Packet can contain Payload,
        //TODO:it can be readily converted into packet, albeit of another layer. Should explore
        public Packet GetPacket(string strPayload)
        {
            //payload(data) from lower layer packet is packet for upper layer
            
            int headIndex    = strPayload.IndexOf('<');
            int headEndIndex = strPayload.IndexOf('>');
            int tailIndex    = strPayload.LastIndexOf('<');
            int tailEndIndex = strPayload.LastIndexOf('>');

            string hdata;
            string tdata;
            string data;
            hdata = tdata = data = String.Empty;

            for (int index = 0; index <= tailEndIndex; index++)
            {
                if (index < headIndex)
                    continue;
                else if (index <= headEndIndex)
                    hdata += strPayload[index];
                else if (index < tailIndex)
                    data += strPayload[index];
                else if (index <= tailEndIndex)
                    tdata += strPayload[index];
            }

            Head head = new Head();
            head.data = hdata;
            Tail tail = new Tail();
            tail.data = tdata;

            Packet packet = new Packet();
            packet.head = head;
            packet.tail = tail;
            packet.payload.data = data;

            return packet;
        }


        public void TransferFromLowerToUpper(int layerIndex, List<Layer> layers)
        {
            while (layers[layerIndex + 1].ToUpperQ.Count > 0)
            {
                //Transferring from lower layer to current layer
                Packet lowerQPacket = (Packet)layers[layerIndex + 1].ToUpperQ.Dequeue();           
                Packet upperQPacket = GetPacket(lowerQPacket.data);
                ToUpperQ.Enqueue(upperQPacket);
            }
        }
    }
    public class OSIStack
    {
        public List<Layer> OsiSevenLayers = new List<Layer>();
        public OSIStack()
        {
            Layer applicationLayer  = new Layer(StackPosition.TOP, 
                                                      "application" , "ah", "at");
            Layer presentationLayer = new Layer(StackPosition.MIDDLE, 
                                                      "presentation","ph", "pt");
            Layer sessionLayer      = new Layer(StackPosition.BOTTOM, 
                                                      "session"     ,"sh","st");

            //Connect application layer to presentation layer
            //applicationLayer.toLowerQ =
            //    presentationLayer.GetPingPongQueueReference(ref presentationLayer.FromUpperQ);
            //applicationLayer.fromLowerQ =
            //    presentationLayer.GetPingPongQueueReference(ref presentationLayer.ToUpperQ);

            ////Connect presentation layer to session layer
            //presentationLayer.toLowerQ =
            //    sessionLayer.GetPingPongQueueReference(ref sessionLayer.fromUpperQ);
            //presentationLayer.fromLowerQ =
            //    sessionLayer.GetPingPongQueueReference(ref sessionLayer.toUpperQ);

            //presentationLayer.FromUpperQ = applicationLayer.To;

            OsiSevenLayers.Add(applicationLayer);
            OsiSevenLayers.Add(presentationLayer);
            OsiSevenLayers.Add(sessionLayer);
        }
    }
    public class Node
    {
        public int id;
        public string? Name;
        public OSIStack stack = new OSIStack();

        //public Hardware hardware = new Hardware();
        public Node(string Name = "Undefined")
        {
            this.id = sglobal.GetGlobalId();
            this.Name = Name;
        }

        public void send(string s)
        {
            sglobal.data = "<hello>";
            Layer.Packet applicationPacket = new Layer.Packet();
            applicationPacket.data = sglobal.data;
            applicationPacket.head = new Layer.Head();
            applicationPacket.tail = new Layer.Tail();

            Layer topLayer = stack.OsiSevenLayers.First();
            topLayer.ToLowerQ.Enqueue(applicationPacket);
        }

        public string receive()
        {
            return String.Empty;
        }
    }
    public class PhysicalSystems
    {
        Queue<byte> hwChannel;
        public int size;
        public PhysicalSystems(int size)
        {
            hwChannel = new Queue<byte>(size);
            this.size = size;
        }
    }
    public class TestHarness
    {
        public int id;
        public string Name;

        public TestHarness(string Name = "Undefined")
        {
            id = sglobal.GetGlobalId();
            this.Name = Name;
        }
        public bool test_case_01()
        {
            //Two point to point connected computers
            //exchanging a single packet
            //application-data link-physical

            //node1: 3 - Layers: Top to Bottom
            Node n1 = new Node("n1");

            int tick = 0;
            List<Layer> layers = n1.stack.OsiSevenLayers;
            Layer topLayer = layers.First();
            Layer bottomLayer = layers.Last();
            Layer upperLayer = topLayer;
            Layer lowerLayer = new Layer(0);
            int curLayerIndex;
            sglobal.InitOsiCycle(BufferCycle.PING_READ_PONG_WRITE);

            while (tick < 10)
            {
                if (tick % 5 == 0)
                {
                    n1.send("hello " + tick);
                }

                curLayerIndex = 0;
                foreach (Layer layer in layers)
                {
                    layer.Ontick(curLayerIndex, layers);
                    curLayerIndex++;
                }

                tick++;
                sglobal.ToggleOsiCycle();
            }
            return false;
        }
    }

}











