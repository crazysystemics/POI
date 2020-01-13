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
        public int minAttndnce;
        public int ATTEND_MIN, ATTEND_MAX;

        public AttendGen()
        {

        }

        public AttendGen(string infilename, string outfilename,
                         int tc, int atmin, int atmax, int minatn, int maxatn)
        {
            sr = new StreamReader(infilename);
            sw = new StreamWriter(outfilename);

            totalCols = tc;
            ATTEND_MIN = atmin;
            ATTEND_MAX = atmax;

            minAttndnce = minatn;
            maxAttn = maxatn;
        }

        public void generate()
        {
            initHeader(sr.ReadLine());

            string s = "";
            while((s = sr.ReadLine()) != null)
            {
                string outs = this.Calibrate(s);
                sw.WriteLine(outs);                
            }

            sr.Close();
            sw.Close();
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

            //TODO: make a generic function getpos(string colname)
            int i, postotal;
            for (i = 0; i < sarr.Length; i++)
            {
                if (colNames[i] == "Total")
                {
                    break;
                }
            }
            postotal = i;

            int poscondon;
            for (i = 0; i < sarr.Length; i++)
            {
                if (colNames[i] == "Condonation")
                {
                    break;
                }
            }
            poscondon = i;




            int totalAttendance = Convert.ToInt32(sarr[i]); //TODO:PARK IT BASED ON COLUMN NAME
            if ((double)totalAttendance / (double)maxAttn > .86)
                return ins;

            int Absnts85 = Convert.ToInt32((double) maxAttn * .14);
            int actAbsnts = maxAttn - totalAttendance;
            double  absentProb = ((double)Absnts85 /(double) actAbsnts) -  0.01; 
                                     //boundary - 1% bias towards not marking student present
                                     // Want to push beyond 85% in one go :-)                              
                                     

            if (sarr.Length > totalCols)
                 throw new Exception("more than max col");

            int index = 0;
            int attndnce = 0;
            Random rn = new Random();
            int endval = (int)((double)maxAttn * 0.86) + (rn.Next()) % 5;

            Random r = new Random();
            foreach (string s in sarr)
            {
                if (index >= ATTEND_MIN && index <= ATTEND_MAX)
                {
                    if ((postotal - 1) - index + 1 > (endval - attndnce + 1))
                    {
                        //you can still leave some A's toss and flip            
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
                    else if  ((postotal - 1) - index + 1 > (endval - attndnce + 1))
                    {
                        //we are at critical point. no more randomization
                        //flip every A                                   
                        if (s == "A")  
                        {
                            attndnce++;
                            outs += attndnce.ToString() + ",";
                        }
                    }
                    else
                    {
                        throw new Exception("not enough spage " + index.ToString());
                    }
                }
                index++;
            }

            outs += sarr[poscondon] + ",";
            int total = attndnce + Convert.ToInt32(sarr[poscondon]);
            double percent = (double)total / (double)maxAttn;
            if (percent < 0.85)
                throw new Exception("less than 85");
            outs += total.ToString() + "," + percent.ToString();


            return outs;

        }

        public string CalibrateReverse(string ins)
        {
            string outs = String.Empty;
            string[] sarr = ins.Split(',');

            //TODO: make a generic function getpos(string colname)
            int i, postotal;
            for (i = 0; i < sarr.Length; i++)
            {
                if (colNames[i] == "Total")
                {
                    break;
                }
            }
            postotal = i;

            int poscondon;
            for (i = 0; i < sarr.Length; i++)
            {
                if (colNames[i] == "Condonation")
                {
                    break;
                }
            }
            poscondon = i;

            int totalAttendance = Convert.ToInt32(sarr[i]); //TODO:PARK IT BASED ON COLUMN NAME
            if ((double)totalAttendance / (double)maxAttn > .86)
                return ins;

            Random rn = new Random();
            int Absnts85 = Convert.ToInt32((double)maxAttn * .14);
            int actAbsnts = maxAttn - totalAttendance;
            int reqflips = actAbsnts - Absnts85;
            double absentProb = ((double)Absnts85 / (double)actAbsnts) - 0.01;
            int startval = (int)((double)maxAttn * 0.86) + (rn.Next()) % 5;
            //boundary - 1% bias towards not marking student present
            // Want to push beyond 85% in one go :-)  
            Random r = new Random();

            for (i = ATTEND_MAX; i > ATTEND_MIN; i--)
            {
                if (reqflips <= 0)
                    break;                          
                if (sarr[i] == "A" && r.Next()> 0.86)
                {
                    
                }
            }
        

        }
    }   
}
