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
        foreach (var battle_sys in physicalSituationalAwareness)
        {

            Console.WriteLine($"\n{battle_sys.Type} {battle_sys.ID} attributes:");
            Console.WriteLine($"Position (x, y): ({battle_sys.CurrentPosition[0]}, {battle_sys.CurrentPosition[1]})");

        }

        Console.WriteLine("\n----------------");
        Console.WriteLine("\nSpatial relations:");

        foreach (var battle_sys in physicalSituationalAwareness)
        {
            foreach (var battle_sys_2 in physicalSituationalAwareness)
            {
                if (battle_sys != battle_sys_2 && battle_sys.Type != battle_sys_2.Type && battle_sys.Type != "Radar")
                {
                    float dist = Globals.DistanceCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                    float angle = Globals.AngleCalculator(battle_sys.CurrentPosition, battle_sys_2.CurrentPosition);
                    Console.WriteLine($"\nBetween {battle_sys.Type} {battle_sys.ID} and {battle_sys_2.Type} {battle_sys_2.ID}:");
                    Console.WriteLine($"Distance = {dist}, Angle = {angle}");
                }
            }
        }
    }

    public override void Set(List<SimulationModel> sim_mod)
    {

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