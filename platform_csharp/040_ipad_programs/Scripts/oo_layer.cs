﻿//using OO_OSI;
using OutOfScope;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace oolayer_Script
{

    static class sglobal
    {
        public static int line = 0;
        public static string caller = String.Empty;

        public static void RegisterInvocationSource(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null)
        {
            //Console.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");
            line = lineNumber;
            sglobal.caller = caller;
        }
    }

    public enum QueueType { FROM_UPPER, FROM_LOWER, TO_UPPER, TO_LOWER }
    public enum StackPosition { TOP, BOTTOM, MIDDLE }
    public enum RWPhase { READ, WRITE }

    public static class TestOptions
    {
        public static bool NEVER_ASSERT = true;
        public static bool ALWAYS_ASSERT = false;
        public static bool VERBOSE = true;
        public static bool SILENT = false;
    }




    //Receiver rcvr = new Receiver();
    //Sender sender = new Sender();
    //sender.Connect(rcvr.queue);
    //sender.putQ("hello");
    //string s = rcvr.getsQ();
    //Console.WriteLine(s);
    abstract class Payload { public abstract string GetSignature(); };
    abstract class Head    { public string prefix, signature, suffix; };
    abstract class Tail    { public string prefix, data, suffix; };
    class Packet : Payload
    {
        public string start = String.Empty;
        public string layer = String.Empty;
        public Head    head;
        public Payload payload;
        public Tail tail;
        public string end = String.Empty;
        

        public Packet(Head head, Payload payload, Tail tail)
        {
            this.head    = head;
            this.payload = payload;
            this.tail    = tail;
            //this.signature = "sig_" + head.signature + "_" + payload.GetSignature(); ;            

        }

        public override string GetSignature()
        {
            //payload is not end packet, so head contains signature
            return head.signature ;
        }

    }

    class EndPacket : Payload
    {
        public string data;
        //interface functions
        public override string GetSignature()
        { return "[" + data + "]"; }
        public EndPacket(string s = "undefined")
        { data = s; }
    }
    class LayerHead : Head
    {
        string layer;
        public LayerHead(string layer, string signature)
        {
            //In this place layer specific computation
            //should be placed
            this.layer = layer;
            prefix = "<" + layer + "h>";
            this.signature = signature;
            suffix = "</" + layer + "h>";
        }
    }

    class LayerTail : Tail
    {
       
        public LayerTail(string layer)
        {
            //In this place layer specific computation
            //should be placed
            data = "</" + layer + "h>";
        }
    }

    //TODO: How to put constraint that every class will have
    //TODO: a public member Name.

    public class ApplicationPayload
    {
        public string data = "top hello";
    }

    class OOLayer
    {
        public string Name;
        public string layer;
        public Packet curPacket;
        public Queue<Packet> downPacketBuf = new Queue<Packet>();
        public Queue<Packet> upPacketBuf = new Queue<Packet>();
        public Packet upPacket;
        public Packet downPacket;
        public StackPosition stackPosition;
        

        public Queue<Packet> fromUpperQ = new Queue<Packet>();
        public Queue<Packet> toUpperQ = new Queue<Packet>();

        public Queue<Packet> toLowerQ = new Queue<Packet>();
        public Queue<Packet> fromLowerQ = new Queue<Packet>();

        LayerHead layerHead;
        LayerTail layerTail;
        public OOLayer(string layer, StackPosition stackPosition = StackPosition.MIDDLE)
        {
            this.layer         = layer;
            this.Name          = layer;
            this.stackPosition = stackPosition;
        }

        public void OnTick(RWPhase rwphase)
        {
            if (rwphase == RWPhase.READ)
            {
                downwardRead();                
                upwardRead();
                
            }
            else
            {
                downwardWrite();
                upwardWrite();
            }
        }

        public Queue<Packet> GetQ(QueueType destination)
        {
            if (destination == QueueType.FROM_UPPER)
                return fromUpperQ;
            else if (destination == QueueType.FROM_LOWER)
                return fromLowerQ;
            else if (destination == QueueType.TO_UPPER)
                return toUpperQ;
            else if (destination == QueueType.TO_LOWER)
                return toLowerQ;
            else
                Debug.Assert(false);

            return null;
        }

        public void setInput()
        {
            //string shead = layer + "_";
            string sendhead = (stackPosition == StackPosition.TOP) ? "top" : "bottom";
            //string stail = layer + "_";
            string sendtail = (stackPosition == StackPosition.TOP) ? "top" : "bottom";          

            

            if (stackPosition == StackPosition.TOP)
            {
                Head topHead      = new LayerHead(layer, "top_hello");
                Tail topTail      = new LayerTail(layer);
                Payload topPacket = new EndPacket("hello");

                curPacket = new Packet(topHead,
                                         topPacket,
                                         topTail);
                fromUpperQ.Enqueue(curPacket);
            }
            else
            {
                //bottom - indicates whatever bottom layers
                //<bottomh>
                //  <sessionh>
                //      <applicationh>
                //          <bottom hello>
                //      <applicationt>
                //   <sessiont>
                //<bottomt>

                //Input to Session Layer is application + session layer
                //It receives this from bottom layer
                
                string tempLayer = "Presentation";               

                Head    bottomHead      = new LayerHead(tempLayer, tempLayer + "hello");
                Tail    bottomTail      = new LayerTail(tempLayer);
                Payload bottomPacket    = new EndPacket("bottom_hello");
                

                curPacket = new Packet(bottomHead, 
                                         bottomPacket,
                                         bottomTail);
                Console.WriteLine("method:setInput in layer: {0},"
                                      + "signature: {1} ",
                                        layer, curPacket.GetSignature());

                tempLayer = "session";
                string curSignature = tempLayer + curPacket.GetSignature();
                Head sessionHead  = new LayerHead(tempLayer, curSignature);
                Tail sessionTail  = new LayerTail(tempLayer);                             
                curPacket = new Packet(sessionHead,
                                         curPacket,
                                         sessionTail);


                tempLayer = "Presentation";
                Head applicationHead = new LayerHead(tempLayer, "");
                Tail applicationTail = new LayerTail(tempLayer);
                curPacket = new Packet(applicationHead,
                                         curPacket,
                                         applicationTail);

                fromLowerQ.Enqueue(curPacket);

            }
            
            
        }

        public string getOutput()
        {
            //if (stackPosition == StackPosition.TOP)
            //{
            //    if (toUpperQ.Count > 0)
            //    {
            //        Packet packet = toUpperQ.Dequeue();
            //        return packet.GetSignature();
            //    }
            //}
            //else
            {
                if (toLowerQ.Count > 0)
                {
                    Packet packet = toLowerQ.Dequeue();
                    return packet.GetSignature();                    
                }
            }

            return "Queue Empty";
        }

        //read write calls are separated to discrete-time-modelling
        //read of all layers are called in one phase
        //write of all layers are called in another phase
        //this prevents race-condition without ping-pong buffer

        //TODO: two separate variables uninited and (undefined)
        //uninited is an error whereas undefined can be design (like don't care)
        public void downwardRead()
        {
            curPacket = null;
            downPacketBuf = new Queue<Packet>();
            while (fromUpperQ.Count > 0)
            {
                Payload packetFromUpperLayer = fromUpperQ.Dequeue();
                string currentSignature = layer + "_" +
                       packetFromUpperLayer.GetSignature();
                layerHead   = new LayerHead(layer, currentSignature);
                layerTail   = new LayerTail(layer);
                curPacket   = new Packet(layerHead,
                                         packetFromUpperLayer,
                                         layerTail);
                //TODO: Generic debug string
                Console.WriteLine("method:downwardRead in layer: {0}, signature: {1} " ,
                                           layer , currentSignature);
                downPacketBuf.Enqueue(curPacket);
            }
        }
        public void downwardWrite()
        {
            //TODO: Read difference between is null and == null
            while (downPacketBuf.Count > 0)
            {
                Packet packet = downPacketBuf.Dequeue();
                toLowerQ.Enqueue(packet);

                Console.WriteLine("method:downwardWrite in layer: {0}, signature: {1} ",
                                                         layer, packet.GetSignature());
            }
        }
        public void upwardRead()
        {            
            
            while (fromLowerQ.Count > 0)
            {               
                Payload packetFromLowerLayer = fromLowerQ.Dequeue();
                Packet curPacket = (Packet)packetFromLowerLayer;
                string curSignature = curPacket.GetSignature();
                if (stackPosition != StackPosition.TOP)
                {
                    upPacketBuf.Enqueue((Packet)curPacket.payload);

                    Console.WriteLine("method:upwardRead in layer: {0},"
                                      + "signature: {1} ",
                                        layer, curSignature);
                }                
            }
        }
        public void upwardWrite()
        {
            while (upPacketBuf.Count > 0)
            {
                Packet curPacket = upPacketBuf.Dequeue();
                string curSignature = curPacket.GetSignature();
                
                toUpperQ.Enqueue(curPacket);
                Console.WriteLine("method:upwardRead in layer: {0}, signature: {1} ",
                                          layer, curSignature);
            }
        }
    }

}


