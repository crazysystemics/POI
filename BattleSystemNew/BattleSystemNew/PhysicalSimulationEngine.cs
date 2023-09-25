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

// Separation of concerns - only pass properties required for computations
class PhysicalSimulationEngine : SimulationModel
{
    public List<SituationalAwareness> physicalSituationalAwareness;

    public PhysicalSimulationEngine()
    {
        physicalSituationalAwareness = new List<SituationalAwareness>();
    }

    public override SituationalAwareness Get()
    {
        float[] temp_values = new float[2];
        temp_values[0] = 0.0f;
        temp_values[1] = 0.0f;
        SituationalAwareness temp = new SituationalAwareness(temp_values, 0, "PhysEngine");
        return temp;
    }

    public override void OnTick()
    {

        foreach (var sit_aw in physicalSituationalAwareness)
        {
            if (sit_aw.Type == "PhysEngine")
            {
                physicalSituationalAwareness.Remove(sit_aw);
            }
        }

        foreach (var battle_sys in physicalSituationalAwareness)
        {

            // Return a list of emitters to the RWR (to do)

            // Outputs position of each object in physical space
            
            Console.WriteLine($"\n----------------\n\n{battle_sys.Type} {battle_sys.ID} position (x, y): ({battle_sys.CurrentPosition[0]}, {battle_sys.CurrentPosition[1]})");

            Console.WriteLine("\n\nSpatial relations:");

            foreach (var battle_sys_2 in physicalSituationalAwareness)
            {
                if (battle_sys != battle_sys_2 && battle_sys.Type != battle_sys_2.Type)
                {
                    float dist = Globals.DistanceCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                    float angle = Globals.AngleCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                    Console.WriteLine($"\n{battle_sys.Type} {battle_sys.ID} and {battle_sys_2.Type} {battle_sys_2.ID}:");
                    Console.WriteLine($"Distance = {dist}");
                    Console.WriteLine($"Azimuth = {Math.Abs(angle)} radians");

                    // The above is being printed twice after the first tick. Reason currently not known.

                }
            }

            Console.WriteLine("\n----------------");

            if (battle_sys.ObjectsVisible.Count > 0)
            {

                // Displays the objects that are registered to ObjectsVisible property

                Console.WriteLine($"\nObjects visible to {battle_sys.Type} {battle_sys.ID}:");
                foreach (var vis_objs in battle_sys.ObjectsVisible)
                {
                    float dist = Globals.DistanceCalculator(battle_sys.CurrentPosition, vis_objs.CurrentPosition);
                    float angle = Globals.AngleCalculator(battle_sys.CurrentPosition, vis_objs.CurrentPosition);
                    Console.WriteLine($"{vis_objs.Type} {vis_objs.VehicleID} at distance = {dist} and angle = {Math.Abs(angle)} radians");
                }
            }
        }
    }

    public override void Set(List<SimulationModel> sim_mod)
    {

/*        physicalSituationalAwareness = sim_mod.ToList();
        if (physicalSituationalAwareness.Contains(this))
        {
            // Avoids a similar error as explain before the OnTick() method
            physicalSituationalAwareness.Remove(this);
        }*/

        foreach (var battle_system in physicalSituationalAwareness)
        {
            foreach (BattleSystemClass sys_model in sim_mod)
            {
                if (sys_model.Type == battle_system.Type && sys_model.VehicleID == battle_system.ID)
                {
                    sys_model.CurrentPosition[0] = battle_system.NewPositionTemp[0];
                    sys_model.CurrentPosition[1] = battle_system.NewPositionTemp[1];
                }
            }

        }
    }
}