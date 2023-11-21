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

    public List<PDW> RxBuf = new();
    public class Out : OutParameter
    {
        public int r;
        public int theta;
        public Position position;
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
        Out rwrParams = new(0, 0, position, 2);
        return rwrParams;
    }

    public override void Set(List<InParameter> inParameters)
    {
        foreach (InParameter inParameter in inParameters)
        {
            if (((In)inParameter).emRecord.pw > 0)
            {
                this.receivedRecord = ((In)inParameter).emRecord;
            }
        }
    }

    public override void OnTick()
    {

        //int pulseCount = 1;
        //foreach (PDW pdw in receivedPDW)
        //{
        //    Console.WriteLine($"Pulse {pulseCount}\nPulse Width: {pdw.pulseWidth} ns\n" +
        //                      $"Amplitudes: {pdw.amplitude[0]}, {pdw.amplitude[1]}, {pdw.amplitude[2]}, {pdw.amplitude[3]}\n" +
        //                      $"Frequency: {pdw.frequency} MHz\nTime of Arrival: {pdw.timeOfArrival} ns\n");
        //    pulseCount++;
        //}

        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            etr.received = false;
        }

        EmitterRecord tempReceived = new EmitterRecord();
        tempReceived.erID = this.receivedRecord.erID;
        tempReceived.pri = this.receivedRecord.pri;
        tempReceived.pw = this.receivedRecord.pw;
        tempReceived.freq = this.receivedRecord.freq;
        tempReceived.erIdentifier = this.receivedRecord.erIdentifier;
        tempReceived.eID = this.receivedRecord.eID;

        if (this.receivingEmitterRecord)
        {
            EmitterRecord emitterRecord = this.receivedRecord;
            EmitterID emitterID = Identify(emitterRecord, PFM.emitterIDTable);
            if (emitterID != null )
            {
                emitterRecord.eID = emitterID.eID;
            }
            ManageTracks(emitterRecord, emitterID, emitterTrackFile);

            Console.WriteLine($"Emitter Record:\n\t\tPRI: {emitterRecord.pri} mcs\n\t\tPulse Width: {emitterRecord.pw} ns\n\t\tFrequency: {emitterRecord.freq} MHz\n");
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
                etr.Record("-");
            }
        }

        emitterTrackFile = tempETF;

        receivingEmitterRecord = false;

        receivedRecord = new EmitterRecord();


        Console.WriteLine("-----------------------------------\n");
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
                    etr.priCurrent = emitterRecord.pri;
                    etr.pwCurrent = emitterRecord.pw;
                    etr.freqCurrent = emitterRecord.freq;

                    Console.WriteLine($"Match found in tracks:\n\t\tID: {etr.eid}\n\t\tFrequency: {etr.freqCurrent} MHz\n\t\t" +
                                      $"PRI: {etr.priCurrent} mcs\n\t\tPulse Width: {etr.pwCurrent} ns");
                }
            }
        }

        if (!foundInETF && emitterID != null)
        {
            EmitterTrackRecord etr = new EmitterTrackRecord();

            etr.trackID = Globals.gTrackID++;
            etr.eid = emitterRecord.eID;

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
            etr.ageOut = emitterID.ageOut;

            etr.identifier = emitterRecord.erIdentifier;

            etr.received = true;

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

    public double azimuthTrackWindow;
    public int ageIn;
    public double ageOut;

    public string identifier;

    public void Record(string label)
    {
        StreamWriter sw = new StreamWriter(Globals.recFileName, true);
        sw.WriteLine($"{label}, {received}, {trackID}, {priCurrent}, {freqCurrent}, {pwCurrent}, {identifier}, {Globals.Tick}");
        sw.Close();
    }
}

public class EmitterRecord
{
    public string erIdentifier;
    public int erID;
    public int eID;

    public double pri;
    public double freq;
    public double pw;
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
    public double ageOut;

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