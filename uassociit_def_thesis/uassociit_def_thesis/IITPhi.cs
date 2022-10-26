using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//JE: Journal Entry: Resuming on 13th March
//
namespace uas_soc_iit_dt
{
    enum row_lbls { b000, b001, b010, b011, b100, b101, b110, b111 };
    enum col_lbls { Ap, Bp, Cp, Ac, Bc, Cc, Af, Bf, Cf }

    static class sglobal
    {
        public static Random r;
        public static StreamWriter sw; //= new StreamWriter("debug.csv");
        //public static StreamWriter dsw = new StreamWriter("ddebug.csv");
        public static row_lbls rl;
        public static col_lbls cl;
        public static bool store_debug;
    }

    class Partition
    {
        public int[]  p1 = new int[3];
        public int    p1_count;
        public int[]  p2 = new int[3];
        public int    p2_count;
        public bool[] p1p2 = new bool[8];
        public int    p1p2_count;
    }


    class Purview
    {
        public static int sid = 0;
        public int id;
        public Mechanism Whole; //ABCc
        //public Triplet<bool?> ElementState;

        public Mechanism InPurviewCurrent;//BCc
        public Mechanism InPurviewCause; //Ap
        public Mechanism InPurviewEffect;//?

        public Mechanism CoreCause;
        public Mechanism CoreEffect;


        public double cause_phi_max;
        public double effect_phi_max;
        public double ce_phi_max;


        public List<Partition> Partitions = new List<Partition>();

        public double cause_phi = 0.0;
        public double effect_phi = 0.0;

        public Purview()
        {
            sid++;
            id = sid;
        }

        public Purview(Purview pur) : this()
        {
            Whole = pur.Whole;
            //ElementState.Set(false, false, false);
            // = new Triplet<bool?>(null, false, false); //BCc=00
            InPurviewCurrent = pur.InPurviewCurrent;
            InPurviewCause = pur.InPurviewCause;
            InPurviewEffect = pur.InPurviewEffect;
            Partitions = pur.Partitions;
        }
    }

    class Mechanism
    {
        int?[,] PartitionTemplates = { { null, 0, 1, 2 }, { 0, null, 1, 2 }, { 0, 1, null, 2 }, { 0, 1, 2, null } };
        Partition[] partns = new Partition[4];
        public double fitness = 0.0;
        bool is_cluster_center = false;

        int SYM_NUM_4 = 4;
        int pcount;

        //only one state per radar
        //making locked variable than array
        //public bool[] locked = new bool[3];

        //three entries refer to past, present and future of 
        //current mechanism - not to be confused with left-current-right
        public bool[] locked = { false, false, false };

        bool[,] state_table = new bool[8, 9];
        bool[] cause_s = { false, false, false, false, false, false, false, false };
        bool[] effect_s = { false, false, false, false, false, false, false, false };

        public void kernel(bool Ap, bool Bp, bool Cp, ref bool An, ref bool Bn, ref bool Cn)
        {
            //cur_time = 0 past, cur_time = 3 current, cur_time = 6
            An = Bp || Cp;
            Bn = Ap && Cp;
            Cn = (Ap != Bp);
        }

        public bool areEqual(bool a1, bool b1, bool c1, bool a2, bool b2, bool c2)
        {
            return (a1 == a2 && b1 == b2 && c1 == c2);
        }

        public void initPartitions()
        {
            pcount = SYM_NUM_4;
            int cur, p1_cur, p2_cur;

            for (int i = 0; i < SYM_NUM_4; i++)
            {
                partns[i] = new Partition();
                partns[i].p1 = new int[] { 0, 0, 0 };
                partns[i].p2 = new int[] { 0, 0, 0 };
                for (p1_cur = 0, cur = 0; cur < pcount && PartitionTemplates[i, cur] != null; cur++)
                {
                    partns[i].p1[p1_cur] = PartitionTemplates[i, cur].Value;
                    partns[i].p1_count++;
                }

                //move p1_cur, one position beyond null - now it is in p2
                for (cur++, partns[i].p2_count = 0; cur < pcount; cur++)
                {
                    partns[i].p2[partns[i].p2_count] = PartitionTemplates[i, cur].Value;
                    partns[i].p2_count++;
                }

            }
        }

