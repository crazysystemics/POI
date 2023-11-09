public class Pulse
{
    public int pulseWidth;
    public int timeOfTraversal;
    public double angleOfTraversal;
    public int amplitude;
    public int frequency;

    public Pulse(int pulseWidth, int amplitude, int frequency, int timeOfTraversal, double angleOfTraversal)
    {
        this.pulseWidth = pulseWidth;
        this.amplitude = amplitude;
        this.timeOfTraversal = timeOfTraversal;
        this.angleOfTraversal = angleOfTraversal;
        this.frequency = frequency;
    }
}

// remove pri attribute

// time of traversal = time of arrival for receiver, time of transmission for emitter