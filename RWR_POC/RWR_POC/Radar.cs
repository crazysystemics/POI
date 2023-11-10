class Radar : BattleSystem
{

    public override bool Stopped { get; set; }
    public Pulse txPulse;
    public int pulseRepetitionInterval;
    public int radius;
    public int txTick;

    public class Out : OutParameter
    {
        public Pulse p;
        public Position pos;
        public int txTick;
        public Out(Pulse p, Position pos, int tcTick, int id) : base(id)
        {
            this.pos = pos;
            this.p = p;
            this.txTick = tcTick;
        }
    }

    public class In : InParameter
    {
        public Pulse echoPulse;
        public In(Pulse echoPulse, int id) : base(id)
        {
            this.echoPulse = echoPulse;
        }
    }

    public override OutParameter Get()
    {
        if (txPulse.amplitude > 0)
        {
            //Globals.debugPrint = true;
        }
        Out radarOutput = new Out(txPulse, position, txTick, 1);
        return radarOutput;
    }

    public override void OnTick()
    {
        Console.WriteLine($"Position: {this.position.x}, {this.position.y}");
    }

    public override void Set(List<InParameter> inParameters)
    {
        foreach (InParameter inParam in inParameters)
        {
            //if (((In)inParam).echoPulse != null)
            //{
            //    if (((In)inParam).echoPulse.symbol == this.pulseSymbol)
            //    {
            //        this.echoPulse = ((In)(inParam)).echoPulse;
            //    }
            //}
        }
    }

    public List<Pulse> GeneratePulseTrain(int dwellTime, int startTime, double angle)
    {
        List<Pulse> pulseTrain = new List<Pulse>();
        int totalPulses;
        int currentTime = startTime;
        int PRI = this.pulseRepetitionInterval;
        totalPulses = (int)(dwellTime / PRI);
        for (int i = 0; i < totalPulses; i++)
        {
            pulseTrain.Add(new Pulse(this.txPulse.pulseWidth, this.txPulse.amplitude, this.txPulse.frequency, currentTime, angle));
            currentTime += PRI;
        }
        return pulseTrain;
    }


    public Radar(Pulse initPulse, Position position, int pulseRepetitionInterval, int txTick, int radius, int id)
    {
        this.txPulse = initPulse;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.pulseRepetitionInterval = pulseRepetitionInterval;
    }
}