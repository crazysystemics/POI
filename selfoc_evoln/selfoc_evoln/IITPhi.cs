using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfoc_evoln
{
    static class sglobal
    {
        public static Random r;
        public static StreamWriter sw;
    }

    class IIT_Phi
    {
        string[] BlueForce;
        bool[] BlueDistance;
        double[] BlueScore;
        string[] BlueCluster;
        string[] RedForce;
        public int num_elems;
        public int cluster_size;
        bool[] RedXmitStatus = { false, false, false };

        //double[] RedScore = new double[num_elems];
        //simple right-rotate is considered. random permutation also can be considered
        public static string[,] Permutations = new string[9, 3];
        public int nbase;
        public int digits;
        public Mechanism[] Mechanisms;


        public UAS_SOC_IIT_Engine(int num_elems, int nbase = 3, int ndigits = 3)
        {
            Debug.Assert(num_elems % 3 == 0);
            this.num_elems = num_elems;
        }

        public void init()
        {

            BlueForce = new string[num_elems];
            BlueDistance = new bool[num_elems];
            BlueScore = new double[num_elems];
            RedForce = new string[num_elems];
            Mechanisms = new Mechanism[num_elems];

            for (int i = 0; i < num_elems; i++)
            {

                Random r = new Random();
                int rn = r.Next(3);
                if (rn == 0) BlueForce[i] = "A";
                else if (rn == 1) BlueForce[i] = "B";
                else if (rn == 2) BlueForce[i] = "C";

                rn = r.Next(3);
                if (rn == 0) RedForce[i] = "A";
                else if (rn == 1) RedForce[i] = "B";
                else if (rn == 2) RedForce[i] = "C";
            }

            //Calculate initial distance vector
            for (int pos = 1; pos < num_elems - 1; pos++)
            {
                Triplet<string> bluecode = new Triplet<string>();
                bluecode.Set(BlueForce[pos - 1], BlueForce[pos], BlueForce[pos + 1]);

                Triplet<string> redcode = new Triplet<string>();
                redcode.Set(RedForce[pos - 1], RedForce[pos], RedForce[pos + 1]);

                Triplet<bool> distance = new Triplet<bool>();
                distance.Set(BlueForce[pos - 1] == RedForce[pos - 1],
                             BlueForce[pos] == RedForce[pos],
                             BlueForce[pos + 1] == RedForce[pos + 1]);

                BlueDistance[pos - 1] = distance.left;
                BlueDistance[pos] = distance.center;
                BlueDistance[pos + 1] = distance.right;

                //Clustering Logic
                if (BlueScore[pos] >= BlueScore[pos - 1] &&
                    BlueScore[pos] >= BlueScore[pos + 1]
                   )
                {

                    if (pos - 2 >= 0 && BlueScore[pos] >= BlueScore[pos - 2] &&
                        pos + 2 < num_elems && BlueScore[pos] >= BlueScore[pos + 2])
                    {
                        //BlueScore[pos] can become center of cluster
                        BlueCluster[pos] = "0";
                        BlueCluster[pos - 1] = "1";
                        BlueCluster[pos + 1] = "-1";
                    }
                    else
                    {
                        BlueCluster[pos] = "*";
                    }
                }
                else
                {
                    BlueCluster[pos] = "*";
                }


            }
        }

        public void next()
        {
            //process every element but first and last in BlueForce
            //The state at this state is t0 or tcur
            //MECHANISM is XOR for RED AND BLUE
            //BUT WHICH CLUSTER

            int mincepos = 0;
            int mutation_begin = -4;
            int mutation_end = 4;

            Triplet<bool?> elemstate = new Triplet<bool?>();

            double mincphi = 0.0;

            //IIT-Phase. find least fit (phi) position
            for (int pos = 1; pos < num_elems - 1; pos++)
            {
                uint?[] cur_locked = Mechanisms[pos].rx_xmit_status;
                double cphi = 0.0;
                double ephi = 0.0;
                Purview core_cause = new Purview();
                Purview core_effect = new Purview();


                //current state of tracking with RED UAS
                Mechanism m = new Mechanism(cur_locked[pos - 1], cur_locked[pos], cur_locked[pos + 1]);
                elemstate.left = sglobal.ui2bool(cur_locked[pos - 1]);
                elemstate.center = sglobal.ui2bool(cur_locked[pos]);
                elemstate.right = sglobal.ui2bool(cur_locked[pos + 1]);

                m.GetPhiMax(elemstate, ref cphi, ref ephi, ref core_cause, ref core_effect);
                double phi = cphi < ephi ? cphi : ephi;

                if (phi < mincphi)
                {
                    mincphi = cphi;
                    mincepos = pos;
                }
            }


            //SOC-Phase. Mutate or Hill Climb based on mincepos
            for (int pos = 1; pos < num_elems - 1; pos++)
            {
                mutation_begin = mincepos - 4;
                mutation_end = mincepos + 4;
                if (pos >= mutation_begin && pos <= mutation_end)
                {
                    //phi as kalman gain => futuristic
                    //mutation

                    string[] sigs = { "ABC", "BCA", "CAB", "BAC", "ACB", "CBA" };

                    Random rfreq = new Random(5);
                    string rsig = sigs[rfreq.Next(5)];
                    Mechanisms[pos].signatures[0] = rsig[0].ToString();
                    Mechanisms[pos].signatures[1] = rsig[1].ToString();
                    Mechanisms[pos].signatures[2] = rsig[2].ToString();
                }
                else
                {
                    //hill_climbing phase
                    //Mechanism cur_mech = new Mechanism(distance.left, distance.center, distance.right);
                    //Multiple Reportoires possible only in case of multiple distributions
                    //Compute Probability Distribution of Causes
                    Mechanisms[pos].rx_xmit_status = Mechanisms[pos].Kernel(Mechanisms[pos].rx_xmit_status);
                }


                //TODO: promote cluster_len as member variable
                int cluster_len = 3;
                for (int sindex = 0; sindex < cluster_len; sindex++)
                {
                    //Mechanisms[pos].locked[sindex] 
                    //Blue UAS captures Red Signatures
                    Mechanisms[pos].rx_xmit_status[sindex] = RedSignatures[sindex];
                }

                //Using OAX machine, and presereving current ones what would be core effect?
                //uint? Diff0cur = (Convert.ToInt32(Mechanisms[pos].locked) == 1) ? (uint?)1 : (uint?)null;
                //uint? Diff1cur = (Convert.ToInt32(Mechanisms[pos].locked) == 1) ? (uint?)1 : (uint?)null;
                //uint? Diff2cur = (Convert.ToInt32(Mechanisms[pos].locked) == 1) ? (uint?)1 : (uint?)null;

                //Find DIFF (with true as is and false as null)
                //Mechanisms[pos].locked[0] = Diff0cur;
                //Mechanisms[pos].locked[1] = Diff1cur;
                //Mechanisms[pos].locked[2] = Diff2cur;
            }

        }
    }
}
