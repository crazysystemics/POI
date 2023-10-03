using System.Net.NetworkInformation;

class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    //PhysicalSimulationEngine physEngine = new PhysicalSimulationEngine();
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();
    public PhysicalSimulationEngine pse = new PhysicalSimulationEngine();
    public int pulseTxTick = 0;
    public bool firstPulseReceived = false;
    public bool echoReceived = false;
    public int firstPulseTick = 0;
    public Pulse echoedPulse = new Pulse(0, 0, 0, 0, "zero");
    public bool echoPulseSet = false;
    public bool activePulseRecorded = false;
    public int[] activePulseChars = new int[4];
    public int echoReceivedTime = 0;
    public string activePulseSymbol = "";

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        Aircraft a = new Aircraft(new Position(40, 35), 0);
        Radar r = new Radar(new Pulse(5, 5, 5, 5, "E1"), new Position(20, 10), 50, 1);


        a.rwr = new RWR(ref a.position, 2);

        simMod.Add(a);
        simMod.Add(r);
        simMod.Add(a.rwr);
        simMod.Add(pse);
    }

    public void RunSimulationEngine()
    {
        List<InParameter> inParameters = new List<InParameter>();

        // Get() on every Simulation Model

        foreach (SimulationModel sim_model in simMod)
        {
            // physEngine.physInParameters.Add(sim_model.Get());
            dtseOutParameter.Add(sim_model.Get());
        }
        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is PhysicalSimulationEngine)
            {
                pse = (PhysicalSimulationEngine)sim_model;
                break;
            }
            else
            {
                inParameters.Add(new PhysicalSimulationEngine.In(sim_model.position, sim_model.id));
                // *1 of notes
            }
        }

        
        // Set() on every Simulation Model

        foreach (SimulationModel sim_model in simMod)
        {
            pse.Set(inParameters);
            int radius = 0;

            if (sim_model is Radar)
            {
                radius = ((Radar)sim_model).radius;
                int dist = pse.Distance(0, 1);
                Console.WriteLine($"Distance between Aircraft and Radar = {dist}\n");
                if (echoPulseSet)
                {
                    List<InParameter> inParameters2 = new List<InParameter>
                    {
                        new Radar.In(echoedPulse, 2)
                    };
                    ((Radar)sim_model).Set(inParameters2);
                }
            }

            if (sim_model is RWR)
            {
                int dist = pse.Distance(2, 1);
                Console.WriteLine($"Distance between Radar and RWR = {dist}\n");
                if (dist < radius)
                {
                    List<InParameter> inParameters2 = new List<InParameter>();
                    int[] amps = new int[] { 10, 10, 10, 10 };
                    inParameters2.Add(new RWR.In(new RWR.Emitter(amps, 10, 10, 10, 10), 1));
                    ((RWR)sim_model).Set(inParameters2);
                }
            }
        }

        // OnTick() on each Simulation Model

        foreach (SimulationModel sim_model in simMod)
        {
            sim_model.OnTick();
        }

        OutParameter pulseOut;
        Pulse txPulse;

        // GetPulse() on PSE

        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is Radar)
            {

                pulseOut = sim_model.Get();
                txPulse = ((Radar.Out)pulseOut).p;
                if (!activePulseRecorded)
                {
                    activePulseChars = new int[] { txPulse.pulseWidth, txPulse.pulseRepetitionInterval, txPulse.timeOfArrival, txPulse.angleOfArrival};
                    activePulseSymbol = txPulse.symbol;
                    activePulseRecorded = true;
                }
                TravellingPulse rxPulse = pse.GetPulse(pulseTxTick, Globals.Tick, sim_model.position, txPulse);

                if (rxPulse.pulseRepetitionInterval == 0)
                {
                    rxPulse.pulseWidth = activePulseChars[0];
                    rxPulse.pulseRepetitionInterval = activePulseChars[1];
                    rxPulse.timeOfArrival = activePulseChars[2];
                    rxPulse.angleOfArrival = activePulseChars[3];
                    rxPulse.symbol = activePulseSymbol;
                }

                Console.WriteLine($"txTick of rxPulse: {rxPulse.txTick}");


                if (((Radar)sim_model).activePulse != ((Radar)sim_model).zeroPulse)
                {
                    foreach (SimulationModel receiver in simMod)
                    {
                        int pulseTravelSpeed = 1; // must be speed of light "c" in actual computation
                        int dist = pse.Distance(receiver.id, sim_model.id);
                        int pulseTravelTime = dist / pulseTravelSpeed;
                        if (!firstPulseReceived && Math.Abs(rxPulse.txTick - Globals.Tick) == pulseTravelTime && Globals.Tick != 0)
                        {
                            if (receiver is RWR)
                            {
                                Console.WriteLine($"Pulse arrived at cell of {receiver}");
                                firstPulseReceived = true;
                            }
                            pulseTxTick = rxPulse.currentTick;
                        }
                        if (rxPulse.pulseRepetitionInterval != 0 && firstPulseReceived && (Math.Abs(rxPulse.txTick - Globals.Tick) % rxPulse.pulseRepetitionInterval == 0))
                        {
                            if (receiver is RWR)
                            {
                                Console.WriteLine($"Pulse arrived at cell of {receiver}");
                                Console.WriteLine($"Pulse reflected by {receiver}");
                            }
                        }
                        if (firstPulseReceived)
                        {
                            if (!echoReceived && Math.Abs(firstPulseTick - Globals.Tick) == 2 * pulseTravelTime && receiver is RWR)
                            {
                                Console.WriteLine("Echo received by Radar");
                                echoReceived = true;
                                echoReceivedTime = Globals.Tick;
                                // Extract Pulse object from rxPulse to be used as Radar's InParameter during Set()
                                echoedPulse = new Pulse(rxPulse.pulseWidth, rxPulse.pulseRepetitionInterval, rxPulse.timeOfArrival, rxPulse.angleOfArrival, rxPulse.symbol);
                                echoPulseSet = true;
                            }
                            else if (echoReceived && rxPulse.pulseRepetitionInterval != 0 && (Math.Abs(echoReceivedTime - Globals.Tick) % rxPulse.pulseRepetitionInterval == 0) && receiver is RWR)
                            {
                                Console.WriteLine("Repeat echo received by radar");
                            }
                        }
                    }
                }
            }
        }
        Globals.Tick++;
    }

    public void ResetTime()
    {
        Globals.Tick = 0;
    }
}
