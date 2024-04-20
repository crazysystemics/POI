public abstract class BattleSystem : SimulationModel
{
    // BattleSystem is a child of SimulationModel but only contains one property that is not being used.
    // Need confirmation on whether to keep both classes, or only one.
    public abstract bool Stopped { get; set; }
}