        //cur_state to prev_state what is phi_cause?
        public double ComputePhi()
        {
            double[] phi_cs = new double[4];
            double phi_c    = 0.0;
            double min_info = 1.0;
            double max_info = 0.0;

            bool?[] s = { false, false, false };
            double[] cause_dstrbn = new double[8];
            double[] min_cause_distribn = new double[8];
            double[] max_cause_distribn = new double[8];


            Partition mip = new Partition();
            Partition maxp = new Partition();

            //assumption: purview contains all columns [a, b, c]
            initPartitions();
            s[2] = false;//locked[2];
            s[1] = true; //locked[1];
            s[0] = true; //locked[0];

            for (int i = 0; i < SYM_NUM_4; i++)
            {
                ComputeCauseDistribution(s, partns[i], ref cause_dstrbn);
                double cdmin = cause_dstrbn.Max();
                if ( cdmin < min_info){   
                    
                    min_info = cause_dstrbn.Max();
                    min_cause_distribn = cause_dstrbn;
                }

                double cdmax = cause_dstrbn.Max();
                if (cdmax > max_info)
                {
                    max_info = cause_dstrbn.Max();
                    max_cause_distribn = cause_dstrbn;
                }
            }

            phi_c = find_distance(min_cause_distribn, max_cause_distribn);

            return phi_c;
        }

        double find_distance(double[] from_dstrbn, double[] to_dstrbn )
        {
            //normalise--hamming's + EMD
            int[] from_coordinates = new int[8];
            int[] to_coordinates   = new int[8];
            int[] distance = new int[8];
            
            for (int i=0; i < from_dstrbn.Length; i++)
            {
                from_coordinates[i] = (int)Math.Round(from_dstrbn[i] * (1 / from_dstrbn.Length));
            }

            for (int i = 0; i < to_dstrbn.Length; i++)
            {
                to_coordinates[i] = (int)Math.Round(to_dstrbn[i] * (1 / to_dstrbn.Length));
            }

            double sum_distance = 0.0;
            for (int i = 0; i < from_dstrbn.Length; i++)
            {
                distance[i] = Math.Abs(to_coordinates[i] - from_coordinates[i]);
                sum_distance += distance[i];
            }

            return (double)sum_distance / (double)from_dstrbn.Length; 
        }


        ////purview is used to identify what is the cause and what is the effect
        //public double ComputePhiCause(bool?[] s, Partition p, ref double[] cd)
        //{
        //    double[] cause_dist = new double[3];
        //    //double[] effect_prob_dist = new double[3];
        //    find_cause_info(ref cause_dist);
        //    //find_effect_info(ref effect_prob_dist);
        //    //find_phi_ce(ref cause_prob_dist, ref effect_prob_dist, ref phi_ce);

        //    return 0.0;
        //}

