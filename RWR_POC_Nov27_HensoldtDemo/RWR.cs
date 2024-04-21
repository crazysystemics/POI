using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using RWR_POC_GUI;

public class RWR : BattleSystem
{
    public override bool Stopped { get; set; }
    public int sensitivity;
    public int toaResolution;
    public int aperture;
    public double[] amps = new double[4];
    public List<EmitterRecord> emRecordList = new List<EmitterRecord>();
    public List<EmitterTrackRecord> emitterTrackFile = new List<EmitterTrackRecord>();
    public List<QState> prevQStates = new List<QState>();
    public QState prevState_at_env_step_begin = new QState(1, 0);
    public QState currentState_at_env_step_begin = new QState(1, 0);
    public int envActionsPerformed = 0;

    public int trackLossCount = 0;
    public int rewardValue = 0;

    public RWR(ref Position positon, int id)
    {
        this.position = positon;
        this.id = id;
    }

    public List<EmitterRecord> RxBuf = new List<EmitterRecord>(); // Reception buffer for emitter records.
    // When Emitter records are processed by DTSE, and Set() is called on RWR, the receivedEmitterRecords from DTSE
    // are first set into this buffer.

    public class Out : OutParameter
    {

        // Information sent to DTSE by RWR when Get() is called.

        public int r;
        public int theta;
        public Position position;
        public List<RadarSignature> visibleRadars = new List<RadarSignature>();
        public Out(int r, int theta, Position position, int id) : base(id)
        {
            this.r = r;
            this.theta = theta;
            this.position = position;
        }
    }

    public class In : InParameter
    {

        // Information set into RWR from DTSE when Set() is called.

        public double[] amps = new double[4];
        public EmitterRecord emRecord = new EmitterRecord();

        public In(EmitterRecord emRecord, int id) : base(id)
        {
            // When an RWR InParameter object is created, it takes emitter record received by DTSE as argument and sets
            // it to the emitterRecord attribute of the InParameter.
            this.emRecord = emRecord;
        }

    }

    public override OutParameter Get()
    {

        // Output information of visible emitters to DTSE to build global situational awareness

        Out rwrOutParams = new Out(0, 0, position, 2);
        foreach (EmitterRecord emitterRecord in RxBuf)
        {
            rwrOutParams.visibleRadars.Add(new RadarSignature(emitterRecord.distance, emitterRecord.azimuth, emitterRecord.maxRange, emitterRecord.erIdentifier));
        }
        return rwrOutParams;
    }

    public override void Set(List<InParameter> inParameters)
    {

        // Add received emitter records from InParameters to RxBuf.

        RxBuf.Clear();
        foreach (InParameter inParameter in inParameters)
        {
            if (((In)inParameter).emRecord.pw > 0)
            {
                this.RxBuf.Add(((In)inParameter).emRecord);
            }
        }
    }

    //Assuming only ONE EMITTER IS THERE
    //TODO: EXTEND TO N EMITTERS

    public QState prevQState = new QState(0, 0);


    public void env_step_1()
    {

        // Choose Actions 0, 1 and 2 sequentially every time env_step is performed

        // Only explores, never exploits

        QState state = prevState_at_env_step_begin;

        string randChoice;

        randChoice = "EXPLORE";
        Console.WriteLine(randChoice);

        // Action on env_step number 0, 3, 6, 9, etc = 0
        // Action on env_step number 1, 4, 7, 10, etc = 1
        // Action on env_step number 2, 5, 8, 11, etc = 2

        Globals.action_t = this.envActionsPerformed % 3;

        // QPoint Execute action_t

        int pfmeid = state.emitterID;

        if (Globals.action_t == 0)
        {
            if (PFM.emitterIDTable[pfmeid].initAgeOut < PFM.emitterIDTable[pfmeid].ceilingAgeOut)
            {
                PFM.emitterIDTable[pfmeid].initAgeOut++;
                Console.WriteLine("Action Performed: ++");
            }
        }
        else if (Globals.action_t == 1)
        {
            //do nothing
            Console.WriteLine("Action Performed: Nothing");
        }
        else if (Globals.action_t == 2)
        {
            if (PFM.emitterIDTable[pfmeid].initAgeOut > PFM.emitterIDTable[pfmeid].floorAgeOut)
            {
                PFM.emitterIDTable[pfmeid].initAgeOut--;
                Console.WriteLine("Action Performed: --");
            }

        }
        this.envActionsPerformed++;
    }


