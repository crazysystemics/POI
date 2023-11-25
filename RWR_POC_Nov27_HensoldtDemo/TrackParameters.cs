using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    //public enum ACTION
    //{
    //    ALL, INSERT, UPDATE, IDELETE, ODELETE
    //}

    public class TrackParameters
    {
        public int start_tick = 0;
        public int end_tick;
        public List<int> tracks = new List<int>();
        public TrackState action = TrackState.ALL;
        public int id = 0;
        public List<int> trackLengths = new List<int>();
    }
