using System.Net.NetworkInformation;

class DiscreteTimeSimulationEngine
{
    public bool allVehiclesStopped = false;
    public List<SimulationModel> simMod;
    PhysicalSimulationEngine physEngine = new PhysicalSimulationEngine();
    public List<InParameter> dtseInpList;
    //public int await = 0;
    //public bool FirstRun = true;

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInpList = new List<InParameter>();
        Globals.CurrentTime = 0.0f;
        simMod = ObjectRegister.objects_registered.ToList();

    }

    public class DTSEIn : InParameter
    {
        public DTSEIn(int id) : base(id)
        {
            
        }
    }
    public void RunSimulationEngine()
    {

        foreach (SimulationModel sim_model in simMod)
        {
            physEngine.physicalSituationalAwareness.Add(sim_model.Get());
        }

        foreach (SimulationModel sim_model in simMod)
        {
            sim_model.OnTick();
        }

        foreach (SimulationModel sim_model in simMod)
        {
            sim_model.Set(dtseInpList);
        }

        Globals.CurrentTime += Globals.TimeResolution;
    }

    public void ResetTime()
    {
        Globals.CurrentTime = 0.0f;
    }
}
