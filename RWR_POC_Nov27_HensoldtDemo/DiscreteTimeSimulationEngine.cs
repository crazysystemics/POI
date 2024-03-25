using RWR_POC_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();
    public List<InParameter> receiverInParams = new List<InParameter>();
    public PhysicalSimulationEngine pse = new PhysicalSimulationEngine(99);
    public bool detection = false;
    public Pulse[,] globalSituationalMatrix;
    public List<Pulse> pulseTrainFromRadar = new List<Pulse>();
    public List<EmitterRecord> emitterRecords = new List<EmitterRecord>();
    public double[] receiverAmps = new double[4];
    public Position detectedAircraftPosition = new Position();
    public EmitterRecord receivedEmitterRecord = new EmitterRecord();
    public double nextWaypointAngle = 0;
    public int patternTick = 0;
    public UDPListener udpListener = new UDPListener();

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        Globals.debugPrint = Globals.DebugLevel.BRIEF;
        Globals.episodeIsDone = false;

        //List<Position> waypts = new List<Position>()
        //{
        //    new Position(25, 25),
        //    new Position(25, 75),
        //    new Position(75, 75),
        //    new Position(75, 25),
        //    new Position(25, 25)
        //};

        //List<Position> waypts2 = new List<Position>()
        //{
        //    new Position(50, 50),
        //    new Position(85, 85),
        //    new Position(135, 85),
        //    new Position(170, 50),
        //    new Position(135, 15),
        //    new Position(85, 15),
        //    new Position(50, 50)
        //};

        //List<Position> waypts3 = new List<Position>()
        //{
        //    new Position(100, 100),
        //    new Position(200, 200),
        //    new Position(300, 200),
        //    new Position(400, 100),
        //    new Position(300, 0),
        //    new Position(200, 0),
        //    new Position(100, 100)
        //};

        //List<Position> wayptsLinear = new List<Position>()
        //{
        //    new Position(50, 50),
        //    new Position(200, 200)
        //};

        //RadarList.simpleRadarList = new List<SimpleRadar>()
        //{
        //    new SimpleRadar(new Position(85, 90), 20, 15, Globals.Tick, 2),
        //    new SimpleRadar(new Position(110, 80), 20, 15, Globals.Tick, 3),
        //    new SimpleRadar(new Position(135, 90), 20, 15, Globals.Tick, 4),
        //    new SimpleRadar(new Position(135, 10), 20, 15, Globals.Tick, 5),
        //    new SimpleRadar(new Position(110, 20), 20, 15, Globals.Tick, 6),
        //    new SimpleRadar(new Position(85, 10), 20, 15, Globals.Tick, 7)
        //};

        //AircraftList.aircraftList = new List<Aircraft>()
        //{
        //    new Aircraft(new List<Position>()
        //    {
        //        new Position(50, 50),
        //        new Position(85, 85),
        //        new Position(135, 85),
        //        new Position(170, 50),
        //        new Position(135, 15),
        //        new Position(85, 15),
        //        new Position(50, 50)
        //    }, 0)
        //};

        //foreach (Aircraft air in AircraftList.aircraftList)
        //{
        //    AircraftList.rwrList.Add(new RWR(ref air.position, air.id + 10));
        //}

        simMod.Clear();

        Scenario newScenario = new Scenario();

        

        string currentTime = DateTime.Now.ToString();
        currentTime = currentTime.Replace(":", "-");
        currentTime = currentTime.Replace(" ", "-");
        currentTime = currentTime.Remove(16);

        Globals.recFileName = $"erOutputFile{currentTime}.csv";
        Globals.trackRecFileName = $"RecordedData{currentTime}.csv";

        PFM.emitterIDTable.Add(new PFMEmitterRecord(0, "E1", 500, 800, 150, 8000, 10000, 500, 50, 150, 25, 3, 100));    
        PFM.emitterIDTable.Add(new PFMEmitterRecord(1, "E2", 800, 1500, 200, 3000, 4000, 500, 100, 200, 50, 3, 100));

        if (Globals.episodesEnded == 0)
        {
            Globals.initializeQlearner();
        }
        

        simMod.Add(newScenario.chosenAircraft);
        simMod.Add(newScenario.chosenAircraft.rwr);
        simMod.AddRange(newScenario.radars);
        //simMod.Add(pse);


        
        //udpListener.StartListener(newScenario.chosenAircraft);


        //pfm.FormPFMTable();

        //Aircraft a = new Aircraft(waypts2, 0);
        //Aircraft a2 = new(waypts2, 1);

        //AcquisitionRadar ar1 = new AcquisitionRadar(new Position(200, 180), 150, 15, Globals.Tick, 5, 359, 0);
        //FireControlRadar fcr1 = new FireControlRadar(new Position(300, 180), 100, 15, Globals.Tick, 7, 359, 0);
        //FireControlRadar fcr2 = new FireControlRadar(new Position(200, 0), 100, 15, Globals.Tick, 9, 359, 0);

        //a.rwr = new RWR(ref a.position, 2);

        //a2.rwr = new RWR(ref a2.position, 3);
        // be careful with ref operator

        //SimpleRadar r = new SimpleRadar(new Position(70, 50), 20, 15, Globals.Tick, 120);

        //SimpleRadar r2 = new SimpleRadar(new Position(110, 15), 75, Globals.Tick, 20, 150, 5);
        //r2.pulseRepetitionInterval = 700;
        //r2.txPulse.frequency = 8500;
        //r2.txPulse.pulseWidth = 75;
        //Radar r3 = new Radar(new Pulse(200, 15, 3000, 5, 0), new Position(110, 0), 70, Globals.Tick, 15, 270, 6);
        //Radar r4 = new Radar(new Pulse(350, 20, 5000, 5, 0), new Position(110, 50), 30, Globals.Tick, 20, 200, 7);


        //simMod.Add(a);
        //simMod.Add(a.rwr);
        //simMod.Add(a2);
        //simMod.Add(a2.rwr);
        //simMod.Add(ar1);
        //simMod.Add(r);
        //simMod.Add(r2);
        //simMod.Add(fcr1.missile1);

        globalSituationalMatrix = new Pulse[simMod.Count, simMod.Count];
        Globals.mainWindow.DisplayFlightPath(newScenario.chosenAircraft);
        // Take single aircraft and multiple radars

    }

    public void RunSimulationEngine()
    {
        int count = 0;

        Globals.timer.Interval = TimeSpan.FromMilliseconds(50);
        Globals.timer.Tick += (sender, e) =>
        {

            if (Globals.episodesEnded <= Globals.numEpisodes)
            {
                Console.WriteLine($"Current Episode: {Globals.episodesEnded + 1}");
                if (Globals.episodeIsDone)
                {
                    this.Init();
                    Globals.Tick = 0;
                }

                emitterRecords.Clear();
                List<InParameter> inParameters = new List<InParameter>();

                if (Globals.debugPrint == Globals.DebugLevel.BRIEF || Globals.debugPrint == Globals.DebugLevel.VERBOSE)
                {
                    Console.WriteLine($"----------\nTick = {Globals.Tick}\n----------");
                }

                // Get() on every Simulation Model

                foreach (SimulationModel sim_model in simMod)
                { // triggred in ln 48 in udp listener get if statement
                  // physEngine.physInParameters.Add(sim_model.Get());
                  // if model is airc
                  //  create a tmp variable_airc_get and store the aircraft's position and heading

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
                // Get the emitter records from dtse(udp)

                // Set() on every Simulation Model

                foreach (SimulationModel receiver in simMod)

                { // triggred in ln 60 in udp listener set if statement. Comment it here.

                    //pse.Set(inParameters);

                    if (receiver is Radar)
                    {
                        List<InParameter> inParameters2 = new List<InParameter>();
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
                                List<InParameter> inParameters3 = new List<InParameter>();
                                if (((AcquisitionRadar)receiver).detection)
                                {
                                    FireControlRadar.In targetPositions = new FireControlRadar.In(((AcquisitionRadar)receiver).targetPosition, 6);
                                    receiverInParams.Add(targetPositions);
                                }
                                fcr.Set(receiverInParams);
                            }
                        }
                    }



                    if (receiver is RWR)
                    {
                        receiverInParams.Clear();
                        if (detection)
                        {
                            foreach (EmitterRecord emitterRecord in emitterRecords)
                            {
                                //if (((RWR)receiver).receivingEmitterRecord)
                                {
                                    RWR.In globalSituation = new RWR.In(emitterRecord, emitterRecord.eID);
                                    receiverInParams.Add(globalSituation);
                                }
                            }
                            detection = false;
                        }
                        ((RWR)receiver).Set(receiverInParams);
                        //Pass the modified emitter records recieved from dtse(udp)
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

                // OnTick() on each Simulation Model
                Console.WriteLine();
                // trigger in if statement (cmd[0]=="ontick") in udp code
                {
                    foreach (SimulationModel sim_model in simMod)
                    {
                        sim_model.OnTick();
                    }
                    foreach (SimulationModel sim_model in simMod)
                    {
                        Globals.mainWindow.DisplayPosition(sim_model);
                    }

                    foreach (OutParameter outParam in dtseOutParameter)
                    {
                        if (outParam is RWR.Out)
                        {
                            Globals.mainWindow.rwrDisplay.DisplaySymbols(((RWR.Out)outParam).visibleRadars);
                        }
                    }

                    Globals.Tick++;
                    Globals.persistentTick++;

                    // Will be done by in UDP instead
                    if (Globals.mainWindow.btn_next_tick.Background == Brushes.Yellow)
                    {
                        count++;
                        if (count > 0)
                        {
                            Globals.timer.Stop();
                            count = 0;
                            Globals.mainWindow.btn_next_tick.Background = Brushes.LightYellow;
                        }
                    }
                }
            }

            else
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadLine();
                    Globals.ExecuteShell();
                }

            }
        };
      
    }




    public void BuildGlobalSituationAwareness()
    {
        //pulseTrainFromRadar.Clear();

        foreach (SimulationModel transmitter in simMod)
        {
            foreach (SimulationModel receiver in simMod)
            {
                if (transmitter is Radar)
                {

                    if (receiver is RWR)
                    {
                        foreach (SimulationModel aircraft in simMod)
                        {
                            if (aircraft is Aircraft)
                            {
                                nextWaypointAngle = ((Aircraft)aircraft).nextWaypointAzimuth;
                            }
                        }

                        int dist = PhysicalSimulationEngine.GetDistance(receiver.position, transmitter.position);
                        double angle = PhysicalSimulationEngine.GetAngle(receiver.position, transmitter.position) + (Math.PI / 2);
                        int radius = ((Radar)transmitter).radius;

                        if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
                        {
                            Console.WriteLine($"Distance between {receiver} {receiver.id} and Radar {transmitter.id} = {dist}");
                            Console.WriteLine($"Azimuth = {angle}");
                        }

                        if (((Radar)transmitter).beamContains(receiver.position))
                        {
                            //bool recordProb = false;
                            if (Globals.Tick % Globals.testCases.TestCases[Globals.testCaseID].detectionPattern.Count == 0)
                            {
                                patternTick = 0;
                            }
                            bool recordProb = Globals.testCases.TestCases[Globals.testCaseID].detectionPattern[patternTick];
                            patternTick++;

                            //if (Globals.Tick < Globals.testCases.TestCases[Globals.testCaseID].detectionPattern.Count)
                            {
                                // Picks boolean from Detection Patterns list from selected test case for current tick.

                            }
                            //else
                            //{
                            //    recordProb = true;
                            //}

                            if (recordProb)
                            {
                                detection = true;
                                if (transmitter is SimpleRadar)
                                {

                                }

                                if (transmitter is AcquisitionRadar)
                                {
                                    ((AcquisitionRadar)transmitter).detection = true;
                                    ((AcquisitionRadar)transmitter).targetPosition = receiver.position;
                                }

                                // List<Pulse> pulseTrainTemp = ((Radar)transmitter).GeneratePulseTrain(Globals.Tick * 1000, angle);
                                // pulseTrainFromRadar.AddRange(pulseTrainTemp);

                                receivedEmitterRecord = new EmitterRecord();
                                receivedEmitterRecord.pri = ((Radar)transmitter).pulseRepetitionInterval;
                                receivedEmitterRecord.freq = ((Radar)transmitter).txPulse.frequency;
                                receivedEmitterRecord.pw = ((Radar)transmitter).txPulse.pulseWidth;
                                receivedEmitterRecord.erID = ((Radar)transmitter).id;
                                receivedEmitterRecord.erIdentifier = ((Radar)transmitter).radarType.ToString();
                                receivedEmitterRecord.distance = dist;
                                receivedEmitterRecord.maxRange = ((Radar)transmitter).radius;
                                receivedEmitterRecord.azimuth = nextWaypointAngle - angle;
                                receivedEmitterRecord.eID = ((Radar)transmitter).id;
                                emitterRecords.Add(receivedEmitterRecord);

                                // PW, PRI, Freq, AOA, ReceivedPower - emitter record received over network

                            }

                            

                            //   ((RWR)receiver).receivingEmitterRecord = true;

                            //int amp = pulseTrainFromRadar[0].amplitude;

                            //receiverAmps[0] = amp * Math.Cos(angle - (Math.PI / 4));
                            //receiverAmps[1] = amp * Math.Cos(angle + (Math.PI / 4));
                            //receiverAmps[2] = amp * Math.Cos(angle - (3 * Math.PI / 4));
                            //receiverAmps[3] = amp * Math.Cos(angle - (5 * Math.PI / 4));

                            //for (int i = 0; i < 4; i++)
                            //{
                            //    if (receiverAmps[i] <= 0)
                            //    {
                            //        receiverAmps[i] = 0;
                            //    }
                            //}

                        }

                    }
                }
            }
        }

    }
}