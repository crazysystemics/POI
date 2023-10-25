public class PulseInputParser
{
    public string fileName;
    public StreamReader reader;
    public string[] pulseDescriptionWords;
    public string? parsedLine;
    public Pulse inputPulse = new(0, 0, 0, 0, 0, "zero");

    // PDW[0] = Clock
    // PDW[1] = Reset
    // PDW[2] = Time of Arrival
    // PDW[3] = Pulse Width
    // PDW[4] = Pulse Repetition Interval
    // PDW[5] = dOut
    // PDW[6] = Pulse

    // dOut (from left to right) = "8 zero bits as padding" + "PRI" + "Pulse Width" + "Time of Arrival" : concatenated string of binary bits

    public PulseInputParser(string fileName)
    {
        this.fileName = fileName;
        this.reader = new StreamReader(fileName);
        this.pulseDescriptionWords = new string[7];
    }

    public Pulse ParseText()
    {
        for (int i = 0; i < this.pulseDescriptionWords.Length; i++)
        {
            this.pulseDescriptionWords[i] = reader.ReadLine();
        }
        this.inputPulse.pulseWidth = Convert.ToInt32(this.pulseDescriptionWords[3], 2);
        this.inputPulse.timeOfTraversal = Convert.ToInt32(this.pulseDescriptionWords[2], 2);

        return this.inputPulse;
    }
}