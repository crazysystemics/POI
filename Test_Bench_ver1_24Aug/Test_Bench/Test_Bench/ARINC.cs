using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Bench
{
    public interface IArincBus
    {
        void tx_arinc(int tick, Dictionary<uint, uint> gLabelMessageDictionary);
        void rx_arinc(int tick, Dictionary<uint, uint> gLabelMessageDictionary, List<SimulatedModel> simulatedModels);
    }

    public class ARINC : BUS
    {
        public Dictionary<uint, uint> gLabelMessageDictionary = new Dictionary<uint, uint>();
    }
}
