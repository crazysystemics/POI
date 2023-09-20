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

            if (!battle_sys.VehicleHasStopped)
            {

                // Computes new positions (and other attributes) based on physical situation

                battle_sys.NewPositionTemp[0] = battle_sys.CurrentPosition[0] + (battle_sys.LegVelocity[0] * timer);
                battle_sys.NewPositionTemp[1] = battle_sys.CurrentPosition[1] + (battle_sys.LegVelocity[1] * timer);
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