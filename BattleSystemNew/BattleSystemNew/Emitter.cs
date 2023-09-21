class Emitter
{
    public float PulseWidth;
    public float PulseRepetitionInterval;
    public float TimeOfArrival;
    public float AngleOfArrival;
    public int EmitterID;
    public string Symbol;

    public Emitter(float pulseWidth, float pulseRepetitionInterval, float timeOfArrival, float angleOfArrival, int emitterID, string symbol)
    {
        PulseWidth = pulseWidth;
        PulseRepetitionInterval = pulseRepetitionInterval;
        TimeOfArrival = timeOfArrival;
        AngleOfArrival = angleOfArrival;
        EmitterID = emitterID;
        Symbol = symbol;
    }
}