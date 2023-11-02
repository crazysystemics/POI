class Radar : BattleSystem
{

    public override bool Stopped { get; set; }
    public List<Pulse> txPulses;
    public List<Pulse> activePulses;
    public Pulse zeroPulse = new Pulse(0, 0, 0, 0, 0, "zero");
    public Pulse echoPulse;
    public int pulseRepetitionInterval;
    //public int targetX;
    //public int targetY;
    public int radius;
    public int txTick;
    public bool hasReceivedEcho = false;
    public string pulseSymbol;
    public List<int> txTicks = new List<int>();
    public int pulsesSent = 0;
    public PulseGenerator radarPulseGenerator;
    //public bool hasPulseReachedTarget = false;

    public class PulseGenerator
    {
        public int pulseWidth;
        public int PRI;
        public int amplitude;
        public int frequency;
        public int dwellTime;
        public List<Pulse> PDW = new List<Pulse>();

        public PulseGenerator(int pulseWidth, int pRI, int amplitude, int frequency, int dwellTime)
        {
            this.pulseWidth = pulseWidth;
            this.PRI = pRI;
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.dwellTime = dwellTime;
        }

        public List<Pulse> GeneratePulseTrain()
        {
            int timeOfTransmission = 0;
            int numberOfPulses = (int)(this.dwellTime / (this.PRI + this.pulseWidth));

            for (int i = 0; i < numberOfPulses; i++)
            {
                PDW.Add(new Pulse(this.pulseWidth, this.amplitude, this.frequency, timeOfTransmission, 0, "X"));
                timeOfTransmission += this.pulseWidth + this.PRI;
            }
            return PDW;
        }
    }

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
        Out radarOutput = new Out(zeroPulse, position, txTick, 1);
        return radarOutput;
    }

    public override void OnTick()
    {
    //    if (Globals.Tick == 0)
    //    {
    //        Console.WriteLine($"Pulse emitted by {this} {id}\n");
    //        Console.WriteLine($"Radar {id}:\n\ttxPulse:\n\t\tPulse width: {txPulses.pulseWidth}\n\t\tPRI: {pulseRepetitionInterval}" +
    //$"\n\t\tTime of transmission: {txPulses.timeOfTraversal}\n\t\tAngle of transmission: {txPulses.angleOfTraversal}\n\t\tSymbol: {txPulses.symbol}" +
    //$"\n\t\tAmplitude: {txPulses.amplitude}\n\t\ttxTick = {txTick}\n");
    //    }
    }

    public override void Set(List<InParameter> inParameters)
    {
        foreach (InParameter inParam in inParameters)
        {
            if (((In)inParam).echoPulse != null)
            {
                if (((In)inParam).echoPulse.symbol == this.pulseSymbol)
                {
                    this.echoPulse = ((In)(inParam)).echoPulse;
                }
            }
        }
    }


    public Radar(Pulse initPulse, Position position, int pulseRepetitionInterval, string pulseSymbol, int txTick, int radius, int id)
    {
        this.radarPulseGenerator = new PulseGenerator(initPulse.pulseWidth, pulseRepetitionInterval, initPulse.amplitude, initPulse.frequency, 250);
        //List<Pulse> radarPulses = radarPulseGenerator.GeneratePulseTrain();
        //this.txPulses = radarPulses;
        //this.activePulses = radarPulses;
        this.echoPulse = zeroPulse;
        this.pulseSymbol = pulseSymbol;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.pulseRepetitionInterval = pulseRepetitionInterval;
        this.txTicks.Add(Globals.Tick);
    }
}