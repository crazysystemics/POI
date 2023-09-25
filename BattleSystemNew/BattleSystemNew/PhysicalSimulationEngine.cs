/* The PhysicalSimulationEngine does the following:
 * 
 * 1. On instantiation, it creates a new list of SituationalAwareness objects.
 * 
 * 2. The Get() returns a SituationalAwareness object with some default attribute values, but this object
 *    is never used.
 * 
 * 3. The OnTick() method:
 *    a. Displays positions of objects in physical space. Also displays the spatial relation between
 *       every object in physicalSituationalAwareness.
 *       
 * 4. The Set() method updates the internal states of all BattleSystemClass objects that are in the situational awareness list.
 *    
 *  */

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