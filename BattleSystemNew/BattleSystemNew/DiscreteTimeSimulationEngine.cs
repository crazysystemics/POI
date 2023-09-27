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

        Globals.CurrentTime += Globals.TimeResolution;
    }

    public void ResetTime()
    {
        Globals.CurrentTime = 0.0f;
    }
}
