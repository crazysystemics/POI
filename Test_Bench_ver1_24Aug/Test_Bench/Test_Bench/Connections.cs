using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Bench
{
    public class Port
    {
        public int id;
        public Port remotePort;
        public int propagation_delay;
        public float? inbuf;
        public float? outbuf;

        public Port(int id)
        {
            this.id = id;
        }

        public float? read()
        {
            if (inbuf is null)
                return null;
            else
                return inbuf;
        }
        public void write(float? val, int pdelay)
        {
            if (this.outbuf is null || this.outbuf is 0)
            {
                this.outbuf = val;
                propagation_delay = pdelay;
            }
        }

        public void ClockTick(int propagationDelay = 0)
        {
            //TODO: Implement a delay queue to accommodate
            //bytes written before reading. Right now assumption is that
            //no new data is written till current data is read.


            if (propagationDelay > 0)
                propagation_delay--;

            if (propagation_delay == 0 && outbuf != null)
            {
                remotePort.inbuf = outbuf;
                outbuf = 0;
            }
        }

        public void Connect(Port remotePort, uint propagation_delay = 0)
        {
            this.remotePort = remotePort;
            remotePort.remotePort = this;
            this.propagation_delay = (int)propagation_delay;
        }
    }
}
