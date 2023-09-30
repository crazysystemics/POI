﻿using System.Net.NetworkInformation;

class DiscreteTimeSimulationEngine
{
    public List<SimulationModel> simMod;
    //PhysicalSimulationEngine physEngine = new PhysicalSimulationEngine();
    public List<InParameter> dtseInParameters;
    public List<OutParameter> dtseOutParameter = new List<OutParameter>();
    public PhysicalSimulationEngine pse = new PhysicalSimulationEngine();
    public int pulseTxTick = 0;

    public DiscreteTimeSimulationEngine()
    {
        simMod = new List<SimulationModel>();
        dtseInParameters = new List<InParameter>();
        Globals.Tick = 0;
        simMod = ObjectRegister.objects_registered.ToList();

    }

    public void Init()
    {
        Aircraft a = new Aircraft(new Position(20,30), 0);
        Radar r = new Radar(new Pulse(5, 5, 5, 5, "E1"), new Position(20, 10), 25, 1);


        a.rwr = new RWR(ref a.position, 2);

        simMod.Add(a);
        simMod.Add(r);
        simMod.Add(a.rwr);
        simMod.Add(pse);
    }

    public void RunSimulationEngine()
    {
        List<InParameter> inParameters = new List<InParameter>();

        foreach (SimulationModel sim_model in simMod)
        {
            // physEngine.physInParameters.Add(sim_model.Get());
            dtseOutParameter.Add(sim_model.Get());
        }
        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is PhysicalSimulationEngine)
            {
                pse = (PhysicalSimulationEngine)sim_model;
                break;
            }
            else
            {
                inParameters.Add(new PhysicalSimulationEngine.In(sim_model.position, sim_model.id));
                // *1 of notes
            }
        }


          

        foreach (SimulationModel sim_model in simMod)
        {
            pse.Set(inParameters);
            int radius = 0;

            if (sim_model is Radar)
            {
                radius = ((Radar)sim_model).radius;
                int dist = pse.Distance(0, 1);
                Console.WriteLine($"Distance between Aricraft and Radar = {dist}");
                if ( dist < radius)
                {
                    List<InParameter> inParameters2 = new List<InParameter>();
                    inParameters2.Add(new Radar.In(new Pulse(10,50,10,20,"E2"),2));
                    ((Radar)sim_model).Set(inParameters2);
                }
            }

            if (sim_model is RWR)
            {
                int dist = pse.Distance(2, 1);
                Console.WriteLine($"Distance between Radar and RWR = {dist}");
                if (dist < radius)
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

        OutParameter pulseOut;
        Pulse txPulse;

        foreach (SimulationModel sim_model in simMod)
        {
            if (sim_model is Radar)
            {

                pulseOut = sim_model.Get();
                txPulse = ((Radar.Out)pulseOut).p;

                TravellingPulse rxPulse = pse.GetPulse(txPulse, sim_model.position, sim_model.position, pulseTxTick, Globals.Tick);

                //Console.WriteLine("\nTravelling pulse attributes:");
                //Console.WriteLine($"\ntxTick: {rxPulse.txTick}" +
                //    $"\ntxPos: ({rxPulse.txPos.x}, {rxPulse.txPos.y})" +
                //    $"\ncurrentPos: ({rxPulse.currentPos.x}, {rxPulse.currentPos.y})" +
                //    $"\nCurrentTick: {rxPulse.currentTick}" +
                //    $"\npulseWidth at receiving cell: {rxPulse.pulseWidth}" +
                //    $"\nPRI at receiving cell: {rxPulse.pulseRepetitionInterval}" +
                //    $"\ntimeOfArrival at receiving cell: {rxPulse.timeOfArrival}" +
                //    $"\nangleOfArrival at receiving cell: {rxPulse.angleOfArrival}\n\n");

                if (((Radar)sim_model).activePulse != ((Radar)sim_model).zeroPulse)
                {
                    foreach (SimulationModel receiver in simMod)
                    {
                        int pulseTravelSpeed = 1; // must be speed of light "c" in actual computation
                        int dist = pse.Distance(receiver.id, sim_model.id);
                        int pulseTravelTime = dist / pulseTravelSpeed;
                        if (Math.Abs(rxPulse.txTick - Globals.Tick) == pulseTravelTime && Globals.Tick != 0)
                        {
                            Console.WriteLine($"Pulse arrived at cell of {receiver}");
                            pulseTxTick = rxPulse.currentTick;
                            if (rxPulse.pulseWidth != 0)
                            {

                            }
                        }
                    }
                }
            }
        }

        Globals.Tick++;
    }

    public void ResetTime()
    {
        Globals.Tick = 0;
    }
}
