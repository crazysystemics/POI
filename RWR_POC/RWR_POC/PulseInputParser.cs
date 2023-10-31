using System.Net;
using System.Runtime.CompilerServices;

public class PulseInputParser
{
    public string fileName;
    public StreamReader reader;
    public string[] inputPulses;
    public string? parsedLine;
    public Pulse received;
    public List<Pulse> receivedPulses = new();
    public string[] pulseCharacteristics;
    public int linesInFile;

    // inputPulseData[0] = Clock
    // inputPulseData[1] = Reset
    // inputPulseData[2] = Time of Arrival
    // inputPulseData[3] = Pulse Width
    // inputPulseData[4] = Pulse Repetition Interval
    // inputPulseData[5] = dOut
    // inputPulseData[6] = Pulse

    // dOut (from left to right) = "8 zero bits as padding" + "PRI" + "Pulse Width" + "Time of Arrival" : concatenated string of binary bits

    // inputPulseData = physical pulse present + RWR's sensor model
    // Make inputPulseData class instead of array
    //public class ReceivedPulse
    //{
    //    public int amplitude;
    //    public int frequency;
    //    public int pulseWidth;
    //}

    public PulseInputParser(string fileName)
    {
        this.linesInFile = File.ReadAllLines(fileName).Length;
        this.fileName = fileName;
        this.reader = new StreamReader(fileName);
        this.inputPulses = new string[linesInFile];
        this.received = new Pulse(0, 0, 0, 0, 0, "zero");
        this.pulseCharacteristics = new string[4];
    }

    public List<Pulse> ParseText()
    {

        for (int i = 0; i < this.linesInFile; i++)
        {
            this.inputPulses[i] = reader.ReadLine();
            for (int j = 0; j < 4; j++)
            {
                this.pulseCharacteristics[j] = inputPulses[i][(8 * j)..(8 * (j + 1))];
            }

            // pulseCharacteristics[0] contains time of arrival information.
            // pulseCharacteristics[1] contains amplitude information.
            // pulseCharacteristics[2] contains frequency information.
            // pulseCharacteristics[3] contains pulse width information.

            // Every packet should have a time stamp
            // Which leading bits represent which information might be different based on the generated pulse data.
            int toa = Convert.ToInt32(this.pulseCharacteristics[0], 2);
            int amp = Convert.ToInt32(this.pulseCharacteristics[1], 2);
            int freq = Convert.ToInt32(this.pulseCharacteristics[2], 2);
            int pwidth = Convert.ToInt32(this.pulseCharacteristics[3], 2);
            this.received = new Pulse(pwidth, amp, freq, toa, 0, i.ToString());
            this.receivedPulses.Add(received);

        }


        // PRI = number of pulses received by RWR in duration of a tick.
        // If each pulse is a 32-bit packet, with leading bits encoding the Pulse Width, Amplitude and Frequency, then:
        // each characteristic is sleected using string slicing.
        // Padding bits are data[..8], first encoded value is data[8..16], second encoded value is data[16..24] and the last one is data[24..32]

        return this.receivedPulses;
    }
}