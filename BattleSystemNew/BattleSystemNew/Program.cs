namespace BattleSystemTest_Aug8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SimulationEngine simulationEngine = new SimulationEngine();
            for(int i = 0; i < 20; i++)
            {
                simulationEngine.RunSimulationEngine();
            }
        }
    }


    abstract class BattleSystem
    {
        public abstract string Type { get; set; }
        public abstract float[] CurrentPosition { get; set; }
        public abstract float[] Velocities { get; set; }
        public abstract float[] Get();
        public abstract void Set();
        public abstract float[] NewPositionTemp { get; set; }
        public abstract void OnTick(float duration);
    }

    class Aircraft : BattleSystem
    {
        public override string Type { get; set; }
        public override float[] CurrentPosition { get; set; }
        public override float[] Velocities { get; set; }
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
                this.NewPositionTemp[i] = this.CurrentPosition[i] + (this.Velocities[i] * duration);
            }
        }
        public Aircraft(float[] init_position, float[] velocities)
        {
            this.Velocities = velocities;
            this.CurrentPosition = init_position;
            this.NewPositionTemp = init_position;
            Type = "Aircraft";
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
            { new Aircraft(new float[] { 0.0f, 0.0f }, new float[] { 1.0f, 1.5f }),
            new Aircraft(new float[] { 0.0f, 0.0f }, new float[] { 2.0f, 4.0f }),};
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
                Console.WriteLine($"{system.CurrentPosition[0]},{system.CurrentPosition[1]}");
            }
        }
    }
}