/* ObjectRegister is a static class for registering BattleSystemClass objects to the DTSE and PhysicalSimulationEngine.
 * This class also holds static variables for identifying BattleSystemClass objects.
 *  */


static class ObjectRegister
{
    public static int s_RadarID = 0;
    public static int s_AircraftID = 0;
    public static List<SimulationModel> objects_registered = new List<SimulationModel>();
    public static void registerObject(BattleSystem batt_obj)
    {
        objects_registered.Add(batt_obj);
    }
}
