using System.Net.NetworkInformation;

class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    //PhysicalSimulationEngine physEngine = new PhysicalSimulationEngine();
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();
    public PhysicalSimulationEngine pse = new PhysicalSimulationEngine();
    public Pulse echoedPulse = new Pulse(0, 0, 0, 0, "zero");
    public bool echoPulseSet = false;
    public int rxTick = Globals.Tick;
    TransmittedPulse rxPulse = new TransmittedPulse(0, 0, new Position(0, 0), 0, 0, 0, 0, "zero");

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        Aircraft a = new Aircraft(new Position(10, 25), 0);
        Radar r = new Radar(new Pulse(5, 5, 5, 5, "E1"), new Position(20, 10), Globals.Tick, 50, 1);


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
                int pulseTxTick = ((Radar.Out)pulseOut).txTick;
                int firstTxTick = ((Radar.Out)pulseOut).firstTxTick;

                if (txPulse.pulseRepetitionInterval != 0)
                {
                    rxPulse = pse.GetPulse(pulseTxTick, rxTick, sim_model.position, txPulse);
                }

                Console.WriteLine($"txTick of rxPulse: {rxPulse.txTick}");
                Console.WriteLine($"rxPulse.rxTick: {rxPulse.rxTick}");


                if (((Radar)sim_model).activePulse != ((Radar)sim_model).zeroPulse)
                {
                    foreach (SimulationModel receiver in simMod)
                    {
                        int pulseTravelSpeed = 1; // must be speed of light "c" in actual computation
                        int dist = pse.Distance(receiver.id, sim_model.id);
                        int pulseTravelTime = dist / pulseTravelSpeed;
                        if (receiver is RWR)
                        {
                            if ((!((RWR)receiver).receivedPulse) && Math.Abs(firstTxTick - Globals.Tick) == pulseTravelTime && Globals.Tick != 0)
                            {
                                Console.WriteLine($"Pulse arrived at cell of {receiver}");
                                rxTick = Globals.Tick;
                                rxPulse.rxTick = Globals.Tick;
                                ((RWR)receiver).receivedPulse = true;
                            }
                            if (((RWR)receiver).receivedPulse && (Math.Abs(rxPulse.rxTick - Globals.Tick) % rxPulse.pulseRepetitionInterval == 0))
                            {
                                Console.WriteLine($"Pulse arrived at cell of {receiver}");
                                Console.WriteLine($"Pulse reflected by {receiver}");
                            }
                            if (((RWR)receiver).receivedPulse)
                            {
                                if (!((Radar)sim_model).receivedEcho && Math.Abs(firstTxTick - Globals.Tick) == 2 * pulseTravelTime && receiver is RWR)
                                {
                                    Console.WriteLine("Echo received by Radar");
                                    ((Radar)sim_model).receivedEcho = true;
                                    ((Radar)sim_model).echoReceivedTime = Globals.Tick;
                                    // Extract Pulse object from rxPulse to be used as Radar's InParameter during Set()
                                    echoedPulse = new Pulse(rxPulse.pulseWidth, rxPulse.pulseRepetitionInterval, rxPulse.timeOfArrival, rxPulse.angleOfArrival, rxPulse.symbol);
                                    echoPulseSet = true;
                                }
                                else if (((Radar)sim_model).receivedEcho && rxPulse.pulseRepetitionInterval != 0 && (Math.Abs(((Radar)sim_model).echoReceivedTime - Globals.Tick) % rxPulse.pulseRepetitionInterval == 0))
                                {
                                    Console.WriteLine("Repeat echo received by radar");
                                }
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
