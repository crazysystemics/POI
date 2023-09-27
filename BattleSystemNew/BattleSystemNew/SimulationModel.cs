public abstract class SimulationModel
{
    public abstract OutParameter Get();
    public abstract void Set(List<InParameter> inparameter);

    // Separation of concerns - list of configs or settings should be applied in Set() - gives in-parameters
    public abstract void OnTick();
}