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
