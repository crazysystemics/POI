public class MissileGuidanceRadar : Radar
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

    public MissileGuidanceRadar(Pulse initPulse, Position position, int pulseRepetitionInterval, int txTick, int radius, int effectiveRadiatedPower, int id) : base(initPulse, position, pulseRepetitionInterval, txTick, radius, effectiveRadiatedPower, id)
    {
        int pulseWidth = (int)((Globals.randomNumberGenerator.Next(10, 15) / 100) * pulseRepetitionInterval);
        this.txPulse = new Pulse(pulseWidth, 10, Globals.randomNumberGenerator.Next(10000, 11000), 0, 0);
        this.activePulse = txPulse;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.pulseRepetitionInterval = Globals.randomNumberGenerator.Next(70, 120);
        this.effectiveRadiatedPower = 128;
        this.radarType = "Missile Guidance";
        this.endToEndDuration = 1;
        this.endToEndScanSector = 90;
    }
}