/* This program contains an Abstract class called BattleSystemClass, from which other classes like
 * Aircraft and Radar inherit.
 * 
 * The static class ObjectRegister starts with registering objects to a List that will be used to initialize
 * the DiscreteTimeSimulationEngine.
 * 
 * The DiscreteTimeSimulationengine only calls the Get(), OnTick() and Set() methods on a PhysicalSimulationEngine.
 * 
 * The PhysicalSimulationEngine is initialized with an empty list of BattleSystemClass objects. The Get() method
 * of this class copies the situationalAwareness list registered to the DTSE in order to perform computations
 * and subsequent manipulations. This is copied into a new list called physicalSituationalAwareness.
 * 
 * The OnTick() method of this class iterates through the physicalSituationalAwareness List and performs relevant
 * computations (currently only computes new positions for Aircraft objects). It also displays current positions
 * and velocities of all the objects in the list and also performs a check for any objects visible
 * to a radar or an Aircraft RWR (currently a part of the Aircraft object) and displays its distance and azimuth.
 * 
 * The Set() method of this class performs a distance check between objects in physicalSituationalAwareness and
 * adds objects to the ObjectsVisible property if it is within the given range. This method also sets new values
 * for position (and other attributes/properties) that were computed in the OnTick() method. The new values are applied
 * to the objects in the original situationalAwareness list maintained by the DTSE. It also updates the ObjectsVisible
 * list in the objects of situationalAwarness, rather than its copy in physicalSitautionalAwareness.
 *  */


class Radar : BattleSystemClass
{
    public override string Type { get; set; }
    public override int VehicleID { get; set; }
    public override int CurrWaypointID { get; set; }
    public override float[] LegVelocity { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] NewPositionTemp { get; set; }
    public override float[] NextWaypoint { get; set; }
    public override float Velocities { get; set; }
    public override float RadarRange { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override bool VelocityChanged { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override List<BattleSystemClass> ObjectsSurveyed { get; set; }
    public override float[] Get(PhysicalSimulationEngine simeng)
    {
        return CurrentPosition;
    }

    public override void OnTick(float timer, PhysicalSimulationEngine simeng)
    {

    }

    public override void Set(PhysicalSimulationEngine simeng)
    {

    }

    public override void DecompVelocity()
    {

    }

    public Radar(List<float[]> waypoints, float velocities, float radar_range)
    {

        // Object of Radar class takes the same arguments as Aircraft, but the List of waypoints only contains one item
        // and the array of velocities has one item with the value 0.0

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        VehiclePath = waypoints;
        Velocities = velocities;
        VehicleHasStopped = true;
        RadarRange = radar_range;
        NextWaypoint = new float[] { 0.0f, 0.0f };
        LegVelocity = new float[] { 0.0f, 0.0f };
        Type = "Radar";
        CurrWaypointID = 0;
        ObjectsVisible = new List<BattleSystemClass>();
        ObjectsSurveyed = new List<BattleSystemClass>();
        ObjectRegister.s_RadarID++;
        VehicleID = ObjectRegister.s_RadarID;
    }
}
