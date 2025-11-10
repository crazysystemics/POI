using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Test_Bench
{
    static class sglobals
    {
        public static UDP discrete_tx_udp;
        public static UDP discrete_rx_udp;
        public static UDP analog_tx_udp;
        public static UDP analog_rx_udp;
        public static UDP arinc_tx_udp;
        public static UDP arinc_rx_udp;
        public static UDP milbus_tx_udp;
        public static UDP milbus_rx_udp;
        public static UDP fpga_rx_udp;
    }

    public enum Globals
    {
        BUS_CYCLE = 1, MFS = 1, ARINC_CYCLE = 1, ANALOG_CYCLE = 1, DISCRETE_CYCLE = 1
    }
}