        public void ComputeCauseDistribution(bool?[] s, Partition p, ref double[] cause_prob_dist)
        {
            bool[] columns = { true, false };
            int row_index;
            int d2, d1, d0;
            //bool An = false, Bn = false, Cn = false;

            //fill every row of state table
            for (row_index = 0; row_index < 8; row_index++)
            {
                d2 = row_index / 100;
                d1 = (row_index / 10) % 10;
                d0 = (row_index) % 10;

                state_table[row_index, (int)col_lbls.Ap] = Convert.ToBoolean(d2);
                state_table[row_index, (int)col_lbls.Bp] = Convert.ToBoolean(d1);
                state_table[row_index, (int)col_lbls.Cp] = Convert.ToBoolean(d0);

                state_table[row_index, (int)col_lbls.Ac] = state_table[row_index, (int)col_lbls.Bp] || state_table[row_index, (int)col_lbls.Cp];
                state_table[row_index, (int)col_lbls.Bc] = state_table[row_index, (int)col_lbls.Ap] && state_table[row_index, (int)col_lbls.Cp];
                state_table[row_index, (int)col_lbls.Cc] = state_table[row_index, (int)col_lbls.Ap] && state_table[row_index, (int)col_lbls.Bp];

                state_table[row_index, (int)col_lbls.Af] = state_table[row_index, (int)col_lbls.Bc] || state_table[row_index, (int)col_lbls.Cp];
                state_table[row_index, (int)col_lbls.Bf] = state_table[row_index, (int)col_lbls.Ac] && state_table[row_index, (int)col_lbls.Cc];
                state_table[row_index, (int)col_lbls.Cf] = state_table[row_index, (int)col_lbls.Ac] && state_table[row_index, (int)col_lbls.Bc];
            }

            //determining cause_s
            for (row_index = 0; row_index < 8; row_index++)
            {
                d2 = row_index / 100;
                d1 = (row_index / 10) % 10;
                d0 = (row_index) % 10;
                bool Acs = false, Bcs = false, Ccs = false;

                kernel(state_table[row_index, d2], state_table[row_index, d1], state_table[row_index, d0], ref Acs, ref Bcs, ref Ccs);

                if (areEqual(state_table[row_index, d2], state_table[row_index, d1], state_table[row_index, d0], Acs, Bcs, Ccs))
                {
                    cause_s[row_index] = true;  
                }
            }

            //
            for (row_index = 0; row_index < 8; row_index++)
            {
                d2 = row_index / 100;
                d1 = (row_index / 10) % 10;
                d0 = (row_index) % 10;
                bool Acs = false, Bcs = false, Ccs = false;

                kernel(state_table[row_index, d2], state_table[row_index, d1], state_table[row_index, d0], ref Acs, ref Bcs, ref Ccs);

                if (areEqual(state_table[row_index, d2], state_table[row_index, d1], state_table[row_index, d0], Acs, Bcs, Ccs))
                {
                    cause_s[row_index] = true;
                    int[] p1p2_cause_s = new int[8];

                    int p1iter, p2iter;

                    //keep p1 fix and noise P2
                    bool[] p1_p2_row = { false, false, false };
                    for (p1iter = 0; p1iter < p.p1_count; p1iter++)
                    {
                        p1_p2_row[p.p1[p1iter]] = state_table[row_index, p.p1[p1iter]];
                    }

                    for (int p2_loop_index = 0; p2_loop_index < Math.Pow(2, p.p2.Length); p2_loop_index++)
                    {
                        for (p2iter = 0; p2iter < p.p2_count; p2iter++)
                        {
                            p1_p2_row[p.p2[p2iter]] = state_table[row_index, p.p2[p2iter]];
                        }
                    }

                    //process p1 all rows and p2 fixed
                    p1_p2_row = new bool[3] { false, false, false };
                    for (p2iter = 0; p2iter < p.p2_count; p2iter++)
                    {
                        p1_p2_row[p.p2[p2iter]] = state_table[row_index, p.p2[p2iter]];
                    }

                    for (int p1_loop_index = 0; p1_loop_index < Math.Pow(2, p.p1.Length); p1_loop_index++)
                    {
                        for (p1iter = 0; p1iter < p.p1_count; p1iter++)
                        {
                            p1_p2_row[p.p1[p1iter]] = state_table[row_index, p.p1[p1iter]];
                        }
                    }

                    int limit = (int)Math.Pow(2, p1_p2_row.Length);
                    for (int p1p2_loop_index = 0;
                         p1p2_loop_index < limit; 
                         p1p2_loop_index++)
                    {

                        if (cause_s[p1p2_loop_index])
                        {
                            //Should p1p2_loop_index be p1p2_count??
                            p.p1p2[p1p2_loop_index] = true;
                        }                 
                    }

                    double info_cause = (double)p.p1p2_count / (double)p.p1p2.Length;
                    for (int i=0; i < p.p1p2.Length; i++)
                    {
                        if (cause_s[i])
                        {
                            cause_prob_dist[i] = 1.0 / (double)p.p1p2.Length;
                        }
                    }                 
                }
            }
        }

        //public void find_effect_info(ref double[] effect_prob_dist)
        //{

        //}

        //public void find_phi_ce(ref double[] cause_prob_dist, ref double[] effect_prob_dist, ref phi)
        //{
        //    //hamming distance

        //}

    }

    

}