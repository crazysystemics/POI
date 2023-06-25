//using OO_OSI;
using oolayer_Script;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, Script World!");
Console.WriteLine("Script is OO_OSI... ");

//TestHarness osiTestHarness = new TestHarness("th01");
//osiTestHarness.test_case_01();

//Receiver rcvr = new Receiver();
//Sender sender = new Sender();
//sender.Connect(rcvr.queue);
//sender.putQ("hello");
//string s = rcvr.getsQ();
//Console.WriteLine(s);

OOLayer applicationLayer = new OOLayer("application");
OOLayer sessionLayer        = new OOLayer ("session");
applicationLayer.toLowerQ   = applicationLayer.GetQ(QueueType.FROM_UPPER);
sessionLayer.toUpperQ       = applicationLayer.GetQ(QueueType.FROM_LOWER);

string inputs = "hello";
applicationLayer.setInput(inputs,StackPosition.TOP);

applicationLayer.OnTick(RWPhase.READ);
sessionLayer.OnTick(RWPhase.READ);
applicationLayer.OnTick(RWPhase.WRITE);
sessionLayer.OnTick(RWPhase.WRITE);

string outputs = sessionLayer.getOutput();
Console.WriteLine(outputs);



//isSymmetric p = new isSymmetric();

//these below work
//p.Value = true;
//if (p.Value) { Console.WriteLine("Worked"); }
//if (p.Equals(true)) {Console.WriteLine("Worked");}
//these are not working
//p =true;
//bool q = p;
//if (p) { Console.WriteLine("Not working"); }

abstract class Predicate
{  public   bool Value { get; set; }   }

class isSymmetric : Predicate { }
