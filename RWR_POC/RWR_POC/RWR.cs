public class RWR : BattleSystem
{

    public override bool Stopped { get; set; }
    public bool hasReceivedPulse = false;
    public int rxTick;
    public Pulse EmitterPulse;
    public int receivedPulseCount = 0;

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
    public Emitter RxBuf = new Emitter();
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
        public Emitter e = new Emitter();

        public In(Emitter e, int id) : base(id)
        {
            this.ID = id;
            this.e = e;
        }

    }

    public override OutParameter Get()
    {
        Out rwrParams = new Out(0, 0, position, 2);
        return rwrParams;
    }

    public override void OnTick()
    {
        //if (Globals.debugPrint)
        {
            //Console.WriteLine($"RWR {id}:\t\tPosition (x, y): ({position.x}, {position.y})");
            //if (RxBuf.id == 0)
            //{
            //    Console.WriteLine("\t\tNo emitter\n");
            //}
            if (RxBuf.Amplitude > 0)
            {
                Console.WriteLine($"RWR {id}\t\tPRI: {RxBuf.PRI}\n\t\tFrequency: {RxBuf.Frequency}\n\t\tAmplitude: {RxBuf.Amplitude}");
                // print all characteristics
            }
        }
    }

    public override void Set(List<InParameter> inParameters)
    {
        RxBuf = ((In)inParameters[0]).e;
    }
}