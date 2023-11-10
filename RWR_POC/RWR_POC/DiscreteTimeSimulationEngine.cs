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

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void GetJSONData()
    {
        JSONReader.AircraftJSON aircraftJSON = new JSONReader.AircraftJSON();
        JSONReader.AircraftListJSON aircraftListJSON = new JSONReader.AircraftListJSON();
        JSONReader.RadarListJSON radarListJSON = new JSONReader.RadarListJSON();
        JSONReader jsonReader = new JSONReader();

        aircraftListJSON = jsonReader.ParseAircraftList("aircraft_inputs.json");
        radarListJSON = jsonReader.ParseRadarList("radar_inputs.json");

        List<Radar> radarList = new List<Radar>();
        List<Aircraft> aircraftList = new List<Aircraft>();

        for (int i = 0; i < aircraftListJSON.aircraftJSONs.Count; i++)
        {
            aircraftList.Add(new Aircraft(aircraftListJSON.aircraftJSONs[i].waypoints, aircraftListJSON.aircraftJSONs[i].id));
        }

        for (int i = 0; i < radarListJSON.radarJSONs.Count; i++)
        {
            radarList.Add(new Radar(new Pulse(radarListJSON.radarJSONs[i].initPulse.pulseWidth,
                                              radarListJSON.radarJSONs[i].initPulse.amplitude,
                                              radarListJSON.radarJSONs[i].initPulse.frequency,
                                              radarListJSON.radarJSONs[i].initPulse.timeOfTraversal,
                                              (int)radarListJSON.radarJSONs[i].initPulse.angleOfTraversal),
                                    radarListJSON.radarJSONs[i].position,
                                    radarListJSON.radarJSONs[i].pulseRepetitionInterval,
                                    radarListJSON.radarJSONs[i].txTick,
                                    radarListJSON.radarJSONs[i].radius,
                                    radarListJSON.radarJSONs[i].id));
        }

        foreach (Aircraft air in aircraftList)
        {
            air.rwr = new RWR(ref air.position, (air.id + 10));
            simMod.Add(air);
            simMod.Add(air.rwr);
        }

        foreach (Radar radar in radarList)
        {
            simMod.Add(radar);
        }
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

        string currentTime = DateTime.Now.ToString();
        currentTime = currentTime.Replace(":", "-");
        currentTime = currentTime.Replace(" ", "-");
        currentTime = currentTime.Remove(16);

        Globals.recFileName = $"erOutputFile{currentTime}.csv";

        PFM.emitterIDTable.Add(new EmitterID(1, "E1", 30, 70, 20, 2000, 3000, 200, 100, 200, 50));
        PFM.emitterIDTable.Add(new EmitterID(2, "E2", 60, 100, 20, 1000, 2500, 500, 50, 150, 25));

        GetJSONData();
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
                        //if (Globals.distDebugPrint)
                        {
                            Console.WriteLine($"Distance between {receiver} {receiver.id} and Radar {transmitter.id} = {dist}");
                            Globals.distDebugPrint = false;
                        }

                        //if (((Radar)transmitter).txTicks.Count != 0)
                        {
                            if (dist <= radius)
                            {
                                detection = true;
                                List<Pulse> pulseTrainTemp = ((Radar)transmitter).GeneratePulseTrain(1000, Globals.Tick * 1000, angle);
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
}