using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Bench
{
    public interface IMilbusBC : IBC
    {
        MinorFrame GetMinorFrame(MajorFrame Schedule, int minorFrameIndex);
        void ExecuteRxMessage(int tick, Message messages, Dictionary<RT, SimulatedModel> RTs_Table);
        void ExecuteTxMessage(int tick, Message messages, Dictionary<RT, SimulatedModel> RTs_Table);
    }

    public class MilbusSchedule : Schedule
    {
        public MajorFrame majorFrame = new MajorFrame();
    }

    public class MIL1553B : BUS
    {
        public IMilbusBC bc = new MC();
        public Dictionary<RT, SimulatedModel> RTModelDictionary = new Dictionary<RT, SimulatedModel>();
        public MilbusSchedule schedule = new MilbusSchedule();
        public int minorFrameIndex = 0;
    }

    public enum MessageType
    {
        RT_BC, BC_RT, RT_RT
    }

    public enum RT
    {
        BC_MC = 1, P, Q, MFWS, COCKPIT,
    }


    public class Message
    {
        public uint MessageID;
        public MessageType messagetype;
        public RT SourceRT;
        public uint SourceSubaddress;
        public int numberOfWords;
        public RT DestinationRT;
        public uint DestinationSubaddress;
        public uint StartWord;
        public uint EndWord;

        //delegate pre_handler
        public delegate void preHandlerDelegate();
        public preHandlerDelegate preHandler;

        //...
        //delegate post_handler
        public delegate void postHandlerDelegate();
        public postHandlerDelegate postHandler;

        public Message(uint pID, MessageType pmessagetype, RT pSRT, RT pDRT, uint pSSA, int pnumberOfWords, uint pDSA, uint pSW, uint pEW)
        {
            MessageID = pID;
            messagetype = pmessagetype;
            SourceRT = pSRT;
            SourceSubaddress = pSSA;
            DestinationRT = pDRT;
            DestinationSubaddress = pDSA;
            StartWord = pSW;
            EndWord = pEW;
        }
    }

    public class MinorFrame
    {
        public Message[] minorframe = new Message[4];
    }

    public class MajorFrame
    {
        public MinorFrame[] majorframe = new MinorFrame[10];
    }
}
