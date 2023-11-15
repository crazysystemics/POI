public class AcquisitionRadar : Radar
{
    public override bool Stopped { get; set; }
    public override Pulse txPulse { get; set; }
    public override Pulse activePulse { get; set; }
    public override int pulseRepetitionInterval { get; set; }
    public override int radius { get; set; }
    public override int txTick { get; set; }
    public override int effectiveRadiatedPower { get; set; }
    public override int aperture { get; set; }
    public override string radarType { get; set; }
    public override int endToEndDuration { get; set; }
    public override int endToEndScanSector { get; set; }
    public override int mainBeamAzimuth { get; set; }
    public override int beamWidth { get; set; }

    public int frameOffSet;
    public int numberOfFrames;
    public int startFrameAzimuth;
    public int endFrameAzimuth;


    public class In : InParameter
    {
        public Pulse echoPulse;
        public In(Pulse echoPulse, int id) : base(id)
        {
            this.echoPulse = echoPulse;
        }
    }

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
    public override OutParameter Get()
    {
        Out radarOutput = new Out(txPulse, position, txTick, 1);
        return radarOutput;
    }

    public override void Set(List<InParameter> inParameters)
    {

    }

    public override void OnTick()
    {
        this.mainBeamAzimuth -= (this.beamWidth);
        if (this.mainBeamAzimuth <= this.endFrameAzimuth + (this.beamWidth / 2))
        {
            this.mainBeamAzimuth = this.startFrameAzimuth - (this.beamWidth / 2);
        }
    }

    public override List<Pulse> GeneratePulseTrain(int startTime, double angle)
    {
        List<Pulse> pulseTrain = new List<Pulse>();
        int totalPulses;
        int currentTime = startTime;
        int PRI = this.pulseRepetitionInterval;
        int dwellTime = beamWidth * endToEndScanSector / endToEndDuration;
        totalPulses = (int)(dwellTime * 1000 / PRI);
        for (int i = 0; i < totalPulses; i++)
        {
            pulseTrain.Add(new Pulse(this.txPulse.pulseWidth, this.txPulse.amplitude, this.txPulse.frequency, currentTime, angle));
            currentTime += PRI;
        }
        return pulseTrain;
    }

    public override bool beamContains(Position targetPosition)
    {
        int radialDistance = PhysicalSimulationEngine.GetDistance(targetPosition, this.position);
        int azimuth = (int)(PhysicalSimulationEngine.GetAngle(targetPosition, this.position) * (180 / Math.PI));
        if (radialDistance < this.radius)
        {
            if (azimuth < (mainBeamAzimuth + (beamWidth / 2)) && azimuth > (mainBeamAzimuth - (beamWidth / 2)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public AcquisitionRadar(Position position, int radius, int beamWidth, int txTick, int id)
    {
        this.pulseRepetitionInterval = Globals.randomNumberGenerator.Next(900, 1400);
        int pulseWidth = (Globals.randomNumberGenerator.Next(10, 15) * pulseRepetitionInterval) / 100;
        this.txPulse = new Pulse(pulseWidth, 10, Globals.randomNumberGenerator.Next(3500, 4000), 0, 0);
        this.activePulse = txPulse;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.effectiveRadiatedPower = 128;
        this.radarType = "Acquisition";
        this.beamWidth = beamWidth;
        this.endToEndDuration = 1;
        this.numberOfFrames = (int)(this.endToEndScanSector / this.beamWidth);
        this.startFrameAzimuth = 135;
        this.endFrameAzimuth = 45;
        this.endToEndScanSector = Math.Abs(this.startFrameAzimuth - this.endFrameAzimuth);
        this.mainBeamAzimuth = this.startFrameAzimuth - (this.beamWidth / 2);
        this.frameOffSet = this.startFrameAzimuth;
    }
}