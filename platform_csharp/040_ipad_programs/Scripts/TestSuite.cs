using oolayer_Script;
using System;
using System.Diagnostics;

//TODO: Implement 
//using System.Runtime.CompilerServices;
//static void SomeMethodSomewhere()
//{
//    ShowMessage("Boo");
//}
//...
//static void ShowMessage(string message,
//    [CallerLineNumber] int lineNumber = 0,
//    [CallerMemberName] string caller = null)
//{
//    MessageBox.Show(message + " at line " + lineNumber + " (" + caller + ")");
//}
//https://stackoverflow.com/questions/12556767/how-do-i-get-the-current-line-number



namespace oolayer_Script
{
	public abstract class TestCase
	{
		public abstract void Setup(bool assertFlag=true, bool verbose=false);
		public abstract void Run(int testNum);
		public abstract void Report();
	}

    //string title = "Two layers Top-down flow of data";
    public class TestCase01: TestCase
	{
        int    testCaseId = 1;
        string title = "Two layers Top-down flow of data";
        OOLayer applicationLayer;
        OOLayer sessionLayer;
        int     bugs;
        bool assertFlag;
        bool verbose;

        
		public override  void Setup(bool assertFlag=true, bool verbose=false)
		{
            this.assertFlag = assertFlag;
            this.verbose = verbose;

            applicationLayer = new OOLayer("application");
            sessionLayer = new OOLayer("session");
            applicationLayer.toLowerQ = sessionLayer.GetQ(QueueType.FROM_UPPER);
            sessionLayer.toUpperQ = applicationLayer.GetQ(QueueType.FROM_LOWER); 
            bugs = 0;
        }
		public override  void Run(int testNum)
		{
            Console.WriteLine();
            Console.WriteLine("Test No:{0} Test Case:{1} Title:{2}",
                               testNum, testCaseId, title);

            string inputs = "hello";
           
            applicationLayer.setInput(inputs, StackPosition.TOP);
            //tick 0 - packet is transferred from application to session layer
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            string outputs = sessionLayer.getOutput(StackPosition.BOTTOM);
            if (outputs != "Queue Empty")
            {
                Debug.Assert(assertFlag);
                bugs++;
            }
            if (verbose)
            {
                Console.WriteLine(outputs);
            }

            //tick 1 - packet is transferred from session to output
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            outputs = sessionLayer.getOutput(StackPosition.BOTTOM);
            if (outputs != "session_application_top hello")
            {
                Debug.Assert(assertFlag);
                bugs++;
            }

            if (verbose)
            {
                Console.WriteLine(outputs);
            }
        }
		public override void Report()
		{
            Console.WriteLine();
            Console.WriteLine("Report" );
            Console.WriteLine("Test Case:{0}", testCaseId);
            Console.WriteLine("{0} bugs found!!", bugs);
        }
	}

    //string title = "Two layers Bottom-Up flow of data";
    public class TestCase02 : TestCase
    {
        int     testCaseId = 2;
        string  title = "Two layers Bottom-Up flow of data";
        OOLayer applicationLayer;
        OOLayer sessionLayer;
        int     bugs;
        bool    assertFlag;
        bool    verbose;

        public override void Setup(bool assertFlag=true, bool verbose=false)
        {
            this.assertFlag = assertFlag;
            this.verbose = verbose;

            applicationLayer = new OOLayer("application");
            sessionLayer = new OOLayer("session");
            applicationLayer.toLowerQ = sessionLayer.GetQ(QueueType.FROM_UPPER);
            sessionLayer.toUpperQ = applicationLayer.GetQ(QueueType.FROM_LOWER);
            bugs = 0;

        }
        public override void Run(int testNum)
        {
            Console.WriteLine();
            Console.WriteLine("Test No:{0} Test Case:{1} Test Case:{2}", 
                               testNum, testCaseId, title);
            
            string inputs = "hello";
            sessionLayer.setInput(inputs, StackPosition.BOTTOM);
            //tick 0 - packet is transferred from application to session layer
            //Read Phase
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            //Write Phase
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            string outputs = applicationLayer.getOutput(StackPosition.TOP);
            //TODO: Incrementing bug count using Debug.Assert
            if (outputs != "Queue Empty")
            {
                Debug.Assert(assertFlag);                
                bugs++; 
            }
            if (verbose)
            {
                Console.WriteLine(outputs);
            }                     

            //tick 1 - packet is transferred from session to output
            ////Read Phase
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            //Write Phase
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            outputs = applicationLayer.getOutput(StackPosition.BOTTOM);
             
            if (outputs != "hello")
            {
                Debug.Assert(assertFlag);
                bugs++;
            }

            if (verbose)
            {
                Console.WriteLine(outputs);
            }

        }
        public override void Report()
        {
            Console.WriteLine();
            Console.WriteLine("Report");
            Console.WriteLine("Test Case:{0}", testCaseId);
            Console.WriteLine("{0} bugs found!!", bugs);
        }
    }
}
