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
    public EmitterRecord emRecord = new EmitterRecord();
    public List<EmitterRecord> emRecordList = new List<EmitterRecord>();
    public List<PDW> receivedPDW = new List<PDW>();
    public EmitterRecord receivedRecord = new EmitterRecord();
    public EmitterID eID;
    public List<EmitterTrackRecord> emitterTrackFile = new List<EmitterTrackRecord>();

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
            rwrOutParams.visibleRadars.Add(new RadarSignature(emitterRecord.distance, emitterRecord.azimuth, emitterRecord.erIdentifier));
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

    public override void OnTick()
    {
        // QPoint Beginning

        Random random = new Random();
        double epsi = random.NextDouble();
        string randChoice = string.Empty;       

        // Search current state in existing qStates
        QState state_t = GetState(RxBuf, emitterTrackFile);
        int stateIndex = Globals.qLearner.QsaMatch(state_t);

        if (epsi < Globals.qLearner.EXPLORE_PROBABILITY)
        {
            randChoice = "EXPLORE";
            Random rand = new Random();
            Globals.action_t = rand.Next(Globals.qLearner.actionSpaceCount);
        }
        //else
        //{
        //      randChoice = "EXPLOIT";
        //    double maxQsa = 0.0;
        //    int maxaction = 0;

        //    for (int action_j = 0; action_j < Globals.qLearner.actionSpaceCount; action_j++)
        //    {
        //        //int stateIndex = Globals.qLearner.QsaMatch(stateT);
        //        double qsa = Globals.qLearner.QSaMatrixGet(stateIndex, action_j);
        //        if (qsa > maxQsa)
        //        {
        //            maxQsa = qsa;
        //            maxaction = action_j;
        //        }
        //    }
        //    Globals.action = maxaction;
        //}
        if(stateIndex == -1)
            Globals.qLearner.qstates.Add(state_t);
       
        //// Execute action_t
        //List<EmitterTrackRecord> updatedList = new List<EmitterTrackRecord>();
        //foreach(EmitterTrackRecord etr in emitterTrackFile)
        //{
        //    updatedList.Add(UpdateTrackingWindows(etr));
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

        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            etr.received = false;
        }

        this.emRecordList.AddRange(RxBuf);

      
        foreach (EmitterRecord e in this.emRecordList)
            {
                EmitterRecord emitterRecord = e;
                EmitterID emitterID = Identify(emitterRecord, PFM.emitterIDTable);
                if (emitterID != null)
                {
                    emitterRecord.eID = emitterID.eID;
                    emitterRecord.erIdentifier = emitterID.erIdentifier + "R" + emitterRecord.eID;
                    
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

            if (etr.received)
            {
                tempETF.Add(etr);
                etr.Record("+");
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
                            new RecordedData(Globals.Tick, etr.erID,etr.trackID, etr.ageIn, etr.ageOut,
                            "NOT RCV ", etr.status, "AGEIN 0", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                    // not copying to tempETF is deleting etr.valid = false;
                }
                else
                {
                    
                    if (etr.ageOut > 0)
                    {
                        etr.ageOut--;
                        etr.AgingOutCount++;

                        etr.valid = true;

                        tempETF.Add(etr);

                       // Console.WriteLine($" {Globals.Tick}\t\t{etr.trackID}\t\t{etr.ageIn}\t\t{etr.ageOut}\t\tNOT RCV\t\tAGING OUT");

                        Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID,etr.trackID, etr.ageIn, etr.ageOut,
                            "NOT RCV ", etr.status, "AGING OUT", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                    }
                    else
                    {
                        etr.exitTick = Globals.Tick;
                        int trackLength = etr.exitTick - etr.entryTick;
                        double reward = trackLength / Math.Abs(etr.freqMax - etr.freqMin);
                        //Globals.qLearner.Qsa_cap(new QState(), 0, null, reward);
                        etr.status = TrackState.ETF_oDELETE;
                        Globals.recordedList.Add(
                           new RecordedData(Globals.Tick,etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                           "NOT RCV ", etr.status, "AGEOUT 0", etr.entryTick, etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
                        etr.Record("-");
                    }
                }
            }
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
                if(freqRange == 0.0)
                {
                    freqRange = -1.0;
                }
                double temp = freqRange + etr.AgingInCount + etr.AgingOutCount;
                if (temp != 0)
                {
                    currentFitness = trackLength / temp;
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

        QState state_t1 = GetState(RxBuf, ETFState);

        Globals.qLearner.Qsa_cap(state_t, Globals.action_t, state_t1, Globals.qLearner.runningAverage);

        if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
        {
            Console.WriteLine($"Tick-State\tEREC-freq\tETR-min-max\tChoice-Type\tAction\t\tNoOfState\tCurFitness\tRunningAverage");
            Console.WriteLine($"{Globals.Tick}\t\t{state_t.freq}\t\t{state_t.maxWindow}\t\t{randChoice}\t\t{Globals.action_t}\t\t{Globals.qLearner.qstates.Count}\t\t{currentFitness}\t\t{Globals.qLearner.runningAverage}");
        }
        //Console.WriteLine("QsaTable");
        //Console.WriteLine("State\t\tAction-0\tAction-1\tAction-2");
        //foreach (List<double> actionList in Globals.qLearner.Qsa)
        //{
        //    Console.WriteLine($"{Globals.qLearner.Qsa.IndexOf(actionList)}\t\t{actionList[0]}\t\t{actionList[1]}\t\t{actionList[2]}");
        //}
         if (Console.KeyAvailable)
            Globals.ExecuteShell();
       
        this.emRecordList.Clear();
        Globals.emitterTrackFile = emitterTrackFile;

        Console.WriteLine("-----------------------------------\n");

     
    }

    public EmitterTrackRecord UpdateTrackingWindows(EmitterTrackRecord etr)
    {
        if(Globals.action_t == 0)
            etr.freqTrackWindow += etr.freqTrackWindow * 0.1;
        if(Globals.action_t == 1)
            etr.freqTrackWindow -= etr.freqTrackWindow * 0.1;

        return etr;
    }

    public QState GetState(List<EmitterRecord> emitterRecords, List<EmitterTrackRecord> emitterTrackRecords)
    {
        QState qState = new QState();
        List<double> freqs = new List<double>();
        List<double> windows = new List<double>();
        foreach (EmitterRecord er in emitterRecords)
        {
            freqs.Add(er.freq);    
        }
        if (freqs.Count > 0)
        {
            qState.freq = freqs.Max() - freqs.Min();
            if(qState.freq == 0)
                qState.freq = -1;
        }
        else
            qState.freq = 0; 
        
        foreach (EmitterTrackRecord etr in emitterTrackRecords)
        {
            windows.Add(etr.freqMax - etr.freqMin);   
        }
        if (windows.Count > 0)
        {
            qState.maxWindow = windows.Max();
            if (qState.maxWindow == 0)
                qState.maxWindow = -1;
        }
        else
            qState.maxWindow = 0;

        return qState;
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

    public EmitterID Identify(EmitterRecord emitterRecord, List<EmitterID> eIDTable)
    {
        foreach (EmitterID eID in eIDTable)
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

    public void ManageTracks(EmitterRecord emitterRecord, EmitterID emitterID, List<EmitterTrackRecord> emitterTrackFile)
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

                    etr.ageOut = emitterID.ageOut;

                    
                    if (etr.ageIn > 0)
                    {
                        etr.ageIn--;
                        etr.valid = false;
                        etr.status = TrackState.ETF_iDELETE;
                        etr.AgingInCount++;

                     //   Console.WriteLine($"{Globals.Tick}\t\tRECEIVED\t\t{etr.trackID}\t\tUPDATE\t\t" +
                          //  $"{etr.ageIn}\t\t{etr.ageOut}\t\tAGING INT");

                        Globals.recordedList.Add(
                            new RecordedData(Globals.Tick, etr.erID,etr.trackID, etr.ageIn, etr.ageOut,
                            "RECEIVED", etr.status, "AGING IN",etr.entryTick,etr.exitTick, etr.AgingInCount, etr.AgingOutCount));
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

            etr.ageOut = etr.baseAgeOut;

            etr.received = true;

            //On Insert

           // etr.AgingInCount++;
            etr.received = true;
            Globals.recordedList.Add(
                            new RecordedData(Globals.Tick,etr.erID, etr.trackID, etr.ageIn, etr.ageOut,
                            "RECEIVED", etr.status, "AGING IN" , etr.entryTick,etr.exitTick,etr.AgingInCount, etr.AgingOutCount));
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
}

public class EmitterID
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

    public EmitterID(int eID, string erIdentifier, double priMin, double priMax, double priTrackWindow, double freqMin, double freqMax, double freqTrackWindow, double pwMin, double pwMax, double pwTrackWindow)
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
    }
}

public class PFM
{
    public static List<EmitterID> emitterIDTable = new List<EmitterID>();

    public void FormPFMTable()
    {
        StreamReader reader = new StreamReader("Input.csv", true);
        string S = reader.ReadLine();
        while (S != null)
        {
            string[] data = S.Split(',');

            if (data[0] == "ER")
            {
                EmitterID eID = GetEmitterID(data, ref reader);
                emitterIDTable.Add(eID);
            }
            S = reader.ReadLine();
        }
    }

    public EmitterID GetEmitterID(string[] data, ref StreamReader reader)
    {
        reader.ReadLine();
        reader.ReadLine();
        string S = reader.ReadLine(); ;
        string[] data1 = S.Split(',');

        EmitterID emitterID = new EmitterID(Convert.ToInt32(data[1]), "E" + data[1], Convert.ToInt32(data1[3]), Convert.ToInt32(data1[4]),
            Convert.ToInt32(data[3]), Convert.ToInt32(data1[7]), Convert.ToInt32(data1[8]), Convert.ToInt32(data[2]),
            Convert.ToInt32(data1[5]), Convert.ToInt32(data1[6]), Convert.ToInt32(data[4]));

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

    public RadarSignature(int r, double theta, string symbol)
    {
        this.r = r;
        this.theta = theta;
        this.symbol = symbol;
    }
}
