abstract class BattleSystemClass : SimulatedModel
{
    public abstract string Type { get; set; }
    public abstract int VehicleID { get; set; }
    public abstract int CurrWaypointID { get; set; }
    public abstract float[] LegVelocity { get; set; }
    public abstract float[] CurrentPosition { get; set; }
    public abstract float[] NewPositionTemp { get; set; }
    public abstract float[] NextWaypoint { get; set; }
    public abstract float Velocities { get; set; }
    public abstract float RadarRange { get; set; }
    public abstract bool VehicleHasStopped { get; set; }
    public abstract List<float[]> VehiclePath { get; set; }
    public abstract List<BattleSystemClass> ObjectsVisible { get; set; }
    public abstract List<BattleSystemClass> ObjectsSurveyed { get; set; }
}


// Add another cklass emitter with proteries - pluse width, PRI, time of arrival, emitter id, angle of arrival, symbol
// Rename this class to BattleSystem