using RWR_POC_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();
    public List<InParameter> receiverInParams = new List<InParameter>();
    public PhysicalSimulationEngine pse = new PhysicalSimulationEngine(99);
    public bool detection = false;
    public List<EmitterRecord> emitterRecords = new List<EmitterRecord>();
    public EmitterRecord receivedEmitterRecord = new EmitterRecord();
    public double nextWaypointAngle = 0;
    public int patternTick = 0;
    public UDPListener udpListener = new UDPListener();

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        Globals.tick = 0;
    }

    public void Init()
    {

        // Initialize Discrete Time Simulation Engine


        Globals.debugPrint = Globals.DebugLevel.BRIEF; // Set Debug level
        Globals.episodeIsDone = false; // Set initial value of episodeIsDone to false. This is to track multiple episodes of Qlearning

        simMod.Clear(); // Clear list of simulation models / battle systems from list

        Scenario newScenario = new Scenario(); // Create a new Scenarion object. Check Scenario.cs for details.

        string currentTime = DateTime.Now.ToString(); // Set currentTime using DateTime module to get unique names for Output files.
        currentTime = currentTime.Replace(":", "-"); // Replacing characters not allowed in filenames.
        currentTime = currentTime.Replace(" ", "-");
        currentTime = currentTime.Remove(16); // Trim filename to exclude "seconds" from DateTime


        // Set Output file names
        Globals.recFileName = $"erOutputFile{currentTime}.csv";
        Globals.trackRecFileName = $"RecordedData{currentTime}.csv";
        Globals.qsaTableFileName = $"QsaTable{currentTime}.csv";


        // Adding arbitrary placeholder entries into PFM table.

        PFM.emitterIDTable.Add(new PFMEmitterRecord(0, "E1", 500, 800, 150, 8000, 10000, 500, 50, 150, 25, 3, 100));    
        PFM.emitterIDTable.Add(new PFMEmitterRecord(1, "E2", 800, 1500, 200, 3000, 4000, 500, 100, 200, 50, 3, 100));
        

        if (Globals.episodesEnded == 0)
        {
            // Create Qlearner object.
            Globals.InitializeQLearner();
        }
        
        // Add the selected set of radars positions and aircraft waypoints to simulation models list

        simMod.Add(newScenario.chosenAircraft);
        simMod.Add(newScenario.chosenAircraft.rwr);
        simMod.AddRange(newScenario.radars);

        // Draw the aircraft waypoints and radar positions on WPF window.
        Globals.mainWindow.DisplayFlightPath(newScenario.chosenAircraft);

    }

    public void RunSimulationEngine()
    {
        int count = 0;

        Globals.timer.Interval = TimeSpan.FromMilliseconds(50);
        Globals.timer.Tick += (sender, e) =>
        {

            if (Globals.episodesEnded <= Globals.numEpisodes)
            {
                // Run Simulation for a set number of episodes.

                Console.WriteLine($"Current Episode: {Globals.episodesEnded + 1}");

                if (Globals.episodeIsDone)
                {
                    // Re-initialize DTSE at the end of an episode.
                    this.Init();
                    Globals.tick = 0;
                }

                emitterRecords.Clear();

                // Initialize list of InParameters to be used with Set() function.
                // InParameters are passed from DTSE's global situational awareness to the relevant Battle System objects.
                // For example, InParameters for RWR are the emitter records

                List<InParameter> inParameters = new List<InParameter>();

                if (Globals.debugPrint == Globals.DebugLevel.BRIEF || Globals.debugPrint == Globals.DebugLevel.VERBOSE || Globals.debugPrint == Globals.DebugLevel.SPOT)
                {
                    Console.WriteLine($"----------\nTick = {Globals.tick}\n----------");
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

                // Any relevant information processed by the DTSE module is sent back to Battle Systems registered to it.

                foreach (SimulationModel receiver in simMod)

                { // triggred in ln 60 in udp listener set if statement. Comment it here.

                    //pse.Set(inParameters);

                    if (receiver is Radar)
                    {

                        // Any logic for setting InParameters for Radar (emitter) goes here.
                        // Earlier, this used to contain information of echoed pulses.
                        // Currently, there is no information being sent back to radars.

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

                    // In case of positive detection, emitter records processed by DTSE are first stored in
                    // a list of In Parameters called receiverInParams, and then set back to the RWR with the Set() method.

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

                // OnTick() on each Simulation Model

                Console.WriteLine();

                // trigger in if statement (cmd[0]=="ontick") in udp code

                {
                    foreach (SimulationModel sim_model in simMod)
                    {
                        // Run OnTick on each simulation model / battle system
                        sim_model.OnTick();
                    }
                    foreach (SimulationModel sim_model in simMod)
                    {
                        // Update WPF window with new positions of simulation models
                        Globals.mainWindow.DisplayPosition(sim_model);
                    }

                    foreach (OutParameter outParam in dtseOutParameter)
                    {
                        if (outParam is RWR.Out)
                        {
                            // Update RWR display with position of detected emitters
                            Globals.mainWindow.rwrDisplay.DisplaySymbols(((RWR.Out)outParam).visibleRadars);
                        }
                    }

                    Globals.tick++; // Increment Tick (resets at the end of each episode)
                    Globals.persistentTick++; // Increment Persistent Tick (persists across episodes)

                    // Will be done by in UDP instead
                    if (Globals.mainWindow.btn_next_tick.Background == Brushes.Yellow)
                    {
                        // Stops/pauses the simulation based on button clicks in UI.
                        // This logic depends on background colour of a visual element and must be changed
                        // to reflect the correct logic for stopping/starting the simulation.
                        // Events must be used here.
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
                    // Allows interrupting the simulation through command line shell
                    // and allows monitoring emitter track file during the simulation.
                    Console.ReadLine();
                    Globals.ExecuteShell();
                }

            }
        };
      
    }




    public void BuildGlobalSituationAwareness()
    {

        foreach (SimulationModel transmitter in simMod)
        {
            foreach (SimulationModel receiver in simMod)
            {
                if (transmitter is Radar)
                {

                    // Only enter this condition if "transmitter" in the outer For loop is a Radar

                    if (receiver is RWR)
                    {

                        // Only enter this condition if "receiver" in the inner For loop is a Radar Warning Receiver

                        // All logic that occurs here will happen between a Radar as Transmitter and RWR as Receiver.

                        foreach (SimulationModel aircraft in simMod)
                        {
                            if (aircraft is Aircraft)
                            {
                                // Find the current heading of the Aircraft in order to find azimuth of all emitters
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

                        if (((Radar)transmitter).BeamContains(receiver.position))
                        {
                            // Check beamContains method of Radar for more details
                            // beamContains returns a boolean based on the detection radius of the radar in question
                            // as well as its scan sector (for radars other than Simple Radar)

                            bool recordProb = true;

                            // recordProb is currently based on fixed emitter patterns used for testing.
                            // This boolean may either always be true when beamContains is true,
                            // but it can be used with a random number generator to imitate environmental noise

                            // Uncomment the lines below to use TestCases patterns.

                            //if (Globals.Tick % Globals.testCases.TestCases[Globals.testCaseID].detectionPattern.Count == 0)
                            //{
                            //    patternTick = 0;
                            //}
                            //bool recordProb = Globals.testCases.TestCases[Globals.testCaseID].detectionPattern[patternTick];
                            //patternTick++;

                            //if (Globals.Tick < Globals.testCases.TestCases[Globals.testCaseID].detectionPattern.Count)
                            {
                                // Picks boolean from Detection Patterns list from selected test case for current tick.

                            }
                            //else
                            //{
                            //    recordProb = true;
                            //}

                            // Uncomment the lines above to use the TestCases patterns.

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

                                // Extract information from the Emitter record seen by the DTSE and set it to
                                // receivedEmitterRecord, which is an EmitterRecord object local to DTSE, and used
                                // during the Set() phase to set the received records (from the Transmitter in this loop)
                                // to the RWR (the receiver in this loop)

                                receivedEmitterRecord = new EmitterRecord();
                                receivedEmitterRecord.pri = ((Radar)transmitter).pulseRepetitionInterval;
                                receivedEmitterRecord.freq = ((Radar)transmitter).txPulse.frequency;
                                receivedEmitterRecord.pulseWidth = ((Radar)transmitter).txPulse.pulseWidth;
                                receivedEmitterRecord.erID = ((Radar)transmitter).id;
                                receivedEmitterRecord.erIdentifier = ((Radar)transmitter).radarType.ToString();
                                receivedEmitterRecord.distance = dist;
                                receivedEmitterRecord.maxRange = ((Radar)transmitter).radius;
                                receivedEmitterRecord.azimuth = nextWaypointAngle - angle;
                                receivedEmitterRecord.eID = ((Radar)transmitter).id;
                                emitterRecords.Add(receivedEmitterRecord);

                                // PW, PRI, Freq, AOA, ReceivedPower - emitter record received over network

                            }

                        }

                    }
                }
            }
        }

    }
}