    public void env_step_2()
    {
        // Explore or Exploit chosen without seeded random
        // Equal probability for explore and exploit

        QState state = prevState_at_env_step_begin;

        string randChoice = String.Empty;
        Random exploration_random = new Random();
        double epsi = exploration_random.NextDouble();

        if (epsi < 0.5)
        {
            randChoice = "EXPLORE";
            Console.WriteLine(randChoice);

            Globals.action_t = Globals.randomNumberGenerator.Next(Globals.qLearner.actionSpaceCount);
        }
        else
        {
            randChoice = "EXPLOIT";
            Console.WriteLine(randChoice);
            double maxQsa = 0.0;
            int maxaction = 0;



            for (int action_j = 0; action_j < Globals.qLearner.actionSpaceCount;
                action_j++)
            {
                double qsa = Globals.qLearner.QsaGet(state, action_j);
                if (qsa > maxQsa)
                {
                    maxQsa = qsa;
                    maxaction = action_j;
                }
            }
            Globals.action_t = maxaction;
        }

        // QPoint Execute action_t

        int pfmeid = state.emitterID;

        if (Globals.action_t == 0)
        {
            PFM.emitterIDTable[pfmeid].initAgeOut++;
            Console.WriteLine("Action Performed: ++");
        }
        else if (Globals.action_t == 1)
        {
            //do nothing
            Console.WriteLine("Action Performed: Nothing");
        }
        else if (Globals.action_t == 2)
        {
            PFM.emitterIDTable[pfmeid].initAgeOut--;
            Console.WriteLine("Action Performed: --");
        }

    }


    public void env_step()
    {

        QState state = prevState_at_env_step_begin; // Store state before env_step() is performed

        //foreach (QState state in Globals.qLearner.qstates)
        {
            
            //QState state_t = GetState(RxBuf, emitterTrackFile);
            //int stateIndex = Globals.qLearner.QsaMatch(state_t); 

            string randChoice = String.Empty;
            double epsi = Globals.randomNumberGenerator.NextDouble();

            if (epsi < Globals.qLearner.EXPLORE_PROBABILITY)
            {
                // If epsilon is less than threshold value, explore, else exploit
                randChoice = "EXPLORE";
                Console.WriteLine(randChoice);

                Globals.action_t = Globals.randomNumberGenerator.Next(Globals.qLearner.actionSpaceCount);
            }
            else
            {
                randChoice = "EXPLOIT";
                Console.WriteLine(randChoice);
                double maxQsa = 0.0;
                int maxaction = 0;



                for (int action_j = 0; action_j < Globals.qLearner.actionSpaceCount;
                    action_j++)
                {


                    //int stateIndex = Globals.qLearner.QsaMatch(stateT);

                    //if (state.restoreClass > 1)
                    //{
                    //    state.restoreClass = 1;
                    //}

                    double qsa = Globals.qLearner.QsaGet(state, action_j);
                    if (qsa > maxQsa)
                    {
                        // Choose action based on maximum value of Qsa from current state
                        maxQsa = qsa;
                        maxaction = action_j;
                    }
                }

                Globals.action_t = maxaction;
            }

            // QPoint Execute action_t

            int pfmeid = state.emitterID; // Extract state of emitter

            if (Globals.action_t == 0)
            {
                if (PFM.emitterIDTable[pfmeid].initAgeOut < PFM.emitterIDTable[pfmeid].ceilingAgeOut)
                {
                    // Only perform action if current ageOut is less than ceiling ageOut
                    PFM.emitterIDTable[pfmeid].initAgeOut++;
                    Console.WriteLine("Action Performed: ++");
                }
            }
            else if (Globals.action_t == 1)
            {
                //do nothing
                Console.WriteLine("Action Performed: Nothing");
            }
            else if (Globals.action_t == 2)
            {
                if (PFM.emitterIDTable[pfmeid].initAgeOut > PFM.emitterIDTable[pfmeid].floorAgeOut)
                {
                    // Only perform action if current ageOut is greater than floorAgeOut
                    PFM.emitterIDTable[pfmeid].initAgeOut--;
                    Console.WriteLine("Action Performed: --");
                }
            }

        }

    }

