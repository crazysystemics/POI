namespace BattleSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            SimulationEngine SimEng = new SimulationEngine();
            SimEng.RegisterVehicle(new Aircraft(new List<float[]>
                                                        {
                                                         // Waypoints

                                                         new float[] { 5.0f, 5.0f },
                                                         new float[] { 10.0f, 10.0f },
                                                         new float[] { 15.0f, 10.0f },
                                                         new float[] { 20.0f, 5.0f },
                                                         new float[] { 15.0f, 0.0f },
                                                         new float[] { 10.0f, 0.0f },
                                                         new float[] { 5.0f, 5.0f }, },

                                                         // Velocities

                                                         1.0f, 7.5f));

            SimEng.RegisterVehicle(new Radar(new List<float[]>
                                                        {new float[] { 15.0f, 10.0f }},
                                                         0.0f, 7.5f));

            SimEng.RegisterVehicle(new Radar(new List<float[]>
                                                        {new float[] { 25.0f, 5.0f }},
                                                         0.0f, 7.5f));

            SimEng.RegisterVehicle(new Radar(new List<float[]>
                                                        {new float[] { 15.0f, 0.0f }},
                                                         0.0f, 7.5f));

            while (!SimEng.allVehiclesStopped)
            {

                // Run until all non-stationary battle_systems come to a stop

                Console.WriteLine($"\nPosition after {i + 1} ticks:");
                SimEng.RunSimulationEngine(1.00f);
                Console.WriteLine("Press Enter/Return to display next tick");
                Console.ReadLine();
                i++;
            }
        }
    }
}


abstract class BattleSystemClass
{
    public abstract string Type { get; set; }
    public abstract int VehicleID { get; set; }
    public abstract List<float[]> VehiclePath { get; set; }
    public abstract float[] LegVelocity { get; set; }
    public abstract float Velocities { get; set; }
    public abstract float[] CurrentPosition { get; set; }
    public abstract float[] NewPositionTemp { get; set; }
    public abstract float[] NextWaypoint { get; set; }
    public abstract int CurrWaypointID { get; set; }
    public abstract int InLeg { get; set; }
    public abstract bool VehicleHasStopped { get; set; }
    public abstract bool VelocityChanged { get; set; }
    public abstract float RadarRange { get; set; }
    public abstract float MissileRange { get; set; }
    public abstract float ElapsedTime { get; set; }
    public abstract List<BattleSystemClass> ObjectsVisible { get; set; }
    public abstract List<BattleSystemClass> ObjectsSurveyed { get; set; }
    public abstract float[] Get();
    public abstract void Set();
    public abstract void OnTick(float timer);
    public abstract void DecompVelocity();
}

