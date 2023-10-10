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
        Aircraft a = new Aircraft(new Position(0, 0), 0);
        Aircraft a2 = new Aircraft(new Position(20, 25), 5);
        Radar r = new Radar(new Pulse(7, 10, 5, 45, "E1"), new Position(0, 6), 40, Globals.Tick, 50, 1);
        Radar r2 = new Radar(new Pulse(5, 15, 5, 45, "E2"), new Position(6, 0), 20, Globals.Tick, 50, 6);

        // PRI for each radar should be greater than 2x the distance to any aircraft (for pulse speed of 1 cell per tick)
        // Minimum unambiguous range for a radar is c * PRI / 2 where c is the speed of light

        a.rwr = new RWR(ref a.position, 2);
        //a2.rwr = new RWR(ref a2.position, 6);
        // be careful with ref operator

        simMod.Add(a);
        simMod.Add(a.rwr);
        //simMod.Add(a2);
        simMod.Add(r);
        simMod.Add(r2);
        //simMod.Add(a2.rwr);
        simMod.Add(pse);

        // Take single aircraft and multiple radars

    }

    public void RunSimulationEngine()
    {
        List<InParameter> inParameters = new List<InParameter>();

        Console.WriteLine($"Tick = {Globals.Tick}");

        // Get() on every Simulation Model

        Globals.debugPrint = true;

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
                    if (sim_model_2 is RWR)
                    {
                        radius = ((Radar)sim_model).radius;
                        int dist = pse.GetDistance(sim_model.id, sim_model_2.id);
                        //if (Globals.debugPrint)
                        //{
                        //    Console.WriteLine($"Distance between {sim_model_2} {sim_model_2.id} and Radar {sim_model.id} = {dist}");
                        //}

                        List<InParameter> inParameters2 = new List<InParameter>();
                        inParameters2.Clear();
                        if (echoPulseSet)
                        {
                            inParameters2.Add(new Radar.In(echoedPulse, 2));
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
                        int dist = pse.GetDistance(sim_model.id, sim_model_2.id);
                        if (Globals.debugPrint)
                        {
                            Console.WriteLine($"Distance between {sim_model_2} {sim_model_2.id} and {sim_model} {sim_model.id} = {dist}");
                        }

                        if (dist < radius)
                        {
                            List<InParameter> inParameters2 = new List<InParameter>();
                            int[] amps = new int[] { 10, 10, 10, 10 };
                            inParameters2.Clear();
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

        Console.WriteLine();

        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is Radar)
            {

                pulseOut = sim_model.Get();
                txPulse = new Pulse(0, 0, 0, 0, "E0");
                if (((Radar.Out)pulseOut).p == ((Radar)sim_model).activePulse)
                {
                    txPulse = ((Radar.Out)pulseOut).p;
                }

                foreach (SimulationModel receiver in simMod)
                {

                    //if (txPulse.pulseRepetitionInterval != 0)
                    //{
                    //rxPulse = pse.GetPulse(txPulse, r.txTick, r.position, Globals.Tick, receiver.position);
                    //}


                    int dist = pse.GetDistance(receiver.id, sim_model.id);
                    int pulseTravelTime = dist / Globals.pulseTravelSpeed;
                    if (receiver is RWR)
                    {
                        if ((!((RWR)receiver).hasReceivedPulse) && Math.Abs(Globals.Tick - ((Radar)sim_model).txTick) == pulseTravelTime && Globals.Tick != 0)
                        {
                            Console.WriteLine($"Pulse from {sim_model} {sim_model.id} arrived at cell of {receiver} {receiver.id}\n");
                            ((RWR)receiver).hasReceivedPulse = true;

                            // at current tick at current location, what will be the amplitude of the pulse
                            // no attenuation in amplitude
                            // only pse should the energy at a location and time
                            // only when we introduce sensor model of RWR
                        }
                        if (((RWR)receiver).hasReceivedPulse && (Math.Abs(Globals.Tick - ((Radar)sim_model).txTick) % ((Radar)sim_model).pulseRepetitionInterval == 0))
                        {
                            Console.WriteLine($"Pulse from {sim_model} {sim_model.id} arrived at cell of {receiver} {receiver.id}\n");
                            Console.WriteLine($"Pulse reflected by {receiver} {receiver.id}");
                        }
                        if (((RWR)receiver).hasReceivedPulse)
                        {
                            if (Math.Abs(Globals.Tick - ((Radar)sim_model).txTick) == 2 * pulseTravelTime)
                            {
                                Console.WriteLine($"Echo received by Radar {sim_model.id}\n");
                                ((Radar)sim_model).echoTimeOfArrival = Globals.Tick;
                                echoedPulse = new Pulse(txPulse.pulseWidth, txPulse.amplitude, txPulse.timeOfTraversal, txPulse.angleOfTraversal, txPulse.symbol);
                                echoPulseSet = true;

                                ((RWR)receiver).hasReceivedPulse = false;
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
