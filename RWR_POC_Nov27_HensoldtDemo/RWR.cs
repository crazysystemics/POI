using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Navigation;
using RWR_POC_GUI;

public class RWR : BattleSystem
{
    public override bool Stopped { get; set; }
    public int sensitivity;
    public int toaResolution;
    public int aperture;
    public double[] amps = new double[4];
    public bool receivingEmitterRecord = false;
    public bool recordLost = false;
    public EmitterRecord emRecord = new EmitterRecord();
    public List<EmitterRecord> emRecordList = new List<EmitterRecord>();
    public List<PDW> receivedPDW = new List<PDW>();
    public EmitterRecord receivedRecord = new EmitterRecord();
    public PFMEmitterRecord eID;
    public List<EmitterTrackRecord> emitterTrackFile = new List<EmitterTrackRecord>();
    public List<QState> prevQStates = new List<QState>();

    public RWR(ref Position positon, int id)
    {
        this.position = positon;
        this.id = id;
    }

    public List<EmitterRecord> RxBuf = new List<EmitterRecord>();
    public class Out : OutParameter
    {
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

        // Pulse as InParameter
        public double[] amps = new double[4];
        public EmitterRecord emRecord = new EmitterRecord();

        public In(EmitterRecord emRecord, int id) : base(id)
        {
            this.emRecord = emRecord;
        }

    }

    public override OutParameter Get()
    {
        Out rwrOutParams = new Out(0, 0, position, 2);
        foreach (EmitterRecord emitterRecord in RxBuf)
        {
            rwrOutParams.visibleRadars.Add(new RadarSignature(emitterRecord.distance, emitterRecord.azimuth, emitterRecord.maxRange, emitterRecord.erIdentifier));
        }
        return rwrOutParams;
    }

    public override void Set(List<InParameter> inParameters)
    {
        RxBuf.Clear();
        foreach (InParameter inParameter in inParameters)
        {
            if (((In)inParameter).emRecord.pw > 0)
            {
                this.RxBuf.Add(((In)inParameter).emRecord);
            }
        }
    }

    public void env_step()
    {
        QState qstate = new QState(etr.eid, restoreClass);
        QState prev_state = new QState(0, 0);
        foreach (QState qs in prevQStates)
        {
            if (qs == qstate)
            {
                prev_state = qs;
                break;
            }
        }
        prevQStates = new List<QState>();
        foreach (QState state in Globals.qLearner.qstates)
        {
            prevQStates.Add(new QState(state.emitterID, state.restoreClass));

            //QState state_t = GetState(RxBuf, emitterTrackFile);
            //int stateIndex = Globals.qLearner.QsaMatch(state_t); 
            // TODO: Issue 1 - StateIndex is always -1
            string randChoice = String.Empty;
            double epsi = new Random().NextDouble(); ;

            if (epsi < Globals.qLearner.EXPLORE_PROBABILITY)
            {
                randChoice = "EXPLORE";
                Random rand = new Random();
                Globals.action_t = rand.Next(Globals.qLearner.actionSpaceCount);
            }
            else
            {
                randChoice = "EXPLOIT";
                double maxQsa = 0.0;
                int maxaction = 0;

                

                for (int action_j = 0; action_j < Globals.qLearner.actionSpaceCount;
                    action_j++)
                {
                   

                    //int stateIndex = Globals.qLearner.QsaMatch(stateT);
                    double qsa = Globals.qLearner.QsaGet(state, action_j);
                    if (qsa > maxQsa)
                    {
                        maxQsa = qsa;
                        maxaction = action_j;
                    }
                }
                Globals.action_t = maxaction;

                //QPoint Execute action_t

                int pfmeid = state.emitterID;


                if (Globals.action_t == 0)
                {
                    PFM.emitterIDTable[pfmeid].ageOut++;
                }
                else if (Globals.action_t == 1)
                {
                    //do nothing
                    break;
                }
                else if (Globals.action_t == 2)
                {
                    PFM.emitterIDTable[pfmeid].ageOut--;
                    break;
                }
            }
        }

    }

