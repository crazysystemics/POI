public class RWR : BattleSystem
{

    public override bool Stopped { get; set; }
    public bool hasReceivedPulse = false;
    public int rxTick;
    public int receivedPulseCount = 0;
    public List<Pulse> receivedPulses = new();

    public RWR(ref Position positon, int id)
    {
        this.position = positon;
        this.id = id;
    }


    public class Emitter
    {
        public int Amplitude;
        public int Frequency;
        public int PRI;
        public int pulseWidth;
        public int AOA;
        public int id;

        public Emitter(int amplitude = 0, int frequency = 0, int pRI = 0, int pulseWidth = 1, int aOA = 0, int id = 0)
        {
            Amplitude = amplitude;
            Frequency = frequency;
            PRI = pRI;
            this.pulseWidth = pulseWidth;
            AOA = aOA;
            this.id = id;
        }
    }
    public List<Pulse> RxBuf = new();
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
        public Pulse p = new(0, 0, 0, 0, 0, "zero");

        public In(Pulse p, int id) : base(id)
        {
            this.p = p;
        }

    }

    public override OutParameter Get()
    {
        Out rwrParams = new(0, 0, position, 2);
        return rwrParams;
    }

    public override void OnTick()
    {
        Console.WriteLine($"\n-----------------------------------\nSignals received by RWR {id}:\n");
        if (RxBuf.Count == 0)
        {
            Console.WriteLine("None");
        }
        foreach (Pulse e in RxBuf)
        {
            if (e.amplitude > 0)
            {
                Console.WriteLine($"RWR {id}\t\tPulse Symbol: {e.symbol}\n\t\tAmplitude: {e.amplitude} dB\n\t\tPulse Width: {e.pulseWidth} ns\n\t\tFrequency: {e.frequency} MHz\n");
                // print all characteristics
            }
        }
        //Console.WriteLine($"\nEmitters scanned during mission:\n");
        //foreach (Pulse emitter in this.receivedPulses)
        //{
        //    Console.WriteLine($"Pulse Symbol: {emitter.symbol}\n\t\tAmplitude: {emitter.amplitude}\n\t\tPulse Width: {emitter.pulseWidth}\n\t\tFrequency: {emitter.frequency}\n");
        //}

        Console.WriteLine("-----------------------------------\n");
    }

    public override void Set(List<InParameter> inParameters)
    {
        RxBuf.Clear();
        foreach (InParameter inParameter in inParameters)
        {
            if (((In)inParameter).p.amplitude > 0)
            {
                RxBuf.Add(((In)inParameter).p);
            }
        }
    }
}