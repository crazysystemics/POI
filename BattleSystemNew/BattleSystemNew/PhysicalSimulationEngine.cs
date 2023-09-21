/* The PhysicalSimulationEngine does the following:
 * 
 * 1. On instantiation, it creates a new list of BattleSystemClass objects for situational awareness.
 * 
 * 2. The Get() method copies the list of BattleSystemClass objects from the DTSE into its own situational awareness list.
 * 
 * 3. The OnTick() method:
 *    a. Displays positions of objects in physical space. If there are any objects in Radar range or Radars being seen
 *       by RWR, it also displays that.
 *    b. Computes new positions for aircraft objects based on velocity and path information.
 *       
 * 4. The Set() method updates the internal states of all BattleSystemClass objects that are in the situational awareness list.
 *    It also updates the ObjectsVisible property of the objects with any other object within radar/rwr range.
 *    
 *  */


class PhysicalSimulationEngine : SimulatedModel
{
    public List<SimulatedModel> physicalSituationalAwareness;

    public PhysicalSimulationEngine()
    {
        physicalSituationalAwareness = new List<SimulatedModel>();
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

    public override PhysicalSimulationEngine Get()
    {
        return this;
    }


    // The following operations are supposed to happen in objects contained in a List
    // of SimulatedModels type. The operations, however must only happen on objects of
    // BattleSystemClass type. So we must remove the PhysicalSimulationEngine instance
    // from this in order to avoid type mismatch error/exceptions. This removal occurs in
    // the first line of the following OnTick() method body. It is re-added to the overall
    // situational awareness during the Get() call.

    public override void OnTick(float timer)
    {
        if (physicalSituationalAwareness.Contains(this))
        {
            physicalSituationalAwareness.Remove(this);
        }

        foreach (BattleSystemClass battle_sys in physicalSituationalAwareness)
        {

            // Return a list of emitters to the RWR (to do)

            // Outputs position of each object in physical space
            
            Console.WriteLine($"\n----------------\n\n{battle_sys.Type} {battle_sys.VehicleID} position (x, y): ({battle_sys.CurrentPosition[0]}, {battle_sys.CurrentPosition[1]})");
            if (battle_sys.Type == "Aircraft")
            {
                Console.WriteLine($"Velocity (Vx, Vy): ({battle_sys.LegVelocity[0]}, {battle_sys.LegVelocity[1]})");
            }

            Console.WriteLine("\n\nSpatial relations:");

            foreach (BattleSystemClass battle_sys_2 in physicalSituationalAwareness)
            {
                if (battle_sys != battle_sys_2 && battle_sys.Type != battle_sys_2.Type)
                {
                    float dist = DistanceCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                    float angle = AngleCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                    Console.WriteLine($"\n{battle_sys.Type} {battle_sys.VehicleID} and {battle_sys_2.Type} {battle_sys_2.VehicleID}:");
                    Console.WriteLine($"Distance = {dist}");
                    Console.WriteLine($"Azimuth = {Math.Abs(angle)} radians");

                    // The above is being printed twice after the first tick. Reason currently not known.

                }
            }

            Console.WriteLine("\n----------------");

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

    public override void Set(List<BattleSystemClass> sit_awareness, List<SimulatedModel> sim_mod)
    {

        physicalSituationalAwareness = sim_mod.ToList();
        if (physicalSituationalAwareness.Contains(this))
        {
            // Avoids a similar error as explain before the OnTick() method
            physicalSituationalAwareness.Remove(this);
        }

        foreach (BattleSystemClass battle_system in physicalSituationalAwareness)
        {

            battle_system.CurrentPosition[0] = battle_system.NewPositionTemp[0];
            battle_system.CurrentPosition[1] = battle_system.NewPositionTemp[1];

        }
    }
}