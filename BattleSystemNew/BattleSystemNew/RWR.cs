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
    public class RWROut : OutParameter
    {
        public int r;
        public int theta;
        public RWROut(int r, int theta, int id) : base(id)
        {
            this.r = r;
            this.theta = theta;
        }
    }

    public class In : InParameter
    {
        public Emitter e = new Emitter();
        public In(int id) : base(id)
        {
            this.ID = id;
        }

    }

    public override OutParameter Get()
    {
        RWROut rwrParams = new RWROut(0, 0, 2);
        return rwrParams;
    }

    public override void OnTick()
    {

    }

    public override void Set(List<InParameter> inParameter)
    {

    }
}