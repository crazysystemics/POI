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
    public List<OutParameter> physicalSituationalAwareness;

    public PhysicalSimulationEngine()
    {
        physicalSituationalAwareness = new List<OutParameter>();
    }

    public class PhysicalSimulationOut : OutParameter
    {
        public PhysicalSimulationOut(int id):base(id)
        {

        }
    }

    public override OutParameter Get()
    {
        PhysicalSimulationOut physoutparamaters = new PhysicalSimulationOut(3);
        return physoutparamaters;
    }

    public override void OnTick()
    {

        // the following is to be implemented:
        // Obtain neighbours of RWR
        // Compute the transmitted power detected in neighbouring cells
        // Update the RWR cells with their coordinates and power

    }

    public override void Set(List<InParameter> inparameter)
    {

        //foreach (var battle_system in physicalSituationalAwareness)
        //{
        //    foreach (BattleSystem sys_model in sim_mod)
        //    {
        //        if (sys_model.Type == battle_system.Type && sys_model.VehicleID == battle_system.ID)
        //        {
        //            sys_model.CurrentPosition[0] = battle_system.NewPositionTemp[0];
        //            sys_model.CurrentPosition[1] = battle_system.NewPositionTemp[1];
        //        }
        //    }

        //}
    }
}