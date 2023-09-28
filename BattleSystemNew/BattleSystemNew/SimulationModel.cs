public abstract class SimulationModel
{
    public int id;
    public Position position = new Position(0, 0);
    public abstract OutParameter Get();
    public abstract void Set(List<InParameter> inParameters);

    // Separation of concerns - list of configs or settings should be applied in Set() - gives in-parameters
    public abstract void OnTick();
}