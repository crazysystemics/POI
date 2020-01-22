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
        public int POS_ATTEND_MIN, POS_ATTEND_MAX;
        public int MAXIMUM_UNCALIB_ATTN;

        public AttendGen()
        {

        }

        public string GetCsvString(string[] sarr)
        {
            string outs = "";

            foreach (string s in sarr)
                outs += s + ",";

            return outs.TrimEnd(',');

        }

        public AttendGen(string infilename, string outfilename,
                         int tc, int atmin, int atmax, int minatn, int maxatn, int minuncalibattn)
        {
            sr = new StreamReader(infilename);
            sw = new StreamWriter(outfilename);

            totalCols = tc;
            POS_ATTEND_MIN = atmin;
            POS_ATTEND_MAX = atmax;
            MAXIMUM_UNCALIB_ATTN = minuncalibattn;

            minAttndnce = minatn;
            maxAttn = maxatn;
        }

        public void generate()
        {
            initHeader(sr.ReadLine());

            string s = "";
            while ((s = sr.ReadLine()) != null)
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




            int totalAttendance = Convert.ToInt32(sarr[postotal]); //TODO:PARK IT BASED ON COLUMN NAME
            if ((double)totalAttendance / (double)maxAttn > .86)
                return ins;

            int Absnts85 = Convert.ToInt32((double)maxAttn * .14);
            int actAbsnts = maxAttn - totalAttendance;
            double flipProb = ((double)Absnts85 / (double)actAbsnts) - 0.01;
            //boundary - 1% bias towards not marking student present
            // Want to push beyond 85% in one go :-)                              


            if (sarr.Length > totalCols)
                throw new Exception("more than max col");

            int index = 0;
            int attndnce = 0;
            Random rn = new Random(13);
            int endval86 = (int)((double)maxAttn * 0.86) + (rn.Next()) % 5;

            Random r = new Random(18);
            for (index = POS_ATTEND_MIN; index <= POS_ATTEND_MAX; index++)
            {
                if (index >= POS_ATTEND_MIN && index <= POS_ATTEND_MAX)
                {
                   
                    if ((POS_ATTEND_MAX - index + 1) > (endval86 - attndnce + 1))
                    {                             
                        //you can still leave some A's toss and flip 
                        //leave first 6 attendance untouched
                        if ((index - POS_ATTEND_MIN > MAXIMUM_UNCALIB_ATTN) && sarr[index] == "A" && r.Next() > flipProb)
                        {
                            attndnce++;
                            sarr[index] = attndnce.ToString();
                        }
                    }
                    else if ((POS_ATTEND_MAX - index + 1) < (endval86 - attndnce + 1))
                    {
                        //we are at critical point. no more randomization
                        //flip every A  
                        attndnce++;
                        sarr[index] = (attndnce).ToString();
                    }                    
                    else
                    {
                        throw new Exception("not enough space " + index.ToString());
                    }
                }

                index++;
            }



            outs = GetCsvString(sarr);
            int total = attndnce + Convert.ToInt32(sarr[poscondon]);
            double percent = (double)total / (double)maxAttn;
            if (percent < 0.85)
                throw new Exception("less than 85");
            outs += total.ToString() + "," + percent.ToString();

            return outs;
        }
    }
 
}
