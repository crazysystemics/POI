using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWR_POC_GUI
{
    public class RecordedData
    {
        public int eid;
        public int tick;
        public int trackID = -1;
        public int ageIn;
        public int ageOut;
        public string visibility;
        public TrackState action;
        public string state;
        public int entry_tick;
        public int exit_tick;
        public int track_length;
        public int agingInCount;
        public int agingOutCount;

        public RecordedData(int tick, int eid,int trackID, int ageIn, int ageOut, string visibility, TrackState action, string state, int entry_tick,int exit_tick,int agingInCount, int agingOutCount)
        {
            this.tick = tick;
            this.trackID = trackID;
            this.ageIn = ageIn;
            this.ageOut = ageOut;
            this.visibility = visibility;
            this.action = action;
            this.state = state;
            this.entry_tick = entry_tick;
            this.exit_tick = exit_tick;
            this.track_length = this.exit_tick - this.entry_tick;
            this.agingInCount = agingInCount;
            this.agingOutCount = agingOutCount;
        }
    }
}
