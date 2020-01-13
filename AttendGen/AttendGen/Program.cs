using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendGen
{
    class Program
    {
        static public string infn  = 
            @"C:\One_Drive_1.2\OneDrive\05_Profession\03_Job_Consultancy_Enterprise\02_AIT\01b_70_ACA_common\7ainput.csv";

        static string outfn = "outattend.csv";
        

        public static AttendGen attgen = new AttendGen(infn, outfn, 67, 6, 64, 4, 57);        

        static void Main(string[] args)
        {
            attgen.generate();        
        }
    }
}
