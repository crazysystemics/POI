class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new();
    public List<InParameter> receiverInParams = new();
    public PhysicalSimulationEngine pse = new(99);
    public bool detection = false;
    public Pulse[,] globalSituationalMatrix;
    public List<Pulse> pulseTrainFromRadar = new();
    public double[] receiverAmps = new double[4];
    public Position detectedAircraftPosition = new Position();

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        List<Position> waypts = new()
        {
            new Position(50, 50),
            new Position(85, 85),
            new Position(135, 85),
            new Position(170, 50),
            new Position(135, 15),
            new Position(85, 15),
            new Position(50, 50)
        };

        List<Position> waypts2 = new()
        {
            new Position(85, 85),
            new Position(135, 85),
            new Position(170, 50),
            new Position(135, 15),
            new Position(85, 15),
            new Position(50, 50),
            new Position(85, 85)
        };

        List<Position> wayptsLinear = new()
        {
            new Position(0, 50),
            new Position(500, 50)
        };


        string currentTime = DateTime.Now.ToString();
        currentTime = currentTime.Replace(":", "-");
        currentTime = currentTime.Replace(" ", "-");
        currentTime = currentTime.Remove(16);

        Globals.recFileName = $"erOutputFile{currentTime}.csv";

        PFM.emitterIDTable.Add(new EmitterID(1, "E1", 800, 1500, 200, 3000, 4000, 500, 100, 200, 50));
        PFM.emitterIDTable.Add(new EmitterID(2, "E2", 500, 800, 150, 8000, 10000, 500, 50, 150, 25));


        Aircraft a = new(wayptsLinear, 0);
        Aircraft a2 = new(waypts2, 1);

        //Radar r = new Radar(new Pulse(Int32.Parse(pulse1PW), Int32.Parse(pulse1Amp), Int32.Parse(pulse1Freq), Int32.Parse(pulse1TimeOfTraversal), Int32.Parse(pulse1AngleOfTraversal), radar1Symbol), new Position(Int32.Parse(radar1PosX), Int32.Parse(radar1PosY)), Int32.Parse(radar1PRI), radar1Symbol, Globals.Tick, 50, 1);
        //Radar r2 = new Radar(new Pulse(Int32.Parse(pulse2PW), Int32.Parse(pulse2Amp), Int32.Parse(pulse2Freq), Int32.Parse(pulse2TimeOfTraversal), Int32.Parse(pulse2AngleOfTraversal), radar2Symbol), new Position(Int32.Parse(radar2PosX), Int32.Parse(radar2PosY)), Int32.Parse(radar2PRI), radar2Symbol, Globals.Tick, 50, 2);

        //Radar r = new(new Pulse(150, 15, 2500, 5, 0), new Position(110, 100), 50, Globals.Tick, 15, 75, 4);
        //Radar r2 = new(new Pulse(100, 10, 1000, 5, 0), new Position(185, 50), 80, Globals.Tick, 20, 150, 5);
        //Radar r3 = new(new Pulse(200, 15, 3000, 5, 0), new Position(110, 0), 70, Globals.Tick, 15, 270, 6);
        //Radar r4 = new(new Pulse(350, 20, 5000, 5, 0), new Position(110, 50), 30, Globals.Tick, 20, 200, 7);
        //Radar r5 = new(new Pulse(350, 20, 5000, 5, 0), new Position(100, 0), 30, Globals.Tick, 20, 200, 8);

        AcquisitionRadar ar1 = new(new Position(70, 0), 100, 15, Globals.Tick, 5);
        FireControlRadar fcr1 = new(new Position(370, 0), 100, 15, Globals.Tick, 7);

        a.rwr = new RWR(ref a.position, 2);
        a2.rwr = new RWR(ref a2.position, 3);
        // be careful with ref operator

        simMod.Add(a);
        simMod.Add(a.rwr);
        //simMod.Add(a2);
        //simMod.Add(a2.rwr);
        //simMod.Add(r);
        //simMod.Add(r2);
        //simMod.Add(r3);
        //simMod.Add(r4);
        simMod.Add(ar1);
        simMod.Add(fcr1);
        simMod.Add(pse);

        globalSituationalMatrix = new Pulse[simMod.Count, simMod.Count];

        // Take single aircraft and multiple radars

    }

    public void RunSimulationEngine()
    {
        List<InParameter> inParameters = new();

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

        BuildGlobalSituationAwareness();

        // Set() on every Simulation Model

        foreach (SimulationModel receiver in simMod)
        {
            //pse.Set(inParameters);

            if (receiver is Radar)
            {
                List<InParameter> inParameters2 = new();
                for (int j = 0; j < simMod.Count; j++)
                {
                    //if (transmitter is RWR)
                    {
                        //if (echoPulseSet)
                        //{
                        //    inParameters2.Add(new Radar.In(globalSituationalMatrix[receiver.id, j], 2));
                        //}
                    }
                }
                ((Radar)receiver).Set(inParameters2);
            }

            if (receiver is FireControlRadar)
            {
                receiverInParams.Clear();
                List<InParameter> inParameters3 = new();
                if (detection)
                {
                    FireControlRadar.In targetPositions = new FireControlRadar.In(detectedAircraftPosition, 6);
                    receiverInParams.Add(targetPositions);
                }
                receiver.Set(receiverInParams);
            }



            if (receiver is RWR)
            {
                receiverInParams.Clear();
                if (detection)
                {
                    foreach (Pulse pulse in pulseTrainFromRadar)
                    {
                        RWR.In globalSituation = new RWR.In(pulse, receiverAmps, receiver.id);
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

    public void BuildGlobalSituationAwareness()
    {
        pulseTrainFromRadar.Clear();

        foreach (SimulationModel transmitter in simMod)
        {
            foreach (SimulationModel receiver in simMod)
            {
                if (transmitter is Radar)
                {

                    if (receiver is RWR)
                    {
                        int dist = PhysicalSimulationEngine.GetDistance(receiver.position, transmitter.position);
                        double angle = PhysicalSimulationEngine.GetAngle(receiver.position, transmitter.position);
                        int radius = ((Radar)transmitter).radius;

                        Console.WriteLine($"Distance between {receiver} {receiver.id} and Radar {transmitter.id} = {dist}");
                        Globals.distDebugPrint = false;

                        if (((Radar)transmitter).beamContains(receiver.position))
                        {
                            detection = true;
                            detectedAircraftPosition = receiver.position;
                            foreach (SimulationModel fcr in simMod)
                            {
                                if (fcr is FireControlRadar)
                                {
                                    if (!(((FireControlRadar)fcr).launchedMissile))
                                    {
                                        Missile missile = new Missile(fcr.position, receiver.position);
                                        ((FireControlRadar)fcr).launchedMissile = true;
                                    }
                                }
                            }
                            List<Pulse> pulseTrainTemp = ((Radar)transmitter).GeneratePulseTrain(Globals.Tick * 1000, angle);
                            pulseTrainFromRadar.AddRange(pulseTrainTemp);

                            int amp = pulseTrainFromRadar[0].amplitude;

                            receiverAmps[0] = amp * Math.Cos(angle - (Math.PI / 4));
                            receiverAmps[1] = amp * Math.Cos(angle + (Math.PI / 4));
                            receiverAmps[2] = amp * Math.Cos(angle - (3 * Math.PI / 4));
                            receiverAmps[3] = amp * Math.Cos(angle - (5 * Math.PI / 4));

                            for (int i = 0; i < 4; i++)
                            {
                                if (receiverAmps[i] <= 0)
                                {
                                    receiverAmps[i] = 0;
                                }
                            }

                        }

                    }
                }
            }
        }

    }
}