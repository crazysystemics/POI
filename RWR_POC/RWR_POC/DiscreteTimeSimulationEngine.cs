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
    public EmitterRecord receivedEmitterRecord = new EmitterRecord();

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

        AcquisitionRadar ar1 = new(new Position(130, 0), 100, 15, Globals.Tick, 5);
        FireControlRadar fcr1 = new(new Position(220, 0), 100, 15, Globals.Tick, 7);
        FireControlRadar fcr2 = new(new Position(370, 0), 75, 15, Globals.Tick, 9);

        a.rwr = new RWR(ref a.position, 2);
        a2.rwr = new RWR(ref a2.position, 3);
        // be careful with ref operator

        simMod.Add(a);
        simMod.Add(a.rwr);
        //simMod.Add(a2);
        //simMod.Add(a2.rwr);
        simMod.Add(ar1);
        simMod.Add(fcr1);
        simMod.Add(fcr2);
        //simMod.Add(fcr1.missile1);
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

            if (receiver is AcquisitionRadar)
            {
                foreach (SimulationModel fcr in simMod)
                {
                    if (fcr is FireControlRadar)
                    {
                        receiverInParams.Clear();
                        List<InParameter> inParameters3 = new();
                        if (((AcquisitionRadar)receiver).detection)
                        {
                            FireControlRadar.In targetPositions = new FireControlRadar.In(((AcquisitionRadar)receiver).targetPosition, 6);
                            receiverInParams.Add(targetPositions);
                        }
                        fcr.Set(receiverInParams);
                    }
                }
            }

            //if (receiver is FireControlRadar)
            //{
            //    receiverInParams.Clear();
            //    List<InParameter> inParameters3 = new();
            //    if (detection)
            //    {
            //        FireControlRadar.In targetPositions = new FireControlRadar.In(detectedAircraftPosition, 6);
            //        receiverInParams.Add(targetPositions);
            //    }
            //    receiver.Set(receiverInParams);
            //}



            if (receiver is RWR)
            {
                receiverInParams.Clear();
                if (detection && ((RWR)receiver).receivingEmitterRecord)
                {
                    RWR.In globalSituation = new RWR.In(receivedEmitterRecord, receiver.id);
                    receiverInParams.Add(globalSituation);
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
                            if (transmitter is AcquisitionRadar)
                            {
                                ((AcquisitionRadar)transmitter).detection = true;
                                ((AcquisitionRadar)transmitter).targetPosition = receiver.position;
                            }

                            List<Pulse> pulseTrainTemp = ((Radar)transmitter).GeneratePulseTrain(Globals.Tick * 1000, angle);
                            pulseTrainFromRadar.AddRange(pulseTrainTemp);

                            receivedEmitterRecord = new EmitterRecord();
                            receivedEmitterRecord.pri = ((Radar)transmitter).pulseRepetitionInterval;
                            receivedEmitterRecord.freq = ((Radar)transmitter).txPulse.frequency;
                            receivedEmitterRecord.pw = ((Radar)transmitter).txPulse.pulseWidth;
                            receivedEmitterRecord.erID = ((Radar)transmitter).id;
                            receivedEmitterRecord.erIdentifier = ((Radar)transmitter).radarType.ToString();
                            receivedEmitterRecord.eID = ((Radar)transmitter).id + 10;

                            ((RWR)receiver).receivingEmitterRecord = true;

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