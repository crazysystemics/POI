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
//Further Test Cases
//
//1. Extending to OSI-7 layers.
//2. Full Duplex bottom-up
//3. [STRETCH GOAL] Simulating network behaviour with parallel stacks




namespace oolayer_Script
{
    public abstract class TestCase
    {
        public string Name;
        public abstract void Setup(bool assertFlag = true, bool verbose = false);
        public abstract void Run(int testNum);
        public abstract void Report();
    }

    //string title = "Two layers Top-down flow of data";
    public class TestCase00 : TestCase
    {
        int testCaseId = 0;
        OOLayer applicationLayer;
        OOLayer sessionLayer;
        int bugs;
        bool assertFlag;
        bool verbose;

        public TestCase00()
        {
            Name = "Two-Layer-Top-Down";
        }


        public override void Setup(bool assertFlag = true, bool verbose = false)
        {
            this.assertFlag = assertFlag;
            this.verbose = verbose;

            applicationLayer = new OOLayer("application", StackPosition.TOP);
            sessionLayer = new OOLayer("session", StackPosition.BOTTOM);
            //Physical Queue is in source. Its reference is assigned to destination
            applicationLayer.toLowerQ = sessionLayer.GetQ(QueueType.FROM_UPPER);
            sessionLayer.toUpperQ = applicationLayer.GetQ(QueueType.FROM_LOWER);
            bugs = 0;
        }
        public override void Run(int testNum)
        {
            Console.WriteLine();
            Console.WriteLine("Test No:{0} Test Case:{1} Title:{2}",
                               testNum, testCaseId, Name);

            string inputs = "hello";

            applicationLayer.setInput();
            //tick 0 - packet is transferred from application to session layer
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            string outputs = sessionLayer.getOutput();
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
            outputs = sessionLayer.getOutput();
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
            Console.WriteLine("Report");
            Console.WriteLine("Test Case:{0}", testCaseId);
            Console.WriteLine("{0} bugs found!!", bugs);
        }
    }

    //string title = "Two layers Bottom-Up flow of data";
    public class TestCase01 : TestCase
    {
        int testCaseId = 1;
        OOLayer applicationLayer;
        OOLayer sessionLayer;
        int bugs;
        bool assertFlag;
        bool verbose;

        public TestCase01()
        {
            Name = "Two-Layer-Bottom-Up";
        }

        public override void Setup(bool assertFlag = true, bool verbose = false)
        {
            this.assertFlag = assertFlag;
            this.verbose = verbose;

            applicationLayer = new OOLayer("application", StackPosition.TOP);
            sessionLayer = new OOLayer("session", StackPosition.TOP);
            //These connections are same as in TC01
            //Physical Queue is in source. Its reference is assigned to destination
            applicationLayer.toLowerQ = sessionLayer.GetQ(QueueType.FROM_UPPER);
            sessionLayer.toUpperQ = applicationLayer.GetQ(QueueType.FROM_LOWER);
            bugs = 0;
        }
        public override void Run(int testNum)
        {
            Console.WriteLine();
            Console.WriteLine("Test No:{0} Test Case:{1} Test Case:{2}",
                               testNum, testCaseId, Name);
            string inputs = "hello";
            sessionLayer.setInput();
            //tick 0 - packet is transferred from application to session layer
            //Read Phase
            applicationLayer.OnTick(RWPhase.READ);
            sessionLayer.OnTick(RWPhase.READ);
            //Write Phase
            applicationLayer.OnTick(RWPhase.WRITE);
            sessionLayer.OnTick(RWPhase.WRITE);
            string outputs = applicationLayer.getOutput();
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
            outputs = applicationLayer.getOutput();

            if (outputs != "bottom hello")
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

    public class TestCase02 : TestCase
    {
        int testCaseId = 2;
        OOLayer applicationLayer;
        OOLayer sessionLayer;
        OOLayer presentationLayer;

        int bugs;
        bool assertFlag;
        bool verbose;

        public TestCase02()
        {
            Name = "Three-Layer-Two-Ways-Half-Duplex";
        }

        public override void Setup(bool assertFlag = true, bool verbose = false)
        {
            this.assertFlag = assertFlag;
            this.verbose = verbose;

            applicationLayer = new OOLayer("application", StackPosition.TOP);
            sessionLayer = new OOLayer("session", StackPosition.MIDDLE);
            presentationLayer = new OOLayer("presentation", StackPosition.BOTTOM);
            //These connections are same as in TC01
            //Physical Queue is in source. Its reference is assigned to destination
            applicationLayer.toLowerQ = sessionLayer.GetQ(QueueType.FROM_UPPER);
            sessionLayer.toLowerQ = presentationLayer.GetQ(QueueType.FROM_UPPER);

            presentationLayer.toUpperQ = sessionLayer.GetQ(QueueType.FROM_LOWER);
            sessionLayer.toUpperQ = applicationLayer.GetQ(QueueType.FROM_LOWER);
            bugs = 0;
        }
        public override void Run(int testNum)
        {
            Console.WriteLine();
            Console.WriteLine("Test No:{0} Test Case:{1} Test Case:{2}",
                               testNum, testCaseId, Name);
            string[] expectedOutputs = new string[] 
                          {  "presentation_session_application_top_namaste",
                             "bottom hello"
                          };

            OOLayer topLayer    = applicationLayer;
            OOLayer midLayer    = sessionLayer;
            OOLayer bottomLayer = presentationLayer;

            sglobal.RegisterInvocationSource("test01");
            

            
            //TODO: Make these variables enumerated
            bool TOP_TO_BOTTOM = true;
            bool BOTTOM_TO_UP = false;
            bool direction = TOP_TO_BOTTOM;
            bool notDone = true;
            int  numLayers = 3; 
            int  loopCount = 0;

            while (notDone)
            {
                if (loopCount == 0)
                {
                    if (direction == TOP_TO_BOTTOM)
                        topLayer.setInput();
                    else
                        bottomLayer.setInput();
                }

                //Every Iteration is a tick and packet moves either down or up one layr
                //based on direction. READ and WRITE phases are for managing concurrency

                //Read Phase
                applicationLayer.OnTick(RWPhase.READ);
                sessionLayer.OnTick(RWPhase.READ);
                presentationLayer.OnTick(RWPhase.READ);
                //Write Phase
                applicationLayer.OnTick(RWPhase.WRITE);
                sessionLayer.OnTick(RWPhase.WRITE);
                presentationLayer.OnTick(RWPhase.WRITE);
                

                if (loopCount == numLayers - 1)
                {
                    //data has reached  either  top or bottom layer
                    //depending on direction of traversal
                    //time to process outputs

                    string outputs = String.Empty;
                    int outputIndex;

                    if (direction == TOP_TO_BOTTOM)
                    {
                        outputs = bottomLayer.getOutput();
                        outputIndex = 0;
                    }
                    else
                    {
                        outputs = topLayer.getOutput();
                        outputIndex = 1;
                    }
                    //TODO: Incrementing bug count using Debug.Assert
                    if (outputs != expectedOutputs[outputIndex])
                    {
                        Debug.Assert(assertFlag);
                        bugs++;
                    }

                    if (verbose)
                    {
                        Console.WriteLine();
                        Console.WriteLine(outputs);
                    }

                    if (direction == TOP_TO_BOTTOM)
                    {
                        direction = BOTTOM_TO_UP;
                    }
                    else if (direction == BOTTOM_TO_UP)
                    {
                        notDone = false;
                    }

                    loopCount = 0;
                }
                else 
                    loopCount++;
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
