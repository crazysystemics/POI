public abstract class Radar : BattleSystem
{

    public override bool Stopped { get; set; }
    //public abstract Pulse txPulse { get; set; }
    //public abstract Pulse activePulse { get; set; }
    //public abstract int pulseRepetitionInterval { get; set; }
    //public abstract int radius { get; set; }
    //public abstract int txTick { get; set; }
    //public abstract int effectiveRadiatedPower { get; set; }
    //public abstract int aperture { get; set; }
    //public abstract string radarType { get; set; }
    //public abstract int endToEndDuration { get; set; }
    //public abstract int endToEndScanSector { get; set; }
    //public abstract int mainBeamAzimuth { get; set; }
    //public abstract int beamWidth { get; set; }

    public Pulse txPulse;
    public Pulse activePulse;
    public int pulseRepetitionInterval;
    public int radius;
    public int txTick;
    public int effectiveRadiatedPower;
    public int aperture;
    public Globals.RadarTypes radarType;
    public int endToEndDuration;
    public int endToEndScanSector;
    public int mainBeamAzimuth;
    public int beamWidth;

    public class Out : OutParameter
    {
        public Pulse p;
        public Position pos;
        public int txTick;
        public Out(Pulse p, Position pos, int tcTick, int id) : base(id)
        {
            this.pos = pos;
            this.p = p;
            this.txTick = tcTick;
        }
    }


    public abstract bool beamContains(Position targetPosition);
    public abstract List<Pulse> GeneratePulseTrain(int startTime, double angle);
}