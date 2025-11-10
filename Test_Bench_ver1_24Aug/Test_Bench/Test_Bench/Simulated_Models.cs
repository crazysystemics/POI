using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Bench
{
    public abstract class SimulatedModel : IArincBus, IAnalog, IDiscrete
    {
        public int id;
        public int offset;

        // MIL1553B Interface
        public RT rt_id;
        public ushort[,] SubAddress_Word = new ushort[32, 32];

        // ARINC Interface
        public List<uint> localLabels = new List<uint>();
        public Dictionary<uint, uint> local_arinc_rx_queue = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> local_arinc_tx_queue = new Dictionary<uint, uint>();

        // Analog Interface
        public AnalogChannel[] analogChannels = new AnalogChannel[10];
        public Dictionary<int, float?> local_analog_hw_rx_memory = new Dictionary<int, float?>();
        public Dictionary<int, float?> local_analog_hw_tx_memory = new Dictionary<int, float?>();

        public Dictionary<int, float?> analog_gin = new Dictionary<int, float?>();
        public Dictionary<int, float?> analog_gout = new Dictionary<int, float?>();

        // Analog Interface
        public DiscreteChannel[] discreteChannels = new DiscreteChannel[10];
        public Dictionary<int, float?> local_discrete_hw_rx_memory = new Dictionary<int, float?>();
        public Dictionary<int, float?> local_discrete_hw_tx_memory = new Dictionary<int, float?>();

        public Dictionary<int, float?> discrete_gin = new Dictionary<int, float?>();
        public Dictionary<int, float?> discrete_gout = new Dictionary<int, float?>();

        //FPGA Interface
        public uint[] fpga_in;

        public abstract void Set();
        public abstract SimulatedModel Get();
        public abstract void Update();

        public void tx_arinc(int tick, Dictionary<uint, uint> gLabelMessageDictionary)
        {
            int totalBytes;
            byte[] byteArray;
            byte[] uintBytes = null;
            List<uint> uintValues = new List<uint>();
            foreach (KeyValuePair<uint, uint> labelMessage in local_arinc_tx_queue)
            {
                if (gLabelMessageDictionary.ContainsKey(labelMessage.Key))
                    gLabelMessageDictionary[labelMessage.Key] = labelMessage.Value;
                else
                    gLabelMessageDictionary.Add(labelMessage.Key, labelMessage.Value);
            }

            foreach (KeyValuePair<uint, uint> labelMessage in gLabelMessageDictionary)
            {
                if (local_arinc_tx_queue.ContainsKey(labelMessage.Key))
                {
                    uintValues.Add(labelMessage.Value);
                    Console.WriteLine("Tick " + tick + ": ARINC Data Sent \t\t : Label : " + labelMessage.Key + "value :" + labelMessage.Value);
                }
            }
            if (local_arinc_tx_queue.Count != 0)
            {
                totalBytes = uintValues.ToArray().Length * sizeof(uint);
                int currentIndex = 0;
                byteArray = new byte[totalBytes + 1];
                byteArray[0] = Convert.ToByte(this.id);
                foreach (uint val in uintValues)
                {
                    uintBytes = BitConverter.GetBytes(val);
                    Buffer.BlockCopy(uintBytes, 0, byteArray, (currentIndex * sizeof(uint)) + 1, uintBytes.Length);
                    currentIndex += 1;
                }
                sglobals.arinc_tx_udp.UDP_Send(byteArray, byteArray.Length);
            }
        }

        public void rx_arinc(int tick, Dictionary<uint, uint> gLabelMessageDictionary, List<SimulatedModel> simulatedModels)
        {
            int i;
            int numbytesPerUint = sizeof(uint);
            int numUInts;

            foreach (SimulatedModel sm in simulatedModels)
            {
                if (sglobals.arinc_rx_udp.UDP_Receive() != -1)
                {
                    foreach (SimulatedModel sm1 in simulatedModels)
                    {
                        if (sglobals.arinc_rx_udp.data_received[0] == sm1.id)
                        {
                            numUInts = (sglobals.arinc_rx_udp.data_received.Length - 1) / numbytesPerUint;
                            uint[] uintValues = new uint[numUInts];

                            for (i = 0; i < uintValues.Length; i++)
                            {
                                uintValues[i] = BitConverter.ToUInt32(sglobals.arinc_rx_udp.data_received, (i * numbytesPerUint) + 1);
                            }

                            foreach (KeyValuePair<uint, uint> labelMessage in sm1.local_arinc_rx_queue)
                            {
                                if (gLabelMessageDictionary.ContainsKey(labelMessage.Key))
                                    gLabelMessageDictionary[labelMessage.Key] = labelMessage.Value;
                                else
                                    gLabelMessageDictionary.Add(labelMessage.Key, labelMessage.Value);
                            }
                            foreach (KeyValuePair<uint, uint> arinc_tx_message in gLabelMessageDictionary)
                            {

                                if (sm1.local_arinc_rx_queue.ContainsKey(arinc_tx_message.Key))
                                {
                                    for (i = 0; i < uintValues.Length; i++)
                                    {
                                        uint deciLabel = 0; //input label is interpreted as decimal number
                                        uint octaLabel = 0;
                                        uint msbWeight = 1;
                                        uint digit;
                                        deciLabel = arinc_tx_message.Key;
                                        while (deciLabel > 0)
                                        {
                                            digit = deciLabel % 10;
                                            octaLabel = digit * msbWeight + octaLabel;
                                            msbWeight = msbWeight * 8;
                                            deciLabel = deciLabel / 10;
                                        }
                                        if (octaLabel == (uintValues[i] & 0x000000ff))
                                        {
                                            sm1.local_arinc_rx_queue[arinc_tx_message.Key] = uintValues[i];
                                            Console.WriteLine("Tick " + tick + ": ARINC Data Received \t\t : Label : " + arinc_tx_message.Key + " value :" + uintValues[i]);
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        public void tx_analog(int tick)
        {
            List<float> floatValues = new List<float>();
            int totalBytes;
            byte[] byteArray;
            byte[] floatBytes = null;
            foreach (KeyValuePair<int, float?> gout in analog_gout)
            {
                local_analog_hw_tx_memory.Add(gout.Key, gout.Value); // / 0.5f);
            }

            foreach (KeyValuePair<int, float?> hw_tx_memory in local_analog_hw_tx_memory)
            {
                foreach (AnalogChannel ac in analogChannels)
                {
                    if (ac == null)
                        break;
                    else if (ac.localPort.id == hw_tx_memory.Key)
                    {
                        ac.localPort.write(hw_tx_memory.Value, 0);
                        floatValues.Add((float)ac.localPort.outbuf);

                        Console.WriteLine("Tick " + tick + ": Analog Data Sent \t\t : " + ac.localPort.outbuf);
                    }
                }
            }
            if (local_analog_hw_tx_memory.Count > 0)
            {
                totalBytes = floatValues.ToArray().Length * sizeof(float);
                int currentIndex = 1;
                byteArray = new byte[totalBytes + 1];
                byteArray[0] = Convert.ToByte(this.id);
                foreach (float val in floatValues)
                {
                    floatBytes = BitConverter.GetBytes(val);
                    Buffer.BlockCopy(floatBytes, 0, byteArray, currentIndex, floatBytes.Length);
                    currentIndex += floatBytes.Length;
                }
                sglobals.analog_tx_udp.UDP_Send(byteArray, byteArray.Length);
            }
            local_analog_hw_tx_memory.Clear();
        }

        public void rx_analog(int tick, List<SimulatedModel> simulatedModels)
        {
            int i;
            int bytesPerFloat = sizeof(float);
            int numberOfFloats;

            foreach (SimulatedModel sm in simulatedModels)
            {
                if (sglobals.analog_rx_udp.UDP_Receive() != -1)
                {
                    foreach (SimulatedModel sm1 in simulatedModels)
                    {
                        sm1.local_analog_hw_rx_memory.Clear();
                        if (sglobals.analog_rx_udp.data_received[0] == sm1.id)
                        {
                            numberOfFloats = (sglobals.analog_rx_udp.data_received.Length - 1) / bytesPerFloat;
                            float[] floatValues = new float[numberOfFloats];

                            for (i = 0; i < floatValues.Length; i++)
                            {
                                floatValues[i] = BitConverter.ToSingle(sglobals.analog_rx_udp.data_received, (i * bytesPerFloat) + 1);
                            }
                            i = 0;
                            foreach (AnalogChannel ac in sm1.analogChannels)
                            {
                                if (ac == null)
                                    break;

                                ac.localPort.inbuf = floatValues[i];
                                Console.WriteLine("Tick " + tick + ": Analog Data Received \t\t : " + ac.localPort.inbuf.Value);
                                i++;

                                if (ac != null && ac.localPort.inbuf != null)
                                {
                                    sm1.local_analog_hw_rx_memory.Add(ac.localPort.id, ac.localPort.inbuf);
                                }
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, float?> hw_rx_memory in local_analog_hw_rx_memory)
            {
                analog_gin[hw_rx_memory.Key] = hw_rx_memory.Value; // * 0.5f;
            }
        }

        public void tx_discrete(int tick)
        {
            List<byte> data = new List<byte>();
            data.Add(Convert.ToByte(this.id));

            foreach (KeyValuePair<int, float?> gout in discrete_gout)
            {
                local_discrete_hw_tx_memory.Add(gout.Key, gout.Value); // / 0.5f);
            }

            foreach (KeyValuePair<int, float?> hw_tx_memory in local_discrete_hw_tx_memory)
            {
                foreach (DiscreteChannel ac in discreteChannels)
                {
                    if (ac == null)
                        break;
                    else if (ac.localPort.id == hw_tx_memory.Key)
                    {
                        ac.localPort.write(hw_tx_memory.Value, 0);
                        //ac.localPort.ClockTick();

                        data.Add(Convert.ToByte(ac.localPort.outbuf));
                        Console.WriteLine("Tick " + tick + ": Discrete Data Sent \t\t : " + Convert.ToByte(ac.localPort.outbuf));
                    }
                }
            }
            if (local_discrete_hw_tx_memory.Count > 0)
                sglobals.discrete_tx_udp.UDP_Send(data.ToArray(), data.Count);
            local_discrete_hw_tx_memory.Clear();
        }

        public void rx_discrete(int tick, List<SimulatedModel> simulatedModels)
        {
            int i = 1;

            foreach (SimulatedModel sm in simulatedModels)
            {
                if (sglobals.discrete_rx_udp.UDP_Receive() != -1)
                {
                    foreach (SimulatedModel sm1 in simulatedModels)
                    {
                        sm1.local_discrete_hw_rx_memory.Clear();
                        if (sglobals.discrete_rx_udp.data_received[0] == sm1.id)
                        {
                            foreach (DiscreteChannel ac in sm1.discreteChannels)
                            {
                                if (ac == null)
                                    break;

                                ac.localPort.inbuf = Convert.ToSingle(sglobals.discrete_rx_udp.data_received[i]);
                                Console.WriteLine("Tick " + tick + ": Discrete Data Received \t\t : " + ac.localPort.inbuf.Value);
                                i++;

                                if (ac != null && ac.localPort.inbuf != null)
                                {
                                    sm1.local_discrete_hw_rx_memory.Add(ac.localPort.id, ac.localPort.inbuf);
                                }
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, float?> hw_rx_memory in local_discrete_hw_rx_memory)
            {
                discrete_gin[hw_rx_memory.Key] = hw_rx_memory.Value; // * 0.5f;
            }
        }

        public void rx_fpga(int tick)
        {
            int i;
            int numbytesPerUint = sizeof(uint);
            int numUInts;

            if (sglobals.fpga_rx_udp.UDP_Receive() != -1)
            {
                numUInts = (sglobals.fpga_rx_udp.data_received.Length - 1) / numbytesPerUint;
                uint[] uintValues = new uint[numUInts];
                fpga_in = new uint[numUInts];
                for (i = 0; i < uintValues.Length; i++)
                {
                    uintValues[i] = BitConverter.ToUInt32(sglobals.fpga_rx_udp.data_received, (i * numbytesPerUint));
                    Console.WriteLine("Tick " + tick + ": FPGA Data Received \t\t value :" + uintValues[i]);
                }
                fpga_in = uintValues;
            }
        }
    }

    class P : SimulatedModel
    {
        public float a;
        public P(float pa)
        {
            a = pa;
            offset = 1;
            id = 1;
            rt_id = RT.P;
            SubAddress_Word[0, 0] = (ushort)pa;
            local_arinc_tx_queue.Add(101, (uint)a);

            analogChannels[0] = new AnalogChannel(1);
            analog_gout.Add(1, a);

            discreteChannels[0] = new DiscreteChannel(1);
            discrete_gout.Add(1, a);

            localLabels.Add(101);
        }

        public override SimulatedModel Get()
        {
            return this;
        }

        public override void Set()
        {

        }

        public override void Update()
        {

        }
    }

    class Q : SimulatedModel
    {
        public float b;
        public Q(float pb)
        {
            b = pb;
            offset = 1;
            id = 2;
            rt_id = RT.Q;
            SubAddress_Word[1, 0] = (ushort)pb;

            local_arinc_tx_queue.Add(201, (uint)b);

            analogChannels[0] = new AnalogChannel(2);
            analog_gout.Add(2, b);

            discreteChannels[0] = new DiscreteChannel(2);
            discrete_gout.Add(2, b);
            localLabels.Add(201);
        }

        public override SimulatedModel Get()
        {
            return this;
        }

        public override void Set()
        {

        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }

    class SU30Cockpit_Simulator : SimulatedModel
    {
        public int c;
        public float c1;
        public uint c2;

        public SU30Cockpit_Simulator()
        {
            offset = 1;
            id = 3;
            rt_id = RT.COCKPIT;
            localLabels.Add(301);
            analogChannels[0] = new AnalogChannel(6);
            analog_gin.Add(6, 0);

            discreteChannels[0] = new DiscreteChannel(6);
            discrete_gin.Add(6, 0);

            local_arinc_rx_queue.Add(301, 0);
        }

        public override SimulatedModel Get()
        {
            return this;
        }

        public override void Set()
        {
            if (local_arinc_rx_queue.ContainsKey(301))
                c = (int)local_arinc_rx_queue[301];

            c1 = (float)analog_gin[6];
            c2 = (uint)discrete_gin[6];
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }

    class MC : SimulatedModel, IMilbusBC
    {
        public MC()
        {
            offset = 1;
            id = 4;
            rt_id = RT.BC_MC;

            SubAddress_Word[0, 0] = 21;
        }

        public void ExecuteTxMessage(int tick, Message m, Dictionary<RT, SimulatedModel> RTs_Table)
        {
            int totalBytes;
            byte[] byteArray;
            byte[] uintBytes = null;
            List<uint> uintValues = new List<uint>();
            uint data;
            if (m != null)
            {
                if (m.messagetype == MessageType.BC_RT)
                {
                    if (rt_id == m.SourceRT)
                    {
                        for (uint j = m.StartWord; j <= m.EndWord; j++)
                        {
                            data = SubAddress_Word[m.SourceSubaddress, j];
                            uintValues.Add(data);
                            Console.WriteLine("Tick " + tick + ": MILBUS Data Sent \t\t : Source :" + m.SourceRT
                                + " Destination : " + m.DestinationRT + " SubAddr : " + m.SourceSubaddress + " Words : " + data);
                        }
                        totalBytes = uintValues.ToArray().Length * sizeof(uint);
                        int currentIndex = 0;
                        byteArray = new byte[totalBytes + 7];
                        byteArray[0] = Convert.ToByte(m.SourceRT);
                        byteArray[1] = Convert.ToByte(m.SourceSubaddress);
                        byteArray[2] = Convert.ToByte(m.numberOfWords);
                        byteArray[3] = Convert.ToByte(m.DestinationRT);
                        byteArray[4] = Convert.ToByte(m.DestinationSubaddress);
                        byteArray[5] = Convert.ToByte(m.StartWord);
                        byteArray[6] = Convert.ToByte(m.EndWord);

                        foreach (uint val in uintValues)
                        {
                            uintBytes = BitConverter.GetBytes(val);
                            Buffer.BlockCopy(uintBytes, 0, byteArray, (currentIndex * sizeof(uint)) + 7, uintBytes.Length);
                            currentIndex += 1;
                        }
                        sglobals.milbus_tx_udp.UDP_Send(byteArray, byteArray.Length);
                    }
                }

                if (m.messagetype == MessageType.RT_RT)
                {
                    foreach (KeyValuePair<RT, SimulatedModel> rt_entry in RTs_Table)
                    {
                        if (rt_entry.Key == m.SourceRT)
                        {
                            for (uint j = m.StartWord; j <= m.EndWord; j++)
                            {
                                data = RTs_Table[m.SourceRT].SubAddress_Word[m.SourceSubaddress, j];
                                uintValues.Add(data);
                                Console.WriteLine("Tick " + tick + ": MILBUS Data Sent \t\t : Source :" + m.SourceRT
                                    + " Destination : " + m.DestinationRT + " SubAddr : " + m.SourceSubaddress + " Words : " + data);
                            }
                            totalBytes = uintValues.ToArray().Length * sizeof(uint);
                            int currentIndex = 0;
                            byteArray = new byte[totalBytes + 7];
                            byteArray[0] = Convert.ToByte(m.SourceRT);
                            byteArray[1] = Convert.ToByte(m.SourceSubaddress);
                            byteArray[2] = Convert.ToByte(m.numberOfWords);
                            byteArray[3] = Convert.ToByte(m.DestinationRT);
                            byteArray[4] = Convert.ToByte(m.DestinationSubaddress);
                            byteArray[5] = Convert.ToByte(m.StartWord);
                            byteArray[6] = Convert.ToByte(m.EndWord);

                            foreach (uint val in uintValues)
                            {
                                uintBytes = BitConverter.GetBytes(val);
                                Buffer.BlockCopy(uintBytes, 0, byteArray, (currentIndex * sizeof(uint)) + 7, uintBytes.Length);
                                currentIndex += 1;
                            }
                            sglobals.milbus_tx_udp.UDP_Send(byteArray, byteArray.Length);
                            break;
                        }
                    }
                }
            }
        }

        public void ExecuteRxMessage(int tick, Message m, Dictionary<RT, SimulatedModel> RTs_Table)
        {
            List<uint> uintValues = new List<uint>();
            int i;
            int numbytesPerUint = sizeof(uint);
            int numUInts;


            if (m.messagetype == MessageType.RT_BC)
            {
                if (sglobals.milbus_rx_udp.data_received[3] == Convert.ToUInt32(m.DestinationRT))
                {
                    if (rt_id == m.DestinationRT)
                    {
                        numUInts = (sglobals.milbus_rx_udp.data_received.Length - 7) / numbytesPerUint;
                        uint[] uintValue = new uint[numUInts];

                        for (i = 0; i < uintValue.Length; i++)
                        {
                            uintValue[i] = BitConverter.ToUInt32(sglobals.milbus_rx_udp.data_received, (i * numbytesPerUint) + 7);
                            if (uintValue[i] != 0)
                                Console.WriteLine("Tick " + tick + ": MILBUS Data Received \t\t : " +
                                    "Source : " + sglobals.milbus_rx_udp.data_received[0] +
                                    " Destination : " + sglobals.milbus_rx_udp.data_received[3] +
                                    " SubAddr : " + sglobals.milbus_rx_udp.data_received[1] + "Words : " + uintValue[i]);
                        }

                        i = 0;
                        for (uint j = m.StartWord; j <= m.EndWord; j++)
                        {
                            SubAddress_Word[m.DestinationSubaddress, j] = (ushort)uintValue[i];
                            i++;
                        }
                    }
                }
            }
            if (m.messagetype == MessageType.RT_RT)
            {
                foreach (KeyValuePair<RT, SimulatedModel> rt_entry in RTs_Table)
                {
                    if (sglobals.milbus_rx_udp.data_received[3] == Convert.ToUInt32(rt_entry.Key) && (m.DestinationRT == rt_entry.Key))
                    {
                        numUInts = (sglobals.milbus_rx_udp.data_received.Length - 7) / numbytesPerUint;
                        uint[] uintValue = new uint[numUInts];

                        for (i = 0; i < uintValue.Length; i++)
                        {
                            uintValue[i] = BitConverter.ToUInt32(sglobals.milbus_rx_udp.data_received, (i * numbytesPerUint) + 7);
                            if (uintValue[i] != 0)
                                Console.WriteLine("Tick " + tick + ": MILBUS Data Received \t\t : " +
                                    "Source : " + sglobals.milbus_rx_udp.data_received[0] +
                                    " Destination : " + sglobals.milbus_rx_udp.data_received[3] +
                                    " SubAddr : " + sglobals.milbus_rx_udp.data_received[1] + "Words : " + uintValue[i]);
                        }

                        i = 0;
                        for (uint j = m.StartWord; j <= m.EndWord; j++)
                        {
                            RTs_Table[m.DestinationRT].SubAddress_Word[m.DestinationSubaddress, j] = (ushort)uintValue[i];
                            i++;
                        }
                        break;

                    }
                }
            }
        }

        public override SimulatedModel Get()
        {
            return this;
        }

        public void Register(SimulatedModel model)
        {
            throw new NotImplementedException();
        }

        public override void Set()
        {

        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void UnRegister(SimulatedModel model)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public MinorFrame GetMinorFrame(MajorFrame Schedule, int minorFrameIndex)
        {
            return Schedule.majorframe[minorFrameIndex];
        }
    }

    class FPGA : SimulatedModel
    {
        public override SimulatedModel Get()
        {
            throw new NotImplementedException();
        }

        public override void Set()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
