using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Test_Bench
{
    class TestBench
    {
        public bool on = true;
        public UDP udp_tick = new UDP(10);
        public SimulationEngine SimE = new SimulationEngine();

        public void init()
        {
            HW_init();
            Ethernet_init();
        }

        public void Run()
        {
            int tick = 1;
            byte[] data = new byte[1];
            SimE.SetUp();

            while (on)
            {
                data = BitConverter.GetBytes(tick);
                udp_tick.UDP_Send(data, data.Length);

                SimE.OnTick(tick);
                tick++;
            }
        }

        public void HW_init()
        {
            float a_testcase = Convert.ToSingle(Console.ReadLine());
            float b_testcase = Convert.ToSingle(Console.ReadLine());

            P p = new P(a_testcase);
            Q q = new Q(b_testcase);
            SU30Cockpit_Simulator cockpit = new SU30Cockpit_Simulator();
            MC mc1 = new MC();

            SimE.SimulatedModels.Add(q);
            SimE.SimulatedModels.Add(p);
            SimE.SimulatedModels.Add(cockpit);
            SimE.SimulatedModels.Add(mc1);

            SimE.millBus.RTModelDictionary.Add(RT.P, p);
            SimE.millBus.RTModelDictionary.Add(RT.Q, q);
            SimE.millBus.RTModelDictionary.Add(RT.COCKPIT, cockpit);

            Message[] messages = new Message[4];
            messages[0] = new Message(1, MessageType.RT_RT, RT.P, RT.MFWS, 0, 1, 0, 0, 0);
            messages[1] = new Message(2, MessageType.RT_RT, RT.Q, RT.MFWS, 1, 1, 1, 0, 0);
          //  messages[2] = new Message(3, MessageType.RT_RT, RT.MFWS, RT.BC_MC, 2, 1, 2, 0, 0);
            messages[2] = new Message(2, MessageType.RT_RT, RT.MFWS, RT.COCKPIT, 2, 1, 2, 0, 0);

            MajorFrame pqSchedule = new MajorFrame();
            pqSchedule.majorframe[0] = new MinorFrame();
            pqSchedule.majorframe[0].minorframe = messages;

            SimE.millBus.schedule.majorFrame = pqSchedule;
        }

        public void Ethernet_init()
        {
            udp_tick.UDP_Connect();

            sglobals.discrete_tx_udp = new UDP(101);
            sglobals.discrete_tx_udp.UDP_Connect();
            sglobals.discrete_rx_udp = new UDP(102);
            sglobals.discrete_rx_udp.UDP_Bind();

            sglobals.analog_tx_udp = new UDP(201);
            sglobals.analog_tx_udp.UDP_Connect();
            sglobals.analog_rx_udp = new UDP(202);
            sglobals.analog_rx_udp.UDP_Bind();

            sglobals.arinc_tx_udp = new UDP(301);
            sglobals.arinc_tx_udp.UDP_Connect();
            sglobals.arinc_rx_udp = new UDP(302);
            sglobals.arinc_rx_udp.UDP_Bind();

            sglobals.milbus_tx_udp = new UDP(401);
            sglobals.milbus_tx_udp.UDP_Connect();
            sglobals.milbus_rx_udp = new UDP(402);
            sglobals.milbus_rx_udp.UDP_Bind();
        }
    }
}
