public class OutParameter
{
    // Parent class used to store OutParameters for BattleSystem objects involved in simulation.
    // OutParameters should contain information that has been extracted from BattleSystem objects,
    // to be processed by DTSE to create Global Sitautional Awareness (still incomplete)
    public int ID;
    public OutParameter(int id)
    {
        this.ID = id;
    }
}