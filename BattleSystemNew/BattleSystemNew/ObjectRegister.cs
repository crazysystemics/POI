﻿/* ObjectRegister is a static class for registering BattleSystemClass objects to the DTSE and PhysicalSimulationEngine.
 * This class also holds static variables for identifying BattleSystemClass objects.
 *  */


static class ObjectRegister
{
    public static int s_RadarID = 0;
    public static int s_AircraftID = 0;
    public static List<BattleSystem> registered_vehicles = new List<BattleSystem>();
    public static void registerObject(BattleSystem batt_obj)
    {
        registered_vehicles.Add(batt_obj);
    }
}
