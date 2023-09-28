public class RWR : BattleSystem
{

    public override bool Stopped { get; set; }
    public class Emitter
    {
        public int[] Amplitudes = new int[4];
        public int Frequency;
        public int PRI;
        public int pulseWidth;
        public int AOA;

        public Emitter(int[] amplitudes = null, int frequency = 0, int pRI = 0, int pulseWidth = 1, int aOA = 0)
        {
            Amplitudes = amplitudes;
            Frequency = frequency;
            PRI = pRI;
            this.pulseWidth = pulseWidth;
            AOA = aOA;
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
        Console.WriteLine("Tick : " + Globals.Tick + "RWR :\t\t amplitude :" + RxBuf.Amplitudes + RxBuf.Amplitudes + ",freq :" + RxBuf.Frequency);
    }

    public override void Set(List<InParameter> inParameters)
    {
        RxBuf = ((In)inParameters[0]).e;
    }
}