//using OO_OSI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace oolayer_Script
{

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
        public Head head;
        public Payload payload;
        public string signature;
        public Tail tail;

        public Packet(Head head, Payload payload, Tail tail)
        {
            this.head = head;
            this.payload = payload;
            this.signature = "sig_" + head.signature + "_" + payload.GetSignature(); ;            
            this.tail = tail;
        }

        public override string GetSignature()
        {
            string payload_sign = payload.GetSignature();
            return head.signature + "_" + payload_sign;
        }

    }

    class EndPacket : Payload
    {
        public string data;
        //interface functions
        public override string GetSignature()
        { return "_endnode_"; }
        public EndPacket(string s = "undefined")
        { data = s; }
    }
    class LayerHead : Head
    {
        public LayerHead()
        { }
        public LayerHead(string layer, string signature)
        {
            //In this place layer specific computation
            //should be placed
            prefix = "<" + layer + "h>";
            this.signature = signature;
            suffix = "</" + layer + "h>";
        }
    }

    class LayerTail : Tail
    {
        public LayerTail()
        {
        }
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
        public Packet layerPacket;
        public Packet upperLayerPacket;
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
                Head topHead      = new LayerHead(layer, "sig top");
                Tail topTail      = new LayerTail(layer);
                Payload topPacket = new EndPacket("top hello");

                layerPacket = new Packet(topHead,
                                         topPacket,
                                         topTail);
                fromUpperQ.Enqueue(layerPacket);
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
                
                string tempLayer = "Application";
                Head    bottomHead      = new LayerHead(tempLayer, "sigapp");
                Tail    bottomTail      = new LayerTail(tempLayer);
                Payload bottomPacket    = new EndPacket("bottom_hello");
                

                layerPacket = new Packet(bottomHead, 
                                         bottomPacket,
                                         bottomTail);

                

                tempLayer = "Session";
                Head sessionHead  = new LayerHead(tempLayer,"sigssn");
                Tail sessionTail  = new LayerTail(tempLayer);
                string layerSignature    = layerPacket.GetSignature();                
                layerPacket = new Packet(sessionHead,
                                         layerPacket,
                                         sessionTail);


                tempLayer = "Presentation";
                Head applicationHead = new LayerHead(tempLayer, "sigpsn");
                Tail applicationTail = new LayerTail(tempLayer);
                layerPacket = new Packet(applicationHead,
                                         layerPacket,
                                         applicationTail);

                fromLowerQ.Enqueue(layerPacket);

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
            layerPacket = null;
            while (fromUpperQ.Count > 0)
            {
                Payload packetFromUpperLayer = fromUpperQ.Dequeue();
                string upperSignature = packetFromUpperLayer.GetSignature();
                string currentSignature = layer + "_" + upperSignature;
                layerHead = new LayerHead(layer, currentSignature);
                layerTail = new LayerTail(layer);
                layerPacket = new Packet(layerHead,
                                         packetFromUpperLayer,
                                         layerTail);
            }
        }
        public void downwardWrite()
        {
            //TODO: Read difference between is null and == null
            if (!(layerPacket == null))
            {
                toLowerQ.Enqueue(layerPacket);
            }
        }
        public void upwardRead()
        {            
            upperLayerPacket = null;
            if (fromLowerQ.Count > 0)
            {               
                Payload packetFromLowerLayer = fromLowerQ.Dequeue();
                Packet curLayerPacket = (Packet)packetFromLowerLayer;
                if (stackPosition != StackPosition.TOP)
                {
                    upperLayerPacket = (Packet)curLayerPacket.payload;
                }                
            }
        }
        public void upwardWrite()
        {
            if (!(upperLayerPacket == null))
            {
                toUpperQ.Enqueue(upperLayerPacket);
            }
        }
    }

}


