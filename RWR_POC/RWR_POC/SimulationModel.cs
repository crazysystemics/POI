public abstract class SimulationModel
{
    public int id { get; set; }
    public Position position = new Position(0, 0);
    public abstract OutParameter Get();
    public abstract void Set(List<InParameter> inParameters);
    public abstract void OnTick();
}