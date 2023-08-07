Aircraft Air = new Aircraft(new float[] { 1.0f, 1.5f });
Tank T1 = new Tank(new float[] { 2.0f, 3.0f });

for (int i = 0; i < 10; i++)
{
    SimulationEngine sim = new SimulationEngine(1.0f);
    foreach (var x in BattleSOS.SystemsOnField)
    {
        Console.WriteLine($"{x.Type} Position: {string.Join(",", x.CurrentPosition)}");
    }
}


abstract class BattleSystem
{
    public abstract string Type { get; set; }
    public abstract float[] CurrentPosition { get; set; }
    public abstract float[] Velocities { get; set; }
}

class Aircraft : BattleSystem
{
    public override string Type { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] Velocities { get; set; }
    public Aircraft(float[] velocities)
    {
        this.Velocities = velocities;
        this.CurrentPosition = new float[] { 0.0f, 0.0f };
        Type = "Aircraft";
        BattleSOS.s_BattleSystemsCount++;
        BattleSOS.SystemsOnField.Add(this);
    }
}

class Tank : BattleSystem
{
    public override string Type { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] Velocities { get; set; }
    public Tank(float[] velocities)
    {
        this.CurrentPosition = new float[] { 0.0f, 0.0f };
        this.Velocities = velocities;
        Type = "Tank";
        BattleSOS.s_BattleSystemsCount++;
        BattleSOS.SystemsOnField.Add(this);
    }
}

class BattleSOS
{
    public static int s_BattleSystemsCount = 0;
    public static List<BattleSystem> SystemsOnField = new List<BattleSystem>();
}
class SimulationEngine
{
    public SimulationEngine(float TickTimer)
    {
        Console.WriteLine($"Tick Timer = {TickTimer} second(s)");
        foreach (var system in BattleSOS.SystemsOnField)
        {
            float[] position = system.CurrentPosition;
            for (int i = 0; i < 2; i++)
            {
                system.CurrentPosition[i] += system.Velocities[i] * TickTimer;
            }
        }
    }
}