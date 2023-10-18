using System.Runtime.CompilerServices;

class Aircraft : BattleSystem
{
    public override bool Stopped { get; set; }
    public RWR rwr;
    public Dictionary<int, Position> waypoints = new Dictionary<int, Position>();
    public int currentWaypointID = 0;
    public int nextWaypointID = 1;
    public int[] legVelocity = new int[2];
    public Position nextWaypoint;
    //   public Position currentPosition = new Position(0, 0);

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
        this.position.x = ((In)inParameter[0]).posX;
        this.position.y = ((In)inParameter[0]).posY;
    }

    public override void OnTick()
    {
        if (Globals.aircraftDebugPrint)
        {
            Console.WriteLine($"Aircraft {id}: \tPosition (x, y): ({position.x}, {position.y})\n");
            Globals.aircraftDebugPrint = false;
        }
        computeVelocity();
        if (computeDistance(position, nextWaypoint) <= 1)
        {
            // Change waypoint
            // this.currentWaypointID = this.nextWaypointID
        }
        else
        {
            // Move aircraft
            // this.position.x += (legVelocity[0] * 1)
            // this.position.y += (legVelocity[0] * 1)
            // How and where to set this?
        }
    }

    public int computeDistance(Position pos1, Position pos2)
    {
        int dist = (int)Math.Sqrt(((pos2.x - pos1.x) * (pos2.x - pos1.x)) + ((pos2.y - pos1.y) * (pos2.y - pos1.y)));
        return dist;
    }

    public int[] computeVelocity()
    {
        int x_dist = Math.Abs(this.waypoints[currentWaypointID].x - this.waypoints[nextWaypointID].x);
        int y_dist = Math.Abs(this.waypoints[currentWaypointID].y - this.waypoints[nextWaypointID].y);
        this.legVelocity[0] = x_dist / (nextWaypointID - currentWaypointID);
        this.legVelocity[1] = y_dist / (nextWaypointID - currentWaypointID);
        return legVelocity;
    }

    public Aircraft(Dictionary<int, Position> waypoints, int id)
    {
        this.position = waypoints[0];
        this.waypoints = waypoints;
        this.nextWaypoint = waypoints[1];
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