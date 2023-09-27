/* The DiscreteTimeSimulationEngine (DTSE) instantiates by adding BattleSystemClass objects registered by the static
 * ObjectRegister class to its situational awareness list. It also creates an instance of PhysicalSimulationEngine class.
 * 
 * The RunSimulationEngine method calls Get() method on all objects in the Simulation Model list, adding
 * their corresponding SituationalAwareness objects to the physicalSituationalAwareness list of the PSE.
 * 
 * It then calls the OnTick() method on each object of the Simulation Model list, followed by calling
 * OnTick() on the PSE.
 * 
 * It finally calls the Set() method on each object of the Simulation Model list, and then
 * calls Set() on the PSE.
 * 
 * After calling Set() on the PSE, it clears its situational awareness list so that duplicates don't get
 * added during the next set of Get() calls.
 * 
 * It then checks whether all moving objects in the simulation have stopped and sets a flag to stop
 * running the DTSE.
 * 
 * After all of the above checks and computations, it increments Globals.CurrentTime by Globals.TimeResolution.
 * 
 *  */

using System.Net.NetworkInformation;

class DiscreteTimeSimulationEngine
{
    public bool allVehiclesStopped = false;
    public List<SimulationModel> sim_mod;
    PhysicalSimulationEngine PhysEngine = new PhysicalSimulationEngine();
    public List<InParameter> dtseinplist;
    //public int await = 0;
    //public bool FirstRun = true;

    public DiscreteTimeSimulationEngine()
    {
        sim_mod = new List<SimulationModel>();
        dtseinplist = new List<InParameter>();
        Globals.CurrentTime = 0.0f;
        sim_mod = ObjectRegister.objects_registered.ToList();

    }

    public class DTSEIn : InParameter
    {
        public DTSEIn(int id) : base(id)
        {
            
        }
    }
    public void RunSimulationEngine()
    {

        foreach (SimulationModel sim_model in sim_mod)
        {
            PhysEngine.physicalSituationalAwareness.Add(sim_model.Get());
        }

        foreach (SimulationModel sim_model in sim_mod)
        {
            sim_model.OnTick();
        }

        foreach (SimulationModel sim_model in sim_mod)
        {
            sim_model.Set(dtseinplist);
        }

        //if (this.FirstRun)
        //{

        //    // This conditional check is only to output the initial states of all objects before
        //    // any values are updated by the method calls that follow this

        //    Console.WriteLine("Initial values:");
        //    foreach (BattleSystem battle_system in sim_mod)
        //    {
        //        Console.WriteLine($"\n{battle_system.Type} {battle_system.VehicleID} position (x, y): ({battle_system.CurrentPosition[0]}, {battle_system.CurrentPosition[1]})");
        //    }
        //    FirstRun = false;
        //}

        //foreach (var battle_system in sim_mod)
        //{
        //    PhysEngine.physicalSituationalAwareness.Add(battle_system.Get());
        //}
        //foreach (var battle_system in sim_mod)
        //{
        //    battle_system.OnTick();
        //}
        //PhysEngine.OnTick();
        //foreach (var battle_system in sim_mod)
        //{
        //    battle_system.Set(sim_mod);
        //}
        //PhysEngine.Set(sim_mod);
        //PhysEngine.physicalSituationalAwareness.Clear();

        //foreach (var battle_system in situationalAwareness)
        //{

        //    // Stops simulation if no dynamic actions are occurring

        //    if (battle_system.VehicleHasStopped)
        //    {
        //        stoppedVehicles++;
        //        if (stoppedVehicles == situationalAwareness.Count)
        //        {
        //            this.await++;
        //            if (this.await == 1)
        //            {
        //                allVehiclesStopped = true;
        //                if (allVehiclesStopped)
        //                {
        //                    Globals.CurrentTime = 0.0f;
        //                }
        //            }
        //        }
        //    }
        //}

        Globals.CurrentTime += Globals.TimeResolution;
    }

    public void ResetTime()
    {
        Globals.CurrentTime = 0.0f;
    }
}
