﻿/* This will have subsets of BattleSystem objects to be passed on to the PSE.
 * It is an abstraction layer to avoid passing entire object instances to the simulation engine
 * and it instead allows creating objects with only some properties to be passed to the engine */

class SituationalAwareness
{
    public float[] CurrentPosition;
    public float[] NewPositionTemp;
    public int ID;
    public string Type;

    public SituationalAwareness(float[] currentPosition, int ID, string veh_type)
    {
        this.CurrentPosition = currentPosition;
        this.NewPositionTemp = currentPosition;
        this.ID = ID;
        this.Type = veh_type;
    }
}