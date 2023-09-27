class Emitter : BattleSystem
{

    public override bool Stopped { get; set; }

    // Derived from BattleSystem

    public int PulseWidth;
    public int PulseRepetitionInterval;
    public int TimeOfArrival;
    public int AngleOfArrival;
    public int EmitterID;
    public string Symbol;

    public Emitter(int pulseWidth, int pulseRepetitionInterval, int timeOfArrival, int angleOfArrival, int emitterID, string symbol)
    {
        PulseWidth = pulseWidth;
        PulseRepetitionInterval = pulseRepetitionInterval;
        TimeOfArrival = timeOfArrival;
        AngleOfArrival = angleOfArrival;
        EmitterID = emitterID;
        Symbol = symbol;
    }

    public class EmitterOut : OutParameter
    {
        public int pulseWidth;
        public int pulseRepetitionInterval;
        public int timeOfArrival;
        public int AngleOfArrival;
        public int EmitterID;
        public EmitterOut(int pulseWidth, int pulseRepetitionInterval, int timeOfArrival, int angleOfArrival, int id) : base(id)
        {
            this.pulseWidth = pulseWidth;
            this.pulseRepetitionInterval = pulseRepetitionInterval;
            this.timeOfArrival = timeOfArrival;
            AngleOfArrival = angleOfArrival;
            EmitterID = id;
        }
    }

    public override EmitterOut Get()
    {
        EmitterOut emitterout = new EmitterOut(0, 1, 1, 0, 5);
        return emitterout;
    }

    public override void OnTick()
    {

    }

    public override void Set(List<InParameter> inparameter)
    {

    }
}