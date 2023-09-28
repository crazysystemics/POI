public abstract class BattleSystem : SimulationModel
{
    public abstract bool Stopped { get; set; }
}

// Don't make every attribute a property
// Only keep common properties of child classes in this class