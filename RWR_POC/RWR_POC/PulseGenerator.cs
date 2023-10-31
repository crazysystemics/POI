public class PulseGenerator
{
    public int pulseWidth;
    public int PRI;
    public int amplitude;
    public int frequency;
    public int dwellTime;
    public List<PDWs> PDW = new List<PDWs>();

    public PulseGenerator(int pulseWidth, int pRI, int amplitude, int frequency, int dwellTime)
    {
        this.pulseWidth = pulseWidth;
        this.PRI = pRI;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.dwellTime = dwellTime;
    }

    public List<PDWs> GeneratePulseTrain()
    {
        int timeOfTransmission = 0;
        int numberOfPulses = (int)(this.dwellTime / (this.PRI + this.pulseWidth));

        for (int i = 0; i < numberOfPulses; i++)
        {
            PDW.Add(new PDWs(this.amplitude, this.frequency, this.pulseWidth, timeOfTransmission));
            timeOfTransmission += this.pulseWidth + this.PRI;
        }
        return PDW;
    }
}