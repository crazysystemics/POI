using System.Collections.Generic;

public abstract class Radar : BattleSystem
{
    // A parent Radar class that derives from Battle System. This has a few child classes, including
    // AcquisitionRadar, FireControlRadar, GCIRadar, SimpleRadar and EarlyWarningRadar.
    public override bool Stopped { get; set; }

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
    public bool detection;
    public Position rPos;

    public class Out : OutParameter
    {

        // The parameters that objects of a Radar class output when Get() is called.

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


    public abstract bool BeamContains(Position targetPosition);
    public abstract List<Pulse> GeneratePulseTrain(int startTime, double angle);
}