    public override void OnTick()
    {
        //QPoint Beginning -select action for QsaMat

        Random random = new Random();
        double epsi = random.NextDouble();
        string randChoice = string.Empty;

        //Search current state in existing qStates
        //ETF empty in the beginning
        //GetState on each record in ETF using a loop

        //foreach (EmitterTrackRecord record in emitterTrackFile)
        //{
        //    if (emRecord.erID == record.erID)
        //    {
        //        state_t = GetState(record);
        //    }
        //}


        env_step();

        // TODO: Issue 2 - Below statement adds a "state" for every tick
        // causing a state to be present even if there is no record being received
        // For example, at tick 34, there are 34 "states" in this list.
        //if (stateIndex == -1 && emitterTrackFile.Count > 0 && !Globals.qLearner.qstates.Contains(state_t))
        //    Globals.qLearner.qstates.Add(state_t);



        //foreach (EmitterTrackRecord etr in emitterTrackFile)
        //{
        //    foreach (EmitterID emitter in PFM.emitterIDTable)
        //    {
        //        if (emitter.eID == etr.erID)
        //        {
        //            if (Globals.action_t == 0)
        //            {
        //                emitter.ageOutMax++;
        //            }
        //            else if (Globals.action_t == 1)
        //            {
        //                emitter.ageOutMax--;
        //            }
        //        }
        //    }
        //}

        //List<EmitterTrackRecord> updatedList = new List<EmitterTrackRecord>();
        //foreach (EmitterTrackRecord etr in emitterTrackFile)
        //{
        //    //updatedList.Add(UpdateTrackingWindows(etr));
        //    //updatedList.Add(UpdateAgeInOutTrack(etr));
        //}

        //emitterTrackFile = updatedList;

        //int pulseCount = 1;
        //foreach (PDW pdw in receivedPDW)
        //{
        //    Console.WriteLine($"Pulse {pulseCount}\nPulse Width: {pdw.pulseWidth} ns\n" +
        //                      $"Amplitudes: {pdw.amplitude[0]}, {pdw.amplitude[1]}, {pdw.amplitude[2]}, {pdw.amplitude[3]}\n" +
        //                      $"Frequency: {pdw.frequency} MHz\nTime of Arrival: {pdw.timeOfArrival} ns\n");
        //    pulseCount++;
        //}


        // env.step(action_t) => state_(t+1)

        //QState new_state_t = Globals.qLearner.QsaStep(state_t, Globals.action_t);

        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            etr.received = false;
        }

        this.emRecordList.AddRange(RxBuf);


        foreach (EmitterRecord e in this.emRecordList)
        {
            EmitterRecord emitterRecord = e;
            PFMEmitterRecord emitterID = Identify(emitterRecord, PFM.emitterIDTable);
            if (emitterID != null)
            {
                emitterRecord.eID = emitterID.eID;
                string idenfitier = "" + (char)(emitterRecord.erID + 64);
                emitterRecord.erIdentifier = idenfitier;
            }
            ManageTracks(emitterRecord, emitterID, emitterTrackFile);

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
                    if (etr.ageOut > 0 && etr.AgingOutCount <= etr.ceilingAgeOut)
                    {
                        Console.WriteLine("Record Lost");
                        etr.ageOut--;
                        etr.AgingOutCount++;
                        etr.AgeOutDecrement++;
                        Console.WriteLine($"Ageout: {etr.ageOut}");

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

                        //double reward = trackLength / etr.AgingOutCount;
                        //Globals.qLearner.Qsa_cap(state_t, action_t, new_state_t, trackLength);
                        //double reward = trackLength / Math.Abs(etr.freqMax - etr.freqMin); AgeOutCount in denominator
                        //Globals.qLearner.Qsa_cap(new QState(), 0, null, reward);

                        
                        string restore = restoreCount(etr.AgeOutRestore);
                        //Globals.qLearner.Qsa.Add(etr.eid, restore);
                        etr.status = TrackState.ETF_oDELETE;

                        int restoreClass = 0;
                        if (etr.AgeOutRestore > 3)
                            restoreClass = 1;

                        
                        



                        if (etr.AgingOutCount != 0)
                        {
                            Globals.qLearner.Qsa_cap(prev_state, 
                                                     Globals.action_t, 
                                                     qstate, 
                                                     etr.trackLength / etr.AgingOutCount);
                        }
                        else
                        {
                            Globals.qLearner.Qsa_cap(prev_state, 
                                                     Globals.action_t, qstate, 
                                                     0);
                        }


                        Globals.recordedList.Add(
                           new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                           "NOT RCV ", etr.status, "AGEOUT 0", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                        etr.Record("-");

                        // In PFM table, for this emitter ID, increase or decrease AgeOut
                    }
                }
            }

            Console.WriteLine($"TrackID: {etr.trackID}");
            Console.WriteLine($"Entry: {etr.entryTick}, Exit: {etr.exitTick}");
            Console.WriteLine($"Current Track Length: {etr.currentTrackLength}");
            Console.WriteLine($"AgeOutRestore (Number of times record in current track was lost, then restored) {etr.AgeOutRestore}");


            Console.WriteLine($"Track Length / N(restores) = {etr.restoresPerTrackLength}");
        }

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
                else
                {
                    Debug.Assert(false);
                    currentFitness = 0.0;
                }
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
        QState qState = new QState(0,0);
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

            etr.received = true;

            //On Insert

            // etr.AgingInCount++;
            etr.received = true;
            Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                            "RECEIVED", etr.status, "AGING IN", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
            emitterTrackFile.Add(etr);
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

    //public ageInMin;
    //public ageInMax;

    public PFMEmitterRecord(int eID, string erIdentifier, double priMin, double priMax, double priTrackWindow, double freqMin, double freqMax, double freqTrackWindow, double pwMin, double pwMax, double pwTrackWindow, int ageOutMax, int ceilingAgeOut)
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
        this.ceilingAgeOut = ceilingAgeOut;
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
