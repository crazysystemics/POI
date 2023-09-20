/* The PhysicalSimulationEngine does the following:
 * 
 * 1. On instantiation, it creates a new list of BattleSystemClass objects for situational awareness.
 * 
 * 2. The Get() method copies the list of BattleSystemClass objects from the DTSE into its own situational awareness list.
 * 
 * 3. The OnTick() method:
 *    a. Displays positions of objects in physical space, and also displays distance and azimuth
 *       relation between the objects. If there are any objects in Radar range or Radars being seen by RWR, display that.
 *    b. Computes new positions for aircraft objects based on velocity and path information. Also determines the next waypoint(s)
 *       for each aircraft object.
 *       
 * 4. The Set() method updates the internal states of all BattleSystemClass objects that are in the situational awareness list.
 *    It also updates the ObjectsVisible property of the objects with any other object within radar/rwr range.
 *    
 *  */


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