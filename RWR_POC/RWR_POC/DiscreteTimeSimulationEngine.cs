class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();
    public List<InParameter> receiverInParams = new List<InParameter>();
    public PhysicalSimulationEngine pse = new PhysicalSimulationEngine(99);
    public Pulse echoedPulse = new Pulse(0, 0, 0, 0, "zero");
    public bool echoPulseSet = false;
    public int rxTick = Globals.Tick;
    public RWR.Emitter detectedRadar = new RWR.Emitter();
    public Pulse[,] globalSituationalMatrix;
    public int transmitterCount;
    OutParameter pulseOut;
    Pulse txPulse = new Pulse(0, 0, 0, 0, "E0");
    //List<Pulse> txPulses = new List<Pulse>();

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        string aircraftPosX;
        string aircraftPosY;
        string radarPosX;
        string radarPosY;
        string radarSymbol;
        string radarPRI;
        string pulsePW;
        string pulseAmp;
        string pulseTimeOfTraversal;
        string pulseAngleOfTravelsal;

        //Console.WriteLine("Enter the position for aircraft:\nX: ");
        //aircraftPosX = Console.ReadLine();
        //Console.WriteLine("Y: ");
        //aircraftPosY = Console.ReadLine();
        //Console.WriteLine("Enter the position for radar:\nX: ");
        //radarPosX = Console.ReadLine();
        //Console.WriteLine("Y: ");
        //radarPosY = Console.ReadLine();
        //Console.WriteLine("Enter the PRI for radar: ");
        //radarPRI = Console.ReadLine();
        //Console.WriteLine("Enter the symbol for radar/pulse: ");
        //radarSymbol = Console.ReadLine();
        //Console.WriteLine("Enter the pulse width: ");
        //pulsePW = Console.ReadLine();
        //Console.WriteLine("Enter the pulse amplitude: ");
        //pulseAmp = Console.ReadLine();
        //Console.WriteLine("Enter the pulse time of traversal: ");
        //pulseTimeOfTraversal = Console.ReadLine();
        //Console.WriteLine("Enter the pulse angle of traversal: ");
        //pulseAngleOfTravelsal = Console.ReadLine();


        //Aircraft a = new Aircraft(new Position(Int32.Parse(aircraftPosX), Int32.Parse(aircraftPosY)), 0);
        Aircraft a = new Aircraft(new Position(10, 10), 0);
        //Radar r = new Radar(new Pulse(Int32.Parse(pulsePW), Int32.Parse(pulseAmp), Int32.Parse(pulseTimeOfTraversal), Int32.Parse(pulseAngleOfTravelsal), radarSymbol), new Position(Int32.Parse(radarPosX), Int32.Parse(radarPosY)), Int32.Parse(radarPRI), radarSymbol, Globals.Tick, 50, 1);
        Radar r2 = new Radar(new Pulse(5, 15, 5, 45, "E2"), new Position(5, 5), 20, "E2", Globals.Tick, 50, 1);
        Radar r3 = new Radar(new Pulse(8, 10, 5, 45, "E3"), new Position(8, 6), 30, "E3", Globals.Tick, 50, 3);

        // PRI for each radar should be greater than 2x the distance to any aircraft (for pulse speed of 1 cell per tick)
        // Minimum unambiguous range for a radar is c * PRI / 2 where c is the speed of light

        a.rwr = new RWR(ref a.position, 2);
        //a2.rwr = new RWR(ref a2.position, 6);
        // be careful with ref operator

        simMod.Add(a);
        simMod.Add(a.rwr);
        simMod.Add(r2);
        simMod.Add(r3);
        //simMod.Add(r2);
        simMod.Add(pse);

        foreach (SimulationModel sim_mod in simMod)
        {
            if (sim_mod is Radar)
            {
                transmitterCount++;
            }
        }

        globalSituationalMatrix = new Pulse[simMod.Count, simMod.Count];

        // Take single aircraft and multiple radars

    }

    public void RunSimulationEngine()
    {
        List<InParameter> inParameters = new List<InParameter>();

        Console.WriteLine($"----------\nTick = {Globals.Tick}\n----------");

        // Get() on every Simulation Model

        if (Globals.Tick != 0)
        {
            Globals.debugPrint = false;
        }

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
                List<InParameter> inParameters2 = new List<InParameter>();
                for (int j = 0; j < simMod.Count; j++)
                {
                    //if (transmitter is RWR)
                    {
                        if (echoPulseSet)
                        {
                            inParameters2.Add(new Radar.In(globalSituationalMatrix[receiver.id, j], 2));
                        }
                    }
                }
                ((Radar)receiver).Set(inParameters2);
            }


            if (receiver is RWR)
            {
                receiverInParams.Clear();
                for (int j = 0; j < simMod.Count; j++)
                {
                    if (globalSituationalMatrix[receiver.id, j] != null)
                    {
                        RWR.In globalSituation = new RWR.In(globalSituationalMatrix[receiver.id, j], receiver.id);
                        receiverInParams.Add(globalSituation);
                    }
                }
                ((RWR)receiver).Set(receiverInParams);

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


        //int transmittorCount = 0;

        foreach (SimulationModel transmitter in simMod)
        {
            foreach (SimulationModel receiver in simMod)
            {


                if (transmitter is Radar)
                {

                    // Note: This will work at the Pulse's scale but not at physical scale of movement of aircraft
                    if (receiver is RWR)
                    {
                        int dist = pse.GetDistance(receiver.id, transmitter.id);
                        int pulseTravelTime = dist / Globals.pulseTravelSpeed;
                        int radius = ((Radar)transmitter).radius;
                        //if (Globals.distDebugPrint)
                        {
                            Console.WriteLine($"Distance between {receiver} {receiver.id} and Radar {transmitter.id} = {dist}");
                            Globals.distDebugPrint = false;
                        }

                        if (((Radar)transmitter).txTicks.Count != 0)
                        {
                            if (Math.Abs(Globals.Tick - ((Radar)transmitter).txTicks.Max()) == pulseTravelTime && Globals.Tick != 0)
                            {
                                Globals.debugPrint = true;
                                Console.WriteLine($"\nPulse from {transmitter} {transmitter.id} arrived at cell of {receiver} {receiver.id}\n");
                                ((RWR)receiver).hasReceivedPulse = true;
                                globalSituationalMatrix[receiver.id, transmitter.id] = ((Radar)transmitter).activePulse;

                                // at current tick at current location, what will be the amplitude of the pulse
                                // no attenuation in amplitude
                                // only pse should the energy at a location and time
                                // only when we introduce sensor model of RWR
                            }

                            else
                            {
                                globalSituationalMatrix[receiver.id, transmitter.id] = ((Radar)transmitter).zeroPulse;
                            }
                        }
                        
                    }
                }

                if (transmitter is RWR)
                {
                    if (receiver is Radar)
                    {
                        int dist = pse.GetDistance(receiver.id, transmitter.id);
                        int pulseTravelTime = dist / Globals.pulseTravelSpeed;
                        if (((Radar)receiver).txTicks.Count != 0)
                        {
                            if (Math.Abs(Globals.Tick - ((Radar)receiver).txTicks.Min()) == 2 * pulseTravelTime && Globals.Tick != 0)
                            {
                                int txTickMin = ((Radar)receiver).txTicks.Min();
                                //if (transmitterCount > 1)
                                {
                                    ((Radar)receiver).txTicks.RemoveAll(item => item == txTickMin);
                                }

                                //((Radar)receiver).pulsesSent--;

                                Console.WriteLine($"\nEcho received by Radar {receiver.id}\n");

                                Console.WriteLine($"Radar {((Radar)receiver).id}:\n\techoPulse:\n\t\tPulse width: {((Radar)receiver).echoPulse.pulseWidth}\n\t\tPRI: {((Radar)receiver).pulseRepetitionInterval}" +
                                                  $"\n\t\tTime of arrival: {((Radar)receiver).echoTimeOfArrival}\n\t\tAngle of arrival: {((Radar)receiver).echoPulse.angleOfTraversal}\n\t\tSymbol: {((Radar)receiver).echoPulse.symbol}" +
                                                  $"\n\t\tAmplitude: {((Radar)receiver).echoPulse.amplitude}\n\t\ttxTick = {((Radar)receiver).txTick}\n");

                                ((Radar)receiver).hasReceivedEcho = true;
                                ((Radar)receiver).echoTimeOfArrival = Globals.Tick;
                                globalSituationalMatrix[receiver.id, transmitter.id] = ((Radar)receiver).activePulse;
                                echoPulseSet = true;
                            }
                        }
                    }
                }
            }
        }

    }
}
