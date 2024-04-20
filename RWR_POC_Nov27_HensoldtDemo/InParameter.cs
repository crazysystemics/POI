public class InParameter
{
    // Parent class used to store InParameters for BattleSystem objects involved in simulation.
    // InParameters contain information that has been processed by DTSE and will be set back into
    // the attributes of BattleSystem objects
    public int ID;

    public InParameter(int ID)
    {
        this.ID = ID;
    }
}