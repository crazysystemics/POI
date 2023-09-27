/* The Aircraft class inherits from BattleSystemClass. It contains data pertaining to flight path
 * RWR range, and its current state. It also inherits Get(), OnTick() and Set() methods from SimulationModel
 * 
 * The Get() method returns a SituationalAwareness object containing its CurrentPosition, VehicleID and Type.
 * 
 * The OnTick() method determines the next waypoint based on velocity and path information and moves to the
 * new position computed by the PSE.
 * 
 * The Set() method updates its internal list of ObjectVisible based on Radars detected by RWR (pending proper implementation)
 * and also display them to the Console.
 * 
 */

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
    public AircraftPosition CurrentPosition = new AircraftPosition(0, 0);


    public int CurrWaypointID;
    public float[] LegVelocity;
    public float[] NextWaypoint;

    // Maintain separate list of radars visible by RWR

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

    // In Parameter class is null
    public override AircraftOut Get()
    {
        AircraftOut aircraft_position = new AircraftOut(CurrentPosition.x, CurrentPosition.y, 0);
        return aircraft_position;
    }

    public override void Set(List<InParameter> inparameter)
    {

    }

    public override void OnTick()
    {

    }

    public Aircraft(List<float[]> waypoints, float radar_range)
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