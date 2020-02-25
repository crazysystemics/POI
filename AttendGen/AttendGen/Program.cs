using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendGen
{
    class Program
    {
        static public string infn_work_pc  =
                    @"C:\One_Drive_1.2\OneDrive\05_Profession\03_Job_Consultancy_Enterprise\02_AIT\01c_ACA_15CS72\01b_70_ACA_common\7ainput.csv";

        static public string infn_home_pc =
            @"C:\Users\rvjos\OneDrive_August_2019\OneDrive\05_Profession\03_Job_Consultancy_Enterprise\02_AIT\01b_70_ACA_common\7ainput.csv";

        static string outfn = "outattend.csv";
        

        public static AttendGen attgen = new AttendGen(infn_work_pc, outfn, 67, 6, 63, 4, 57,6);        

        static void Main(string[] args)
        {
            attgen.generate();        
        }
    }
}
