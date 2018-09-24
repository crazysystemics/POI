using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POI_XCS
{
    class GA
    {
        double theta;
        double psi;
        double mu;

        Classifier parent1;
        Classifier parent2;
        Classifier child1;
        Classifier child2;
        Random random;

        public GA(double t, double p, double m)
        {
            theta = t;
            psi = p;
            mu = m;
        }


        public void Run(ref List<Classifier> A, List<Radar> sigma, ref List<Classifier> Population)
        {
            uint sum_tn=0;
            uint sum_n = 0;

            foreach(Classifier acl in A)
            {
                sum_tn += acl.sim_time * acl.n;
                sum_n += acl.n; 
            }

            if (globals.tick - sum_tn/sum_n > theta)
            {
                foreach(Classifier cl in A)
                {
                    cl.sim_time = globals.tick;
                }

                parent1 = SelectOffspring(A);
                parent2 = SelectOffspring(A);
                child1 = parent1;
                child2 = parent2;

                if (random.NextDouble() < psi)
                {
                    ApplyCrossover(child1, child2);
                    child1.prediction = (parent1.prediction + parent2.prediction) / 2;
                    child1.epsilon = (parent1.epsilon + parent2.epsilon) / 2;
                    child1.Fitness = (parent1.Fitness + parent2.Fitness) / 2;
                    child2.prediction = child1.prediction;
                    child2.epsilon = child1.epsilon;
                    child2.Fitness = child1.Fitness;
                    child1.Fitness = child1.Fitness * 0.1;
                    child2.Fitness = child2.Fitness * 0.1;

                    ApplyMutation(child1, sigma);
                    ApplyMutation(child2, sigma);

                    InsertInPopulation(child1, Population);
                    InsertInPopulation(child2, Population);
                    DeleteFromPopulation(ref Population);

                }

                child1.n = child2.n = 1;
                child1.experience = child2.experience = 0;                
            }
        }

        //Roulette Wheele Selection
        public Classifier SelectOffspring(List<Classifier> Population)
        {
            double fitnessSum = 0;
            double choicePoint = 0.0;

            foreach(Classifier cl in Population)
            {
                fitnessSum += cl.Fitness;
            }

            choicePoint = random.NextDouble() * fitnessSum;
            fitnessSum = 0;

            foreach (Classifier cl in Population)
            {
                fitnessSum += cl.Fitness;
                if (fitnessSum > choicePoint)
                {
                    return cl;
                }
            }

            return new Classifier();
        }

        //two-point crossover
        public void ApplyCrossover(Classifier cl1, Classifier cl2)
        {
            int cond_count = 0;
            //if (cl1.conditions.Count < cl2.conditions.Count + 1)
            //    cond_count = cl1.conditions.Count;
            //else
            //    cond_count = cl2.conditions.Count;

            cond_count = cl1.conditions.Count;
            uint x = (uint)(random.NextDouble() * (cond_count + 1));
            uint y = (uint)(random.NextDouble() * (cond_count + 1));

            if (x > y)
            {
                uint temp;
                temp = x; x = y; y = temp;
            }

            for(uint i=x; i < y; i++)
            {
                Condition temp_cond = cl1.conditions.ToArray()[x];
                cl1.conditions.ToArray()[x] = cl1.conditions.ToArray()[y];
                cl1.conditions.ToArray()[y] = temp_cond;
            }
        }

        public void InsertInPopulation(Classifier cl, List<Classifier> population)
        {
            foreach (Classifier c in population)
            {
                if (c.conditions.Count != cl.conditions.Count)
                    continue;

                if (c.actions.Count != cl.actions.Count)
                    continue;

                int i = 0;
                for (i = 0; i < c.conditions.Count; i++)
                {
                    if (!c.conditions[i].isEqual(cl.conditions[i]))
                        break;
                }

                if (i < c.conditions.Count)
                {
                    continue;
                }

                //conditions are same, check for actions
                for (i = 0; i < c.actions.Count; i++)
                {
                    if (!c.actions[i].isEqual(cl.actions[i]))
                        break;
                }

                if (i < c.actions.Count)
                {
                    continue;
                }

                //conditions and actions are same, insert it in population
                c.n++;
                return;
            }

            population.Add(cl);
        }

        public void ApplyMutation(Classifier cl, List<Radar> sigma)
        {
            int i = 0;

            do
            {
                if (random.NextDouble() < mu)
                {
                    if (cl.conditions[i].tfwin.mint == -1)
                    {
                        cl.conditions[i].tfwin.mint = (int)sigma.ToArray()[i].posx;
                    }
                    else
                    {
                        cl.conditions[i].tfwin.mint = -1;
                    }

                }

                if (random.NextDouble() < mu)
                {
                    if (cl.conditions[i].tfwin.maxt == -1)
                    {
                        cl.conditions[i].tfwin.maxt = (int)sigma.ToArray()[i].posx;
                    }
                    else
                    {
                        cl.conditions[i].tfwin.maxt = -1;
                    }

                }

                if (random.NextDouble() < mu)
                {
                    if (cl.conditions[i].tfwin.minf == -1)
                    {
                        cl.conditions[i].tfwin.minf = (int)sigma.ToArray()[i].freq[0];
                    }
                    else
                    {
                        cl.conditions[i].tfwin.minf = -1;
                    }

                }

                if (random.NextDouble() < mu)
                {
                    if (cl.conditions[i].tfwin.maxf == -1)
                    {
                        cl.conditions[i].tfwin.maxf = (int)sigma.ToArray()[i].freq[0];
                    }
                    else
                    {
                        cl.conditions[i].tfwin.maxf = -1;
                    }
                }
            } while (i < cl.conditions.Count);
        }

        //TODO:Rewrite Correctly
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


    }
}
