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

class TravellingPulse : Pulse
{
    public int currentTick;
    public int txTick;
    public Position txPos;
    public Position currentPos;
    public TravellingPulse(int txTick, int currentTick, Position txPos, Position currentPos, int pulseWidth, int pulseRepetitionInterval, int timeOfArrival, int angleOfArrival, string symbol) : base(pulseWidth, pulseRepetitionInterval, timeOfArrival, angleOfArrival, symbol)
    {
        this.txTick = txTick;
        this.currentPos = currentPos;
        this.txPos = txPos;
        this.currentTick = currentTick;
    }
}