    public override void OnTick()
    {
        //QPoint Beginning -select action for QsaMat

        if (Globals.episodeIsDone)
        {
            // Reset emitter record list and track file at the end of each episode.
            this.emRecordList.Clear();
            this.emitterTrackFile.Clear();
            Globals.gTrackID = 0;
        }

        double epsi = Globals.randomNumberGenerator.NextDouble();
        string randChoice = string.Empty;

        if (PFM.emitterIDTable[1].restoreCount > 3)
        {
            PFM.emitterIDTable[1].restoreCount = 1;
        }
        prevQState = new QState(1, PFM.emitterIDTable[1].restoreCount / 3); // Since the second argument here must be either
        // 0 or 1, we reset the restoreCount if it becomes greater than 3.
        // Note: I think it should be reset to 3 in the above If statement, but it needs some confirmation
        // by experimentation


        // env.step(action_t) => state_(t+1)


        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            // Set received to false at the beginning of each ontick, only set it to true when a emitter record is being received.
            etr.received = false;
        }

        // Add RxBuf to emitter record list.
        this.emRecordList.AddRange(RxBuf);


        foreach (EmitterRecord e in this.emRecordList)
        {
            EmitterRecord emitterRecord = e;
            PFMEmitterRecord emitterID = Identify(emitterRecord, PFM.emitterIDTable); // Identify emitter from PFM table
            if (emitterID != null)
            {
                // If emitter exists in PFM table
                emitterRecord.eID = emitterID.eID;
                string idenfitier = "" + (char)(emitterRecord.erID + 64);
                emitterRecord.erIdentifier = idenfitier;
            }
            ManageTracks(emitterRecord, emitterID, emitterTrackFile); // Check if emitter already exists in emitter track file

            if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
            {
                Console.WriteLine($"Emitter Record:\n\t\tPRI: {emitterRecord.pri} mcs\n\t\tPulse Width: {emitterRecord.pw} ns\n\t\tFrequency: {emitterRecord.freq} MHz\n");
            }


        }

        List<EmitterTrackRecord> tempETF = new List<EmitterTrackRecord>(); // Create temporary ETF while tracks are being managed


