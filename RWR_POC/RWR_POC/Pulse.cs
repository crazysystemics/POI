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