class Aircraft : BattleSystemClass
{
    public override string Type { get; set; }
    public override int VehicleID { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override float[] LegVelocity { get; set; }
    public override float Velocities { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] NewPositionTemp { get; set; }
    public override float[] NextWaypoint { get; set; }
    public override int CurrWaypointID { get; set; }
    public override int InLeg { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override bool VelocityChanged { get; set; }
    public override float RadarRange { get; set; }
    public override float MissileRange { get; set; }
    public override float ElapsedTime { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override List<BattleSystemClass> ObjectsSurveyed { get; set; }

    public override float[] Get()
    {
        return CurrentPosition;
    }
    public override void OnTick(float timer)
    {

        // Compute new positions
        if (!VehicleHasStopped)
        {
            NewPositionTemp[0] = CurrentPosition[0] + (LegVelocity[0] * timer);
            NewPositionTemp[1] = CurrentPosition[1] + (LegVelocity[1] * timer);
        }

        foreach (var battle_system in BattleSOS.BattleSysList)
        {
            float dist = DistanceCalculator(battle_system.CurrentPosition, this.CurrentPosition);

            if (this != battle_system)
            {
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

        Console.WriteLine($"\n{this.Type} {this.VehicleID}");
        Console.WriteLine($"(x, y) = ({this.CurrentPosition[0]},{this.CurrentPosition[1]})" +
                          $"\n(Vx, Vy) = {this.LegVelocity[0]},{this.LegVelocity[1]}");

        Console.WriteLine($"\nObjects visible to {this.Type} {this.VehicleID}:");

        if (this.ObjectsVisible.Count == 0)
        {
            Console.WriteLine("None");
        }
        foreach (var veh in this.ObjectsVisible)
        {
            float obj_dist = DistanceCalculator(this.CurrentPosition, veh.CurrentPosition);
            float obj_angle = AngleCalculator(this.CurrentPosition, veh.CurrentPosition);
            Console.WriteLine($"{veh.Type} {veh.VehicleID} (Distance = {obj_dist}), (Angle = {Math.Abs(obj_angle) * (180 / MathF.PI)} degrees)");
        }
        this.DecompVelocity();

        // RAVIJ: Move Waypoint computation to Aircraft (done)
        // RAVIJ: Rename vehicle battleSystem or similar (done)

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
    }
    public override void Set()
    {
        // Set current position to new position
        CurrentPosition[0] = NewPositionTemp[0];
        CurrentPosition[1] = NewPositionTemp[1];
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

    public override void DecompVelocity()
    {
        this.LegVelocity[0] = this.Velocities * MathF.Cos(AngleCalculator(this.CurrentPosition, this.NextWaypoint));
        this.LegVelocity[1] = this.Velocities * MathF.Sin(AngleCalculator(this.CurrentPosition, this.NextWaypoint));
    }
    public Aircraft(List<float[]> waypoints, float velocities, float radar_range)
    {

        // Object of Aircraft class takes a List of waypoints (float array of size 2), an array of velocities (size = waypoint list size - 1)
        // and a float indicating Radar range.

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        VehiclePath = waypoints;
        Velocities = velocities;
        VehicleHasStopped = false;
        RadarRange = radar_range;
        NextWaypoint = VehiclePath[1];
        Type = "Aircraft";
        BattleSOS.s_AircraftID++;
        VehicleID = BattleSOS.s_AircraftID;
        ObjectsVisible = new List<BattleSystemClass>();
        ObjectsSurveyed = new List<BattleSystemClass>();
        InLeg = 0;
        CurrWaypointID = 0;
        LegVelocity = new float[2];
        DecompVelocity();
        // Velocities are in direction of any given waypoint leg, decomposing velocities into Vx and Vy
    }
}

class Radar : BattleSystemClass
{
    public override string Type { get; set; }
    public override int VehicleID { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override float[] LegVelocity { get; set; }
    public override float Velocities { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] NewPositionTemp { get; set; }
    public override float[] NextWaypoint { get; set; }
    public override int CurrWaypointID { get; set; }
    public override int InLeg { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override bool VelocityChanged { get; set; }
    public override float RadarRange { get; set; }
    public override float MissileRange { get; set; }
    public override float ElapsedTime { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override List<BattleSystemClass> ObjectsSurveyed { get; set; }

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
    public override float[] Get()
    {
        return CurrentPosition;
    }
    public override void OnTick(float timer)
    {
        Console.WriteLine($"\n{this.Type} {this.VehicleID}");
        Console.WriteLine($"(x, y) = ({this.CurrentPosition[0]},{this.CurrentPosition[1]})" +
                          $"\n(Vx, Vy) = {this.LegVelocity[0]},{this.LegVelocity[1]}");

        Console.WriteLine($"\nObjects visible to {this.Type} {this.VehicleID}:");
        if (this.ObjectsVisible.Count == 0)
        {
            Console.WriteLine("None");
        }
        foreach (var veh in this.ObjectsVisible)
        {
            float obj_dist = DistanceCalculator(this.CurrentPosition, veh.CurrentPosition);
            float obj_angle = AngleCalculator(this.CurrentPosition, veh.CurrentPosition);
            Console.WriteLine($"{veh.Type} {veh.VehicleID} (Distance = {obj_dist}), (Angle = {Math.Abs(obj_angle) * (180 / MathF.PI)} degrees)");
        }
    }
    public override void Set()
    {
        // No postitional computation required for stationary objects
    }

    public override void DecompVelocity()
    {
        // No velocities to decompose
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
        MissileRange = 0;
        Type = "Radar";
        InLeg = 0;
        CurrWaypointID = 0;
        ObjectsVisible = new List<BattleSystemClass>();
        ObjectsSurveyed = new List<BattleSystemClass>();
        BattleSOS.s_RadarID++;
        VehicleID = BattleSOS.s_RadarID;
    }
}

class BattleSOS
{
    public static int s_RadarID = 0;
    public static int s_AircraftID = 0;
    public static int s_AntiAirID = 0;
    public static List<BattleSystemClass> BattleSysList; // Maintains a list of all Vehicles on field
}

class SimulationEngine
{
    public bool allVehiclesStopped = false;
    public SimulationEngine()
    {
        BattleSOS.BattleSysList = new List<BattleSystemClass>();
    }
    public void RegisterVehicle(BattleSystemClass newVehicle)
    {
        BattleSOS.BattleSysList.Add(newVehicle);
    }

    public void RunSimulationEngine(float timer)
    {
        int stoppedVehicles = 0;
        int num_radars = 0;
        int num_aircraft = 0;

        foreach (var battle_system in BattleSOS.BattleSysList)
        {

            if (battle_system.Type == "Radar")
            {
                num_radars++;
            }
            if (battle_system.Type == "Aircraft")
            {
                num_aircraft++;
            }

        }

        // EXECUTE Set() method on every battle_system on field

        foreach (var battle_system in BattleSOS.BattleSysList)
        {
            if (battle_system.Type == "Radar" || battle_system.Type == "Aircraft")
            {
                battle_system.Set();
            }

            if (battle_system.VehicleHasStopped)
            {
                stoppedVehicles++;
                if (battle_system.Type == "Aircraft")
                {
                    Console.WriteLine($"\n{battle_system.Type} {battle_system.VehicleID} reached the end of path");
                    Console.WriteLine($"Objects surveyed by {battle_system.Type} {battle_system.VehicleID}:");
                    foreach (var obj_surv in battle_system.ObjectsSurveyed)
                    {
                        Console.WriteLine($"{obj_surv.Type} {obj_surv.VehicleID} at ({obj_surv.CurrentPosition[0]}, {obj_surv.CurrentPosition[1]})");
                    }
                }
                if (stoppedVehicles == BattleSOS.BattleSysList.Count)
                {
                    allVehiclesStopped = true;
                }
            }
        }

        // EXECUTE OnTick() METHOD for each battle_system on field

        foreach (var battle_system in BattleSOS.BattleSysList.ToList())
        {
            battle_system.OnTick(timer);
        }
    }
}
