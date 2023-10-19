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