        // Logic for emitter track file
        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            etr.currentTrackLength = Globals.Tick - etr.entryTick;
            if (etr.received)
            {
                // If emitter track record is current being received
                trackLossCount = 0;
                tempETF.Add(etr); // Add record to temporary ETF
                etr.Record("+"); // For output file. Print the symbol "+" when an emitter record is being received.
                if (etr.recordLost)
                {
                    // If an emitter track record was previously lost and is now restored
                    // Increment AgeOutRestore and set recordLost to false
                    Console.WriteLine("Record Restored");
                    etr.AgeOutRestore++;
                    etr.recordLost = false;
                }
            }
            else
            {
                // On Delete

                // If no emitter record is being received on the current Tick
                // Increment trackLossCount
                trackLossCount++;

                if (etr.ageIn > 0)
                {
                    // For currently not implemented AgeIn logic
                    // If a received emitter track record was lost before AgeIn was completed, the record
                    // will not be considered valid, as the RWR must wait for AgeIn duration before validating
                    // the emitter record.

                    // Since Emitter is not Legal yet it will be immediately deleted

                    etr.valid = false;
                    etr.status = TrackState.ETF_iDELETE;
                    Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                            "NOT RCV ", etr.status, "AGEIN 0", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                    // not copying to tempETF is deleting etr.valid = false;
                }
                else
                {

                    // When emitter track record is lost and AgeIn is completed (note that AgeIn is not implemented currently)

                    etr.recordLost = true;
                    //etr.received = false;
                    if (etr.ageOut > 0)
                    {
                        // When aging out is not completed, i.e. ageOut > 0
                        // we begin aging out by decrementing ageOut value and incrementing the agingOutCount
                        Console.WriteLine("Record Lost");
                        etr.ageOut--;
                        etr.AgingOutCount++;

                        etr.valid = true;

                        tempETF.Add(etr);

                        // Console.WriteLine($" {Globals.Tick}\t\t{etr.trackID}\t\t{etr.ageIn}\t\t{etr.ageOut}\t\tNOT RCV\t\tAGING OUT");

                        Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                            "NOT RCV ", etr.status, "AGING OUT", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                    }
                    else
                    {
                        //QPoint - At Track Deletion
                        etr.exitTick = Globals.Tick;
                        //int trackLength = etr.exitTick - etr.entryTick;
                        etr.trackLength = etr.exitTick - etr.entryTick;


                        //Globals.qLearner.qstates


                        etr.status = TrackState.ETF_oDELETE;

                        // The below If statements are for determining which env_step() function to use
                        // This is chosen in the Globals class

                        if (Globals.envStepType == 0)
                        {
                            env_step();
                        }
                        else if (Globals.envStepType == 1)
                        {
                            env_step_1();
                        }

                        int restoreClass = 0;

                        PFM.emitterIDTable[etr.eid].restoreCount = etr.AgeOutRestore;

                        if (PFM.emitterIDTable[etr.eid].restoreCount >= 6)
                            restoreClass = 1;

                        prevState_at_env_step_begin = currentState_at_env_step_begin; // Set previous state to current state
                        currentState_at_env_step_begin = new QState(1, restoreClass); // Set current state based on calculated restoreClass


                        // Creating previousQSA table to calculate Delta QSA
                        foreach (QState state in Globals.qLearner.Qsa.Keys)
                        {
                            foreach (double actionVal in Globals.qLearner.Qsa[state].ToList())
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    Globals.qLearner.previousQSA[state][i] = Globals.qLearner.Qsa[state][i];
                                }
                            }
                        }


                        // Update Qsa Table using states, action and reward
                        if (etr.AgingOutCount != 0)
                        {
                            Globals.qLearner.Qsa_cap(prevState_at_env_step_begin,
                                                     Globals.action_t,
                                                     currentState_at_env_step_begin,
                                                     etr.trackLength / etr.AgingOutCount);
                        }
                        else
                        {
                            Globals.qLearner.Qsa_cap(prevState_at_env_step_begin,
                                                     Globals.action_t,
                                                     currentState_at_env_step_begin,
                                                     0);
                        }

                        // After updating Q-table, store the values in currentQSA
                        foreach (QState state in Globals.qLearner.Qsa.Keys)
                        {
                            foreach (double actionVal in Globals.qLearner.Qsa[state].ToList())
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    Globals.qLearner.currentQSA[state][i] = Globals.qLearner.Qsa[state][i];
                                }
                            }
                        }


                        // Find the element-wise difference between previous and current Q-table to find Delta QSA
                        foreach (QState state in Globals.qLearner.Qsa.Keys)
                        {
                            foreach (double actionVal in Globals.qLearner.Qsa[state].ToList())
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    if (Globals.qLearner.currentQSA[state][i] - Globals.qLearner.previousQSA[state][i] != 0)
                                    {
                                        Globals.qLearner.deltaQSA[state][i] = Globals.qLearner.currentQSA[state][i] - Globals.qLearner.previousQSA[state][i];
                                    }
                                }
                            }
                        }


                        // Printing all Q-tables to console
                        Console.WriteLine("Current Qsa table: \n");
                        foreach (var Q in Globals.qLearner.Qsa)
                        {
                            foreach (var qsa in Q.Value)
                            {
                                Console.Write($"  {qsa}  ");
                            }
                            Console.WriteLine();
                        }

                        Console.WriteLine("\nPrevious Qsa table: \n");
                        foreach (var Q in Globals.qLearner.previousQSA)
                        {
                            foreach (var qsa in Q.Value)
                            {
                                Console.Write($"  {qsa}  ");
                            }
                            Console.WriteLine();
                        }

                        Console.WriteLine("\nDelta Qsa table: \n");
                        foreach (var Q in Globals.qLearner.deltaQSA)
                        {
                            foreach (var qsa in Q.Value)
                            {
                                Console.Write($"  {qsa}  ");
                            }
                            Console.WriteLine();
                        }


                        Globals.recordedList.Add(
                           new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                           "NOT RCV ", etr.status, "AGEOUT 0", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                        etr.Record("-");
                    }
                }
            }

            // Console output for debugging
            Console.WriteLine($"Global tick: {Globals.persistentTick}");
            Console.WriteLine($"TrackID: {etr.trackID}");
            Console.WriteLine($"EmitterID: {etr.eid}");
            Console.WriteLine($"Ageout: {etr.ageOut}");
            Console.WriteLine($"Init AgeOut: {etr.baseAgeOut}");
            //Console.WriteLine($"Entry: {etr.entryTick}, Exit: {etr.exitTick}");
            //Console.WriteLine($"Current Track Length: {etr.currentTrackLength}");
            int rClass;
            if (etr.AgeOutRestore >= 6)
            {
                rClass = 1;
            }
            else
            {
                rClass = 0;
            }
            Console.WriteLine($"RestoreClass {rClass}");

            StreamWriter qsaWriter = new StreamWriter(Globals.qsaTableFileName, true);

            foreach (var Q in Globals.qLearner.Qsa)
            {
                foreach (var qsa in Q.Value)
                {
                    qsaWriter.Write($"  {qsa}  ");

                }
                if (etr.AgingOutCount != 0)
                {
                    qsaWriter.Write($",Tick: {Globals.persistentTick} ,RestoreCount: {etr.AgeOutRestore}");
                }
                qsaWriter.WriteLine("");
            }
            qsaWriter.WriteLine("");
            qsaWriter.Close();
        }


        StreamWriter writer = new StreamWriter(Globals.trackRecFileName, true);
        foreach (EmitterRecord er in RxBuf)
        {
            writer.WriteLine($"{Globals.Tick},EREC,{er.erID},{er.azimuth}," +
                $"{er.amplitudes[0]}," +
                $"{er.freq},{er.pri}");
        }

        // Some of the lines of code below are for Q-learning computations based on frequency window
        // This will come into play later, but it might not have the same logic or structure that is currently
        // used here. Keep in mind that eventually, we need to apply Q-learning to optimize frequency windows


        double currentFitness = 0.0;

        List<EmitterTrackRecord> ETFState = emitterTrackFile;

        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            if (etr.exitTick > -1)
            {
                currentFitness = 0.0;
                Globals.qLearner.count++;
                int trackLength = etr.exitTick - etr.entryTick;

                double freqRange = Math.Abs(etr.freqMax - etr.freqMin);
                if (freqRange == 0.0)
                {
                    freqRange = -1.0;
                }
                //double temp = freqRange + etr.AgingInCount + etr.AgingOutCount;

                // The lines of code below are relevant to our current problem of optimizing AgeOut

                double temp = etr.AgingOutCount;

                if (temp != 0)
                {
                    //currentFitness = trackLength / temp;
                    currentFitness = etr.restoresPerTrackLength;
                }

                //else
                //{
                //    Debug.Assert(false);
                //    currentFitness = 0.0;
                //}

                Globals.qLearner.runningSum += currentFitness;
                Globals.qLearner.runningAverage = Globals.qLearner.runningSum / Globals.qLearner.count;

                if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
                {
                    Console.WriteLine($"Tick\tETR\ttrackID\t" +
                      $"freqCurrent\tfreqMin\tfreqMax\t" +
                      $"freqTrackWindow\tAgingInCount\tAgingOutCount\tentryTick\tTrackLength\t" +
                      $"currentFitness");

                    Console.WriteLine($"{Globals.Tick}\tETR\t{etr.trackID}\t" +
                      $"{etr.freqCurrent}\t\t{etr.freqMin}\t{etr.freqMax}\t" +
                      $"{etr.freqTrackWindow.ToString("N2")}\t\t{etr.AgingInCount}\t\t{etr.AgingOutCount}\t\t{etr.entryTick}\t\t{trackLength}\t\t" +
                      $"{currentFitness}");
                }
                ////  if (Console.KeyAvailable)
                //Globals.ExecuteShell();

                writer.WriteLine($"{Globals.Tick},ETR,{etr.trackID},{etr.azimuthCurrent}," +
              $"{etr.amplitude},{etr.freqCurrent},{etr.freqMin},{etr.freqMax}," +
              $"{etr.freqTrackWindow},{etr.priCurrent},{etr.AgingInCount},{etr.AgingOutCount},{etr.entryTick},{etr.exitTick}," +
              $"{currentFitness}");
            }
            else
            {
                writer.WriteLine($"{Globals.Tick},ETR,{etr.trackID},{etr.azimuthCurrent}," +
                   $"{etr.amplitude},{etr.freqCurrent},{etr.freqMin},{etr.freqMax}," +
                   $"{etr.freqTrackWindow},{etr.priCurrent},{etr.AgingInCount},{etr.AgingOutCount},{etr.entryTick},{etr.exitTick}");
            }
        }
        if (RxBuf.Count > 0)
            writer.WriteLine($"{Globals.Tick},SUMMARY,{Globals.qLearner.runningAverage}");

        writer.Close();
        
        // Following track management and applying Q-learning algorithm, we set the TempETF to our
        // RWR's actual emitter track file.

        emitterTrackFile = tempETF;


        if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
        {
            Console.WriteLine($"Tick-State\tEREC-freq\tETR-min-max\tChoice-Type\tAction\t\tNoOfState\tCurFitness\tRunningAverage");
            Console.WriteLine($"{Globals.Tick}\t\t{randChoice}\t\t{Globals.action_t}\t\t{Globals.qLearner.qstates.Count}\t\t{currentFitness}\t\t{Globals.qLearner.runningAverage}");
        }

        if (Console.KeyAvailable)
        {
            Console.ReadLine();
            Globals.ExecuteShell();
        }

        this.emRecordList.Clear();
        Globals.emitterTrackFile = emitterTrackFile;

        //Console.WriteLine($"Previous Q-state: {prevState_at_env_step_begin.emitterID}, {prevState_at_env_step_begin.restoreClass}");
        //Console.WriteLine($"Current Q-state: {currentState_at_env_step_begin.emitterID}, {currentState_at_env_step_begin.restoreClass}");



        Console.WriteLine("-----------------------------------\n");


    }

    public EmitterTrackRecord UpdateTrackingWindows(EmitterTrackRecord etr)
    {

        // Meant for frequency window optimization

        //if(Globals.action_t == 0)
        //    etr.freqTrackWindow += etr.freqTrackWindow * 0.1;
        //if(Globals.action_t == 1)
        //    etr.freqTrackWindow -= etr.freqTrackWindow * 0.1;

        return etr;
    }

    public string restoreCount(int ageOutRestores)
    {
        if (ageOutRestores < 5)
        {
            return "LOW";
        }
        else
        {
            return "HIGH";
        }
    }


    public QState GetState(EmitterTrackRecord emitterTrackRecord)
    {

        // Since our state is currently based only on emitter ID and restore class,
        // this method does not do a whole lot except return an empty state.

        QState qState = new QState(0, 0);
        return qState;
    }

    public EmitterRecord BuildEmitterRecord(List<PDW> PDWs)
    {

        // This method takes a list of PDWs and converts them into an emitter record
        // However, since we are directly using emitter records in our program, rather than building them from PDWs,
        // this function is not being used

        int totalPulses = PDWs.Count;
        int dwellTime = PDWs.Last().timeOfArrival - PDWs[0].timeOfArrival;
        int PRI = (int)(dwellTime / (totalPulses - 1));
        int freqTotal = 0;
        int pwTotal = 0;
        int avgFreq;
        int avgPW;
        foreach (PDW pdw in PDWs)
        {
            freqTotal += pdw.frequency;
            pwTotal += pdw.pulseWidth;
        }
        avgFreq = freqTotal / totalPulses;
        avgPW = pwTotal / totalPulses;

        EmitterRecord emRecord = new EmitterRecord();
        emRecord.erID = Globals.guID++;
        emRecord.eID = -1;
        emRecord.erIdentifier = "Emitter1";
        emRecord.freq = avgFreq;
        emRecord.pri = PRI;
        emRecord.pw = avgPW;

        return emRecord;
    }

    public PFMEmitterRecord Identify(EmitterRecord emitterRecord, List<PFMEmitterRecord> eIDTable)
    {

        // This method matches a received emitter record with the list of emitters already
        // present in PFM table. If a match is found, it returns the emitter ID of the matched emitter
        // Attributes compared: PRI and Frequency

        foreach (PFMEmitterRecord eID in eIDTable)
        {
            bool match = true;

            if (emitterRecord.pri < eID.priMin || emitterRecord.pri > eID.priMax)
            {
                match = false;
            }

            if (emitterRecord.freq < eID.freqMin || emitterRecord.freq > eID.freqMax)
            {
                match = false;
            }

            if (match)
            {
                return eID;
            }
        }
        return null;
    }

    public bool IsMatch(EmitterRecord emitterRecord, EmitterTrackRecord emitterTrackRecord)
    {

        // This method checks if the emitter record being received is already in the emitter track file
        // or if it ia new emitter found in the field.

        bool match = true;

        if (emitterRecord.pri < emitterTrackRecord.priCurrent - emitterTrackRecord.priTrackWindow ||
            emitterRecord.pri > emitterTrackRecord.priCurrent + emitterTrackRecord.priTrackWindow)
        {
            match = false;
        }

        if (emitterRecord.freq < emitterTrackRecord.freqCurrent - emitterTrackRecord.freqTrackWindow ||
            emitterRecord.freq > emitterTrackRecord.freqCurrent + emitterTrackRecord.freqTrackWindow)
        {
            match = false;
        }

        return match;
    }

    public void ManageTracks(EmitterRecord emitterRecord, PFMEmitterRecord emitterID, List<EmitterTrackRecord> emitterTrackFile)
    {
        bool foundInETF = false;

        // This method compares the received emitter record to the emitter track file
        // It checks if it is a consistent or repeated appearance of an emitter that has already been added
        // to the emitter track file, or if it a new emitter that matches characteristics of emitters
        // present in the PFM table, or if it is an unknown emitter altogether.

        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            if (etr.eid != -1)
            {

                // If emitter track record is valid

                if (IsMatch(emitterRecord, etr))
                {

                    // If received emitter record matches an emitter already in the track file
                    // Update the emitter track file

                    foundInETF = true;
                    etr.received = true;
                    etr.erID = emitterRecord.erID;

                    etr.status = TrackState.ETF_UPDATED;
                    etr.priCurrent = emitterRecord.pri;
                    etr.pwCurrent = emitterRecord.pw;
                    etr.freqCurrent = emitterRecord.freq;

                    if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
                    {
                        Console.WriteLine($"Match found in tracks:\n\t\tID: {etr.eid}\n\t\tFrequency: {etr.freqCurrent} MHz\n\t\t" +
                  $"PRI: {etr.priCurrent} mcs\n\t\tPulse Width: {etr.pwCurrent} ns");
                    }


                    //OnUpdate

                    etr.ageOut = emitterID.initAgeOut;
                    etr.baseAgeOut = emitterID.initAgeOut;


                    if (etr.ageIn > 0)
                    {
                        etr.ageIn--;
                        etr.valid = false;
                        etr.status = TrackState.ETF_iDELETE;
                        etr.AgingInCount++;

                        Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                            "RECEIVED", etr.status, "AGING IN", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                    }
                    else
                    {
                        etr.valid = true;

                        etr.UpdateCount++;
                        Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                            "RECEIVED", etr.status, "STEADY", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                    }
                }
            }
        }

        if (!foundInETF && emitterID != null)
        {

            // If emitter record is valid but is not in the emitter track file
            // add the emitter record to the emitter track file

            EmitterTrackRecord etr = new EmitterTrackRecord();

            etr.status = TrackState.ETF_INSERTED;
            etr.trackID = Globals.gTrackID++;
            etr.eid = emitterRecord.eID;
            etr.erID = emitterRecord.erID;

            etr.entryTick = Globals.Tick;
            etr.priCurrent = emitterRecord.pri;
            etr.priMin = emitterRecord.pri;
            etr.priMax = emitterRecord.pri;
            etr.priTrackWindow = emitterID.priTrackWindow;

            etr.freqCurrent = emitterRecord.freq;
            etr.freqMin = emitterRecord.freq;
            etr.freqMax = emitterRecord.freq;
            etr.freqTrackWindow = emitterID.freqTrackWindow;

            etr.pwCurrent = emitterRecord.pw;
            etr.pwMin = emitterRecord.pw;
            etr.pwMax = emitterRecord.pw;
            etr.pwTrackWindow = emitterID.pwTrackWindow;

            etr.azimuthTrackWindow = emitterID.azimuthTrackWindow;
            etr.ageIn = emitterID.ageIn;
            // etr.ageOut = emitterID.ageOut;
            //etr.baseAgeOut = QLearner.GetAgeOut(etr.eid);

            etr.ageOut = emitterID.initAgeOut;
            etr.ceilingAgeOut = emitterID.ceilingAgeOut;
            etr.floorAgeOut = emitterID.floorAgeOut;
            etr.baseAgeOut = emitterID.initAgeOut;

            //On Insert

            // etr.AgingInCount++;
            etr.received = true;
            Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                            "RECEIVED", etr.status, "AGING IN", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
            emitterTrackFile.Add(etr);


            if (etr.trackID != 0)
            {
                Console.WriteLine($"trackLossCount = {trackLossCount}");
                rewardValue = 99 - trackLossCount;
            }

        }

        return;
    }

}

