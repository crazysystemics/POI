abstract class SimulatedModel
{
    public abstract SimulatedModel Get();
    public abstract void Set(List<BattleSystemClass> batt_sys, List<SimulatedModel> sim_mod);
    public abstract void OnTick(float timer);
}