﻿class PhysicalSimulationEngine : SimulationModel
{
    public Dictionary<int, Position> physicalObjects = new();
    //public Pulse previousActivePulse;

    public PhysicalSimulationEngine(int id)
    {
        this.id = id;
    }

    public static int GetDistance(Position p1, Position p2)
    {
        int distance_x = p1.x - p2.x;
        int distance_y = p1.y - p2.y;
        int distance = (int)Math.Sqrt((distance_x * distance_x) + (distance_y * distance_y));
        return distance;
    }

    public static double GetAngle(Position p1, Position p2)
    {
        int distance_x = p1.x - p2.x;
        int distance_y = p1.y - p2.y;
        double angle = Math.Atan2(distance_y, distance_x);
        return angle;
    }

    public class PhysicalSimulationOut : OutParameter
    {
        public PhysicalSimulationOut(int id) : base(id)
        {

        }
    }

    public class In : InParameter
    {
        public Position position = new();
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

    //public Pulse GetPulse(Pulse txPulse, int txTick, Position txPosition,
    //                      int rxTick, Position rxPosition)
    //{
    //    pulseInspace = txPulse;
    //    return pulseInspace;
    //}
}