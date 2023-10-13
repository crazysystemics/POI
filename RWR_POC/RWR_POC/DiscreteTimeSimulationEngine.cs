using System.Security.Cryptography.X509Certificates;

class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();
    public PhysicalSimulationEngine pse = new PhysicalSimulationEngine();
    public Pulse echoedPulse = new Pulse(0, 0, 0, 0, "zero");
    public bool echoPulseSet = false;
    public int rxTick = Globals.Tick;
    public RWR.Emitter detectedRadar = new RWR.Emitter();
    OutParameter pulseOut;
    Pulse txPulse = new Pulse(0, 0, 0, 0, "E0");

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        Aircraft a = new Aircraft(new Position(0, 0), 0);

        Radar r = new Radar(new Pulse(7, 10, 5, 45, "E1"), new Position(0, 6), 30, "E1", Globals.Tick, 50, 1);
        Radar r2 = new Radar(new Pulse(5, 15, 5, 45, "E2"), new Position(6, 0), 20, "E2", Globals.Tick, 50, 6);

        // PRI for each radar should be greater than 2x the distance to any aircraft (for pulse speed of 1 cell per tick)
        // Minimum unambiguous range for a radar is c * PRI / 2 where c is the speed of light

        a.rwr = new RWR(ref a.position, 2);
        //a2.rwr = new RWR(ref a2.position, 6);
        // be careful with ref operator

        simMod.Add(a);
        simMod.Add(a.rwr);
        simMod.Add(r);
        simMod.Add(r2);
        simMod.Add(pse);

        // Take single aircraft and multiple radars

    }

    public void RunSimulationEngine()
    {
        List<InParameter> inParameters = new List<InParameter>();

        Console.WriteLine($"----------\nTick = {Globals.Tick}\n----------");

        // Get() on every Simulation Model

        Globals.debugPrint = false;

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

        buildGlobalSituationAwareness();
        
        // Set() on every Simulation Model

        foreach (SimulationModel receiver in simMod)
        {
            pse.Set(inParameters);
            int radius = 0;

            if (receiver is Radar)
            {
                foreach (SimulationModel transmitter in simMod)
                {
                    if (transmitter is RWR)
                    {
                        List<InParameter> inParameters2 = new List<InParameter>();
                        inParameters2.Clear();
                        if (echoPulseSet)
                        {
                            inParameters2.Add(new Radar.In(echoedPulse, 2));
                            ((Radar)receiver).Set(inParameters2);
                        }
                    }
                }
            }

            if (receiver is RWR)
            {
                foreach (SimulationModel transmitter in simMod)
                {
                    if (transmitter is Radar)
                    {
                        List<InParameter> inParameters2 = new List<InParameter>();
                        inParameters2.Clear();
                        inParameters2.Add(new RWR.In(detectedRadar, ((Radar)transmitter).id));
                        ((RWR)receiver).Set(inParameters2);
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

        Globals.Tick++;
    }

    public void buildGlobalSituationAwareness()
    {

        // TODO: Build semantic net or graph connecting all simulation models

        int transmittorCount = 0;
        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is Radar)
            {
                transmittorCount++;
            }
        }

        foreach (SimulationModel transmitter in simMod)
        {
            if (transmitter is Radar)
            {

                pulseOut = transmitter.Get();
                //if (((Radar.Out)pulseOut).p == ((Radar)transmitter).activePulse)
                {
                    txPulse = ((Radar.Out)pulseOut).p;
                }

                foreach (SimulationModel receiver in simMod)
                {

                    // Note: This will work at the Pulse's scale but not at physical scale of movement of aircraft

                    int dist = pse.GetDistance(receiver.id, transmitter.id);
                    int pulseTravelTime = dist / Globals.pulseTravelSpeed;
                    if (receiver is RWR)
                    {
                        int radius = ((Radar)transmitter).radius;
                        if (Globals.distDebugPrint)
                        {
                            Console.WriteLine($"Distance between {receiver} {receiver.id} and Radar {transmitter.id} = {dist}");
                            Globals.distDebugPrint = false;
                        }

                        if (Math.Abs(Globals.Tick - ((Radar)transmitter).txTick) == pulseTravelTime && Globals.Tick != 0)
                        {
                            Globals.debugPrint = true;
                            Console.WriteLine($"\nPulse from {transmitter} {transmitter.id} arrived at cell of {receiver} {receiver.id}\n");
                            ((RWR)receiver).hasReceivedPulse = true;

                            // at current tick at current location, what will be the amplitude of the pulse
                            // no attenuation in amplitude
                            // only pse should the energy at a location and time
                            // only when we introduce sensor model of RWR
                        }

                        if (((RWR)receiver).hasReceivedPulse)
                        {
                            if (Math.Abs(Globals.Tick - ((Radar)transmitter).txTick) == 2 * pulseTravelTime)
                            {
                                Console.WriteLine($"\nEcho received by Radar {transmitter.id}");
                                ((Radar)transmitter).hasReceivedEcho = true;
                                ((Radar)transmitter).echoTimeOfArrival = Globals.Tick;
                                echoedPulse = new Pulse(txPulse.pulseWidth, txPulse.amplitude, ((Radar)transmitter).echoTimeOfArrival, txPulse.angleOfTraversal, txPulse.symbol);
                                echoPulseSet = true;
                            }
                        }

                        // below If condition is unnecessary


                        if (((RWR)receiver).hasReceivedPulse)
                        {
                            detectedRadar = new RWR.Emitter(txPulse.amplitude, 0, ((Radar)transmitter).pulseRepetitionInterval, txPulse.pulseWidth, txPulse.angleOfTraversal, ((Radar)transmitter).id);
                        }
                    }
                }

            }
        }
    }
}
