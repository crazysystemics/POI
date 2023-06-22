using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oolayer_Script
{
    abstract class Payload { public abstract string GetSignature(); };
    abstract class Head { public string prefix, data, suffix; };
    abstract class Tail { public string prefix, data, suffix; };
    class Packet:Payload
    {
        public Head head;
        public Payload payload;
        public string signature;
        public Tail tail;

        public Packet(Head head, Payload payload, string signature, Tail tail)
        {
            this.head    = head;
            this.payload = payload;
            this.signature = signature;
            this.tail    = tail;
        }

        public override string GetSignature()
        {
            return signature;
        } 

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
            data   = signature;
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


    internal class OOLayer
    {
        public string layer;
        public Packet layerPacket;
        public Packet upperLayerPacket;

        Queue<Packet> fromUpperQ = new Queue<Packet>();
        Queue<Packet> toUpperQ = new Queue<Packet>();

        Queue<Packet> toLowerQ      = new Queue<Packet>();
        Queue<Packet> fromLowerQ    = new Queue<Packet>();

        LayerHead layerHead; 
        LayerTail layerTail;
        public OOLayer(string layer)
        {
            this.layer = layer;
        }

        public void OnTick()
        {
            upwardRead();         
        }


        public void downardRead()
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

        public void downwardWrite()
        {
            toLowerQ.Enqueue(layerPacket);
        }
        


        public void upwardRead()
        {
            Payload packetFromLowerLayer  = fromLowerQ.Dequeue();
              upperLayerPacket      = 
                            (Packet)(((Packet)packetFromLowerLayer).payload);
            
        }

        public void upwardWrite()
        {
            toUpperQ.Enqueue(upperLayerPacket);
        }

        


    }

    //Another major design change queues in Q-Complex
    //Is it good modeling practice?
    static class QComplex
    {
        public static  Queue<Packet> appm2presnQ = new Queue<Packet>();
        public static Queue<Packet> presn2session = new Queue<Packet>();
        public static Queue<Packet> session2presn = new Queue<Packet>();
        public static Queue<Packet> presn2appn = new Queue<Packet>();
    }


    class ooLayerWorld
    {
       OOLayer applicationLayer = new OOLayer("application");
    }
}
