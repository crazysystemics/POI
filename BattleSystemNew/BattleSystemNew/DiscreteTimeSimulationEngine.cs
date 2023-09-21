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


// Create class simulated model and add battle systm class and simulated engine


class DiscreteTimeSimulationEngine
{
    public bool allVehiclesStopped = false;
    public List<BattleSystemClass> situationalAwareness;
    PhysicalSimulationEngine PhysEngine;
    List<SimulatedModel> sim_mod = new List<SimulatedModel>();
    public int await = 0;
    public bool FirstRun = true;
    public DiscreteTimeSimulationEngine()
    {
        situationalAwareness = new List<BattleSystemClass>();
        situationalAwareness = ObjectRegister.registered_vehicles.ToList();
        PhysEngine = new PhysicalSimulationEngine();
        foreach (var objs in situationalAwareness)
        {
            sim_mod.Add(objs);
        }
        sim_mod.Add(PhysEngine);
    }

    public void RunSimulationEngine(float timer)
    {
        int stoppedVehicles = 0;

        if (this.FirstRun)
        {

            // This conditional check is only to output the initial states of all objects before
            // any values are updated by the method calls that follow this

            if (sim_mod.Contains(PhysEngine))
            {
                sim_mod.Remove(PhysEngine);
            }
            Console.WriteLine("Initial values:");
            foreach (BattleSystemClass battle_system in sim_mod)
            {
                Console.WriteLine($"\n{battle_system.Type} {battle_system.VehicleID} position (x, y): ({battle_system.CurrentPosition[0]}, {battle_system.CurrentPosition[1]})");
                if (battle_system.Type == "Aircraft")
                {
                    Console.WriteLine($"Velocity (Vx, Vy): ({battle_system.LegVelocity[0]}, {battle_system.LegVelocity[1]})");
                }
            }
            FirstRun = false;
            sim_mod.Add(PhysEngine);
        }

        foreach (var battle_system in sim_mod)
        {
            PhysEngine.physicalSituationalAwareness.Add(battle_system.Get());
        }
        foreach (var battle_system in sim_mod)
        {
            battle_system.OnTick(timer);
        }
        foreach (var battle_system in sim_mod)
        {
            battle_system.Set(situationalAwareness, sim_mod);
        }

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
