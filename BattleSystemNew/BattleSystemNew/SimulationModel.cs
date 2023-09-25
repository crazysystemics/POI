abstract class SimulationModel
{
    public abstract SituationalAwareness Get();
    public abstract void Set(List<SimulationModel> sim_mod);
    public abstract void OnTick();
}