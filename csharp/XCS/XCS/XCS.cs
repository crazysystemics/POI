using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public List<Radar>  sigma_radars = new List<Radar>();
        public List<Action> alpha_actions = new List<Action>();
        public List<Action> default_actions = new List<Action>();
        public ReInforcementProgram rp;
        public XCS  xcs;
        public bool eop;       

        public void runNextCycle()
        {
            if (rp == null)
                rp = new ReInforcementProgram();

            alpha_actions = xcs.get_action_sequence(sigma_radars);           
        }
    }

    class XCS
    {
        public List<Classifier> Population  = new List<Classifier>();
        public List<Action> default_actions = new List<Action>();
        double P; //Population number

        public List<Classifier> MatchSet    = new List<Classifier>();
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


        public List<Action> get_action_sequence(List<Radar> env_sigma_radars)
        {

                sigma_radars = env_sigma_radars;
                MatchSet=genMatchSet(Population, sigma_radars);
                PredictionArray = genPredictionArray(MatchSet);
                act_seq = SelectAction(PredictionArray);
                ActionSet = GenerateActionSet(MatchSet, act_seq);

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

        public List<Classifier> genMatchSet(List<Classifier> P, List<Radar> sigma)
        {
            List<Classifier> M = new List<Classifier>();

            while (M.Count == 0)
            {
                foreach (Classifier pcl in Population)
                    foreach (Radar scl in sigma)
                    {
                        if (pcl.DoesMatch(scl))
                        {
                            M.Add(pcl);
                        }
                    }

                //Count the number of actions in M
                int num_actions = 0;
                foreach(Classifier cl in M)
                {
                    num_actions += cl.actions.Count;                    
                }


                if (num_actions < theta_mna)
                {
                    Classifier clc = GenerateCoveringClassifier(M, sigma);
                    Population.Add(clc);
                    DeleteFromPopulation(ref Population);
                    M = new List<Classifier>();
                }
                
            }

                        return M;
        }

        public void DeleteFromPopulation(ref List<Classifier> population)
        {
            double min_fitness = 1.00;
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

            foreach(Radar r in sigma)
            {
                Condition cond=new Condition();

                var x = random.NextDouble();

                if (x < prob_dont_care)
                    cond.tfwin.mint = -1;
                else
                    cond.tfwin.mint = Convert.ToInt32(r.posx - 10);

                x = random.NextDouble();
                if (x < prob_dont_care)
                    cond.tfwin.maxt = -1;
                else
                    cond.tfwin.mint = Convert.ToInt32(r.posx +10);

                x = random.NextDouble();
                if (x < prob_dont_care)
                    cond.tfwin.minf = -1;
                else
                    cond.tfwin.minf = Convert.ToInt32(r.posy - 10);

                x = random.NextDouble();
                if (x < prob_dont_care)
                    cond.tfwin.minf = -1;
                else
                    cond.tfwin.minf = Convert.ToInt32(r.posy + 10);

                cl.conditions.Add(cond);
            }

            cl.actions = default_actions;
            cl.prediction = cl.initial_prediction;
            cl.epsilon = cl.initial_epsilon;
            cl.Fitness = cl.initial_Fitness;
            cl.experience = 0;
            cl.sim_time = 0;
            cl.action_set_size = 1;
            cl.n = 1;


            return null;
        }

    

       public double[] genPredictionArray(List<Classifier> M)
        {
            double[] pa = new double[M.Count];
            int index = 0;
            foreach (Classifier cl in M)
            {
                double prediction = 0.0;
                
                foreach(Action act in cl.actions)
                {
                    prediction += act.band * act.duration;
                }
                pa[index] = prediction / M.Count;
                index++;                
            }
            return pa;
        }

        public List<Action> SelectAction(double[] PredictionArray)
        {
            int i;
            int max_index=0;
            double max=-1.0;

            for (i=0; i < PredictionArray.Length; i++)
            {
                if  (PredictionArray[i] > max)
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
            List<Classifier> act_set=new List<Classifier>();

            foreach(Classifier cl in MatchSet)
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
            foreach(Classifier cl in ActionSet)
            {
                cl.experience++;
                
                //update prediction,p
                if (cl.experience < 1/beta)
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
                    cl.action_set_size =(uint)( cl.action_set_size + ((sum_as - cl.action_set_size) / cl.experience));
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

            foreach(Classifier cl in ActionSet)
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
            if (tfwin.mint >= 0 && sigma_r.posx < tfwin.mint)
                return false;

            if (tfwin.maxt >= 0 && sigma_r.posx >tfwin.mint)
                return false;

            if (tfwin.minf >= 0 && sigma_r.posy < tfwin.minf)
                return false;

            if (tfwin.maxf >= 0 && sigma_r.posy > tfwin.minf)
                return false;

            return true;
        } 
    
    }

    class Action
    {
        public uint band;
        public uint duration;

        public Action(uint band, uint duration)
        {

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
        
        public bool DoesMatch(Radar sigma_r)
        {
            foreach(Condition cond in conditions)
            {
                if (cond.DoesMatch(sigma_r) == false)
                    return false;
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
