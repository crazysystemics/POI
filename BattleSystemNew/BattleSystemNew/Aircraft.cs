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
    public AircraftPosition currentPosition = new AircraftPosition(0, 0);

    public class AircraftOut:OutParameter
    {
        public int Ox;
        public int Oy;
        public AircraftOut(int x, int y, int id):base(id)
        {
            this.Ox = x;
            this.Oy = y;
        }
    }
    public override AircraftOut Get()
    {
        AircraftOut aircraftPosition = new AircraftOut(currentPosition.x, currentPosition.y, 0);
        return aircraftPosition;
    }

    public override void Set(List<InParameter> inParameter)
    {

    }

    public override void OnTick()
    {

    }

    public Aircraft()
    {

    }
}

class AircraftPosition
{
    public int x;
    public int y;

    public AircraftPosition(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }
}