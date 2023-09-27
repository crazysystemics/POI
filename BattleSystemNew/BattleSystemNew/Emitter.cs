class Emitter : BattleSystem
{

    public override bool Stopped { get; set; }

    // Derived from BattleSystem

    public int pulseWidth;
    public int pulseRepetitionInterval;
    public int timeOfArrival;
    public int angleOfArrival;
    public int emitterID;
    public string Symbol;

    public Emitter(int pulseWidth, int pulseRepetitionInterval, int timeOfArrival, int angleOfArrival, int emitterID, string symbol)
    {
        this.pulseWidth = pulseWidth;
        this.pulseRepetitionInterval = pulseRepetitionInterval;
        this.timeOfArrival = timeOfArrival;
        this.angleOfArrival = angleOfArrival;
        this.emitterID = emitterID;
        Symbol = symbol;
    }

    public class EmitterOut : OutParameter
    {
        public int pulseWidth;
        public int pulseRepetitionInterval;
        public int timeOfArrival;
        public int angleOfArrival;
        public int emitterID;
        public EmitterOut(int pulseWidth, int pulseRepetitionInterval, int timeOfArrival, int angleOfArrival, int id) : base(id)
        {
            this.pulseWidth = pulseWidth;
            this.pulseRepetitionInterval = pulseRepetitionInterval;
            this.timeOfArrival = timeOfArrival;
            this.angleOfArrival = angleOfArrival;
            emitterID = id;
        }
    }

    public override EmitterOut Get()
    {
        EmitterOut emitterOut = new EmitterOut(0, 1, 1, 0, 5);
        return emitterOut;
    }

    public override void OnTick()
    {

    }

    public override void Set(List<InParameter> inParameter)
    {

    }
}