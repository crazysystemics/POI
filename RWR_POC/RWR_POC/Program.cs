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
                if (Globals.debugPrint)
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
 * 3. How to get radius of the radar irrespective of order of BattleSystems in the sim_model list.
 * 4. How to handle detection of echo pulse. Should 2D world be built first or through PSE.
 * 5. Condition for setting visible radars for RWR should not check radius, but detection of pulse (how to write the code)?
 
 */

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