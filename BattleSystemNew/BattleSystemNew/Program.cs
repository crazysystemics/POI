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


namespace BattleSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;

            ObjectRegister.registerObject(new Aircraft(new List<float[]>
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

            ObjectRegister.registerObject(new Radar(new List<float[]>
                                                        {new float[] { 15.0f, 10.0f }},
                                                         0.0f, 7.5f));

            ObjectRegister.registerObject(new Radar(new List<float[]>
                                                        {new float[] { 25.0f, 5.0f }},
                                                         0.0f, 7.5f));

            ObjectRegister.registerObject(new Radar(new List<float[]>
                                                        {new float[] { 15.0f, 0.0f }},
                                                         0.0f, 7.5f));

            DiscreteTimeSimulationEngine DTSE = new DiscreteTimeSimulationEngine();

            while (!DTSE.allVehiclesStopped)
            {
                DTSE.RunSimulationEngine(1.00f);
                Console.WriteLine("\nPress Enter/Return to display next tick");
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
    public abstract float[] Get();
    public abstract void Set();
    public abstract void OnTick(float timer);
    public abstract void DecompVelocity();
}

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
    public override bool VelocityChanged { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override List<BattleSystemClass> ObjectsSurveyed { get; set; }

    public override float[] Get()
    {
        return CurrentPosition;
    }

    public override void Set()
    {

    }

    public override void OnTick(float timer)
    {

    }

    public override void DecompVelocity()
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
    public override float[] Get()
    {
        return CurrentPosition;
    }

    public override void OnTick(float timer)
    {

    }

    public override void Set()
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

static class ObjectRegister
{
    public static int s_RadarID = 0;
    public static int s_AircraftID = 0;
    public static List<BattleSystemClass> registered_vehicles = new List<BattleSystemClass>();
    public static void registerObject(BattleSystemClass batt_obj)
    {
        registered_vehicles.Add(batt_obj);
    }
}

class DiscreteTimeSimulationEngine
{
    public bool allVehiclesStopped = false;
    public List<BattleSystemClass> situationalAwareness;
    PhysicalSimulationEngine PhysEngine;
    public DiscreteTimeSimulationEngine()
    {
        situationalAwareness = new List<BattleSystemClass>();
        situationalAwareness = ObjectRegister.registered_vehicles.ToList();
        PhysEngine = new PhysicalSimulationEngine();
    }

    public void RunSimulationEngine(float timer)
    {
        int stoppedVehicles = 0;
        int num_radars = 0;
        int num_aircraft = 0;

        foreach (var battle_system in situationalAwareness)
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

        PhysEngine.Get(situationalAwareness);
        PhysEngine.OnTick(timer);
        PhysEngine.Set(situationalAwareness);

        foreach (var battle_system in situationalAwareness)
        {

            // Stops simulation if no dynamic actions are occurring

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
                if (stoppedVehicles == situationalAwareness.Count)
                {
                    allVehiclesStopped = true;
                }
            }
        }
    }
}

class PhysicalSimulationEngine
{
    public List<BattleSystemClass> physicalSituationalAwareness;

    public PhysicalSimulationEngine()
    {
        physicalSituationalAwareness = new List<BattleSystemClass>();
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

    public void Get(List<BattleSystemClass> sit_awareness)
    {
        physicalSituationalAwareness = sit_awareness.ToList();
    }

    public void OnTick(float timer)
    {
        foreach (var battle_sys in physicalSituationalAwareness)
        {

            // Outputs position of each object in physical space

            Console.WriteLine($"\n{battle_sys.Type} {battle_sys.VehicleID} position (x, y): ({battle_sys.CurrentPosition[0]}, {battle_sys.CurrentPosition[1]})");
            if (battle_sys.Type == "Aircraft")
            {
                Console.WriteLine($"Velocity (Vx, Vy): ({battle_sys.LegVelocity[0]}, {battle_sys.LegVelocity[1]})");
            }

            foreach (var battle_sys_2 in physicalSituationalAwareness)
            {

                // Outputs distances and angles between every object in physical space

                if (battle_sys != battle_sys_2)
                {
                    if (battle_sys.Type == "Radar" && battle_sys_2.Type != "Radar")
                    {
                        float dist = DistanceCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                        float angle = AngleCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                        Console.WriteLine($"\nDistance between {battle_sys.Type} {battle_sys.VehicleID} and {battle_sys_2.Type} {battle_sys_2.VehicleID} = {dist}");
                        Console.WriteLine($"Angle = {Math.Abs(angle)} radians");
                        if (battle_sys.Type == "Radar")
                        {
                            Console.WriteLine($"Range of {battle_sys.Type} {battle_sys.VehicleID} = {battle_sys.RadarRange}");
                        }
                    }
                }
            }

            if (!battle_sys.VehicleHasStopped)
            {

                // Computes new positions (and other attributes) based on physical situation

                battle_sys.NewPositionTemp[0] = battle_sys.CurrentPosition[0] + (battle_sys.LegVelocity[0] * timer);
                battle_sys.NewPositionTemp[1] = battle_sys.CurrentPosition[1] + (battle_sys.LegVelocity[1] * timer);
            }

            if (battle_sys.Type == "Aircraft")
            {

                // Finds the next waypoint(s) for all Aircraft type objects

                battle_sys.DecompVelocity();
                for (int i = 0; i < battle_sys.VehiclePath.Count - 1; i++)
                {
                    if (MathF.Abs(DistanceCalculator(battle_sys.CurrentPosition, battle_sys.NextWaypoint)) <= (battle_sys.Velocities * timer))
                    {
                        if (!battle_sys.VehicleHasStopped || battle_sys.NextWaypoint != battle_sys.VehiclePath.Last())
                        {
                            battle_sys.CurrWaypointID++;
                            if (battle_sys.CurrWaypointID < battle_sys.VehiclePath.Count)
                            {
                                battle_sys.NextWaypoint = battle_sys.VehiclePath[battle_sys.CurrWaypointID];
                            }
                            else if (battle_sys.CurrWaypointID == battle_sys.VehiclePath.Count)
                            {
                                battle_sys.VehicleHasStopped = true;
                            }
                        }
                    }
                }
            }
            if (battle_sys.ObjectsVisible.Count > 0)
            {

                // Displays the objects that are registered to ObjectsVisible property

                Console.WriteLine($"\nObjects visible to {battle_sys.Type} {battle_sys.VehicleID}:");
                foreach (var vis_objs in battle_sys.ObjectsVisible)
                {
                    float dist = DistanceCalculator(battle_sys.CurrentPosition, vis_objs.CurrentPosition);
                    float angle = AngleCalculator(battle_sys.CurrentPosition, vis_objs.CurrentPosition);
                    Console.WriteLine($"{vis_objs.Type} {vis_objs.VehicleID} at distance = {dist} and angle = {Math.Abs(angle)} radians");
                }
            }
        }
    }

    public void Set(List<BattleSystemClass> batt_sys)
    {
        foreach (var battle_system in physicalSituationalAwareness)
        {

            // Adds objects to ObjectVisible property if they are within range of radar or RWR and removes them when they are not

            foreach (var battle_system_2 in physicalSituationalAwareness)
            {
                if (battle_system != battle_system_2)
                {
                    float dist = DistanceCalculator(battle_system.CurrentPosition, battle_system_2.CurrentPosition);
                    if (dist <= battle_system.RadarRange && !battle_system.ObjectsVisible.Contains(battle_system_2))
                    {
                        battle_system.ObjectsVisible.Add(battle_system_2);
                    }
                    else if (dist > battle_system.RadarRange && battle_system.ObjectsVisible.Contains(battle_system_2))
                    {
                        battle_system.ObjectsVisible.Remove(battle_system_2);
                    }
                }
            }

            foreach (var batt_system in batt_sys)
            {

                // Sets the new values of object properties as computed by the Physics simulation to the original objects

                if ((battle_system.Type.ToString() + battle_system.VehicleID.ToString()) == (batt_system.Type.ToString() + batt_system.VehicleID.ToString()))
                {
                    batt_system.CurrentPosition[0] = battle_system.NewPositionTemp[0];
                    batt_system.CurrentPosition[1] = battle_system.NewPositionTemp[1];
                    batt_system.ObjectsVisible = battle_system.ObjectsVisible;
                }
            }
        }
    }
}