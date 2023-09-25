/* The Radar class inherits from BattleSystemClass. It contains data pertaining to its state, such as current position
 * and its range. This radar is assumed to have an isometric scan, so beam direction is irrelevant in this case.
 * 
 * The Get() method is currently empty, since any data from the object is obtained by the PhysicalSimulationEngine.
 * 
 * The OnTick() method is currently not computing anything. Potential to add direction/angle based computations
 * when the Radar gets more complex.
 * 
 * The Set() method updates its internal list of ObjectVisible based aircraft that are within its range.
 * 
 * It also has its own methods called DistanceCalculator and AngleCalculator to determine its distance from other
 * points in 2D space.
 */


class Radar : BattleSystemClass
{
    public override string Type { get; set; }
    public override int VehicleID { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] NewPositionTemp { get; set; }
    public override float RadarRange { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override List<BattleSystemClass> ObjectsSurveyed { get; set; }

    public override SituationalAwareness Get()
    {
        SituationalAwareness sit_aw_obj = new SituationalAwareness(this.CurrentPosition, this.VehicleID, this.Type);
        return sit_aw_obj;
    }

    public override void OnTick()
    {
/*        Console.WriteLine("\n----------------");
        Console.WriteLine($"Objects visible to {this.Type} {this.VehicleID}:");
        foreach (var vis_obj in this.ObjectsVisible)
        {
            float dist = Globals.DistanceCalculator(this.CurrentPosition, vis_obj.CurrentPosition);
            float angle = Globals.AngleCalculator(this.CurrentPosition, vis_obj.CurrentPosition);
            Console.WriteLine($"{vis_obj.Type} {vis_obj.VehicleID} at distance = {dist} and angle = {Math.Abs(angle)} radians");
        }*/
    }

    public override void Set(List<SimulationModel> sim_mod)
    {
        // Adds objects to ObjectVisible property if they are within range of radar or RWR and removes them when they are not

        foreach (BattleSystemClass battle_system in sim_mod)
        {
            if (this != battle_system)
            {
                float dist = Globals.DistanceCalculator(this.CurrentPosition, battle_system.CurrentPosition);
                if (dist <= this.RadarRange && !this.ObjectsVisible.Contains(battle_system))
                {
                    this.ObjectsVisible.Add(battle_system);
                }
                else if (dist > this.RadarRange && this.ObjectsVisible.Contains(battle_system))
                {
                    this.ObjectsVisible.Remove(battle_system);
                }
            }
        }
    }
    public Radar(List<float[]> waypoints, float radar_range)
    {

        // Object of Radar class takes the same arguments as Aircraft, but the List of waypoints only contains one item
        // and the array of velocities has one item with the value 0.0

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        VehiclePath = waypoints;
        VehicleHasStopped = true;
        RadarRange = radar_range;
        Type = "Radar";
        ObjectsVisible = new List<BattleSystemClass>();
        ObjectsSurveyed = new List<BattleSystemClass>();
        ObjectRegister.s_RadarID++;
        VehicleID = ObjectRegister.s_RadarID;
    }
}