public class EmitterTrackRecord
{
    public int trackID;
    public bool received;
    public int eid = -1;
    public int erID;

    public int entryTick;
    public int exitTick = -1;
    public int baseAgeOut;

    public int AgingOutCount;
    public int AgingInCount;
    public int UpdateCount;

    public int AgeOutDecrement;
    public int AgeOutRestore;

    public bool valid;

    public double amplitude;

    public double priCurrent;
    public double priMin;
    public double priMax;
    public double priTrackWindow;

    public double freqCurrent;
    public double freqMin;
    public double freqMax;
    public double freqTrackWindow;

    public double pwCurrent;
    public double pwMin;
    public double pwMax;
    public double pwTrackWindow;


    public double azimuthCurrent;
    public double azimuthMin;
    public double azimuthMax;
    public double azimuthTrackWindow;

    public int ageIn;
    public int ageOut;
    public int ceilingAgeOut;
    public int floorAgeOut;

    public int trackLength;
    public double restoresPerTrackLength;
    public int currentTrackLength;
    public bool recordLost;
    // Testing if such an attribute is a better reward metric
    //public int zeroAgeOutDuration = 0;

    public TrackState status = TrackState.ETF_NOT_DEFINED;

    public string identifier;
    public int ecmTechnique;

    public void Record(string label)
    {
        StreamWriter sw = new StreamWriter(Globals.recFileName, true);
        sw.WriteLine($"{label}, {received}, {trackID}, {identifier}, {priCurrent}, {freqCurrent}, {pwCurrent}, {Globals.Tick}");
        sw.Close();
    }
}

