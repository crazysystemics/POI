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
    Pulse rxPulse = new Pulse(0, 0, 0, 0, "zero");

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        Aircraft a = new Aircraft(new Position(10, 25), 0);
        Aircraft a2 = new Aircraft(new Position(20, 25), 5);
        Radar r = new Radar(new Pulse(5, 20, 5, 5, "E1"), new Position(20, 10), Globals.Tick, 50, 1);


        a.rwr = new RWR(ref a.position, 2);
        a2.rwr = new RWR(ref a2.position, 6);

        simMod.Add(a);
        simMod.Add(a2);
        simMod.Add(r);
        simMod.Add(a.rwr);
        simMod.Add(a2.rwr);
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
                foreach (SimulationModel sim_model_2 in simMod)
                {
                    if (sim_model_2 is Aircraft)
                    {
                        radius = ((Radar)sim_model).radius;
                        int dist = pse.Distance(sim_model.id, sim_model_2.id);
                        Console.WriteLine($"Distance between Aircraft {sim_model_2.id} and Radar {sim_model.id} = {dist}");
                        if (echoPulseSet)
                        {
                            List<InParameter> inParameters2 = new List<InParameter>
                            {
                                new Radar.In(echoedPulse, 2)
                            };
                            ((Radar)sim_model).Set(inParameters2);
                        }
                    }
                }
            }

            if (sim_model is RWR)
            {
                foreach (SimulationModel sim_model_2 in simMod)
                {
                    if (sim_model_2 is Radar)
                    {
                        int dist = pse.Distance(sim_model.id, sim_model_2.id);
                        Console.WriteLine($"Distance between Radar {sim_model_2.id} and RWR {sim_model.id} = {dist}");
                        if (dist < radius)
                        {
                            List<InParameter> inParameters2 = new List<InParameter>();
                            int[] amps = new int[] { 10, 10, 10, 10 };
                            inParameters2.Add(new RWR.In(new RWR.Emitter(amps, 10, 10, 10, 10), 1));
                            ((RWR)sim_model).Set(inParameters2);
                        }
                    }
                }

            }
        }

        // OnTick() on each Simulation Model
        Console.WriteLine();

        foreach (SimulationModel sim_model in simMod)
        {
            sim_model.OnTick();
        }

        OutParameter pulseOut;
        Pulse txPulse;

        // GetPulse() on PSE

        Console.WriteLine();

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
                    rxPulse = pse.GetPulse(txPulse);
                }


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
                                Console.WriteLine($"Pulse arrived at cell of {receiver} {receiver.id}");
                                rxTick = Globals.Tick;
                                ((RWR)receiver).rxTick = Globals.Tick;
                                ((RWR)receiver).receivedPulse = true;
                            }
                            if (((RWR)receiver).receivedPulse && (Math.Abs(((RWR)receiver).rxTick - Globals.Tick) % rxPulse.pulseRepetitionInterval == 0))
                            {
                                Console.WriteLine($"Pulse arrived at cell of {receiver} {receiver.id}");
                                Console.WriteLine($"Pulse reflected by {receiver} {receiver.id}");
                            }
                            if (((RWR)receiver).receivedPulse)
                            {
                                if (!((Radar)sim_model).receivedEcho && Math.Abs(firstTxTick - Globals.Tick) == 2 * pulseTravelTime)
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
                                    Console.WriteLine("Repeat echo received by Radar");
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
