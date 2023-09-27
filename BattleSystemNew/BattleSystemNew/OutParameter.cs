/* This will have subsets of BattleSystem objects to be passed on to the PSE.
 * It is an abstraction layer to avoid passing entire object instances to the simulation engine
 * and it instead allows creating objects with only some properties to be passed to the engine */

public class OutParameter
{
    public int ID;
    public OutParameter(int id)
    {
        this.ID = id;
    }
}