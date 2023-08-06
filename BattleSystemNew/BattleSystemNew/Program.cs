Aircraft Air = new Aircraft(new float[] { 1.0f, 1.5f });

for(int i = 0; i<10; i++)
{
    SimulationEngine sim = new SimulationEngine(1.0f);
    foreach (Aircraft x in BattleSOS.SystemsOnField)
    {
        Console.WriteLine(string.Join(",", x.CurrentPosition));
    }
}


/*Air.CurrentPosition = new float[] { 4.3f, 2.1f };
Air2.CurrentPosition = new float[] { 2.9f, 3.1f };*/
/*foreach (Aircraft x in BattleSOS.SystemsOnField)
{
    Console.WriteLine(string.Join(",",x.CurrentPosition));
}*/

/*Console.WriteLine(BattleSOS.s_BattleSystemsCount);*/


abstract class BattleSystem
{
    public abstract string Type { get; set; }
}

class Aircraft : BattleSystem
{
    public override string Type { get; set; }
    public float[] CurrentPosition { get; set; }
    public float[] Velocities { get; set; }
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
    public float[] CurrentPosition { get; set; }
    public float[] Velocities { get; set; }
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
        foreach (Aircraft system in BattleSOS.SystemsOnField)
        {
            float[] position = system.CurrentPosition;
            for (int i = 0; i < 2; i++)
            {
                system.CurrentPosition[i] += system.Velocities[i] * TickTimer;
            }
        }
/*        foreach (Tank system in BattleSOS.SystemsOnField)
        {
            for (int i = 0; i < 2; i++)
            {
                system.CurrentPosition[i] += system.Velocities[i] * TickTimer;
            }
        }*/
    }
}