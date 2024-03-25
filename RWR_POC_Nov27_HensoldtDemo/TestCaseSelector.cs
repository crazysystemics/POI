using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWR_POC_GUI
{
    public class TestCaseSelector
    {
        public int testCaseID = 0;
        public List<bool> detectionPattern = new List<bool>();

        public TestCaseSelector(int id, List<bool> patterns)
        {
            this.testCaseID = id;
            this.detectionPattern = patterns;
        }
    }
}
