using System.Collections;

class Aircraft : BattleSystem
{
    public override bool Stopped { get; set; }
    public RWR rwr;
    public List<Position> waypoints = new();
    public Position currentWaypoint;
    public int currentWaypointID;
    public Position nextWaypoint;
    public int nextWaypointID;
    public int movementTicks;
    public int[] legVelocity = new int[2];
    //public Position currentPosition = new Position(0, 0);

    public class Out : OutParameter
    {
        public int Ox;
        public int Oy;
        public Out(int x, int y, int id) : base(id)
        {
            this.Ox = x;
            this.Oy = y;
        }
    }

    public class In : InParameter
    {

        public int posX;
        public int posY;
        public In(int posX, int posY, int id) : base(id)
        {
            this.posX = posX;
            this.posY = posY;
            this.ID = id;
        }
    }
    public override Out Get()
    {
        Out aircraftPosition = new(position.x, position.y, 0);
        return aircraftPosition;
    }

    public override void Set(List<InParameter> inParameter)
    {

    }

    public override void OnTick()
    {
        // Aircraft moves once every tick.

        // We first find the ratio of x and y distances to the next waypoint. This ratio is moveRatio.

        // We always choose the bigger value between the two as the numerator.

        // On each tick, the aircraft moves moveRatio number of cells in the direction of bigger distance
        // and 1 cell in the direction of smaller distance.

        // If one of the distances is zero, the aircraft moves one cell per tick only in that direction and moveRatio is not used.

        // The number of steps taken by Aircraft to reach the next waypoint is equal to the smaller distance value that is not zero.
        // In case one of the distances is zero, Aircraft covers the non-zero distance in number of steps equal to the distance.

        // For example, if the aircraft moves from (5, 5) to (10, 15), the x-distance is the smaller distance, and is 5 cells away.
        // The aircraft in this case will cover 5 cells in x direction in 5 ticks and 10 cells in y direction the same 5 ticks.

        // But if the aircraft is moving from (5, 5) to (20, 5), the y-distance is zero, and hence the aircraft takes x-distance
        // number of ticks to reach the next waypoint, i.e. 15 ticks, since the x-distance is 15.

        //int[] distToNextWaypoint = computeDistance(currentWaypoint, nextWaypoint);

        Console.WriteLine($"Aircraft {id}: \tPosition (x, y): ({position.x}, {position.y})\n");
        Console.WriteLine($"nextWaypoint = {nextWaypoint.x}, {nextWaypoint.y}");
        //Console.WriteLine($"distance to next waypoint = {distToNextWaypoint[0]}, {distToNextWaypoint[1]}");

        MoveAircraft();
    }

    public static int[] ComputeDistance(Position pos1, Position pos2)
    {
        int[] dist = new int[] { (int)(pos2.x - pos1.x), (int)(pos2.y - pos1.y) };
        return dist;
    }
    public void MoveAircraft()
    {
        int moveRatio;
        int[] displacementArray = ComputeDistance(this.currentWaypoint, this.nextWaypoint);
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

        if (this.position.x == nextWaypoint.x && this.position.y == nextWaypoint.y)
        {
            if (nextWaypointID != waypoints.Count - 1)
            {
                nextWaypointID++;
                nextWaypoint = waypoints[nextWaypointID];
            }
        }
    }


    public Aircraft(List<Position> waypoints, int id)
    {
        this.position = waypoints.ToList()[0];
        this.waypoints = waypoints;
        this.currentWaypoint = waypoints[0];
        this.nextWaypoint = waypoints[1];
        this.currentWaypointID = 0;
        this.movementTicks = 0;
        this.nextWaypointID = 1;
        this.id = id;
    }
}

public class Position
{
    public int x;
    public int y;

    public Position(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }
}