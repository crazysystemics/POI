class Radar : BattleSystem
{

    public override bool Stopped { get; set; }
    public Pulse txPulse;
    public Pulse activePulse;
    public Pulse zeroPulse = new Pulse(0, 0, 0, 0, "zero");
    public Pulse echoPulse;
    public int targetX;
    public int targetY;
    public int radius;


    public class Out : OutParameter
    {
        public Pulse p;
        public Position pos;
        public Out(Pulse p, Position pos, int id) : base(id)
        {
            this.pos = pos;
            this.p = p;
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
        Out radarOutput = new Out(txPulse, position, 1);
        return radarOutput;
    }

    public override void OnTick()
    {
        if (Globals.Tick % activePulse.pulseRepetitionInterval == 0)
        {
            txPulse = activePulse;
        }
        else
        {
            txPulse = zeroPulse;
        }

        Console.WriteLine($"Tick : {Globals.Tick} Radar :\t\t txPulse : {txPulse.pulseWidth}, {txPulse.pulseRepetitionInterval}, " +
            $"{txPulse.timeOfArrival}, {txPulse.angleOfArrival}, {txPulse.symbol}");

        if (echoPulse != null)
        {
            Console.WriteLine($"Tick : {Globals.Tick} Radar :\t\t echoPulse : {echoPulse.pulseWidth}, {echoPulse.pulseRepetitionInterval}, " +
            $"{echoPulse.timeOfArrival}, {echoPulse.angleOfArrival}, {echoPulse.symbol}");
        }
    }

    public override void Set(List<InParameter> inParameters)
    {
        echoPulse = ((In)(inParameters[0])).echoPulse;
    }


    public Radar(Pulse initPulse, Position position, int radius, int id)
    {
        this.txPulse = zeroPulse;
        this.activePulse = initPulse;
        this.position = position;
        this.id = id;
        this.radius = radius;
    }
}