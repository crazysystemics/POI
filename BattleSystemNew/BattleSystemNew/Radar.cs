class Radar : BattleSystem
{

    public override bool Stopped { get; set; }

    public float PulseWidth;
    public float PulseRepetitionInterval;
    public float TimeElapsed = 0.0f;

    public RadarPosition CurrentPosition = new RadarPosition(0, 0);

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
        Out radar_output = new Out(CurrentPosition.r, CurrentPosition.theta, 1);
        return radar_output;
    }

    public override void OnTick()
    {
        if (TimeElapsed == PulseRepetitionInterval)
        {
            EmitPulse();
            TimeElapsed = 0;
        }
        TimeElapsed += Globals.TimeResolution;
    }

    public override void Set(List<InParameter> inparameter)
    {

    }


    public Radar(List<float[]> waypoints, float radar_range)
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