namespace BattleSystemTest_Aug8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SimulationEngine simulationEngine = new SimulationEngine();
            for (int i = 0; i < 20; i++)
            {
                simulationEngine.RunSimulationEngine();
            }
        }
    }


    abstract class BattleSystem
    {
        public abstract string Type { get; set; }
        public abstract float[] CurrentPosition { get; set; }
        public abstract float[] LegVelocity { get; set; }
        public abstract float[] Get();
        public abstract void Set();
        public abstract float[] NewPositionTemp { get; set; }
        public abstract void OnTick(float duration);
        public abstract List<float[]> VehiclePath { get; set; }
    }

    class Aircraft : BattleSystem
    {
        public override string Type { get; set; }
        public override float[] CurrentPosition { get; set; }
        public override float[] LegVelocity { get; set; }
        public override float[] NewPositionTemp { get; set; }
        public override float[] Get()
        {
            return this.CurrentPosition;
        }
        public override void Set()
        {
            for (int i = 0; i < 2; i++)
            {
                this.CurrentPosition[i] = this.NewPositionTemp[i];
            }
        }
        public override void OnTick(float duration)
        {
            for (int i = 0; i < this.CurrentPosition.Length; i++)
            {
                // Need to split velocity into x and y compoinents for each leg in order to compute new positions
                float x_vel;
                float y_vel;
                float[] decomp_vel = new float[2];
                this.NewPositionTemp[i] = this.CurrentPosition[i] + (this.LegVelocity[i] * duration);
            }
        }
        public override List<float[]> VehiclePath { get; set; }
        public Aircraft(List<float[]> waypoints)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                this.LegVelocity[i] = waypoints[i][2];
            }
            this.CurrentPosition = this.NewPositionTemp = waypoints[0][0..1];
            Type = "Aircraft";
            this.VehiclePath = waypoints;
        }
    }

    /*    class Tank : BattleSystem
        {
            public override string Type { get; set; }
            public override float[] CurrentPosition { get; set; }
            public override float[] Velocities { get; set; }
            public override int VehicleID { get; set; }
            public override float[] Get()
            {
                float[] a = new float[2];
                return a;
            }
            public override void Set(string argument)
            {

            }
            public override void OnTick()
            {

            }
            public Tank(float[] velocities, float[] init_position)
            {
                this.Velocities = velocities;
                this.CurrentPosition = init_position;
                Type = "Tank";
                BattleSOS.s_BattleSystemsCount++;
                this.VehicleID = BattleSOS.s_BattleSystemsCount;
                BattleSOS.SystemsOnField.Add(this);
            }
        }*/

    class BattleSOS
    {
        //
        public static int s_BattleSystemsCount = 0;
        public static List<BattleSystem> SystemsOnField;
    }
    class SimulationEngine
    {
        public SimulationEngine()
        {
            BattleSOS.SystemsOnField = new List<BattleSystem>
            { new Aircraft(new List<float[]> {new float[] { 0.0f, 0.0f, 0.0f },
                                              new float[] { 3.0f, 4.0f, 5.0f },
                                              new float[] { 15.0f, 4.0f, 2.0f },
                                              new float[] { 20.0f, 0.0f, 0.0f} })
            };
        }
        public void RunSimulationEngine()
        {
            List<float[]> globalSituationAwareness = new List<float[]>();



            foreach (var system in BattleSOS.SystemsOnField)
            {
                globalSituationAwareness.Add(system.Get());
            }

            foreach (var system in BattleSOS.SystemsOnField)
            {
                system.OnTick(1.0f);
            }

            foreach (var system in BattleSOS.SystemsOnField)
            {
                system.Set();
                Console.WriteLine($"({system.CurrentPosition[0]},{system.CurrentPosition[1]})");
            }
        }
    }
}