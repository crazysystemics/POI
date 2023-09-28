using System.Net.NetworkInformation;

class DiscreteTimeSimulationEngine
{
    public bool allVehiclesStopped = false;
    public List<SimulationModel> simMod;
    PhysicalSimulationEngine physEngine = new PhysicalSimulationEngine();
    public List<InParameter> dtseInParameters;

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
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
            physEngine.physInParameters.Add(sim_model.Get());
        }

        foreach (SimulationModel sim_model in simMod)
        {
            sim_model.OnTick();
        }

        foreach (SimulationModel sim_model in simMod)
        {
            sim_model.Set(dtseInParameters);
        }

        Globals.Tick++;
    }

    public void ResetTime()
    {
        Globals.Tick = 0;
    }
}
