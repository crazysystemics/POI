using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    class range
    {
        public int min, max, step;
        public range(int min, int max, int step)
        {
            this.min = min;
            this.max = max;
            this.step = step;
        }

        public range() { }
    }
}
