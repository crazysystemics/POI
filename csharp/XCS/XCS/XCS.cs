using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace POI_XCS
{
    class Utils
    {
        static public bool rect_contain(double[] x)
        {
            //if a point x[0] lies between x-axis of rect x[2] and x[3] and
            //           x[1] lies between y-axis of rect x[4] and x[5] then,
            //           {x[0],x[1]} lies in the rectangle { x[2] x[3] x[4] x[5] }
            return false;
        }
    }

    class Xcs_Environment
    {
        public List<Radar> sigma_radars = new List<Radar>();
        public List<Action> alpha_actions = new List<Action>();


        public ReInforcementProgram rp;
        public XCS xcs;
        public bool eop;

        public Xcs_Environment()
        {
            xcs = new XCS();

        }

        public void fieldvalsToString(ref List<string> sl)
        {
            sl.Add("XCS_ENVIRONMENT");
            xcs.fieldvalsToString(ref sl);
        }

        public void runNextCycle()
        {
            if (rp == null)
                rp = new ReInforcementProgram();



            alpha_actions = xcs.get_action_sequence(sigma_radars);


        }
    }

    class XCS
    {
        public List<Classifier> Population = new List<Classifier>();
        public List<Action> default_actions = new List<Action>();
        double P; //Population number

        public List<Classifier> MatchSet = new List<Classifier>();
        public List<Radar> sigma_radars = new List<Radar>();
        public List<Radar> sigma_radars_1 = new List<Radar>();

        public List<Classifier> ActionSet = new List<Classifier>();
        public List<Classifier> ActionSet_1 = new List<Classifier>();
        public List<Action> act_seq;

        public double[] PredictionArray = new double[32];
        public double rho;
        public double rho_1 = 0.0;
        public int theta_mna = 0;
        public GA ga = new GA(0.5, 0.5, 0.5);

        double alpha;
        double beta;
        double gamma;
        double prob_dont_care;



        public XCS()
        {
            Action a = new Action(1, 5);
            default_actions.Add(a);

            a = new Action(2, 5);
            default_actions.Add(a);

            a = new Action(3, 5);
            default_actions.Add(a);

            a = new Action(4, 5);
            default_actions.Add(a);


        }

        public void fieldvalsToString(ref List<string> sl)
        {
            sl.Add("POPULATION[" + Population.Count.ToString() + "]:");
            foreach (Classifier cl in Population)
                cl.fieldvalsToString(ref sl);


            sl.Add("MATCHING_SET:");
            foreach (Classifier cl in MatchSet)
                cl.fieldvalsToString(ref sl);

            sl.Add("SIGMA-RADARS");
            foreach (Radar r in sigma_radars)
                r.fieldvalsToString(ref sl);
            sl.Add("SIGMA-RADARS-MINUS-1");
            foreach (Radar r in sigma_radars_1)
                r.fieldvalsToString(ref sl);

            sl.Add("ACTION_SET:");
            foreach (Classifier cl in ActionSet)
                cl.fieldvalsToString(ref sl);
            sl.Add("ACTION_SET_MINUS_1:");
            foreach (Classifier cl in ActionSet_1)
                cl.fieldvalsToString(ref sl);


            if (act_seq != null && act_seq.Count > 0)
            {
                sl.Add("SELECTED_ACTION_SEQ:");
                foreach (Action act in act_seq)
                    act.fieldvalsToString(ref sl);
            }
            else
            {
                sl.Add("EMPTY_ACTION_SEQ:");
            }


            string s = string.Empty;
            sl.Add("PREDICTION_ARRAY:");
            foreach (double pa in PredictionArray)
            {
                s += pa.ToString() + "\t";
            }
            sl.Add("GA");
            ga.fieldvalsToString(ref sl);


            sl.Add("SCALARS: rho rho_1 theta_mna prob_don_care alpha beta gamma");
            sl.Add(rho.ToString() + "\t" + rho_1.ToString() + "\t" + theta_mna.ToString() + "\t" +
                   prob_dont_care.ToString() + "\t" + alpha.ToString() + "\t" + beta.ToString() + "\t" + gamma.ToString());

        }





        public List<Action> get_action_sequence(List<Radar> env_sigma_radars)
        {
            List<Action> act_seq = new List<Action>();
            Classifier cl = new Classifier();


            if (Population.Count > 0)
            {
                if (globals.onconsole)
                    Console.WriteLine(globals.tick.ToString());
            }



            if (env_sigma_radars.Count == 0)
            {
                //no emitters were intercepted in cycle
                //generate based on past history
                globals.dumpLog("XCS:get_action_sequence:env_sigma_radars.count==0", globals.onconsole, globals.onfile);

                Random random = new Random();

                uint band;
                uint duration;
                int cur_duration = 0;
                cl.conditions = new List<Condition>();

                //TODO: change i  < 2 to i < num_slots
                //TODO: 20 to rx duration
                for (int i = 0; cur_duration < 20; i++)
                {
                    Condition cond = new Condition();


                    band = (uint)(random.Next(3) + 1);
                    duration = (uint)random.Next(20 - cur_duration) + 1;

                    cond.tfwin.mint = cur_duration;
                    cur_duration += (int)duration;
                    cond.tfwin.maxt = cur_duration;

                    cond.tfwin.minf = (int)band;
                    cond.tfwin.maxf = (int)band;

                    cl.conditions.Add(cond);

                    act_seq.Add(new Action(band, duration));
                }

                cl.actions = act_seq;
                cl.prediction = cl.initial_prediction;
                cl.epsilon = cl.initial_epsilon;
                cl.Fitness = cl.initial_Fitness;
                cl.experience = 0;
                cl.sim_time = 0;
                cl.action_set_size = 1;
                cl.n = 1;

                Population.Add(cl);
            }
            else
            {


                globals.dumpLog("XCS:GET_ACTION_SEQUENCE:env_sigma_radars.Count > 0", globals.onconsole, globals.onfile);

                List<string> xcs_sl = new List<string>();
                xcs_sl.Add("POPULATION");
                foreach (Classifier pcl in Population)
                {
                    pcl.fieldvalsToString(ref xcs_sl);
                }
                globals.dumpLog(xcs_sl, globals.onconsole, globals.onfile);



                sigma_radars = env_sigma_radars;

                xcs_sl = new List<string>();
                xcs_sl.Add("SIGMA-RADARS");
                foreach (Radar r in sigma_radars)
                {
                    r.fieldvalsToString(ref xcs_sl);
                }
                globals.dumpLog(xcs_sl, globals.onconsole, globals.onfile);


                //Action-Sequence for Matchset is
                //generated in genMatchSet=>genCoverageSetRountine
                MatchSet = genMatchSet( sigma_radars);

                xcs_sl = new List<string>();
                xcs_sl.Add("MATCH-SET");
                foreach (Classifier mcl in MatchSet)
                {
                    mcl.fieldvalsToString(ref xcs_sl);
                }
                globals.dumpLog(xcs_sl, globals.onconsole, globals.onfile);



                PredictionArray = genPredictionArray(MatchSet);

                string s = string.Empty;
                xcs_sl = new List<string>();
                xcs_sl.Add("PREDICTION-ARRAY");
                int pred_index = 0;


                foreach (double pa in PredictionArray)
                {
                    s = MatchSet.ToArray()[pred_index].clid.ToString() + "\t" + pa.ToString();
                }
                globals.dumpLog(s, globals.onconsole, globals.onfile);


                act_seq = SelectAction(PredictionArray);

                xcs_sl = new List<string>();
                xcs_sl.Add("ACTION-SEQ");
                foreach (Action act in act_seq)
                {
                    act.fieldvalsToString(ref xcs_sl);
                }
                globals.dumpLog(xcs_sl, globals.onconsole, globals.onfile);


                ActionSet = GenerateActionSet(MatchSet, act_seq);

                xcs_sl = new List<string>();
                xcs_sl.Add("ACTION-SET");
                foreach (Classifier acl in ActionSet)
                {
                    acl.fieldvalsToString(ref xcs_sl);
                }
                globals.dumpLog(xcs_sl, globals.onconsole, globals.onfile);
            }

            //update prediction and fitness (currently same)





            return act_seq;
        }

        public void OptimizeXcsStructures(double rp_rho, bool eop)
        {
            rho = rp_rho;
            if (ActionSet_1 != null)
            {
                P = rho_1 + gamma * PredictionArray.Max();
                UpdateSet(ref ActionSet_1, P, ref Population);
                ga.Run(ref ActionSet_1, sigma_radars_1, ref Population);
            }

            if (eop)
            {
                P = rho;
                UpdateSet(ref ActionSet, P, ref Population);
                ga.Run(ref ActionSet, sigma_radars, ref Population);
            }
            else
            {
                ActionSet_1 = ActionSet;
                rho_1 = rho;
                sigma_radars_1 = sigma_radars;
            }

        }

        public List<Classifier> genMatchSet(List<Radar> sigma)
        {
            List<Classifier> M = new List<Classifier>();

            while (M.Count == 0)
            {
                List<Classifier> new_popn = new List<Classifier>();
                foreach (Classifier pcl in Population)
                {
                    foreach (Radar scl in sigma)
                    {
                        if (pcl.ModifyOnMatch(scl))
                            M.Add(pcl);
                        new_popn.Add(pcl);                       
                    }
                }
                Population = new_popn;

                //Count the number of actions in M
                int num_actions = 0;
                foreach (Classifier cl in M)
                {
                    num_actions += cl.actions.Count;
                }


                //theta_mna is threshold for minimum number of actions
                //TODO change back from <= to <

                if (num_actions <= theta_mna)
                {
                    globals.dumpLog("Insufficient num actions", globals.onconsole, globals.onfile);
                    Classifier clc = GenerateCoveringClassifier(M, sigma);
                    Population.Add(clc);
                    DeleteFromPopulation(ref Population);
                    M = new List<Classifier>();
                }
                else
                {
                    globals.dumpLog("Sufficient MNA, Go ahead", globals.onconsole, globals.onfile);
                }

            }

            foreach (Classifier cl in M)
            {
                double rmint = 0.0, rmaxt = 0.0;
                double rmin, rmax = 0.0;
                cl.index = 0;

                rmin = -1.0;
                for (cl.index = 0; cl.index < cl.actions.Count; cl.index++)
                {
                    if (rmin.Equals(-1.0))
                    {
                        rmin = globals.tick;
                    }
                    else
                    {
                        rmin = rmax;
                    }
                    rmax = rmin + cl.actions.ToArray()[cl.index].duration;

                    cl.ref_mints.Add((int)rmin);
                    cl.ref_mints.Add((int)rmax);

                }
            }        

            return M;
        }

    public void DeleteFromPopulation(ref List<Classifier> population)
    {
        double min_fitness = 1.00;

        if (population.Count < 15)
            return;

        foreach (Classifier cl in population)
        {
            if (cl.Fitness < min_fitness)
            {
                min_fitness = cl.Fitness;
            }
        }

        List<Classifier> new_population = new List<Classifier>();
        foreach (Classifier cl in population)
        {
            if (cl.Fitness > min_fitness)
            {
                new_population.Add(cl);
            }

        }

        population = new_population;
    }

    public Classifier GenerateCoveringClassifier(List<Classifier> M, List<Radar> sigma)
    {
        Classifier cl = new Classifier();
        Random random = new Random();
        cl.conditions = new List<Condition>();
        prob_dont_care = 0.5;


        foreach (Radar r in sigma)
        {
            Condition cond = new Condition();

            var x = random.NextDouble();

            if (x < prob_dont_care)
                cond.tfwin.mint = -1;
            else
                cond.tfwin.mint = Convert.ToInt32(r.scan_interval - 10);

            x = random.NextDouble();
            if (x < prob_dont_care)
                cond.tfwin.maxt = -1;
            else
                cond.tfwin.maxt = Convert.ToInt32(r.scan_interval + 10);

            x = random.NextDouble();
            if (x < prob_dont_care)
                cond.tfwin.minf = -1;
            else
                cond.tfwin.minf = Convert.ToInt32(r.freq - 3);

            x = random.NextDouble();
            if (x < prob_dont_care)
                cond.tfwin.maxf = -1;
            else
                cond.tfwin.maxf = Convert.ToInt32(r.freq + 3);


            //cond.tfwin.mint = random.NextDouble() < prob_dont_care ? -1 :
            //cond.tfwin.maxt = 82;
            //cond.tfwin.minf = 1;
            //cond.tfwin.maxf = 5;
            cl.conditions.Add(cond);
        }

        cl.actions = new List<Action>();
        foreach (Condition clc in cl.conditions)
        {
            Action act = new Action(0, 0);
            //act.band = (uint) (act.band * random.Next(3)+1);
            //act.duration = (uint)(act.duration * random.NextDouble()) + 1;
            act.band = (uint)(clc.tfwin.maxf - clc.tfwin.minf * random.NextDouble()) % 4;
            act.duration = (uint)(clc.tfwin.maxf - clc.tfwin.minf * random.NextDouble());
            cl.actions.Add(act);
        }

        cl.prediction = rho;
        cl.epsilon = cl.initial_epsilon;
        cl.Fitness = cl.initial_Fitness;
        cl.experience = 0;
        cl.sim_time = 0;
        cl.action_set_size = 1;
        cl.n = 1;


        return cl;
    }



    public double[] genPredictionArray(List<Classifier> M)
    {
            double[] pa = new double[M.Count];
            int index = 0;

            for (int mindex=0; mindex < M.Count; mindex++)
            {
                
                for (M[mindex].index = 0; M[mindex].index < M[mindex].actions.Count; M[mindex].index++)
                {
                    M[mindex].predn_degs.ToArray()[index] += M[mindex].predn_degs_per_sec.ToArray()[index];

                    if ((globals.tick + M.ElementAt(index).predn_degs.ElementAt(index) / M.ElementAt(index).predn_degs_per_sec.ElementAt(index))
                                           >= M.ElementAt(index).ref_mints.ElementAt(index) &&
                       (globals.tick + M.ElementAt(index).predn_degs.ElementAt(index) / M.ElementAt(index).predn_degs_per_sec.ElementAt(index))
                                           <= M.ElementAt(index).ref_maxts.ElementAt(index))
                       M.ToArray()[mindex].prediction_array.Add(0.9);
                    else
                        M.ToArray()[mindex].prediction_array.Add(0.1); ;
                    
                }            
          }

            return pa;

            //double[] pa = new double[M.Count];
            //int index = 0;
            //foreach (Classifier cl in M)
            //{
            //    double prediction = 0.0;

                //    foreach (Action act in cl.actions)
                //    {
                //        prediction += act.band * act.duration;
                //    }
                //    pa[index] = prediction / M.Count;
                //    index++;
                //}
                //return pa;
        }

    public List<Action> SelectAction(double[] PredictionArray)
    {
        int i;
        int max_index = 0;
        double max = -1.0;

        for (i = 0; i < PredictionArray.Length; i++)
        {
            if (PredictionArray[i] > max)
            {
                max = PredictionArray[i];
                max_index = i;
            }
        }

        Classifier bestCl = MatchSet.ToArray()[max_index];

        return bestCl.actions;
    }

    public List<Classifier> GenerateActionSet(List<Classifier> MatchSet, List<Action> act_seq)
    {
        List<Classifier> act_set = new List<Classifier>();

        foreach (Classifier cl in MatchSet)
        {
            foreach (Action act in cl.actions)
            {
                foreach (Action act2 in act_seq)
                {
                    if (act == act2)
                    {
                        act_set.Add(cl);
                    }
                }
            }

        }
        return act_set;
    }

    public void UpdateSet(ref List<Classifier> ActionSet_1, double P, ref List<Classifier> Population)
    {
        foreach (Classifier cl in ActionSet)
        {
            cl.experience++;

            //update prediction,p
            if (cl.experience < 1 / beta)
            {
                cl.prediction = cl.prediction + (P - cl.prediction) / cl.experience;
            }
            else
            {
                cl.prediction = cl.prediction + beta * (P - cl.prediction);
            }

            //update prediction error, cl.epsilon
            if (cl.epsilon < 1 / beta)
            {
                cl.epsilon = cl.epsilon + (Math.Abs(P - cl.prediction) - cl.epsilon) / cl.experience;
            }
            else
            {
                cl.epsilon = cl.epsilon + beta * (Math.Abs(P - cl.prediction) - cl.epsilon);
            }

            //update action set size, estimate cl.as
            uint sum_as = 0;
            foreach (Classifier acl in ActionSet)
            {
                sum_as += acl.n;
            }
            if (cl.experience < 1 / beta)
            {
                cl.action_set_size = (uint)(cl.action_set_size + ((sum_as - cl.action_set_size) / cl.experience));
            }
            else
            {
                cl.action_set_size = (uint)(cl.action_set_size + beta * (sum_as - cl.action_set_size));
            }


        }

    }

    public void UpdateFitness(List<Classifier> act_set, double v)
    {
        int accuracySum = 0;
        double[] vec_k = new double[10];
        int i = 0;
        int clid = 0;

        foreach (Classifier cl in ActionSet)
        {
            if (cl.epsilon < cl.epsilon0)
                vec_k[i] = 1;
            else
                vec_k[i] = Math.Pow((alpha * cl.epsilon / cl.epsilon0), -gamma);
            accuracySum = accuracySum + (int)(vec_k[clid] * cl.n);
        }

        foreach (Classifier cl in ActionSet)
            cl.Fitness = cl.Fitness + beta * (vec_k[clid] * cl.n);

    }
}

class TimeFreqWindow
{
    public int mint, maxt, minf, maxf;
    public TimeFreqWindow(int mint, int maxt, int minf, int maxf)
    {
        this.mint = mint;
        this.maxt = maxt;
        this.minf = minf;
        this.maxf = maxf;
    }

    public void fieldvalsToString(ref List<string> sl)
    {
        string s = mint.ToString() + "\t" + maxt.ToString() + "\t" +
                   minf.ToString() + "\t" + maxf.ToString();
        sl.Add(s);
    }
}

class Condition
{
    public TimeFreqWindow tfwin = new TimeFreqWindow(0, 0, 0, 0);

    public Condition(TimeFreqWindow tfw)
    {
        tfwin = tfw;
    }

    public Condition()
    {

    }

    public void fieldvalsToString(ref List<string> sl)
    {
        sl.Add("CONDITION:");
        tfwin.fieldvalsToString(ref sl);
    }

    public bool isEqual(Condition c)
    {
        if (tfwin.mint == c.tfwin.mint && tfwin.maxt == c.tfwin.maxt &&
            tfwin.minf == c.tfwin.mint && tfwin.maxt == c.tfwin.maxf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DoesMatch(Radar sigma_r)
    {
        //don't care conditions will be considered 
        if (tfwin.mint >= 0 && sigma_r.beam_width / 2 < tfwin.mint)
            return false;

        if (tfwin.maxt >= 0 && sigma_r.beam_width / 2 > tfwin.maxt)
            return false;

        if (tfwin.minf >= 0 && sigma_r.freq < tfwin.minf)
            return false;

        if (tfwin.maxf >= 0 && sigma_r.freq > tfwin.maxf)
            return false;



        return true;
    }



}

class Action
{
    public uint prediction;
    public uint band;
    public uint duration;
    public string symbol = string.Empty;

    public Action(uint band, uint duration)
    {
        this.band = band;
        this.duration = duration;
    }

    public void fieldvalsToString(ref List<string> sl)
    {
        sl.Add(band.ToString() + "\t" + duration.ToString());
    }


    public bool isEqual(Action a)
    {
        if (band == a.band && duration == a.duration)
            return true;
        else
            return false;
    }
}

class Classifier
{
    public List<Condition> conditions;
    public List<Action> actions;

    public static int clcnt = 0;
    public int clid;
    public List<int> sysPredictions;
    public List<int> sysOffsets;

    public int index;
    public List<int> predn_degs;
    public List<int> predn_degs_per_sec;
    public List<double> prediction_array;
    public List<double> fitnessl;
    public List<int> ref_mints;
    public List<int> ref_maxts;

    public double prediction;
    public double initial_prediction;
    public double epsilon;
    public double epsilon0;

    public double initial_epsilon;
    public double Fitness;
    public double initial_Fitness;
    public double experience;
    public uint sim_time;
    public uint action_set_size;
    public uint n;

    public Classifier()
    {
        clid = clcnt++;
        ref_mints = new List<int>();
        ref_maxts = new List<int>();
        predn_degs_per_sec = new List<int>();
        prediction_array = new List<double>();
        fitnessl = new List<double>();
        ref_mints =new  List<int>();
        ref_maxts = new  List<int>();
    }


    public void fieldvalsToString(ref List<string> sl)
    {
        sl.Add("CLASSIFIER[" + clid.ToString() + "]");
        foreach (Condition cond in conditions)
        {
            cond.fieldvalsToString(ref sl);
        }
        sl.Add("ACTIONS");
        foreach (Action act in actions)
        {
            act.fieldvalsToString(ref sl);
        }


        string s = clid.ToString() + "\t" + prediction.ToString() + "\t" + initial_prediction.ToString() + "\t" +
                   epsilon.ToString() + "\t" + epsilon0.ToString() + "\t" + initial_epsilon.ToString() + "\t" +
                   Fitness.ToString() + "\t" + initial_Fitness.ToString() + "\t" +
                   experience.ToString() + "\t" + sim_time.ToString() + "\t" + action_set_size.ToString() + "\t" +
                   n.ToString();


    }


    public bool ModifyOnMatch(Radar sigma_r)
    //public bool DoesMatch(Radar sigma_r)
    {

        foreach (Condition cond in conditions)
        {

            if (cond.DoesMatch(sigma_r) == false)
              {
                sysPredictions = new List<int>();
                return false;
            }
            if (predn_degs != null)
            {
                //where will this be initialized?
                //on first hit fields are populated on radar
                predn_degs.ToArray()[index] = (int)sigma_r.mb_azim;
                predn_degs_per_sec.ToArray()[index] = (int)sigma_r.beam_width * 2;
                //symbol =    sigma_r.symbol;
            }            
        }
        return true;
    }

}

class ReInforcementProgram
{
    List<Radar> prevRadarList = new List<Radar>();
    public double evalReward(List<Radar> newRadar)
    {
        return 0.0;
    }
}
}
