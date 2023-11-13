public class Radar : BattleSystem
{

    public override bool Stopped { get; set; }
    public Pulse txPulse;
    public Pulse activePulse;
    public Pulse zeroPulse = new Pulse(0, 0, 0, 0, 0);
    public int pulseRepetitionInterval;
    public int radius;
    public int txTick;
    public int effectiveRadiatedPower;
    public int aperture;
    public string radarType;

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
    //    if (Globals.Tick == 0)
    //    {
    //        Console.WriteLine($"Pulse emitted by {this} {id}\n");
    //        Console.WriteLine($"Radar {id}:\n\ttxPulse:\n\t\tPulse width: {txPulse.pulseWidth} ns\n\t\tPRI: {pulseRepetitionInterval} ns" +
    //$"\n\t\tTime of transmission: {txPulse.timeOfTraversal} ns\n\t\tAngle of transmission: {txPulse.angleOfTraversal} rad\n\t\tSymbol: {txPulse.symbol}" +
    //$"\n\t\tAmplitude: {txPulse.amplitude} dB\n\t\ttxTick = {txTick}\n");
    //    }
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


    public Radar(Pulse initPulse, Position position, int pulseRepetitionInterval, int txTick, int radius, int effectiveRadiatedPower, int id)
    {
        this.txPulse = initPulse;
        this.activePulse = initPulse;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.pulseRepetitionInterval = pulseRepetitionInterval;
        this.effectiveRadiatedPower = effectiveRadiatedPower;
    }
}