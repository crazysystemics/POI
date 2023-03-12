using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using uas_soc_iit_dt;

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

    class red_uas
    {
        public int num_radars;
        public int num_clusters;
        public int[] radars = new int[32];
        public int[] cluster_starts = new int[32];
        public int[] cluster_ends = new int[32];

        public red_uas(int pnum_radars, int pnum_clusters,
                       int radarval = null)
        {

            num_radars = pnum_radars;
            num_clusters = pnum_clusters;

            radars = new int[num_radars];
            cluster_starts = new int[num_clusters];
            cluster_ends = new int[num_clusters];

            if (radarval == null)
            {
                for (int i = 0; i < num_radars; i++)
                {
                    radars[i] = 0;
                }
            }
            else
            {
                for (int i = 0; i < num_radars; i++)
                {
                    radars[i] = radarval[i];
                }

            }
        }

        public void next()
        {
            for (int cluster_index = 0; cluster_index < num_clusters; cluster_index++)
            {
                int ia = cluster_starts[cluster_index];
                int ib = cluster_ends[cluster_index + 1];
                int ic = cluster_ends[cluster_index + 2];

                radars[ia] = (radars[ib] + radars[ic]) > 0 ? 1 : 0;
                radars[ib] = radars[ia] * radars[ic];
                radars[ic] = (radars[ib] + radars[ic]) == 1 ? 1 : 0;
            }
        }
    }
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
    public red_uas red_uas_list = new red_uas(32, 1);
    public Mechanism[] buf;
    public double[] noise_level = new double[population_size];




    //public int min_fit_pos;
    //public double min_fitness;

    public soc_population(int psize, int csize = 3)
    {
        population_size = psize;
        buf = new Mechanism[population_size];
        cluster_size = csize;
        for (int i = 0; i < population_size; i++)
        {
            noise_level[i] =
        }
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
        red_uas_list.next();
        int minci = minfit();
        double minf = buf[minci].fitness;


        //min_fit_pos = minci;
        //min_fitness = minf;

        //sglobal.sw.WriteLine(minf.ToString());                     

        //replace 1-left, 1-cur, 1-right with random numbers
        //one cluster means one left one right
        //3 clusters mean 1 left cluster 1 right cluster
        //Left Cluster will have Left member. 
        //This makes 2 LEFT and similarly 2 RIGHT

        int index;
        int min_index = minci;
        //min_index=1, population_size=8
        //left_limit=(1+8-2)%8=7%8=7
        int left_limit = (min_index + population_size - 2) % population_size;
        //min_index=6, population_size=8
        //right_limit=(6+8+2)%8=16%8=0
        int right_limit = (min_index + population_size + 2) % population_size;

        string fitness_csv = string.Empty;

        
        double total_fitness = 0.0, average_fitness = 0.0;
        int key1;

        for (index = left_limit; index != right_limit;)
        {
            //fitness is formed randomly. This results in reformation
            //of clusters, mutation.
            buf[index].fitness = sglobal.r.NextDouble();
            total_fitness += buf[index].fitness;
            key1 = sglobal.r.NextInt(2);

            //key1 determines makes past random
            //if key1 == 0, it is clean, belongs to partion      1
            //if key1 == 1, it is noisy, belonging to partition  2
            //here purview is formed
            if (key1 == 0)
            {
                buf[index].partition1[p1++];
            }
            else
            {
                buf[index].partition[p2++];
            }

            //fitness_csv += buf[index].fitness + ",";
            //buf[index].locked[0] = Convert.ToBoolean(sglobal.r.Next(2));
            //buf[index].locked[1] = Convert.ToBoolean(sglobal.r.Next(2));
            //buf[index].locked[2] = Convert.ToBoolean(sglobal.r.Next(2));
            index = (index + 1) % population_size;
        }
        average_fitness = (total_fitness / population_size);
        //fitness_csv += average_fitness;
        //}
    }
}

