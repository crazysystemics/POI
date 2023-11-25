using System;
using System.Linq.Expressions;

namespace RWRPOC
{
    class Program
    {
        static void Main(string[] args)
        {

            DiscreteTimeSimulationEngine DTSE = new DiscreteTimeSimulationEngine();
            DTSE.Init();

            while (true)
            {
                DTSE.RunSimulationEngine();
                {
                    Console.WriteLine("\n----------------\n\nPress Enter/Return to display next tick");
                    Console.ReadLine();
                }
            }

            //MissionInputParser missionInputParser = new MissionInputParser();
            //missionInputParser.ParseJSON();

            //List<Pulse> inpPulse;
            //string inputfile = @"C:\Users\RohitChaoji\source\repos\crazysystemics\POI\RWR_POC\RWR_POC\testfile.txt";
            //PulseInputParser pInp = new(inputfile);
            //inpPulse = pInp.ParseText();

            //foreach (Pulse inp in inpPulse )
            //{
            //    Console.WriteLine($"Pulse {inp.symbol}:\n");
            //    Console.WriteLine($"Pulse Width = {inp.pulseWidth}");
            //    Console.WriteLine($"Amplitude = {inp.amplitude}");
            //    Console.WriteLine($"Frequency = {inp.frequency}\n");
            //}
        }
    }
}