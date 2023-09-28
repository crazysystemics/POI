using System.Security.Cryptography.X509Certificates;

class Radar : BattleSystem
{

    public override bool Stopped { get; set; }

    public float pulseWidth;
    public float pulseRepetitionInterval;
    public Pulse txPulse;
    public Pulse activePulse;
    public Pulse zeroPulse = new Pulse(0, 0, 0, 0, "zero");
    public Pulse echoPulse;

    public RadarPosition currentPosition = new RadarPosition(0, 0);


    public class Out : OutParameter
    {
        public Pulse p;
        public Out(Pulse p, int id) : base(id)
        {
            this.p = p;
        }
    }

    public class In : InParameter
    {
        public Pulse echoPulse;
        public In(Pulse echoPulse, int id):base(id)
        {
            this.echoPulse = echoPulse;
        }
    }

    public override OutParameter Get()
    {
        Out radarOutput = new Out(txPulse, 1);
        return radarOutput;
    }

    public override void OnTick()
    {
        if (Globals.Tick % pulseRepetitionInterval == 0)
        {
            txPulse = activePulse;
        }
        else
        {
            txPulse = zeroPulse;
        }
    }

    public override void Set(List<InParameter> inParameters)
    {
        echoPulse = ((In)(inParameters[0])).echoPulse;
    }


    public Radar(Pulse inPulse)
    {
        this.txPulse = zeroPulse;
        this.activePulse = inPulse;
    }
}

class RadarPosition
{
    public int r;
    public int theta;

    public RadarPosition(int r = 0, int theta = 0)
    {
        this.r = r;
        this.theta = theta;
    }
}

class Pulse
{
    public int pulseWidth;
    public int pulseRepetitionInterval;
    public int timeOfArrival;
    public int angleOfArrival;
    public string symbol;

    public Pulse(int pulseWidth, int pulseRepetitionInterval, int timeOfArrival, int angleOfArrival, string symbol)
    {
        this.pulseWidth = pulseWidth;
        this.pulseRepetitionInterval = pulseRepetitionInterval;
        this.timeOfArrival = timeOfArrival;
        this.angleOfArrival = angleOfArrival;
        this.symbol = symbol;
    }
}