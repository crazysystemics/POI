class Aircraft : BattleSystem
{
    public override bool Stopped { get; set; }
    public RWR rwr;
    public List<Position> waypoint = new List<Position>();

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
        public In(int id) : base(id)
        {
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
        if (Globals.aircraftDebugPrint)
        {
            Console.WriteLine($"Aircraft {id}: \tPosition (x, y): ({position.x}, {position.y})\n");
        }
    }

    public Aircraft(Position position, int id)
    {
        this.position = position;
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