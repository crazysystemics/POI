public class FireControlRadar : Radar
{
    public override bool Stopped { get; set; }

    public int frameOffSet;
    public int numberOfFrames;
    public int startFrameAzimuth;
    public int endFrameAzimuth;
    public bool targetObtained;
    public bool launchedMissile;
    public Position targetPosition = new Position(0, 0);
    public Missile missile1 = new Missile(new Position(0, 0), new Position(0, 0));


    public class In : InParameter
    {
        public Position targPos;
        public In(Position targPos, int id) : base(id)
        {
            this.targPos = targPos;
        }
    }
    public override OutParameter Get()
    {
        Out radarOutput = new Out(txPulse, position, txTick, 1);
        return radarOutput;
    }

    public override void Set(List<InParameter> inParameters)
    {
        foreach (InParameter inParameter in inParameters)
        {
            if (((In)inParameter).targPos != null && !this.targetObtained)
            {    
                this.targetObtained = true;
                this.targetPosition = ((In)inParameter).targPos;
            }
        }
    }

    public override void OnTick()
    {
        int targetDistance = PhysicalSimulationEngine.GetDistance(this.targetPosition, this.position);
        if (targetObtained && targetDistance <= radius)
        {
            this.mainBeamAzimuth = (int)(PhysicalSimulationEngine.GetAngle(this.targetPosition, this.position) * (180 / Math.PI));
            if (targetDistance <= radius)
            {
                this.missile1.launched = true;
                this.missile1.targetPosition = this.targetPosition;
            }
        }

        Console.WriteLine($"Target obtained: {this.targetObtained}");
        Console.WriteLine($"Target position: {this.targetPosition.x}, {this.targetPosition.y}");
        Console.WriteLine($"Fire control radar azimuth: {this.mainBeamAzimuth}");
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

    public FireControlRadar(Position position, int radius, int beamWidth, int txTick, int id, int startFrameAzimuth = 135, int endFrameAzimuth = 45)
    {
        this.pulseRepetitionInterval = Globals.randomNumberGenerator.Next(500, 800);
        int pulseWidth = (Globals.randomNumberGenerator.Next(10, 15) * pulseRepetitionInterval) / 100;
        this.txPulse = new Pulse(pulseWidth, 10, Globals.randomNumberGenerator.Next(8500, 9000), 0, 0);
        this.targetObtained = false;
        this.activePulse = txPulse;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.effectiveRadiatedPower = 128;
        this.radarType = Globals.RadarTypes.FIRECONTROL;
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

        this.endToEndScanSector = Math.Abs(this.startFrameAzimuth - this.endFrameAzimuth);
        this.mainBeamAzimuth = this.startFrameAzimuth - (this.beamWidth / 2);
        this.frameOffSet = this.startFrameAzimuth;
        this.launchedMissile = false;
        this.missile1.position = new Position(this.position.x, this.position.y);
    }

    public class Missile : BattleSystem
    {
        public Position targetPosition;
        public bool reachedTarget;
        public bool launched;
        public override bool Stopped { get; set; }

        public override OutParameter Get()
        {
            return null;
        }

        public override void Set(List<InParameter> inParameters)
        {

        }

        public override void OnTick()
        {
            if (this.launched)
            {
                Console.WriteLine($"Missile position: {this.position.x}, {this.position.y}");
                MoveMissile();
            }
        }

        public static int[] ComputeDistance(Position pos1, Position pos2)
        {
            int[] dist = new int[] { (int)(pos2.x - pos1.x), (int)(pos2.y - pos1.y) };
            return dist;
        }

        public void MoveMissile()
        {
            int moveRatio;
            int[] displacementArray = ComputeDistance(this.position, this.targetPosition);
            int[] distanceToNextWaypoint = new int[2];
            distanceToNextWaypoint[0] = Math.Abs(displacementArray[0]);
            distanceToNextWaypoint[1] = Math.Abs(displacementArray[1]);

            bool minIsZero = distanceToNextWaypoint.Min() == 0;

            // Case: neither of the displacements are zero

            if (!minIsZero)
            {
                moveRatio = (int)(distanceToNextWaypoint.Max() / distanceToNextWaypoint.Min());

                // Case 1: Both x-displacment and y-displacement are positive

                if (displacementArray[0] > 0 && displacementArray[1] > 0)
                {

                    // If x_distance > y-distance

                    if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                    {
                        // x = distanceToNextWaypoint[0] * moveRatio
                        this.position.x += moveRatio;
                        this.position.y += 1;
                    }

                    // If y-distance > x-distance

                    else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                    {
                        this.position.x += 1;
                        this.position.y += moveRatio;
                    }

                    // If x-distance = y-distance

                    else
                    {
                        this.position.x += 1;
                        this.position.y += 1;
                    }
                }

                // Case 2: Both x-displacement and y-displacement are negative

                if (displacementArray[0] < 0 && displacementArray[1] < 0)
                {

                    // If x-distance > y-distance

                    if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                    {
                        this.position.x += -moveRatio;
                        this.position.y += -1;
                    }

                    // If y-distance > x-distance

                    else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                    {
                        this.position.x += -1;
                        this.position.y += -moveRatio;
                    }

                    // If x-distance = y-distance

                    else
                    {
                        this.position.x += -1;
                        this.position.y += -1;
                    }
                }

                // Case 3: x-displacement is positive, y-displacement is negative

                if (displacementArray[0] > 0 && displacementArray[1] < 0)
                {

                    // If x-distance > y-distance

                    if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                    {
                        this.position.x += moveRatio;
                        this.position.y += -1;
                    }

                    // If y-distance > x-distance

                    else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                    {
                        this.position.x += 1;
                        this.position.y += -moveRatio;
                    }

                    // If x-distance = y-distance

                    else
                    {
                        this.position.x += 1;
                        this.position.y += -1;
                    }
                }

                // Case 4: x-displacement is negative, y-displacement is positive

                if (displacementArray[0] < 0 && displacementArray[1] > 0)
                {

                    // If x-distance > y-distance

                    if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                    {
                        this.position.x += -moveRatio;
                        this.position.y += 1;
                    }

                    // If y-distance > x-distance

                    else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                    {
                        this.position.x += -1;
                        this.position.y += moveRatio;
                    }

                    // If x-distance = y-distance

                    else
                    {
                        this.position.x += -1;
                        this.position.y += 1;
                    }
                }
            }

            // Case: At least one of the displacements is zero

            else
            {
                // Case 1: If x-displacement is zero

                if (displacementArray[0] == 0)
                {

                    // If y-displacement is positive

                    if (displacementArray[1] > 0)
                    {
                        this.position.y += 1;
                    }

                    // If y-displacement is negative

                    else if (displacementArray[1] < 0)
                    {
                        this.position.y += -1;
                    }
                }

                // Case 2: If y-displacement is zero

                if (displacementArray[1] == 0)
                {

                    // If x-displacement is positive

                    if (displacementArray[0] > 0)
                    {
                        this.position.x += 1;
                    }

                    // If x-displacement is negative

                    else if (displacementArray[0] < 0)
                    {
                        this.position.x += -1;
                    }
                }
            }

            if (this.position.x == targetPosition.x && this.position.y == targetPosition.y)
            {
                this.reachedTarget = true;
            }
        }

        public Missile(Position missilePosition, Position targetPosition)
        {
            this.position = missilePosition;
            this.targetPosition = targetPosition;
        }
    }
}