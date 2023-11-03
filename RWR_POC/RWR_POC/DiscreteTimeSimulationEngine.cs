﻿class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new();
    public List<InParameter> receiverInParams = new();
    public PhysicalSimulationEngine pse = new(99);
    public Pulse echoedPulse = new(0, 0, 0, 0, 0, "zero");
    public bool echoPulseSet = false;
    public int rxTick = Globals.Tick;
    public RWR.Emitter detectedRadar = new();
    public Pulse[,] globalSituationalMatrix;
    public int transmitterCount;
    //List<Pulse> txPulses = new List<Pulse>();

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
    }

    public void Init()
    {
        //string aircraftPosX;
        //string aircraftPosY;

        //string radar1PosX;
        //string radar1PosY;
        //string radar1Symbol;
        //string radar1PRI;
        //string pulse1PW;
        //string pulse1Amp;
        //string pulse1Freq;
        //string pulse1TimeOfTraversal;
        //string pulse1AngleOfTraversal;

        //string radar2PosX;
        //string radar2PosY;
        //string radar2Symbol;
        //string radar2PRI;
        //string pulse2PW;
        //string pulse2Amp;
        //string pulse2Freq;
        //string pulse2TimeOfTraversal;
        //string pulse2AngleOfTraversal;

        //static string GetValidInput(string prompt, string errorMessage)
        //{
        //    string userInput;
        //    int parseOut;

        //    do
        //    {
        //        Console.WriteLine(prompt);
        //        userInput = Console.ReadLine();
        //        if (prompt == "Enter the symbol for radar/pulse:")
        //        {
        //            if (string.IsNullOrEmpty(userInput))
        //            {
        //                Console.WriteLine("Please enter a valid non-empty and non-null value.");
        //                continue;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        if (string.IsNullOrEmpty(userInput) || !int.TryParse(userInput, out parseOut))
        //        {
        //            Console.WriteLine(errorMessage);
        //        }
        //    }
        //    while (string.IsNullOrEmpty(userInput) || !int.TryParse(userInput, out parseOut));

        //    return userInput;
        //}

        //aircraftPosX = GetValidInput("Enter the position for aircraft (X):", "Please enter a valid integer value.");
        //aircraftPosY = GetValidInput("Enter the position for aircraft (Y):", "Please enter a valid integer value.");

        //radar1PosX = GetValidInput("Enter the position for radar 1 (X):", "Please enter a valid integer value.");
        //radar1PosY = GetValidInput("Enter the position for radar 1 (Y):", "Please enter a valid integer value.");
        //radar1PRI = GetValidInput("Enter the PRI for radar 1:", "Please enter a valid integer value.");
        //radar1Symbol = GetValidInput("Enter the symbol for radar/pulse:", "Please enter a valid non-empty and non-null value.");
        //pulse1PW = GetValidInput("Enter the pulse 1 width:", "Please enter a valid integer value.");
        //pulse1Amp = GetValidInput("Enter the pulse 1 amplitude:", "Please enter a valid integer value.");
        //pulse1Freq = GetValidInput("Enter the pulse 1 frequency:", "Please enter a valid integer value.");
        //pulse1TimeOfTraversal = GetValidInput("Enter the pulse 1 time of traversal:", "Please enter a valid integer value.");
        //pulse1AngleOfTraversal = GetValidInput("Enter the pulse 1 angle of traversal:", "Please enter a valid integer value.");

        //radar2PosX = GetValidInput("Enter the position for radar 2 (X):", "Please enter a valid integer value.");
        //radar2PosY = GetValidInput("Enter the position for radar 2 (Y):", "Please enter a valid integer value.");
        //radar2PRI = GetValidInput("Enter the PRI for radar 2:", "Please enter a valid integer value.");
        //radar2Symbol = GetValidInput("Enter the symbol for radar/pulse:", "Please enter a valid non-empty and non-null value.");
        //while (radar2Symbol == radar1Symbol)
        //{
        //    Console.WriteLine("Radar 2 symbol cannot be same as Radar 1 symbol. Please try again.");
        //    radar1Symbol = GetValidInput("Enter the symbol for radar/pulse:", "Please enter a valid non-empty and non-null value.");
        //}
        //pulse2PW = GetValidInput("Enter the pulse 2 width:", "Please enter a valid integer value.");
        //pulse2Amp = GetValidInput("Enter the pulse 2 amplitude:", "Please enter a valid integer value.");
        //pulse2Freq = GetValidInput("Enter the pulse 2 frequency:", "Please enter a valid integer value.");
        //pulse2TimeOfTraversal = GetValidInput("Enter the pulse 2 time of traversal:", "Please enter a valid integer value.");
        //pulse2AngleOfTraversal = GetValidInput("Enter the pulse 2 angle of traversal:", "Please enter a valid integer value.");


        //Aircraft a = new Aircraft(new Position(Int32.Parse(aircraftPosX), Int32.Parse(aircraftPosY)), 0);

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




        Aircraft a = new(waypts, 0);
        Aircraft a2 = new(waypts2, 1);

        //Radar r = new Radar(new Pulse(Int32.Parse(pulse1PW), Int32.Parse(pulse1Amp), Int32.Parse(pulse1Freq), Int32.Parse(pulse1TimeOfTraversal), Int32.Parse(pulse1AngleOfTraversal), radar1Symbol), new Position(Int32.Parse(radar1PosX), Int32.Parse(radar1PosY)), Int32.Parse(radar1PRI), radar1Symbol, Globals.Tick, 50, 1);
        //Radar r2 = new Radar(new Pulse(Int32.Parse(pulse2PW), Int32.Parse(pulse2Amp), Int32.Parse(pulse2Freq), Int32.Parse(pulse2TimeOfTraversal), Int32.Parse(pulse2AngleOfTraversal), radar2Symbol), new Position(Int32.Parse(radar2PosX), Int32.Parse(radar2PosY)), Int32.Parse(radar2PRI), radar2Symbol, Globals.Tick, 50, 2);

        Radar r = new(new Pulse(150, 15, 2500, 5, 45, "E1"), new Position(110, 100), 4000, "E1", Globals.Tick, 15, 75, 4);
        Radar r2 = new(new Pulse(100, 10, 1000, 5, 45, "E2"), new Position(185, 50), 8000, "E2", Globals.Tick, 15, 150, 5);
        Radar r3 = new(new Pulse(200, 15, 3000, 5, 45, "E3"), new Position(110, 0), 10000, "E3", Globals.Tick, 15, 270, 6);
        Radar r4 = new(new Pulse(350, 20, 5000, 5, 45, "E4"), new Position(110, 50), 15000, "E4", Globals.Tick, 50, 200, 7);

        // PRI for each radar should be greater than 2x the distance to any aircraft (for pulse speed of 1 cell per tick)
        // Minimum unambiguous range for a radar is c * PRI / 2 where c is the speed of light

        a.rwr = new RWR(ref a.position, 2);
        a2.rwr = new RWR(ref a2.position, 3);
        // be careful with ref operator

        simMod.Add(a);
        simMod.Add(a.rwr);
        simMod.Add(a2);
        simMod.Add(a2.rwr);
        simMod.Add(r);
        simMod.Add(r2);
        simMod.Add(r3);
        simMod.Add(r4);
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
                    //if (globalSituationalMatrix[receiver.id, j] is Radar)
                    {
                        // Condition not needed but some validation is required
                        // TODO: It should be possible to selective pick up the sources

                        //int dist = pse.GetDistance(receiver.position, ((Radar)simMod[j]).position);
                        //int radius = ((Radar)simMod[j]).radius;
                        if (globalSituationalMatrix[receiver.id, j] != null)
                        {
                            RWR.In globalSituation = new(globalSituationalMatrix[receiver.id, j], receiver.id);
                            receiverInParams.Add(globalSituation);
                        }
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

        // TODO: Build semantic net or graph connecting all simulation models


        //int transmittorCount = 0;

        foreach (SimulationModel transmitter in simMod)
        {
            foreach (SimulationModel receiver in simMod)
            {
                if (transmitter is Radar)
                {

                    if (receiver is RWR)
                    {
                        int dist = PhysicalSimulationEngine.GetDistance(receiver.position, transmitter.position);
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
                                Globals.debugPrint = true;
                                Console.WriteLine($"{transmitter} {transmitter.id} is visible to {receiver}{receiver.id}\n");
                                ((RWR)receiver).hasReceivedPulse = true;
                                ((RWR)receiver).receivedPower = ((Radar)transmitter).effectiveRadiatedPower;
                                globalSituationalMatrix[receiver.id, transmitter.id] = ((Radar)transmitter).activePulse;
                                if (!((RWR)receiver).receivedPulses.Contains(globalSituationalMatrix[receiver.id, transmitter.id]))
                                {
                                    ((RWR)receiver).receivedPulses.Add(globalSituationalMatrix[receiver.id, transmitter.id]);
                                }
                            }
                            else
                            {
                                globalSituationalMatrix[receiver.id, transmitter.id] = ((Radar)transmitter).zeroPulse;
                            }
                        }
                    }
                }
            }
        }

    }
}
//if (transmitter is RWR)
//{
//    if (receiver is Radar)
//    {
//        int dist = pse.GetDistance(receiver.id, transmitter.id);
//        int radius = ((Radar)receiver).radius;
//        //if (((Radar)receiver).txTicks.Count != 0)
//        {
//            if (dist <= radius && Globals.Tick != 0)
//            {

//                Console.WriteLine($"\nEcho received by Radar {receiver.id}\n");

//                Console.WriteLine($"Radar {((Radar)receiver).id}:\n\techoPulse:\n\t\tPulse width: {((Radar)receiver).echoPulse.pulseWidth}\n\t\tPRI: {((Radar)receiver).pulseRepetitionInterval}" +
//                                  $"\n\t\tTime of arrival: {((Radar)receiver).echoTimeOfArrival}\n\t\tAngle of arrival: {((Radar)receiver).echoPulse.angleOfTraversal}\n\t\tSymbol: {((Radar)receiver).echoPulse.symbol}" +
//                                  $"\n\t\tAmplitude: {((Radar)receiver).echoPulse.amplitude}\n\t\ttxTick = {((Radar)receiver).txTick}\n");

//                ((Radar)receiver).hasReceivedEcho = true;
//                globalSituationalMatrix[receiver.id, transmitter.id] = ((Radar)receiver).activePulse;
//                echoPulseSet = true;
//            }
//        }
//    }
//}