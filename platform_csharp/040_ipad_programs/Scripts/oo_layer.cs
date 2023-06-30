//using OO_OSI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oolayer_Script
{

    public enum QueueType { FROM_UPPER, FROM_LOWER }
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
    abstract class Head { public string prefix, data, suffix; };
    abstract class Tail { public string prefix, data, suffix; };
    class Packet : Payload
    {
        public Head head;
        public Payload payload;
        public string signature;
        public Tail tail;

        public Packet(Head head, Payload payload, string signature, Tail tail)
        {
            this.head = head;
            this.payload = payload;
            this.signature = signature;
            this.tail = tail;
        }

        public override string GetSignature()
        {
            return signature;
        }

    }

    class EndPacket : Payload
    {
        public string data;
        //interface functions
        public override string GetSignature()
        { return data; }
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
            prefix = "<head:layer=" + layer;
            data = signature;
            suffix = "</head>";
        }
    }

    class LayerTail : Tail
    {
        public LayerTail()
        {
        }
        public LayerTail(string layer, string signature)
        {
            //In this place layer specific computation
            //should be placed
            prefix = "<tail:layer=" + layer;
            data = signature;
            suffix = "<tail/head>";
        }
    }

    //TODO: How to put constraint that every class will have
    //TODO: a public member Name.

    class OOLayer
    {
        public string Name;
        public string layer;
        public Packet layerPacket;
        public Packet upperLayerPacket;

        public Queue<Packet> fromUpperQ = new Queue<Packet>();
        public Queue<Packet> toUpperQ = new Queue<Packet>();

        public Queue<Packet> toLowerQ = new Queue<Packet>();
        public Queue<Packet> fromLowerQ = new Queue<Packet>();

        LayerHead layerHead;
        LayerTail layerTail;
        public OOLayer(string layer)
        {

            this.layer = layer;
            this.Name = layer;
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
            else
                return fromLowerQ;
        }

        public void setInput(string s, StackPosition position)
        {

            string shead = (position == StackPosition.TOP) ? "top" : "bottom";
            string stail = shead;
            Head endHead = new LayerHead(layer, shead);
            Tail endTail = new LayerTail(layer, stail);
            Payload endPacket = new EndPacket(shead + " hello");
          
            layerPacket = new Packet(endHead, endPacket, endPacket.GetSignature(),
                                     endTail);

            if (position == StackPosition.TOP)
                fromUpperQ.Enqueue(layerPacket);
            else
                fromLowerQ.Enqueue(layerPacket);
        }

        public string getOutput(StackPosition position)
        {
            if (position == StackPosition.TOP)
            {
                if (toLowerQ.Count > 0)
                {
                    Packet packet = toLowerQ.Dequeue();
                    return packet.GetSignature();
                }
            } 
            else 
            {
                if (toUpperQ.Count > 0)
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
                layerTail = new LayerTail(layer, currentSignature);
                layerPacket = new Packet(layerHead,
                                                     packetFromUpperLayer,
                                                     currentSignature,
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
                Packet tempPacket = (Packet)packetFromLowerLayer;
                upperLayerPacket = tempPacket;
                              //(Packet)(((Packet)packetFromLowerLayer).payload);
            }

        }
        public void upwardWrite()
        {
            if (!(upperLayerPacket == null))
            {
                fromLowerQ.Enqueue(upperLayerPacket);
            }
        }
    }

}


