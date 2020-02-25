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
        public int maxAttendance;
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
            maxAttendance = maxatn;
        }

        public void generate()
        {
            string s = sr.ReadLine();
            initHeader(s);
            sw.WriteLine(s);
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

            //if first field is not integer, return as it is
            if (!int.TryParse(sarr[0], out _))
                return ins;

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





            int totalAttendance = Convert.ToInt32(sarr[postotal]) +
                               Convert.ToInt32(sarr[poscondon]) ; //TODO:PARK IT BASED ON COLUMN NAME
            
            if ((double)totalAttendance / (double)maxAttendance > .86)
                return ins;

            int Attendance85 = Convert.ToInt32((double)maxAttendance * .86);
            int gapAttendance = Attendance85  - Convert.ToInt32(sarr[postotal]) + 2;
            double flipProb = 1.0;// ((double)gapAttendance / (double)maxAttendance) - 0.01;
            //boundary - 1% bias towards not marking student present
            // Want to push beyond 85% in one go :-)                              


            if (sarr.Length > totalCols)
                throw new Exception("more than max col");

            int index = 0;
            int presents = 0;
            Random rn = new Random(13);
            int endval86 = (int)((double)maxAttendance * 0.86) + rn.Next(-1, 3);
            int tempTotal;
            if (int.TryParse(sarr[postotal], out tempTotal))
            {
                if (tempTotal < 40)
                {
                    MAXIMUM_UNCALIB_ATTN = 0;
                }
            }

            Random r = new Random(18);

            //for (index = POS_ATTEND_MIN; index < POS_ATTEND_MAX / 2; index++)
            //{
            //    int rem_a;

            //    if (sarr[index] != "A")
            //    {
            //        //already present, increase attndnce
            //        presents++;

            //    }
            //    else if ((POS_ATTEND_MAX - index + 1) - (endval86 - presents + 1) > 3)
            //    {
            //        //you can still leave some A's toss and flip 
            //        //leave first 6 attendance untouched
            //        double rn0 = r.NextDouble();
            //        if ((index - POS_ATTEND_MIN > MAXIMUM_UNCALIB_ATTN) && sarr[index] == "A" &&
            //             rn0 < (flipProb))
            //        {
            //            presents++;
            //            sarr[index] = presents.ToString();
            //        }

            //        if (sarr[POS_ATTEND_MAX - index] == "A" &&
            //            r.Next(100) < (flipProb * 100))
            //        {
            //            //leaving attndnce incrementing for the count when 
            //            //processes in forward direction
            //            sarr[POS_ATTEND_MAX - index] = "P";
            //        }
            //    }
            //    else if ((rem_a = (POS_ATTEND_MAX - index + 1) - (endval86 - presents + 1)) > 0 && rem_a <= 3)
            //    {
            //        //we are at critical point. no more randomization
            //        //flip every A  
            //        presents++;
            //        sarr[index] = (presents).ToString();
            //    }
            //    else
            //    {
            //        throw new Exception("not enough space " + index.ToString());
            //    }
            //}

            //A's are being over written, but attendance is not tracked properly
            //As a quick-fix increment A's over all non As
            presents = 0;
            int condonation = 0;
            int.TryParse(sarr[poscondon], out condonation);
            //int absents = 0;
            int left_index, right_index;
            left_index = right_index = (POS_ATTEND_MIN + POS_ATTEND_MAX) / 2;
            while (gapAttendance > 0 )
            {
                if (left_index < POS_ATTEND_MIN && right_index > POS_ATTEND_MAX)
                {
                    throw new Exception("hit the wall");
                }

                if (left_index >= POS_ATTEND_MIN)
                {
                    if (sarr[left_index] == "A")
                    {
                        gapAttendance--;
                        sarr[left_index] = "P";
                    }
                    left_index--;
                }

                if (right_index <= POS_ATTEND_MAX)
                {
                    if (sarr[left_index] == "A")
                    {
                        gapAttendance--;
                        sarr[right_index] = "P";                        
                    }

                    right_index++;
                }
            }

            for (presents=0, index = 0; index < POS_ATTEND_MAX; index++)
            {
                if (sarr[index] != "A")
                {
                    presents++;
                    sarr[index] = presents.ToString();
                }
            }



            //if (sarr[index] == "A" && presents + condonation < Attendance85)
            //{
            //    presents++;
            //    sarr[index] = "P";
            //}
            //else if (sarr[index] != "A")
            //{
            //    presents++;
            //    sarr[index] = "P";
            //}

            //if (sarr[POS_ATTEND_MAX - index] == "A" &&
            //     (presents + condonation) < Attendance85)
            //{

            //    presents++;
            //    sarr[index] = "P";
            //}
            //else if (sarr[index] != "A")
            //{
            //    presents++;
            //    sarr[index] = "P";
            //}

            //else if (sarr[index] != "A" && presents + condonation )// || ((POS_ATTEND_MAX - index) - (Attendance85 -attndnce) < 0))
            //{
            //    absents++;
            //    sarr[index] = "A";
            //}

            int total = presents + Convert.ToInt32(condonation);
            double percent = (double)total / (double)maxAttendance;
            if (percent < 0.85)
                sarr[0] = "exception";

            sarr[poscondon] = condonation.ToString();
            sarr[postotal] = total.ToString();
            sarr[postotal + 1] = percent.ToString();

            outs = GetCsvString(sarr);
            outs += total.ToString() + "," + percent.ToString();
            return outs;
        }
    }
}

