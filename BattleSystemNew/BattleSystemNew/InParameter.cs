/* This will have subsets of BattleSystem objects to be passed on to the PSE.
 * It is an abstraction layer to avoid passing entire object instances to the simulation engine
 * and it instead allows creating objects with only some properties to be passed to the engine */

public class InParameter
{
    public int ID;

    public InParameter(int ID)
    {
        this.ID = ID;
    }
}