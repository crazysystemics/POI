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
    public override float RadarRange { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override List<BattleSystemClass> ObjectsSurveyed { get; set; }

    // Maintain separate list of radars visible by RWR

    public override SituationalAwareness Get()
    {
        SituationalAwareness sit_aw_obj = new SituationalAwareness(this.CurrentPosition, this.VehicleID, this.Type);
        return sit_aw_obj;
    }

    public override void Set(List<SimulationModel> sim_mod)
    {

        // Adds objects to ObjectVisible property if they are within range of radar or RWR and removes them when they are not

        // Parameters = list of battle systems

        // Computation to be done in PSE

        foreach (var batt_cls in sim_mod)
        {
            if (batt_cls is PhysicalSimulationEngine)
            {
                sim_mod.Remove(batt_cls);
            }
        }

        foreach (BattleSystemClass battle_system in sim_mod)
        {

            if (this != battle_system)
            {
                float dist = Globals.DistanceCalculator(this.CurrentPosition, battle_system.CurrentPosition);
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

    public override void OnTick()
    {

        float time_resolution = Globals.TimeResolution;

        // Compute next position here

        this.ComputeVelocity();
        for (int i = 0; i < this.VehiclePath.Count - 1; i++)
        {
            if (MathF.Abs(Globals.DistanceCalculator(this.CurrentPosition, this.NextWaypoint)) <= (this.LegVelocity[2] * time_resolution))
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

        Console.WriteLine($"(Vx, Vy) = ({this.LegVelocity[0]}, {this.LegVelocity[1]})");
        Console.WriteLine($"(x, y) = ({this.CurrentPosition[0]}, {this.CurrentPosition[1]})");

        Console.WriteLine("\n----------------");
        Console.WriteLine($"Objects visible to {this.Type} {this.VehicleID}:");
        foreach(var vis_obj in this.ObjectsVisible)
        {
            float dist = Globals.DistanceCalculator(this.CurrentPosition, vis_obj.CurrentPosition);
            float angle = Globals.AngleCalculator(this.CurrentPosition, vis_obj.CurrentPosition);
            Console.WriteLine($"{vis_obj.Type} {vis_obj.VehicleID} at distance = {dist} and angle = {Math.Abs(angle)} radians");
        }

        if (!this.VehicleHasStopped)
        {

            // Computes new positions (and other attributes) based on physical situation

            this.NewPositionTemp[0] = this.CurrentPosition[0] + (this.LegVelocity[0] * time_resolution);
            this.NewPositionTemp[1] = this.CurrentPosition[1] + (this.LegVelocity[1] * time_resolution);
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

    public void ComputeVelocity()
    {
        this.LegVelocity[0] = (this.NextWaypoint[0] - this.CurrentPosition[0]) / MathF.Abs(Globals.CurrentTime - this.NextWaypoint[2]);
        this.LegVelocity[1] = (this.NextWaypoint[1] - this.CurrentPosition[1]) / MathF.Abs(Globals.CurrentTime - this.NextWaypoint[2]);
        this.LegVelocity[2] = MathF.Sqrt((this.LegVelocity[0] * this.LegVelocity[0]) + (this.LegVelocity[1] * this.LegVelocity[1]));
    }

    public Aircraft(List<float[]> waypoints, float radar_range)
    {
        // Waypoint should should have waypoints and time of arrival and compute velocity from that
        // Object of Aircraft class takes a List of waypoints (float array of size 2), an array of velocities (size = waypoint list size - 1)
        // and a float indicating Radar range.

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        NextWaypoint = waypoints[1];
        VehiclePath = waypoints;
        RadarRange = radar_range;
        VehicleHasStopped = false;
        Type = "Aircraft";
        ObjectsVisible = new List<BattleSystemClass>();
        ObjectsSurveyed = new List<BattleSystemClass>();
        LegVelocity = new float[3];
        CurrWaypointID = 0;
        ObjectRegister.s_AircraftID++;
        VehicleID = ObjectRegister.s_AircraftID;
        ComputeVelocity();
        // Velocities are in direction of any given waypoint leg, decomposing velocities into Vx and Vy
    }
}
