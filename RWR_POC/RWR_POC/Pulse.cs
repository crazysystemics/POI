public class Pulse
{
    public int pulseWidth;
    public int timeOfTraversal;
    public int angleOfTraversal;
    public int amplitude;
    public string symbol;

    public Pulse(int pulseWidth, int amplitude, int timeOfTraversal, int angleOfTraversal, string symbol)
    {
        this.pulseWidth = pulseWidth;
        this.amplitude = amplitude;
        this.timeOfTraversal = timeOfTraversal;
        this.angleOfTraversal = angleOfTraversal;
        this.symbol = symbol;
    }
}

// remove pri attribute

// time of traversal = time of arrival for receiver, time of transmission for emitter