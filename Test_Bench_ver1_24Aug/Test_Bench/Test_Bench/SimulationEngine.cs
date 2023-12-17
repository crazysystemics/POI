using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Bench
{
    class SimulationEngine
    {
        public List<SimulatedModel> SimulatedModels = new List<SimulatedModel>();
        public MIL1553B millBus = new MIL1553B();
        public ARINC arincBus = new ARINC();
        public List<Port> AnalogRxPorts = new List<Port>();
        public List<Port> AnalogTxPorts = new List<Port>();
        public MinorFrame mf;

        public SimulationEngine()
        {

        }

        public void SetUp()
        {

        }

        public void Fuse()
        {

        }

        public void OnTick(int tick)
        {
            foreach (SimulatedModel sm in SimulatedModels)
            {
                sm.tx_discrete(tick);
                sm.tx_analog(tick);
                sm.tx_arinc(tick, arincBus.gLabelMessageDictionary);
            }
            if (tick % Convert.ToInt32(Globals.DISCRETE_CYCLE) == 0 && tick != 0)
                SimulatedModels[0].rx_discrete(tick, SimulatedModels);

            if (tick % Convert.ToInt32(Globals.ANALOG_CYCLE) == 0 && tick != 0)
                SimulatedModels[0].rx_analog(tick, SimulatedModels);

            if (tick % Convert.ToInt32(Globals.ARINC_CYCLE) == 0 && tick != 0)
                SimulatedModels[0].rx_arinc(tick, arincBus.gLabelMessageDictionary, SimulatedModels);

            foreach (SimulatedModel sm in SimulatedModels)
            {
                if (sm is IMilbusBC)
                {
                    if (tick % Convert.ToInt32(Globals.BUS_CYCLE) == 0 && tick != 0)
                    {
                        mf = ((MC)sm).GetMinorFrame(millBus.schedule.majorFrame, millBus.minorFrameIndex++ % Convert.ToInt32(Globals.MFS));

                        foreach (Message msg in mf.minorframe)
                        {
                            if (msg != null && msg.DestinationRT == RT.MFWS)
                                ((MC)sm).ExecuteTxMessage(tick, msg, millBus.RTModelDictionary);
                        }

                        foreach (Message msg in mf.minorframe)
                        {
                            if (msg != null && msg.SourceRT == RT.MFWS)
                            {
                                if (sglobals.milbus_rx_udp.UDP_Receive() != -1)
                                {
                                    ((MC)sm).ExecuteRxMessage(tick, msg, millBus.RTModelDictionary);
                                }
                            }
                        }
                    }
                }
            }

            if (tick % Convert.ToInt32(Globals.ARINC_CYCLE) == 0 && tick != 0)
                arincBus.gLabelMessageDictionary.Clear();

            foreach (SimulatedModel sm in SimulatedModels)
            {
                sm.Set();
                if (sm is FPGA)
                {
                    ((FPGA)sm).rx_fpga(tick);
                    //mainWindow.outputScreen.fpgaDisplay(((FPGA)sm).fpga_in);
                }
            }
        }
    }

    public class GlobalGRID
    {
        public void Query()
        {

        }

        public void Put()
        {

        }
    }
}
