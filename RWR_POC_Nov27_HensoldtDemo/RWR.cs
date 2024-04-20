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

        QState state = prevState_at_env_step_begin;
        //foreach (QState state in Globals.qLearner.qstates)
        {
            
            //QState state_t = GetState(RxBuf, emitterTrackFile);
            //int stateIndex = Globals.qLearner.QsaMatch(state_t); 
            string randChoice = String.Empty;
            double epsi = Globals.randomNumberGenerator.NextDouble(); ;

            if (epsi < Globals.qLearner.EXPLORE_PROBABILITY)
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


                    //int stateIndex = Globals.qLearner.QsaMatch(stateT);

                    //if (state.restoreClass > 1)
                    //{
                    //    state.restoreClass = 1;
                    //}

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
        prevQState = new QState(1, PFM.emitterIDTable[1].restoreCount / 3);


        // env.step(action_t) => state_(t+1)


        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            // Set received to false, only set it to true when a emitter record is being received.
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

        List<EmitterTrackRecord> tempETF = new List<EmitterTrackRecord>();

        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            etr.currentTrackLength = Globals.Tick - etr.entryTick;
            if (etr.received)
            {
                trackLossCount = 0;
                tempETF.Add(etr);
                etr.Record("+");
                if (etr.recordLost)
                {
                    Console.WriteLine("Record Restored");
                    etr.AgeOutRestore++;
                    etr.recordLost = false;
                }
            }
            else
            {
                // On Delete
                trackLossCount++;

                if (etr.ageIn > 0)
                {
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
                    etr.recordLost = true;
                    //etr.received = false;
                    if (etr.ageOut > 0)
                    {
                        Console.WriteLine("Record Lost");
                        etr.ageOut--;
                        etr.AgingOutCount++;
                        etr.AgeOutDecrement++;

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

                        //double reward = trackLength / etr.AgingOutCount;
                        //Globals.qLearner.Qsa_cap(state_t, action_t, new_state_t, trackLength);
                        //double reward = trackLength / Math.Abs(etr.freqMax - etr.freqMin); AgeOutCount in denominator
                        //Globals.qLearner.Qsa_cap(new QState(), 0, null, reward);

                        string restore = restoreCount(etr.AgeOutRestore);
                        //Globals.qLearner.Qsa.Add(etr.eid, restore);
                        etr.status = TrackState.ETF_oDELETE;

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

                        prevState_at_env_step_begin = currentState_at_env_step_begin;
                        currentState_at_env_step_begin = new QState(1, restoreClass);

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

                        // Find delta Qsa


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

                        //QState curQState = new QState(1, PFM.emitterIDTable[1].restoreCount/3);

                        // Issue - Agent always chooses the first updated QSA during exploit since that state-action pair will
                        // always have the highest value.


                        // Store previous Qsa

                        // Store Qsa value after applying Qsa_cap



                        Globals.recordedList.Add(
                           new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                           "NOT RCV ", etr.status, "AGEOUT 0", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                        etr.Record("-");

                        // In PFM table, for this emitter ID, increase or decrease AgeOut
                    }
                }
            }

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




        // Issue - Assertion Failed bug
        StreamWriter writer = new StreamWriter(Globals.trackRecFileName, true);
        foreach (EmitterRecord er in RxBuf)
        {
            writer.WriteLine($"{Globals.Tick},EREC,{er.erID},{er.azimuth}," +
                $"{er.amplitudes[0]}," +
                $"{er.freq},{er.pri}");
        }
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

                // QPoint

                //QState qState_next = GetState(RxBuf, ETFState);

                //Globals.qLearner.Qsa_cap(qState_next, Globals.qLearner.runningAverage);

                //Console.WriteLine($"Tick-State\tEREC-freq\tETR-min-max\tChoice-Type\tAction\t\tNoOfState\tCurFitness\tRunningAverage");
                //Console.WriteLine($"{Globals.Tick}\t\t{state_t.freq}\t\t{state_t.maxWindow}\t\t{randChoice}\t\t{Globals.action_t}\t\t{Globals.qLearner.qstates.Count}\t\t{currentFitness}\t\t{Globals.qLearner.runningAverage}");

                //Console.WriteLine("QsaTable");
                //Console.WriteLine("State\t\tAction-0\tAction-1\tAction-2");
                //foreach (List<double> actionList in Globals.qLearner.Qsa)
                //{
                //    Console.WriteLine($"{Globals.qLearner.Qsa.IndexOf(actionList)}\t\t{actionList[0]}\t\t{actionList[1]}\t\t{actionList[2]}");
                //}

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
        
        emitterTrackFile = tempETF;

        // QPoint

        //QState state_t1 = GetState(RxBuf, ETFState);

        //Globals.qLearner.Qsa_cap(state_t, Globals.action_t, state_t1, Globals.qLearner.runningAverage);

        if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
        {
            Console.WriteLine($"Tick-State\tEREC-freq\tETR-min-max\tChoice-Type\tAction\t\tNoOfState\tCurFitness\tRunningAverage");
            Console.WriteLine($"{Globals.Tick}\t\t{randChoice}\t\t{Globals.action_t}\t\t{Globals.qLearner.qstates.Count}\t\t{currentFitness}\t\t{Globals.qLearner.runningAverage}");
        }
        //Console.WriteLine("QsaTable");
        //Console.WriteLine("State\t\tAction-0\tAction-1\tAction-2");
        //foreach (List<double> actionList in Globals.qLearner.Qsa)
        //{
        //    Console.WriteLine($"{Globals.qLearner.Qsa.IndexOf(actionList)}\t\t{actionList[0]}\t\t{actionList[1]}\t\t{actionList[2]}");
        //}
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

    //public EmitterTrackRecord UpdateAgeInOutTrack(EmitterTrackRecord etr)
    //{
    //    if (Globals.action_t == 0)
    //    {
    //        etr.ageOut++;
    //    }
    //    else if (Globals.action_t == 1)
    //    {
    //        etr.ageOut--;
    //    }
    //    else if (Globals.action_t == 2)
    //    {
    //        etr.ageIn++;
    //    }
    //    else if (Globals.action_t == 3)
    //    {
    //        etr.ageIn--;
    //    }

    //    return etr;
    //}

    public QState GetState(EmitterTrackRecord emitterTrackRecord)
    {
        QState qState = new QState(0, 0);
        return qState;

        //qState.ageOutLength = emitterTrackRecord.AgingOutCount;
        //qState.restoreCount = emitterTrackRecord.AgeOutRestore;

        // Get AgeOut only for current record

        //foreach (EmitterRecord er in emitterRecords)
        //{
        //    freqs.Add(er.freq);    
        //}
        //if (freqs.Count > 0)
        //{
        //    qState.freq = freqs.Max() - freqs.Min();
        //    if(qState.freq == 0)
        //        qState.freq = -1;
        //}
        //else
        //    qState.freq = 0; 

        //foreach (EmitterTrackRecord etr in emitterTrackRecords)
        //{
        //    windows.Add(etr.freqMax - etr.freqMin);   
        //}
        //if (windows.Count > 0)
        //{
        //    qState.maxWindow = windows.Max();
        //    if (qState.maxWindow == 0)
        //        qState.maxWindow = -1;
        //}
        //else
        //    qState.maxWindow = 0;

        //foreach (EmitterTrackRecord etr in emitterTrackRecords)
        //{
        //    ageOuts.Add(etr.baseAgeOut);
        //}
        //if (ageOuts.Count > 0)
        //{
        //    qState.ageOutCount = ageOuts.Max();
        //    if (qState.ageOutCount == 0)
        //        qState.ageOutCount = -1;
        //}

        // return qState;
    }

    public EmitterRecord BuildEmitterRecord(List<PDW> PDWs)
    {
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

        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            if (etr.eid != -1)
            {
                if (IsMatch(emitterRecord, etr))
                {
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
                    //etr.ceilingAgeOut = emitterID.ceilingAgeOut;


                    if (etr.ageIn > 0)
                    {
                        etr.ageIn--;
                        etr.valid = false;
                        etr.status = TrackState.ETF_iDELETE;
                        etr.AgingInCount++;

                        //   Console.WriteLine($"{Globals.Tick}\t\tRECEIVED\t\t{etr.trackID}\t\tUPDATE\t\t" +
                        //  $"{etr.ageIn}\t\t{etr.ageOut}\t\tAGING INT");

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
