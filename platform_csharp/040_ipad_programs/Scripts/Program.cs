//using OO_OSI;
using oolayer_Script;



// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, Script World!");
Console.WriteLine("Script is OO_OSI... ");


List<TestCase> testCases = new List<TestCase>();
testCases.Add(new TestCase01());
testCases.Add(new TestCase02());

int runno = 0;
foreach (TestCase testCase in testCases)
{    
    testCase.Setup(TestOptions.NEVER_ASSERT, TestOptions.VERBOSE);
    testCase.Run(runno);
    testCase.Report();
    runno++;
}














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
