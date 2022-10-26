using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace uas_soc_iit_dt
{
    class cyclic_counter
    {
        public int size;
        public int count;
        public int deltas;

        public static implicit operator int(cyclic_counter c) => c.count;
        public static implicit operator cyclic_counter(int ci) => new cyclic_counter(ci);
        //public static explicit operator int(cyclic_counter c) => c.count;



        public cyclic_counter(int c = 32, int d = 0)
        {
            size = 32;
            count = 0;
            deltas = d;
        }

        static public cyclic_counter operator ++(cyclic_counter c)
        {
            c.count = (c.count + 1) % c.size;
            c.deltas++;
            return c;
        }

        static public cyclic_counter operator --(cyclic_counter c)
        {
            c.count = (c.count + c.size - 1) % c.size;
            //TODO:Manage deltas in case of --
            c.deltas = -1;
            return c;
        }
        public void reset_deltas()
        {
            deltas = 0;
        }

    }

    class soc_species
    {
        public double fitness;

        public static implicit operator double(soc_species ss) => ss.fitness;

        public soc_species()
        {

        }
    }

    class soc_population
    {
        public int population_size;
        public int cluster_size;
        public int generation = 0;
        public Mechanism[] buf;
        //public int min_fit_pos;
        //public double min_fitness;
                 
        public soc_population(int psize, int csize = 3)
        {
            population_size = psize;
            buf             = new Mechanism[population_size];
            cluster_size    = csize;
        }

        public Mechanism this[cyclic_counter key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        public Mechanism GetValue(cyclic_counter index)
        {
            Debug.Assert(index < 2 * population_size);
            if (index < 0)
            {
                index = population_size - index;
            }
            else if (index >= population_size)
            {
                index = population_size + index;
            }
            return buf[index];
        }

        public void SetValue(cyclic_counter index, Mechanism value)
        {
            buf[index] = value;
        }

        override public string ToString()
        {
            string s = String.Empty;
            for (int i = 0; i < buf.Length; i++)
            {
                s = s + buf[i].fitness.ToString(); ;
                if (i < buf.Length - 1)
                {
                    s = s + ",";
                }
            }
            return s;
        }

        public void init_population()
        {
            //bool first = true;

            cyclic_counter ci = 0;
            ci.reset_deltas();
            for (; ci.deltas < ci.size; ci++)
            {
                //if (ci == 0)
                //{
                //    if (first)
                //    {
                //        first = false;
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}

                buf[ci] = new Mechanism();
                buf[ci].fitness = sglobal.r.NextDouble();
            }

        }

        public int minfit()
        {
            double minf = 1.0;
            int mini = 0;
            //bool first = true;


            for (int i = 0; i < population_size; i++)
            {
                //if (ci == 0)
                //{
                //    if (first)
                //    {
                //        first = false;
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}

                if (buf[i].fitness < minf)
                {
                    minf = buf[i].fitness;
                    mini = i;
                }
            }
            return mini;
        }
        
        public void evolve_next_step()
        {
            generation++;
            int minci = minfit();
            double minf = buf[minci].fitness;

            //min_fit_pos = minci;
            //min_fitness = minf;

            //sglobal.sw.WriteLine(minf.ToString());                       
           

            //replace 1-left, 1-cur, 1-right with random numbers
            //totally 3 clusters and 3 cells for each totally 3*3 =9
            //since 9 cells have to be replaced, 4-left 1-cur 4-right
            int index;
            int min_index = minci;
            int brndm_index = (min_index + population_size - 4) % population_size;
            int erndm_index = (min_index + population_size + 4 + 1) % population_size;

            string fitness_csv = string.Empty;

            
            int REAL = 0;
            int DUMMY = 1;
            int mode_comp_fitness = DUMMY;

            if (mode_comp_fitness == REAL)
            {

                for (index = 0; index < buf.Length; index++)
                {
                    buf[index].fitness = buf[index].ComputePhi();
                    //fitness of all species in population
                    //fitness_csv += buf[index].fitness + ",";
                }

            }
            else
            {
                //brute-force and short-cut computation of fitness
                //of a Species (Mechanism or UAV)
                //just get random number
                double total_fitness = 0.0, average_fitness = 0.0;

                for (index = brndm_index; index != erndm_index;)
                {
                    buf[index].fitness = sglobal.r.NextDouble();
                    total_fitness += buf[index].fitness;

                    //fitness_csv += buf[index].fitness + ",";
                    //buf[index].locked[0] = Convert.ToBoolean(sglobal.r.Next(2));
                    //buf[index].locked[1] = Convert.ToBoolean(sglobal.r.Next(2));
                    //buf[index].locked[2] = Convert.ToBoolean(sglobal.r.Next(2));
                    index = (index + 1) % population_size;
                }
                average_fitness = (total_fitness / population_size);
                //fitness_csv += average_fitness;
            }    
        }
    }
}