public class EmitterRecord
{
    public string erIdentifier;
    public int erID;
    public int eID;
    public int ecmTechnique;

    public double pri;
    public double freq;
    public double pw;

    public double azimuth;
    public int distance;
    public double[] amplitudes = new double[4];
    public int maxRange;
}

public class PFMEmitterRecord
{
    public string erIdentifier;
    public int eID;

    public double priMin;
    public double priMax;
    public double priTrackWindow;

    public double freqMin;
    public double freqMax;
    public double freqTrackWindow;

    public double pwMin;
    public double pwMax;
    public double pwTrackWindow;

    public double azimuthTrackWindow;
    public int ageIn;
    public int ageOut;

    public int ageOutMin;
    public int ageOutMax;

    public int floorAgeOut;
    public int ceilingAgeOut;
    public int initAgeOut;

    public int restoreCount;

    //public ageInMin;
    //public ageInMax;

    public PFMEmitterRecord(int eID, string erIdentifier, double priMin, 
                            double priMax, double priTrackWindow, double freqMin,
                            double freqMax, double freqTrackWindow, double pwMin, 
                            double pwMax, double pwTrackWindow, int ageOutMax, 
                            int ceilingAgeOut, int restoreCount=0)
    {
        this.erIdentifier = erIdentifier;
        this.eID = eID;
        this.priMin = priMin;
        this.priMax = priMax;
        this.priTrackWindow = priTrackWindow;
        this.freqMin = freqMin;
        this.freqMax = freqMax;
        this.freqTrackWindow = freqTrackWindow;
        this.pwMin = pwMin;
        this.pwMax = pwMax;
        this.pwTrackWindow = pwTrackWindow;
        this.ageOutMax = ageOutMax;
        //this.ageOut = ageOutMax;
        this.initAgeOut = ageOutMax;
        this.ageOut = initAgeOut;
        this.ceilingAgeOut = ceilingAgeOut;
        this.restoreCount = restoreCount;
        this.floorAgeOut = 0;
    }
}

