﻿using System;
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
namespace selfoc_evoln
{

    class Program
    {
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

                bool boundary_crossed = false;
                int index = min_index;
                while (index != left_limit)
                {
                    //change
                    if (!boundary_crossed)
                    {
                        //change state
                        index--;
                    }
                    else if (index == 0)
                    {
                        //change state
                        index = current_population_species_fitness_list.Count - 1;
                        boundary_crossed = true;
                    }
                    else if (boundary_crossed)
                    {
                        //change state
                        index++;
                    }
                    else
                    {
                        Debug.Assert(false, "Invalid  LEFT Side Mutation");
                    }
                }

                boundary_crossed = false;
                index = min_index;
                while (index != right_limit)
                {
                    //change
                    if (!boundary_crossed)
                    {
                        //change state
                        index = 0;
                    }
                    else if (index == current_population_species_fitness_list.Count - 1)
                    {
                        //change state
                        index = 0;
                        boundary_crossed = true;
                    }
                    else if (boundary_crossed)
                    {
                        //change state
                        index--;
                    }
                    else
                    {
                        Debug.Assert(false, "Invalid  RIGHT Side Mutation");
                    }


                }

                evolve_next_step();

            }

            sglobal.sw.Close();
        }


    }


}


