using System.Linq.Expressions;

namespace RWRPOC
{
    class Program
    {
        static void Main(string[] args)
        {

            //DiscreteTimeSimulationEngine DTSE = new();
            //DTSE.Init();

            //while (true)
            //{
            //    DTSE.RunSimulationEngine();
            //    {
            //        Console.WriteLine("\n----------------\n\nPress Enter/Return to display next tick");
            //        Console.ReadLine();
            //    }
            //}

            //List<Pulse> inpPulse;
            //string inputfile = @"C:\Users\RohitChaoji\source\repos\crazysystemics\POI\RWR_POC\RWR_POC\testfile.txt";
            //PulseInputParser pInp = new(inputfile);
            //inpPulse = pInp.ParseText();

            //foreach (Pulse inp in inpPulse )
            //{
            //    Console.WriteLine($"Pulse {inp.symbol}:\n");
            //    Console.WriteLine($"Pulse Width = {inp.pulseWidth}");
            //    Console.WriteLine($"Amplitude = {inp.amplitude}");
            //    Console.WriteLine($"Frequency = {inp.frequency}");
            //    Console.WriteLine($"Time of Arrival = Current Tick + (PRI * {inp.timeOfTraversal})\n");
            //}

            PulseGenerator pg = new PulseGenerator(5, 20, 10, 200, 125);
            List<Pulse> pulses = new List<Pulse>();
            pulses = pg.GeneratePulseTrain();
            int counter = 1;
            Console.WriteLine($"PRI for emitter = {pg.PRI}");
            foreach (Pulse p in pulses)
            {
                Console.WriteLine($"\nPulse {counter}:\n");
                Console.WriteLine($"Width = {p.pulseWidth} ns");
                Console.WriteLine($"Amplitude = {p.amplitude} dB");
                Console.WriteLine($"Frequency = {p.frequency} MHz");
                Console.WriteLine($"Time of Arrival = {p.timeOfTraversal} ns\n----------------");
                counter++;
            }
        }
    }
}


/* Notes:
 * 1. InParameters of PSE, Radar/RWR should be taken from Out parameters list of DTSE.
 * 2. Modeling physical grid and detection by RWR.
 * 3. How to obtain Emitter characteristics from RxBuf if Rxbuf is of Pulse type.
 * */

//Method:
/*
Pulse GetPulse(Pulse txPulse, int txTick, Position txPos, Position currentPos, int currentTick)
above method in PSE
can be invoked by DTSE as of now
it will know the position of RWR (txPulse, txTick) and Radar
in DTSE:
Pulse txPulse = Radar.Get().txPulse
if (txPulse.amps != 0)
{
    previousActivePulse = txPulse;
    txTick = Tick;
}
pse.GetPulse(previousActivePulse, txTick, txPos, rwr.Pos, Tick);

Assume velocity (cells/tick)
After 22 ticks, pulse will be at cell of RWR

if Tick - txTick == time taken to travel for pulse to travel from emitter to receiver, it will detect the pulse
 */

/* October 17:
 * 
 * 1. Maintained a list to keep track of txTick for each radar.
 * 2. txTick being removed from list on receiving echoPulse (DTSE).
 * 3. Used globalSitautionMatrix to detect RWR received pulses and echoPulse.
 * 4. Console output shows correct distances (only showed zero distance at Tick 0 earlier).
 * 5. Handled case for different distance values (different positions for radars and aircraft).
 *
 * Observations:
 * 1. Console output for distances has a one tick delay.

 */