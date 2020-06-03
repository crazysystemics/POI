using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleCount
{
    class clique
    {
        int order;
        public int[,] am; //adjacency_matrix

        
        public clique(int order)
        {
            am = new int[order, order];
            this.order = order;
            for (int i = 0; i < order; i++)
                for (int j = 0; j < order; j++)
                    am[i, j] = 1;
        }

        public List<BigInteger> numCycles
        { 
            get
            {
                CycleCount cc = new CycleCount(order);
                List<BigInteger> cclist = new List<BigInteger>();
                for (int len = 1; len <= order; len++)
                {
                    //cclist.Add(cc.countCycles(am, len));
                      cclist.Add(cc.numCycles( order, len,true));
                }
                return cclist;
            }
        }


    }
    class Program
    {
        
        static void Main(string[] args)
        {
            clique cn = new clique(6);
            List<BigInteger> numcycles = cn.numCycles;
            int index = 1;
            foreach(BigInteger nc in numcycles)
            {
                Console.WriteLine("There are {0} cycles of Length {1}", nc, index);
                index++;
            }

        }
    }
}
