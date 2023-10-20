using System.Runtime.CompilerServices;

class Aircraft : BattleSystem
{
    public override bool Stopped { get; set; }
    public RWR rwr;
    public List<Position> waypoints = new List<Position>();
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
        Out aircraftPosition = new Out(position.x, position.y, 0);
        return aircraftPosition;
    }

    public override void Set(List<InParameter> inParameter)
    {

    }

    public override void OnTick()
    {

        // Aircraft moves once every tick.

        // We first find the ratio of y-distance and x-distance to the next waypoint. This ratio is moveRatio.

        // We always choose the bigger value between the two as the numerator.

        // On each tick, the aircraft moves moveRatio number of cells in the direction of bigger distance
        // and 1 cell in the direction of smaller distance.

        // If one of the distances is zero, the aircraft moves one cell per tick only in that direction and moveRatio is not used.

        // The number of steps taken by Aircraft to reach the next waypoint is equal to the smaller distance value that is not zero.
        // In case one of the distances is zero, Aircraft covers the non-zero distance in number of steps equal to the distance.
        // For example, if the aircraft moves from (5, 5) to (10, 15), the x-distance is the smaller distance, and is 5 cells away.
        // The aircraft in this case will cover 5 cells in x direction in 5 ticks and 10 cells in y direction the same 5 ticks.

        int moveRatio = 0;

        Console.WriteLine($"Aircraft {id}: \tPosition (x, y): ({position.x}, {position.y})\n");

        if (this.position.x == nextWaypoint.x && this.position.y == nextWaypoint.y)
        {
            if (nextWaypointID != waypoints.Count - 1)
            {
                nextWaypointID++;
                nextWaypoint = waypoints[nextWaypointID];
            }
        }

        int[] distToNextWaypoint = computeDistance(currentWaypoint, nextWaypoint);

        Console.WriteLine($"nextWaypoint = {nextWaypoint.x}, {nextWaypoint.y}");
        Console.WriteLine($"distance to next waypoint = {distToNextWaypoint[0]}, {distToNextWaypoint[1]}");

        if (distToNextWaypoint.Min() != 0)
        {
            moveRatio = distToNextWaypoint.Max() / distToNextWaypoint.Min();

            if (Math.Abs(distToNextWaypoint[0]) == Math.Abs(distToNextWaypoint[1]))
            {

                if (distToNextWaypoint[0] == distToNextWaypoint[1])
                {
                    if (distToNextWaypoint[0] < 0 && distToNextWaypoint[1] < 0)
                    {
                        this.position.x += -moveRatio;
                        this.position.y += -moveRatio;
                    }
                    else
                    {
                        this.position.x += moveRatio;
                        this.position.y += moveRatio;
                    }
                }

                if (distToNextWaypoint[0] > 0 && moveRatio < 0)
                {
                    this.position.x += -moveRatio;
                    this.position.y += moveRatio;
                }
                if (distToNextWaypoint[1] > 0 && moveRatio < 0)
                {
                    this.position.y += -moveRatio;
                    this.position.x += moveRatio;
                }

            }
            else if (distToNextWaypoint[0] < 0 && distToNextWaypoint[1] == 0)
            {
                this.position.x += -1;
            }
            else if (distToNextWaypoint[1] < 0 && distToNextWaypoint[0] == 0)
            {
                this.position.y += -1;
            }

            else if (distToNextWaypoint[0] > distToNextWaypoint[1] && distToNextWaypoint[0] != 0)
            {
                this.position.x += moveRatio;
                this.position.y += 1;
            }
            else if (distToNextWaypoint[1]  > distToNextWaypoint[0] && distToNextWaypoint[1] != 0)
            {
                this.position.y += moveRatio;
                this.position.x += 1;
            }

        }
        else
        {
            if (Math.Abs(distToNextWaypoint[0]) > Math.Abs(distToNextWaypoint[1]))
            {
                if (distToNextWaypoint[1] < 0)
                {
                    this.position.x += -1;
                }
                else
                {
                    this.position.x += 1;
                }
            }
            else if(Math.Abs(distToNextWaypoint[0]) <= Math.Abs(distToNextWaypoint[1]))
            {
                if (distToNextWaypoint[0] < 0)
                {
                    this.position.y += -1;
                }
                else
                {
                    this.position.y += 1;
                }
            }
        }
    }

    public int[] computeDistance(Position pos1, Position pos2)
    {
        int[] dist = new int[] { (int)(pos2.x - pos1.x), (int)(pos2.y - pos1.y) };
        return dist;
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