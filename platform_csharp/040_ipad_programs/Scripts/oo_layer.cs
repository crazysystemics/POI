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

    class TopPacket : Payload
    {
        public string data;
        //interface functions
        public override string GetSignature()
        { return data; }
        public TopPacket(string s = "undefined")
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



    class OOLayer
    {
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
        }

        public void OnTick(RWPhase rwphase)
        {
            if (rwphase == RWPhase.READ)
                downwardRead();
            else
                downwardWrite();
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

            Head topHead = new LayerHead(layer, "top");
            Tail topTail = new LayerTail(layer, "top");
            Payload topPacket = new TopPacket("top hello");
            layerPacket = new Packet(topHead, topPacket, topPacket.GetSignature(),
                                     topTail);
            fromUpperQ.Enqueue(layerPacket);


        }

        public string getOutput()
        {
            if (toLowerQ.Count > 0 )
            {
                Packet packet = toLowerQ.Dequeue();
                return packet.GetSignature();
            }

            return "Queue Empty";
            
        }

        //read write calls are separated to discrete-time-modelling
        //read of all layers are called in one phase
        //write of all layers are called in another phase
        //this prevents race-condition without ping-pong buffer
        public void downwardRead()
        {
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
                toLowerQ.Enqueue(layerPacket);
            }
        }
        public void downwardWrite()
        {
            toLowerQ.Enqueue(layerPacket);
        }
        public void upwardRead()
        {
            Payload packetFromLowerLayer = fromLowerQ.Dequeue();
            upperLayerPacket =
                          (Packet)(((Packet)packetFromLowerLayer).payload);

        }
        public void upwardWrite()
        {
            toUpperQ.Enqueue(upperLayerPacket);
        }
    }


}
