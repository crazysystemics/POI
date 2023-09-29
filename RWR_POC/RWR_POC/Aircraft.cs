// Naming conventions:
// avoid names with underscores

// Class names: Begin with Upper case. CamelCase
// Member objects: Begin with lowercase. camelCase
// Parameters: Begin with lowercase. camelCase
// Local variables: Begin with lowercase. camelCase
// Method names: Begin with Upper case. CamelCase
// Use Microsoft standard naming conventions for C#

using System.Dynamic;

class Aircraft : BattleSystem
{
    public override bool Stopped { get; set; }
    public RWR rwr;
 //   public Position currentPosition = new Position(0, 0);

    public class Out:OutParameter
    {
        public int Ox;
        public int Oy;
        public Out(int x, int y, int id):base(id)
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
        Console.WriteLine("Tick : " + Globals.Tick +" Aircraft :\t\t Current Position x:" + position.x +", y:" + position.y);
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