using System;

namespace Test_Bench
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBench testbench = new TestBench();

            testbench.init();
            testbench.Run();
        }
    }
}
