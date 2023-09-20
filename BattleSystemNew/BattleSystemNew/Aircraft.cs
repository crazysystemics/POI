/* The Aircraft class inherits from BattleSystemClass. It contains data pertaining to flight path, velocities
 * RWR range, and its current state. It also inherits Get(), OnTick() and Set() methods.
 * 
 * The Get() method is currently empty, since any data from the object is obtained by the PhysicalSimulationEngine.
 * 
 * The OnTick() method determines the next waypoint based on velocity and path information.
 * 
 * The Set() method updates its internal list of ObjectVisible based on Radars detected by RWR (pending proper implementation)
 * 
 * It also has its own methods called DistanceCalculator, AngleCalculator and DecompVelocity, which are used for calculating
 * distances to other points in space, as well as decompose its velocity vector into cartesian components.
 */


class Aircraft : BattleSystemClass
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
    public override List<float[]> VehiclePath { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override List<BattleSystemClass> ObjectsSurveyed { get; set; }

    public override void Get()
    {

    }

    public override void Set(PhysicalSimulationEngine simeng)
    {

        // Adds objects to ObjectVisible property if they are within range of radar or RWR and removes them when they are not

        foreach (var battle_system in simeng.physicalSituationalAwareness)
        {
            if (this != battle_system)
            {
                float dist = DistanceCalculator(this.CurrentPosition, battle_system.CurrentPosition);
                if (dist <= this.RadarRange && !this.ObjectsVisible.Contains(battle_system))
                {
                    this.ObjectsVisible.Add(battle_system);
                    this.ObjectsSurveyed.Add(battle_system);
                }
                else if (dist > this.RadarRange && this.ObjectsVisible.Contains(battle_system))
                {
                    this.ObjectsVisible.Remove(battle_system);
                }
            }
        }
    }

    public override void OnTick(float timer)
    {

        this.DecompVelocity();
        for (int i = 0; i < this.VehiclePath.Count - 1; i++)
        {
            if (MathF.Abs(DistanceCalculator(this.CurrentPosition, this.NextWaypoint)) <= (this.Velocities * timer))
            {
                if (!this.VehicleHasStopped || this.NextWaypoint != this.VehiclePath.Last())
                {
                    this.CurrWaypointID++;
                    if (this.CurrWaypointID < this.VehiclePath.Count)
                    {
                        this.NextWaypoint = this.VehiclePath[this.CurrWaypointID];
                    }
                    else if (this.CurrWaypointID == this.VehiclePath.Count)
                    {
                        this.VehicleHasStopped = true;
                    }
                }
            }
        }

        if (VehicleHasStopped)
        {
            Console.WriteLine($"\n{this.Type} {this.VehicleID} reached the end of path");
            Console.WriteLine($"Objects surveyed by {this.Type} {this.VehicleID}:");
            foreach (var obj_surv in this.ObjectsSurveyed)
            {
                Console.WriteLine($"{obj_surv.Type} {obj_surv.VehicleID} at ({obj_surv.CurrentPosition[0]}, {obj_surv.CurrentPosition[1]})");
            }
        }
    }

    public void DecompVelocity()
    {
        this.LegVelocity[0] = this.Velocities * MathF.Cos(AngleCalculator(this.CurrentPosition, this.NextWaypoint));
        this.LegVelocity[1] = this.Velocities * MathF.Sin(AngleCalculator(this.CurrentPosition, this.NextWaypoint));
    }

    public float DistanceCalculator(float[] obj1, float[] obj2)
    {
        float x = obj1[0] - obj2[0];
        float y = obj1[1] - obj2[1];
        return MathF.Sqrt((x * x) + (y * y));
    }

    public float AngleCalculator(float[] obj1, float[] obj2)
    {
        float x = obj2[0] - obj1[0];
        float y = obj2[1] - obj1[1];
        float v = MathF.Atan2(y, x);
        return v;
    }

    public Aircraft(List<float[]> waypoints, float velocities, float radar_range)
    {

        // Object of Aircraft class takes a List of waypoints (float array of size 2), an array of velocities (size = waypoint list size - 1)
        // and a float indicating Radar range.

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        NextWaypoint = waypoints[1];
        VehiclePath = waypoints;
        Velocities = velocities;
        RadarRange = radar_range;
        VehicleHasStopped = false;
        Type = "Aircraft";
        ObjectsVisible = new List<BattleSystemClass>();
        ObjectsSurveyed = new List<BattleSystemClass>();
        LegVelocity = new float[2];
        CurrWaypointID = 0;
        ObjectRegister.s_AircraftID++;
        VehicleID = ObjectRegister.s_AircraftID;
        DecompVelocity();
        // Velocities are in direction of any given waypoint leg, decomposing velocities into Vx and Vy
    }
}
