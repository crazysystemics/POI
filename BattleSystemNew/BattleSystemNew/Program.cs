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
            float[] decomp_vel = new float[2];
            for (int i = 0; i < this.LegVelocity.Length; i++)
            {
                while (true)
                {
                    float x_len = this.VehiclePath[i + 1][0] - this.VehiclePath[i][0];
                    float y_len = this.VehiclePath[i + 1][1] - this.VehiclePath[i][1];
                    float euclidean_distance = (float)Math.Sqrt(((x_len) * (x_len)) + ((y_len) * (y_len)));
                    float x_vel = (x_len / euclidean_distance) * this.LegVelocity[i];
                    float y_vel = (y_len / euclidean_distance) * this.LegVelocity[i];
                    decomp_vel[0] = x_vel;
                    decomp_vel[1] = y_vel;
                    this.NewPositionTemp[0] = this.CurrentPosition[0] + (decomp_vel[0] * duration);
                    this.NewPositionTemp[1] = this.CurrentPosition[1] + (decomp_vel[1] * duration);
                    if (this.NewPositionTemp[0] >= this.VehiclePath[i + 1][0])
                    {
                        break;
                    }
                }
            }
        }
        public override List<float[]> VehiclePath { get; set; }
        public Aircraft(List<float[]> waypoints, float[] leg_velocity)
        {
            this.LegVelocity = leg_velocity;
            this.CurrentPosition = waypoints[0];
            this.NewPositionTemp = new float[] { 0.0f, 0, 0f };
            Type = "Aircraft";
            this.VehiclePath = waypoints;
        }
    }

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
            { new Aircraft(new List<float[]> {new float[] { 0.0f, 0.0f },
                                              new float[] { 3.0f, 4.0f },
                                              new float[] { 15.0f, 4.0f },
                                              new float[] { 20.0f, 0.0f } }, new float[] {3.0f, 10.0f, 3.0f })
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