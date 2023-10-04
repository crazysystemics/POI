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

class TransmittedPulse : Pulse
{
    public int txTick;
    public int rxTick;
    public Position txPos;
    public TransmittedPulse(int txTick, int rxTick, Position txPos, int pulseWidth, int pulseRepetitionInterval, int timeOfArrival, int angleOfArrival, string symbol) : base(pulseWidth, pulseRepetitionInterval, timeOfArrival, angleOfArrival, symbol)
    {
        this.txTick = txTick;
        this.txPos = txPos;
        this.rxTick = rxTick;
    }
}