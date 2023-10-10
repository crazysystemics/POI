class Aircraft : BattleSystem
{
    public override bool Stopped { get; set; }
    public RWR rwr;
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
        if (Globals.debugPrint)
        {
            Console.WriteLine($"Aircraft {id}: \t\t Position (x, y): ({position.x}, {position.y})\n");
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