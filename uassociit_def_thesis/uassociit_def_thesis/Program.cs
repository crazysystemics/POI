using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Tasks 
//DONE
//00_01: Initial Baseline: fitness for  m(100) steps x n(32) species in each step 
//TODO
//00_02: [*]Evolving fitness using SOC algorithm (mutating over neighbors fitness)                        
//00_03: IIT (Compute-Phi, Given a Mechanism, Inputs and Outputs)
//00_04: IIT + SOC :: Phi as evolving parameter
//00_05: UAS_Swarm (Linear), Blue vs Red Battle (a Random Simulation)
//00_06: UAS_Swarm => (IIT Network) => phi => phi as SOC parameter
//00_07: Merging with earlier code. blue_uas[i].evolve_next_step
//00_08: Conditiona formatting for cluster should be seen, Marking VVV before mutation...
//git test 01
namespace uas_soc_iit_dt
{

    class Program
    {
        //Algorithm: 
        //1. Compute Clusters based on Algorithm
        //2. Given Current State s and previous purview p
        //3. When same cluster continues, with noisy state s (noisy a, b or c)
        //4. When same 

        //1. Every Mechanism in Population is candidate for center of cluster.
        //2. Its fitness is computed by taking its and its neighbors data.
        //3. Transitions in sequence formed by its LEFT, ITSELF, its RIGHT are observed
        //4. and fitness computed. Thus phi is nothing but fitness of Mechanism-x is 
        //5. fitness of Cluster with Mechanism-x as center and its neighbors following OAX sequence

        //6. By replacing random fitness, in fact random cluster centers are being declared in the 
        //7. neighborhood thus disturbing equillibrium in ad-hoc way (mutation)

        public static soc_population blue_uas = new soc_population(32);
        public static cyclic_counter ci = new cyclic_counter(32);
        public static Random r = new Random(5);

        static void Main(string[] args)
        {
            //sglobal.sw  = new StreamWriter("soc.csv");
            sglobal.r = new Random(5);
            blue_uas.init_population();
            //sglobal.dsw = new StreamWriter("debug1.csv");

            //TODO:Encapsulate in a method
            //open the file
            if (sglobal.sw == null)
            {
                sglobal.sw = new StreamWriter("soc.csv");
            }
            else
            {
                sglobal.sw = new StreamWriter("soc.csv", true);
            }

            //TODO: make a proper conversion function
            //      within the class
            double[] bcopy = new double[blue_uas.buf.Length];
            for (int bi = 0; bi < blue_uas.buf.Length; bi++)
            {
                bcopy[bi] = blue_uas.buf[bi].fitness;
            }

            //log: Priniting initial state
            //sglobal.sw.WriteLine(blue_uas[blue_uas.minfit()].ToString());
            string fitness_s = String.Empty;
            for (int index = 0; index < blue_uas.buf.Length; index++)
            {
                string delim = String.Empty;
                delim = (index == 0 ? "init_fit ," : ",");
                double d = blue_uas.buf[index].fitness;
                fitness_s += delim + blue_uas.buf[index].fitness.ToString();
            }
            sglobal.sw.WriteLine(fitness_s);

            //log: Priniting Header
            string header = String.Empty;
            header = "step";
            for (int i = 0; i < 32; i++)
            {
                header += ", fit-" + i.ToString();
            }

            sglobal.sw.WriteLine(header);

            //log: Main Evolution Loop

            List<string> current_population_species_fitness_list;
            for (int step = 0; step < 100; step++)
            {
                current_population_species_fitness_list = new List<string>();

                //double[] fitness = new double[blue_uas.buf.Length];
                //Write Current step and population fitness into file                
                for (int i = 0; i < blue_uas.buf.Length; i++)
                {
                    current_population_species_fitness_list.Add(blue_uas.buf[i].fitness.ToString());
                }

                int count = 0;
                sglobal.sw.Write(step);

                int min_index = blue_uas.minfit();
                int left_limit = min_index - blue_uas.cluster_size
                                       + blue_uas.population_size % blue_uas.population_size;
                int right_limit = count + blue_uas.cluster_size % blue_uas.population_size;

                foreach (string s in current_population_species_fitness_list)
                {
                    // (min_fit_pos + 1 ) is to skip "step" column
                    //left_limit <= right_limit:  a1
                    //left_limit > right_limit:   !a1
                    //count >= left_limit         a2
                    // count <= right_limit       a3
                    //count >=  0                 b2
                    //count < right_limit         b3
                    //count >= left_limit         b4
                    //count <= blue_uas.population_size   b5

                    //(a1 * a2 * a3) + (b1 * (b2 * b3) * (b4 + b5))
                    //= (a1 + !a1) * ((b1 * b2 * b3) * (b4 +b5))
                    //= (a1.a2.a3) + (b1.b2.b3.b4 + b1.b2.b3.b5)
                    //= a1..a13 + b1..b5 + b2..b5

                    if (( //[a * b * c]
                        left_limit <= right_limit &&
                        count >= left_limit && count <= right_limit
                    )
                    ||
                    ( //[b1b2b3]
                        left_limit > right_limit
                         && count >= 0 && count < right_limit
                         && count >= left_limit
                    ))
                    {
                        //||
                        //(
                        //        count >= 0 && count < right_limit
                        //     && count >= left_limit
                        //     && count <= blue_uas.population_size
                        //)
                    }


       
      
     )
 )

                    if (
                          ( 
                             left_limit <= right_limit &&
                             count >= left_limit && count <= right_limit
                           )
                          ||
                          ( //[!a * d * e]
                            //nested compound statements
                              left_limit > right_limit
                              &&
                              (
                                count >= 0 && count < right_limit
                                ||
                                count >= left_limit && count <= blue_uas.population_size
                              )
                          )
                    )
                    {
                        sglobal.sw.Write("," + "VVV " + s);
                    }
                    else
                    {
                        sglobal.sw.Write("," + s);
                    }

                    count++;
                }
                sglobal.sw.WriteLine();


                blue_uas.evolve_next_step();

            }

            sglobal.sw.Close();
        }


    }


}


