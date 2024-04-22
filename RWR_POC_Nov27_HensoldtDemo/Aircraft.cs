using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Aircraft : BattleSystem
{
    public override bool Stopped { get; set; }
    public RWR rwr;
    public List<Position> waypoints { get; set; }
    public Position currentWaypoint;
    public Position startPos = new Position(-1, -1);
    public int currentWaypointID;
    public Position nextWaypoint;
    public int nextWaypointID;
    public double nextWaypointAzimuth;

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
    public override OutParameter Get()
    {
        Out aircraftPosition = new Out(position.x, position.y, 0);
        return aircraftPosition;
    }

    public override void Set(List<InParameter> inParameter)
    {

    }

    public override void OnTick()
    {
        if (Globals.debugPrint == Globals.DebugLevel.BRIEF)
        {
            Console.WriteLine($"Aircraft {id}: \tPosition (x, y): ({position.x}, {position.y})\n");
        }

        this.nextWaypointAzimuth = PhysicalSimulationEngine.GetAngle(this.currentWaypoint, this.nextWaypoint);
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
            if (nextWaypointID < waypoints.Count - 1)
            {
                nextWaypointID++;
                nextWaypoint = waypoints[nextWaypointID];
            }
            else
            {
                //Globals.isDone = true;
                this.position = this.startPos;
                this.currentWaypoint = this.startPos;
                Globals.episodeIsDone = true;
                Globals.episodesEnded++;
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
        this.nextWaypointID = 1;
        this.id = id;
        this.startPos.x = waypoints[0].x;
        this.startPos.y = waypoints[0].y;
    }

}

public class Position
{
    public int x { get; set; }
    public int y { get; set; }

    public Position(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }
}