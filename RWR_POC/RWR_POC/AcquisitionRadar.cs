﻿public class AcquisitionRadar : Radar
{
    public Pulse txPulse;
    public Pulse activePulse;
    public Pulse zeroPulse = new Pulse(0, 0, 0, 0, 0);
    public int pulseRepetitionInterval;
    public int radius;
    public int txTick;
    public int effectiveRadiatedPower;
    public int aperture;
    public string radarType;
    public int endToEndDuration;
    public int endToEndScanSector;
    public int beamWidth;

    public AcquisitionRadar(Pulse initPulse, Position position, int pulseRepetitionInterval, int txTick, int radius, int effectiveRadiatedPower, int id) : base(initPulse, position, pulseRepetitionInterval, txTick, radius, effectiveRadiatedPower, id)
    {
        int pulseWidth = (int)((Globals.randomNumberGenerator.Next(10, 15) / 100) * pulseRepetitionInterval);
        this.txPulse = new Pulse(pulseWidth, 10, Globals.randomNumberGenerator.Next(3500, 4000), 0, 0);
        this.activePulse = txPulse;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.pulseRepetitionInterval = Globals.randomNumberGenerator.Next(900, 1400);
        this.effectiveRadiatedPower = 128;
        this.radarType = "Acquisition";
        this.endToEndDuration = 1;
        this.endToEndScanSector = 90;
    }

    public List<Pulse> GeneratePulseTrain(int dwellTime, int startTime, double angle)
    {
        List<Pulse> pulseTrain = new List<Pulse>();
        int totalPulses;
        int currentTime = startTime;
        int PRI = this.pulseRepetitionInterval;
        totalPulses = (int)(dwellTime / PRI);
        for (int i = 0; i < totalPulses; i++)
        {
            pulseTrain.Add(new Pulse(this.txPulse.pulseWidth, this.txPulse.amplitude, this.txPulse.frequency, currentTime, angle));
            currentTime += PRI;
        }
        return pulseTrain;
    }
}