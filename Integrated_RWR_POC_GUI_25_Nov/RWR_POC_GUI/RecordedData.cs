using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWR_POC_GUI
{
    public class RecordedData
    {
        public int tick;
        public int trackID = -1;
        public int ageIn;
        public int ageOut;
        public string visibility;
        public TrackState action;
        public string state;
        public int agingInCount;
        public int agingOutCount;

        public RecordedData(int tick, int trackID, int ageIn, int ageOut, string visibility, TrackState action, string state, int agingInCount, int agingOutCount)
        {
            this.tick = tick;
            this.trackID = trackID;
            this.ageIn = ageIn;
            this.ageOut = ageOut;
            this.visibility = visibility;
            this.action = action;
            this.state = state;
            this.agingInCount = agingInCount;
            this.agingOutCount = agingOutCount;
        }
    }
}
