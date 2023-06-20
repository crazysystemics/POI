using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oolayer_Scripts
{
    abstract class Payload { public string GetSignature(); };
    abstract class Head { };
    abstract class Tail { };
    class Packet
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

        public string GetSignature()
        {
            return signature;
        } 

    }

    class LayerHead : Head
    {
        public LayerHead()
        { }
        public LayerHead(string layer)
        {
            //In this place layer specific computation
            //should be placed
            return "<layer:" + layer + ">";
        }
    }

    class LayerTail : Tail
    {
        public LayerTail()
        {
        }
        public LayerTail(string layer)
        {
            return "<layer:" + layer + ">";
        }
    }


    internal class OOLayer
    {
        public string layer;
        public Packet layerPacket;

        Queue<Packet> fromUpperQ = new Queue<Packet>();
        Queue<Packet> toUpperQ = new Queue<Packet>();

        Queue<Packet> toLowerQ      = new Queue<Packet>();
        Queue<Packet> fromLowerQ    = new Queue<Packet>();

        LayerHead layerHead = new LayerHead(layer); 
        LayerTail layerTail      = new LayerTail(layer);
        public OOLayer(string layer)
        {
            this.layer = layer;
        }


        public void downardMovement()
        {
            Packet packetFromUpperLayer = fromUpperQ.Dequeue();
            
            
        }

    }

    static class QComplex
    {
        public static  Queue<Packet> appm2presnQ = new Queue<Packet>();
        public static Queue<Packet> presn2session = new Queue<Packet>();
        public static Queue<Packet> session2presn = new Queue<Packet>();
        public static Queue<Packet> presn2appn = new Queue<Packet>();

    }


    class ooLayerWorld
    {
       OOLayer applicationLayer = new OOLayer();
    }
}
