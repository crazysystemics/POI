class PhysicalSimulationEngine : SimulationModel
{
    public Dictionary<int, Position> physicalObjects = new Dictionary<int, Position>();
    //public Pulse previousActivePulse;

    public PhysicalSimulationEngine()
    {
        id = 99;
    }

    public int Distance(int objID1, int objID2)
    {
        if (!physicalObjects.ContainsKey(objID1) || !physicalObjects.ContainsKey(objID2)) return 0;
        Position p1 = physicalObjects[objID1];
        Position p2 = physicalObjects[objID2];
        int distance_x = p1.x - p2.x;
        int distance_y = p1.y - p2.y;
        int distance = (int)Math.Sqrt((distance_x * distance_x) + (distance_y * distance_y));
        return distance;
    }

    public class PhysicalSimulationOut : OutParameter
    {
        public PhysicalSimulationOut(int id) : base(id)
        {

        }
    }

    public class In : InParameter
    {
        public Position position = new Position();
        public In(Position position, int id) : base(id)
        {
            this.position = position;
        }
    }

    public override OutParameter Get()
    {
        //PhysicalSimulationOut physOutParamaters = new PhysicalSimulationOut(0);
        return null;
    }

    public override void OnTick()
    {

    }

    public override void Set(List<InParameter> inParameters)
    {
        // Set the new parameters on BattleSystem objects following the OnTick() computation
        foreach (InParameter inParameter in inParameters)
        {
            int id = inParameter.ID;
            Position pos = ((In)inParameter).position;
            if (!physicalObjects.ContainsKey(inParameter.ID))
            {
                physicalObjects.Add(id, pos);
            }
            else
            {
                physicalObjects[id] = pos;
            }
        }
        //int id = inParameters[0].ID;

    }

    public TravellingPulse GetPulse(int txTick, int currentTick, Position txPos, Pulse txPulse)
    {
        TravellingPulse pulse = new(txTick, currentTick, txPos, txPulse.pulseWidth, txPulse.pulseRepetitionInterval, txPulse.timeOfArrival, txPulse.angleOfArrival, txPulse.symbol);
        return pulse;
    }
}