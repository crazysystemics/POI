class PhysicalSimulationEngine : SimulationModel
{
    public List<OutParameter> physInParameters;

    public PhysicalSimulationEngine()
    {
        physInParameters = new List<OutParameter>();
    }

    public class PhysicalSimulationOut : OutParameter
    {
        public PhysicalSimulationOut(int id):base(id)
        {

        }
    }

    public class In : InParameter
    {
        public In(int id) : base(id)
        {
            this.ID = id;
        }
    }

    public override OutParameter Get()
    {
        PhysicalSimulationOut physOutParamaters = new PhysicalSimulationOut(0);
        return physOutParamaters;
    }

    public override void OnTick()
    {

        // the following is to be implemented:
        // Obtain neighbours of RWR
        // Compute the transmitted power detected in neighbouring cells
        // Update the RWR cells with their coordinates and power

    }

    public override void Set(List<InParameter> inParameters)
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