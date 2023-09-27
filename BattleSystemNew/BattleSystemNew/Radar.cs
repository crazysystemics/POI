class Radar : BattleSystem
{

    public override bool Stopped { get; set; }

    public float pulseWidth;
    public float pulseRepetitionInterval;
    public float timeElapsed = 0.0f;

    public RadarPosition currentPosition = new RadarPosition(0, 0);

    public float[] EmitPulse()
    {
        return null;
    }

    public class Out : OutParameter
    {
        public int r;
        public int theta;
        public Out(int r, int theta, int id) : base(id)
        {
            this.r = r;
            this.theta = theta;
        }
    }

    public override OutParameter Get()
    {
        Out radarOutput = new Out(currentPosition.r, currentPosition.theta, 1);
        return radarOutput;
    }

    public override void OnTick()
    {
        if (timeElapsed == pulseRepetitionInterval)
        {
            EmitPulse();
            timeElapsed = 0;
        }
        timeElapsed += Globals.TimeResolution;
    }

    public override void Set(List<InParameter> inParameter)
    {

    }


    public Radar()
    {

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