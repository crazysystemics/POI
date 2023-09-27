abstract class BattleSystem : SimulationModel
{
/*    public abstract string Type { get; set; }*/
/*    public abstract int VehicleID { get; set; }*/
/*    public abstract float[] CurrentPosition { get; set; }
    public abstract float[] NewPositionTemp { get; set; }*/
/*    public abstract float RadarRange { get; set; }*/
    public abstract bool Stopped { get; set; }
/*    public abstract List<float[]> VehiclePath { get; set; }*/
/*    public abstract List<BattleSystem> ObjectsVisible { get; set; }
    public abstract List<BattleSystem> ObjectsSurveyed { get; set; }*/
}

// Don't make every attribute a property
// Only keep common properties of child classes in this class