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
        Radar r = new Radar(new Pulse(5, 40, 5, 5, "E1"), new Position(0, 12), Globals.Tick, 50, 1);
        Radar r2 = new Radar(new Pulse(5, 25, 5, 5, "E1"), new Position(0, 5), Globals.Tick, 50, 6);


        a.rwr = new RWR(ref a.position, 2);
        //a2.rwr = new RWR(ref a2.position, 6);
        // be careful with ref operator

        simMod.Add(a);
        //simMod.Add(a2);
        simMod.Add(r);
        simMod.Add(r2);
        simMod.Add(a.rwr);
        //simMod.Add(a2.rwr);
        simMod.Add(pse);

        // Take single aircraft and multiple radars

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
                    if (sim_model_2 is RWR)
                    {
                        radius = ((Radar)sim_model).radius;
                        int dist = pse.Distance(sim_model.id, sim_model_2.id);
                        Console.WriteLine($"Distance between {sim_model_2} {sim_model_2.id} and Radar {sim_model.id} = {dist}");
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
                        int dist = pse.Distance(sim_model.id, sim_model_2.id);
                        Console.WriteLine($"Distance between {sim_model_2} {sim_model_2.id} and {sim_model} {sim_model.id} = {dist}");
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

        // GetPulse() on PSE

        Console.WriteLine();

        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is Radar)
            {

                pulseOut = sim_model.Get();
                txPulse = ((Radar.Out)pulseOut).p;

                if (txPulse.pulseRepetitionInterval != 0)
                {
                    rxPulse = pse.GetPulse(txPulse);
                }

                foreach (SimulationModel receiver in simMod)
                {
                    int pulseTravelSpeed = 1; // must be speed of light "c" in actual computation
                    int dist = pse.Distance(receiver.id, sim_model.id);
                    int pulseTravelTime = dist / pulseTravelSpeed;
                    if (receiver is RWR)
                    {
                        if ((!((Radar)sim_model).hasPulseReachedTarget) && Math.Abs(Globals.Tick - ((Radar)sim_model).txTick) == pulseTravelTime && Globals.Tick != 0)
                        {
                            Console.WriteLine($"Pulse from {sim_model} {sim_model.id} arrived at cell of {receiver} {receiver.id}");
                            ((Radar)sim_model).hasPulseReachedTarget = true;

                            // all booleans should be in the form of predicates
                            // do not change variables to make your model/code work
                            // make sure changes in foreach are reflected in list
                        }
                        if (((Radar)sim_model).hasPulseReachedTarget && (Math.Abs(Globals.Tick - ((Radar)sim_model).txTick) % rxPulse.pulseRepetitionInterval == 0))
                        {
                            Console.WriteLine($"Pulse from {sim_model} {sim_model.id} arrived at cell of {receiver} {receiver.id}");
                            Console.WriteLine($"Pulse reflected by {receiver} {receiver.id}");
                        }
                        if (((Radar)sim_model).hasPulseReachedTarget)
                        {
                            if (Math.Abs(Globals.Tick - ((Radar)sim_model).txTick) == 2 * pulseTravelTime)
                            {
                                Console.WriteLine($"Echo received by Radar {sim_model.id}");
                                ((Radar)sim_model).echoReceivedTime = Globals.Tick;
                                echoedPulse = new Pulse(rxPulse.pulseWidth, rxPulse.pulseRepetitionInterval, rxPulse.timeOfArrival, rxPulse.angleOfArrival, rxPulse.symbol);
                                echoPulseSet = true;
                                ((Radar)sim_model).hasPulseReachedTarget = false;
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
