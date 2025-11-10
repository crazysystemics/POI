using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG_UC
{
    public class EmitterTrackRecord
    {
        public int Tick { get; set; }     // To check with sir
        public bool received { get; set; }
        public int eid { get; set; }
        public int erID { get; set; }
        public int trackID { get; set; }

        public double priCurrent { get; set; }
        public double priMin { get; set; }
        public double priMax { get; set; }
        public double priTrackWindow { get; set; }

        public double freqCurrent { get; set; }
        public double freqMin { get; set; }
        public double freqMax { get; set; }
        public double freqTrackWindow { get; set; }

        public double pwCurrent { get; set; }
        public double pwMin { get; set; }
        public double pwMax { get; set; }
        public double pwTrackWindow { get; set; }

        public double azimuthTrackWindow { get; set; }
        public int ageIn { get; set; }
        public double ageOut { get; set; }

        public EmitterTrackRecord()
        {
            eid = -1;
            erID = -1;
        }
    }
}
