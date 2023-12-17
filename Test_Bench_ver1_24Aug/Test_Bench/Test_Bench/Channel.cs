using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Bench
{
    public class Channel
    {
        public Port localPort;
    }

    public class AnalogChannel : Channel
    {
        public double minVolt, maxVolt;
        public double minValidVolt, maxValidVolt;

        public AnalogChannel(int local_id)
        {
            localPort = new Port(local_id);
        }
    }

    public class DiscreteChannel : Channel
    {
        public int level; //1, 0 or -1 
        public double min1v, max1v, min0v, max0v;
        public double minVolt, maxVolt;

        public DiscreteChannel(int local_id)
        {
            localPort = new Port(local_id);
        }
    }

    public interface IAnalog
    {
        void tx_analog(int tick);

        void rx_analog(int tick, List<SimulatedModel> simulatedModels);

        //void configAnalog(int channel, double min_volt, double max_volt);
        //void setAnalog(int channel, double vol_level, double current_level);
    }

    public interface IDiscrete
    {
        void tx_discrete(int tick);

        void rx_discrete(int tick, List<SimulatedModel> simulatedModels);

        //void configAnalog(int channel, double min_volt, double max_volt);
        //void setAnalog(int channel, double vol_level, double current_level);
    }
}

