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

        // have consistent cases for variable names, etc
        
        // Adds objects to ObjectVisible property if they are within range of radar or RWR and removes them when they are not

/*        foreach (BattleSystemClass battle_system in sim_mod)
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

        if (this.ObjectsVisible.Count > 0)
        {
            Console.WriteLine("\n----------------");
            Console.WriteLine($"\nObjects visible to {this.Type} {this.VehicleID}:\n");
            foreach (var vis_obj in this.ObjectsVisible)
            {
                float dist = Globals.DistanceCalculator(this.CurrentPosition, vis_obj.CurrentPosition);
                float angle = Globals.AngleCalculator(this.CurrentPosition, vis_obj.CurrentPosition);
                Console.WriteLine($"{vis_obj.Type} {vis_obj.VehicleID} at distance = {dist} and angle = {Math.Abs(angle)} radians");
            }
        }*/


    }

    public override void OnTick()
    {

        float time_resolution = Globals.TimeResolution;

        // Compute next position here

        // this.ComputeVelocity();

        //for (int i = 0; i < this.VehiclePath.Count - 1; i++)
        //{
        //    if (MathF.Abs(Globals.DistanceCalculator(this.CurrentPosition, this.NextWaypoint)) <= (this.LegVelocity[2] * time_resolution))
        //    {
        //        if (!this.VehicleHasStopped || this.NextWaypoint != this.VehiclePath.Last())
        //        {
        //            this.CurrWaypointID++;
        //            if (this.CurrWaypointID < this.VehiclePath.Count)
        //            {
        //                this.NextWaypoint = this.VehiclePath[this.CurrWaypointID];
        //            }
        //            else if (this.CurrWaypointID == this.VehiclePath.Count)
        //            {
        //                this.VehicleHasStopped = true;
        //            }
        //        }
        //    }
        //}

        //Console.WriteLine("\n----------------");


        //if (!this.VehicleHasStopped)
        //{

        //    // Computes new positions (and other attributes) based on physical situation

        //    this.NewPositionTemp[0] = this.CurrentPosition[0] + (this.LegVelocity[0] * time_resolution);
        //    this.NewPositionTemp[1] = this.CurrentPosition[1] + (this.LegVelocity[1] * time_resolution);
        //}

        //if (VehicleHasStopped)
        //{
        //    Console.WriteLine($"\n{this.Type} {this.VehicleID} reached the end of path");
        //    Console.WriteLine($"Objects surveyed by {this.Type} {this.VehicleID}:");
        //    foreach (var obj_surv in this.ObjectsSurveyed)
        //    {
        //        Console.WriteLine($"{obj_surv.Type} {obj_surv.VehicleID} at ({obj_surv.CurrentPosition[0]}, {obj_surv.CurrentPosition[1]})");
        //    }
        //}
    }

    //public void ComputeVelocity()
    //{
    //    this.LegVelocity[0] = (this.NextWaypoint[0] - this.CurrentPosition[0]) / MathF.Abs(Globals.CurrentTime - this.NextWaypoint[2]);
    //    this.LegVelocity[1] = (this.NextWaypoint[1] - this.CurrentPosition[1]) / MathF.Abs(Globals.CurrentTime - this.NextWaypoint[2]);
    //    this.LegVelocity[2] = MathF.Sqrt((this.LegVelocity[0] * this.LegVelocity[0]) + (this.LegVelocity[1] * this.LegVelocity[1]));
    //}

    public Aircraft(List<float[]> waypoints, float radar_range)
    {
        // Waypoint should should have waypoints and time of arrival and compute velocity from that
        // Object of Aircraft class takes a List of waypoints (float array of size 2), an array of velocities (size = waypoint list size - 1)
        // and a float indicating Radar range.

        //NewPositionTemp = waypoints[0];
        //CurrentPosition = waypoints[0];
        //NextWaypoint = waypoints[1];
        //VehiclePath = waypoints;
        //RadarRange = radar_range;
        //VehicleHasStopped = false;
        //Type = "Aircraft";
        //ObjectsVisible = new List<BattleSystem>();
        //ObjectsSurveyed = new List<BattleSystem>();
        //LegVelocity = new float[3];
        //CurrWaypointID = 0;
        //ObjectRegister.s_AircraftID++;
        //VehicleID = ObjectRegister.s_AircraftID;
        //ComputeVelocity();
        // Velocities are in direction of any given waypoint leg, decomposing velocities into Vx and Vy
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