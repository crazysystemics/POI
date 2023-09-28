using System.Net.NetworkInformation;

class DiscreteTimeSimulationEngine
{
    public bool allVehiclesStopped = false;
    public List<SimulationModel> simMod;
    PhysicalSimulationEngine physEngine = new PhysicalSimulationEngine();
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
        simMod = ObjectRegister.objects_registered.ToList();

    }

    public void Init()
    {
        Aircraft a = new Aircraft(new Position(20,30));
        Radar r = new Radar(new Pulse(5, 5, 5, 5, "E1"));
        PhysicalSimulationEngine pse = new PhysicalSimulationEngine();

        a.rwr.position = a.position;

        simMod.Add(a);
        simMod.Add(r);
        simMod.Add(a.rwr);
        simMod.Add(pse);
    }

    //public class In : InParameter
    //{
    //    public In(int id) : base(id)
    //    {
            
    //    }
    //}
    //public class Out : OutParameter
    //{
    //    public Radar.In radarIn;
    //    public RWR.In rwrIn;
    //    public Out(Radar.In radarIn, RWR.In rwrIN, int id) : base(id)
    //    {
    //        this.radarIn = radarIn;
    //        this.rwrIn = rwrIN;
    //    }
    //}

    public void RunSimulationEngine()
    {

        foreach (SimulationModel sim_model in simMod)
        {
            // physEngine.physInParameters.Add(sim_model.Get());
            dtseOutParameter.Add(sim_model.Get());
        }
        PhysicalSimulationEngine pse2 = new PhysicalSimulationEngine();
        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is PhysicalSimulationEngine)
            {
                pse2 = (PhysicalSimulationEngine)sim_model;
                break;
            }
        }


          

        foreach (SimulationModel sim_model in simMod)
        {
            //sim_model.Set(dtseInParameters);
            //if (sim_model is PhysicalSimulationEngine)
            //{


            //}
            List<InParameter> inParameters = new List<InParameter>();
            inParameters.Add(new PhysicalSimulationEngine.In(sim_model.position, sim_model.id));
            pse2.Set(inParameters);

            if (sim_model is Radar)
            {
                int dist = physEngine.Distance(0, 1);
                if ( dist < 25)
                {
                    List<InParameter> inParameters2 = new List<InParameter>();
                    inParameters2.Add(new Radar.In(new Pulse(10,50,10,20,"E1"),3));
                    ((Radar)sim_model).Set(inParameters2);
                }
            }

            if (sim_model is RWR)
            {
                int dist = physEngine.Distance(2, 1);
                if (dist < 25)
                {
                    List<InParameter> inParameters2 = new List<InParameter>();
                    int[] amps = new int[] { 10, 10, 10, 10 };
                    inParameters2.Add(new RWR.In(new RWR.Emitter(amps,10,10,10,10), 1));
                    ((RWR)sim_model).Set(inParameters2);
                }
            }
        }

        foreach (SimulationModel sim_model in simMod)
        {
            sim_model.OnTick();
        }

        Globals.Tick++;
    }

    public void ResetTime()
    {
        Globals.Tick = 0;
    }
}
