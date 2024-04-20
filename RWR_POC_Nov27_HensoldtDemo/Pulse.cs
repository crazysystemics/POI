public class Pulse
{

    // Class for modeling the pulse sent by an emitter. However, since the program in its current state
    // only uses Emitter Records directly, the generated pulses are not processed directly. This role
    // might now be taken over by the FPGA.

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