public class PFM
{
    public static List<PFMEmitterRecord> emitterIDTable = new List<PFMEmitterRecord>();

    public void FormPFMTable()
    {
        StreamReader reader = new StreamReader("Input.csv", true);
        string S = reader.ReadLine();
        while (S != null)
        {
            string[] data = S.Split(',');

            if (data[0] == "ER")
            {
                PFMEmitterRecord eID = GetEmitterID(data, ref reader);
                emitterIDTable.Add(eID);
            }
            S = reader.ReadLine();
        }
    }

    public PFMEmitterRecord GetEmitterID(string[] data, ref StreamReader reader)
    {
        reader.ReadLine();
        reader.ReadLine();
        string S = reader.ReadLine(); ;
        string[] data1 = S.Split(',');

        PFMEmitterRecord emitterID = new PFMEmitterRecord(Convert.ToInt32(data[1]), "E" + data[1], Convert.ToInt32(data1[3]), Convert.ToInt32(data1[4]),
            Convert.ToInt32(data[3]), Convert.ToInt32(data1[7]), Convert.ToInt32(data1[8]), Convert.ToInt32(data[2]),
            Convert.ToInt32(data1[5]), Convert.ToInt32(data1[6]), Convert.ToInt32(data[4]), 5, 5);

        return emitterID;
    }

}

public class PDW
{
    public int pulseWidth;
    public double[] amplitude = new double[4];
    public int frequency;
    public int timeOfArrival;

    public PDW(int pulseWidth, double[] amplitude, int frequency, int timeOfArrival)
    {
        this.pulseWidth = pulseWidth;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.timeOfArrival = timeOfArrival;
    }
}

public class RadarSignature
{
    public int r;
    public double theta;
    public string symbol;
    public int maxRange;
    public bool rangeFound = false;

    public RadarSignature(int r, double theta, int maxRange, string symbol)
    {
        this.r = r;
        this.theta = theta;
        this.symbol = symbol;
        this.maxRange = maxRange;
    }
}
