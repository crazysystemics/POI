using System;
using System.Collections.Generic;

public class AcquisitionRadar : Radar
{
    public override bool Stopped { get; set; }

    public int frameOffSet;
    public int numberOfFrames;
    public int startFrameAzimuth;
    public int endFrameAzimuth;
    public Position targetPosition = new Position(0, 0);


    public class In : InParameter
    {
        public Pulse echoPulse;
        public In(Pulse echoPulse, int id) : base(id)
        {
            this.echoPulse = echoPulse;
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
        int angle = (int)(PhysicalSimulationEngine.GetAngle(targetPosition, this.position) * (180 / Math.PI));
        if (angle < 0)
        {
            angle += 360;
        }
        this.mainBeamAzimuth -= (this.beamWidth / 2);

        if (Globals.debugPrint == Globals.DebugLevel.VERBOSE)
        {
            Console.WriteLine($"Acquisition radar azimuth: {this.mainBeamAzimuth}");
            Console.WriteLine($"Angle to target: {angle}");
        }


        if (this.mainBeamAzimuth <= this.endFrameAzimuth + (this.beamWidth / 2))
        {
            this.mainBeamAzimuth = this.startFrameAzimuth - (this.beamWidth / 2);
        }

        if (this.mainBeamAzimuth <= 0)
        {
            this.mainBeamAzimuth += 360;
        }

        if (PhysicalSimulationEngine.GetDistance(this.targetPosition, this.position) <= radius)
        {
            this.detection = false;
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
        Console.WriteLine($"Azimuth: {azimuth}");
        if (azimuth < 0)
        {
            azimuth += 360;
        }
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

    public AcquisitionRadar(Position position, int radius, int beamWidth, int txTick, int id, int startFrameAzimuth = 135, int endFrameAzimuth = 45)
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
        this.radarType = Globals.RadarTypes.ACQUISITION;
        this.beamWidth = beamWidth;
        this.endToEndDuration = 1;
        this.numberOfFrames = (int)(this.endToEndScanSector / this.beamWidth);
        this.startFrameAzimuth = startFrameAzimuth;
        this.endFrameAzimuth = endFrameAzimuth;


        // Ensuring that the scan goes clockwise from startFrameAzimuth to endFrameAzimuth.
        // Handles cases for negative values of azimuth as well as values greater than 360.
        // Also handles the case for 0 degree cross-over.

        if (this.startFrameAzimuth < 0)
        {
            if (this.startFrameAzimuth < -360)
            {
                this.startFrameAzimuth %= 360;
            }
            this.startFrameAzimuth += 360;
        }
        if (this.startFrameAzimuth > 360)
        {
            this.startFrameAzimuth %= 360;
        }
        if (this.endFrameAzimuth < 0)
        {
            if (this.endFrameAzimuth < -360)
            {
                this.endFrameAzimuth %= 360;
            }
            this.endFrameAzimuth += 360;
        }
        if (this.endFrameAzimuth > 360)
        {
            this.endFrameAzimuth %= 360;
        }
        if (this.endFrameAzimuth > this.startFrameAzimuth)
        {
            int temp1 = this.startFrameAzimuth;
            int temp2 = this.endFrameAzimuth;
            this.startFrameAzimuth = temp2;
            this.endFrameAzimuth = temp1;
        }


        this.endToEndScanSector = Math.Abs(this.startFrameAzimuth - this.endFrameAzimuth);
        this.mainBeamAzimuth = this.startFrameAzimuth - (this.beamWidth / 2);
        this.frameOffSet = this.startFrameAzimuth;
    }
}