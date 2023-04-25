using OO_OSI;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, Script World!");
Console.WriteLine("Script is OO_OSI... ");

TestHarness osiTestHarness = new TestHarness();
osiTestHarness.test_case_01();



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
