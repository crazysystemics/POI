/* The DiscreteTimeSimulationEngine (DTSE) instantiates by adding BattleSystemClass objects registered by the static
 * ObjectRegister class to its situational awareness list. It also creates an instance of PhysicalSimulationEngine class.
 * 
 * The RunSimulationEngine method calls the Get(), OnTick() and Set() methods on the instance of PhysicalSimulationEngine
 * once. The number of ticks are set in the Main program.
 * 
 * <Yet to be implemented: RunSimulationEngine should also call Get(), OnTick() and Set() methods in BattleSystemClass objects
 *  but the operations to be performed by those objects is yet to be confirmed. Some operations might require moving from
 *  PhysicalSimulationEngine to Aircraft or Radar class, depending on the actions>
 *  */


class DiscreteTimeSimulationEngine
{
    public bool allVehiclesStopped = false;
    public List<BattleSystemClass> situationalAwareness;
    PhysicalSimulationEngine PhysEngine;
    public int await = 0;
    public DiscreteTimeSimulationEngine()
    {
        situationalAwareness = new List<BattleSystemClass>();
        situationalAwareness = ObjectRegister.registered_vehicles.ToList();
        PhysEngine = new PhysicalSimulationEngine();
    }

    public void RunSimulationEngine(float timer)
    {
        int stoppedVehicles = 0;

        PhysEngine.Get(situationalAwareness);
        PhysEngine.OnTick(timer);

        foreach (var battle_system in situationalAwareness)
        {
            battle_system.OnTick(timer, PhysEngine);
        }
        foreach (var battle_system in situationalAwareness)
        {
            battle_system.Set(PhysEngine);
        }
        PhysEngine.Set(situationalAwareness);

        foreach (var battle_system in situationalAwareness)
        {

            // Stops simulation if no dynamic actions are occurring

            if (battle_system.VehicleHasStopped)
            {
                stoppedVehicles++;
                if (stoppedVehicles == situationalAwareness.Count)
                {
                    this.await++;
                    if (this.await == 2)
                    {
                        allVehiclesStopped = true;
                    }
                }
            }
        }
    }
}
