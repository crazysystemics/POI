using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AttendGen
{
    class AttendGen
    {
        public StreamReader sr;
        public StreamWriter sw;

        public int totalCols;
        public string[] colNames;
        public int maxAttn;
        public int ATTEND_MIN, ATTEND_MAX;

        public AttendGen(string infilename, string outfilename,
                         int tc, int atmin, int atmax)
        {
            sr = new StreamReader(infilename);
            sw = new StreamWriter(outfilename);

            totalCols = tc;
            ATTEND_MIN = atmin;
            ATTEND_MAX = atmax;
        }

        public void initHeader(string sheader)
        {
            colNames = sheader.Split(',');
        }

        public string getNextRow()
        {
            return sr.ReadLine();
        }

        public void putCalibedRow(string s)
        {
            sw.WriteLine(s);
        }

        public string Calibrate(string ins)
        {
            string outs = String.Empty;                     
            string[] sarr = ins.Split(',');
            

            int totalAttendance = Convert.ToInt32(sarr[61]); //TODO:PARK IT BASED ON COLUMN NAME
            if ((double)totalAttendance / (double)maxAttn > .86)
                return ins;

            int Absnts85 = Convert.ToInt32((double) maxAttn * .14);
            int actAbsnts = maxAttn - totalAttendance;
            double  absentProb = (Absnts85 / actAbsnts) -  0.01; 
                                     //boundary - 1% bias towards not marking student present
                                     // Want to push beyond 85% in one go :-)                              
                                     

            if (sarr.Length > totalCols)
                 throw new Exception("more than max col");

            int index = 0;
            int attndnce = 0;
            Random r = new Random();
            foreach (string s in sarr)
            {

                if (index >=  ATTEND_MIN && index < ATTEND_MAX)
                {
                    if (s == "A" && r.Next() < absentProb)
                    {
                        outs += s + ",";
                    }
                    else
                    {
                        attndnce++;
                        outs += attndnce.ToString() + ",";
                    }
                        
                }
            }                  
            return null;
        }
    }   
}
