public class PulseGenerator
{
    public int pulseWidth;
    public int PRI;
    public int amplitude;
    public int frequency;
    public int dwellTime;
    public List<Pulse> PDW = new List<Pulse>();

    public PulseGenerator(int pulseWidth, int pRI, int amplitude, int frequency, int dwellTime)
    {
        this.pulseWidth = pulseWidth;
        this.PRI = pRI;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.dwellTime = dwellTime;
    }

    public List<Pulse> GeneratePulseTrain()
    {
        int timeOfTransmission = 0;
        int numberOfPulses = (int)(this.dwellTime / (this.PRI + this.pulseWidth));

        for (int i = 0; i < numberOfPulses; i++)
        {
            PDW.Add(new Pulse(this.pulseWidth, this.amplitude, this.frequency, timeOfTransmission, 0, "X"));
            timeOfTransmission += this.pulseWidth + this.PRI;
        }
        return PDW;
    }
}

// This will be in Radar Class
// Send array of 4 values from GeneratePulseTrain

// Should we calculate LSB from min and max values