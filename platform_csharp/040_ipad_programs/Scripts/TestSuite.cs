using oolayer_Script;
using System;
using System.Diagnostics;

namespace oolayer_Script
{
	public abstract class TestCase
	{
		public abstract void Setup();
		public abstract void Run();
		public abstract void Report();
	}

	
    public class TestCase01: TestCase
	{
        string title = "Two lavers Top-down flow of data";
        OOLayer applicationLayer;
        OOLayer sessionLayer;
        int     NumBugs = 0;
        
		public override  void Setup()
		{
            applicationLayer = new OOLayer("application");
            sessionLayer = new OOLayer("session");
            applicationLayer.toLowerQ = sessionLayer.GetQ(QueueType.FROM_UPPER);
            sessionLayer.toUpperQ = applicationLayer.GetQ(QueueType.FROM_LOWER);            
        }
		public override  void Run()
		{
            string inputs = "hello";
            applicationLayer.setInput(inputs, StackPosition.TOP);
            //tick 0 - packet is transferred from application to session layer
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            string outputs = sessionLayer.getOutput();
            Debug.Assert(outputs == "Queue Empty");
            Console.WriteLine(outputs);

            //tick 1 - packet is transferred from session to output
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            outputs = sessionLayer.getOutput();
            Debug.Assert(outputs == "session_application_top hello");
            Console.WriteLine(outputs);
        }
		public override void Report()
		{
            
        }
	}

    public class TestCase02 : TestCase
    {
        public override void Setup()
        { }
        public override void Run()
        { }
        public override void Report()
        { }
    }
}
