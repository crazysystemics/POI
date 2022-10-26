using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uas_soc_iit_dt
{
    class UAS
    {
        string[] BlueForce;
        bool  [] BlueDistance;
        double[] BlueScore;
        string[] BlueCluster;
        string[] RedForce;

        public int cluster_size;
        bool[] RedXmitStatus = { false, false, false };
    }
}
