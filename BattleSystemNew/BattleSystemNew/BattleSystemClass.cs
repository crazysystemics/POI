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


abstract class BattleSystemClass
{
    public abstract string Type { get; set; }
    public abstract int VehicleID { get; set; }
    public abstract int CurrWaypointID { get; set; }
    public abstract float[] LegVelocity { get; set; }
    public abstract float[] CurrentPosition { get; set; }
    public abstract float[] NewPositionTemp { get; set; }
    public abstract float[] NextWaypoint { get; set; }
    public abstract float Velocities { get; set; }
    public abstract float RadarRange { get; set; }
    public abstract bool VehicleHasStopped { get; set; }
    public abstract bool VelocityChanged { get; set; }
    public abstract List<float[]> VehiclePath { get; set; }
    public abstract List<BattleSystemClass> ObjectsVisible { get; set; }
    public abstract List<BattleSystemClass> ObjectsSurveyed { get; set; }
    public abstract float[] Get(PhysicalSimulationEngine simeng);
    public abstract void Set(PhysicalSimulationEngine simeng);
    public abstract void OnTick(float timer, PhysicalSimulationEngine simeng);
    public abstract void DecompVelocity();
}
