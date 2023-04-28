using System;
using System.Net.Sockets;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Common;

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
    public enum StackPosition { TOP, BOTTOM, MIDDLE }
    public static class sglobal
    {
        public static string data = String.Empty;
        private static int global_id = 0;
        public static int GetGlobalId() { return global_id++; }

    }


    public abstract class Payload { public string data; }
    public class PingPongQueue<T> : Queue<T> where T : class?
    {
        public Queue<T> pingBuffer = new Queue<T>();
        public Queue<T> pongBuffer = new Queue<T>();
        public Buffer readBuffer, writeBuffer;
        public Buffer buffer;
        public PingPongQueue()
        {
            readBuffer = Buffer.PING;
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
                if (pingBuffer.Count > 0)
                {
                    return pingBuffer.Dequeue();
                }
            }
            else
            {
                if (pongBuffer.Count > 0)
                {
                    return pongBuffer.Dequeue();
                }
            }

            return null;
        }

        public Buffer Toggle(Buffer b)
        {
            if (b == Buffer.PING)
                return Buffer.PONG;
            else
                return Buffer.PING;
        }

        public void ToggleReadWriteBuffers()
        {
            Toggle(readBuffer);
            Toggle(writeBuffer);
        }

    }
    class ApplicationPayload : Payload
    {
        public ApplicationPayload(string data)
        {
            this.data = data;
        }
    }
    class PresentationPayload : Payload
    { }
    class SessionPayload : Payload
    { }
    public class Layer
    {
        int id;
        
        public class Packet : Payload
        {
            public Head head;
            public Payload payload;
            public Tail tail;

            public Packet()
            { }

            public Packet(Head head, Payload payload, Tail tail)
            { this.head = head; this.payload = payload; this.tail = tail; }
        }
        public class Head
        {
            string data;
            public Head()
            {
                data = "<head>";
            }
        }
        public class Tail
        {
            string data;
            public Tail()
            {
                data = "<tail>";
            }
        }

        //Members
        public string Name;
        public StackPosition position;        
        public PingPongQueue<Payload> fromUpperQ = new PingPongQueue<Payload>();
        public PingPongQueue<Payload> toUpperQ = new PingPongQueue<Payload>();
           
        public Head head = new Head();
        public Tail tail = new Tail();
  
        public PingPongQueue<Payload> toLowerQ
            = new PingPongQueue<Payload>();
        public PingPongQueue<Payload> fromLowerQ
            = new PingPongQueue<Payload>();
        public Packet layer_packet = new Packet();

        //Methods
        public Layer(StackPosition position, 
                     string Name = "undefined")
        {
            this.id = sglobal.GetGlobalId();
            this.Name = Name;
            this.position = position; 
        }
        public void Ontick()
        {
            TransferFromUpperToLower();
            //TransferFromLowerToUpper();
        }       
        public Head ComputeHead(Payload upperPacket)
        { return new Head(); }
        public Tail ComputeTail(Payload upperPacket)
        { return new Tail(); }
        //On deque from upper layer, PayloadT i.e., Payload of Transport come
        //On enque to   upper layer, Payloadt i.e.., Payload of Transform is sent upwards
        //Payload(Transform)=Session
        public ref PingPongQueue<Payload>
        GetPingPongQueueReference(ref PingPongQueue<Payload> ppQueue)
        {
            return ref ppQueue;
        }
        public void TransferFromUpperToLower()
        {
            //switch buffers of upper and lower queues
            Buffer toggledBuffer = fromUpperQ.Toggle(fromUpperQ.readBuffer);
            fromUpperQ.SetReadBuffer(toggledBuffer);

            toggledBuffer = toLowerQ.Toggle(toLowerQ.readBuffer);
            toLowerQ.SetReadBuffer(toggledBuffer);
            fromUpperQ.ToggleReadWriteBuffers();

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

            toLowerQ.ToggleReadWriteBuffers();            
            if (position == StackPosition.BOTTOM)
            {
                Payload p = fromUpperQ.Dequeue();
                Console.WriteLine(p.data);
            }
            else
            {
                Debug.Assert(position == StackPosition.MIDDLE);
                while (fromUpperQ.Count > 0)
                {
                    Payload upper_packet = fromUpperQ.Dequeue();
                    head = ComputeHead(upper_packet);
                    tail = ComputeTail(upper_packet);
                    Packet layer_packet =
                           new Packet(head, upper_packet, tail);
                    toLowerQ.Enqueue(layer_packet);
                }
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

                //We are interested only in Payload, Head and Tail
                //can be stripped off
                toUpperQ.Enqueue(lower_packet.payload);
            }
        }      

  
    }
    public class OSIStack
    {
        public List<Layer> OsiSevenLayers = new List<Layer>();


        public OSIStack()
        {
            Layer applicationLayer  = new Layer(StackPosition.TOP, "application");
            Layer presentationLayer = new Layer(StackPosition.MIDDLE, "presentation");
            Layer sessionLayer      = new Layer(StackPosition.BOTTOM,  "session");

            //Connect application layer to presentation layer
            applicationLayer.toLowerQ =
                presentationLayer.GetPingPongQueueReference(ref presentationLayer.fromUpperQ);
            applicationLayer.fromLowerQ =
                presentationLayer.GetPingPongQueueReference(ref presentationLayer.toUpperQ);
            
            //Connect presentation layer to session layer
            presentationLayer.toLowerQ =
                sessionLayer.GetPingPongQueueReference(ref sessionLayer.fromUpperQ);
            presentationLayer.fromLowerQ =
                sessionLayer.GetPingPongQueueReference(ref sessionLayer.toUpperQ);

            OsiSevenLayers.Add(applicationLayer);
            OsiSevenLayers.Add(presentationLayer);
            OsiSevenLayers.Add(sessionLayer);
        }
    }
    public class Node
    {
        public OSIStack stack = new OSIStack();

        //public Hardware hardware = new Hardware();

        public void send(string s)
        {
           
            sglobal.data = "<hello>"; 
            Layer.Packet applicationPacket = new Layer.Packet();
            applicationPacket.data = sglobal.data;
            applicationPacket.head = new Layer.Head();
            applicationPacket.tail = new Layer.Tail();

            Layer topLayer = stack.OsiSevenLayers[0];

                     
            topLayer.toUpperQ.SetReadBuffer(Buffer.PING);
            topLayer.toUpperQ.Enqueue(applicationPacket);
            //topLayer.TransferFromUpperToLower();
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

                //In rest of 90% time
                List<Layer> layers = n1.stack.OsiSevenLayers;
                foreach (Layer layer in layers)
                {
                    layer.Ontick();
                }
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
            //    int tick = 0;
            //    while (true)
            //    {

            //        foreach(Layer<Payload> l in node1.layers)
            //        {
            //            //dequeue phase
            //            l.TransferFromUpperToLower();
            //            l.TransferFromLowerToUpper();
            //        }

            //    }            
        }
    }
}











