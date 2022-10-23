using System;
using System.Collections.Generic;
using System.Numerics;

namespace CycleCount
{
    class CycleCount
    {
        // C# program to calculate cycles of 
        // length n in a given graph 
        
             
        // Number of vertices 
        public  int V = 5;
        public int count = 0;
        List<int> path = new List<int>();

        public BigInteger factorial(BigInteger n)
        {
            if (n == 0 || n == 1)
                return 1;
            else
                return n * factorial(n - 1);
        }

        public BigInteger npr(int n, int r)
        {
            BigInteger bn, br;
            bn = n;
            br = r;
            return (factorial(bn) / factorial(bn - br));
        }

        public BigInteger ncr(int n, int r)
        {
            BigInteger bn, br;
            bn = n;
            br = r;
            return (factorial(bn) / (factorial(br) * factorial(bn - br)));
        }


        
        public BigInteger  numCycles(int order, int cycleLen, bool iscomplete)
        {
            //for cycles of cycleLen, number of vertices have to be
            //cycleLen. Find out how many cycles of cycleLen vertices 
            //can be there [n* C * cycleLen]

            //This is applicable only if the graph is complete
            if (iscomplete)
            {
               BigInteger b = npr(order, cycleLen);// * factorial(cycleLen - 1) / 2;
               return b;
            }
            else
            {
                return 0;
            }
        }

        public CycleCount(int v)
        {
            V     = v;
            count = 0;
        }



        public void DFS(int[,] graph, bool[] marked,
                        int n, int vert, int start)
        {

            // mark the vertex vert as visited 
            marked[vert] = true;

            // if the path of length (n-1) is found 
            if (n == 0)
            {

                // mark vert as un-visited to  
                // make it usable again 
                marked[vert] = false;

                // Check if vertex vert end  
                // with vertex start 
                if (graph[vert, start] == 1)
                {
                    count++;
                    return;
                }
                else
                    return;
            }

            // For searching every possible  
            // path of length (n-1) 
            for (int i = 0; i < V; i++)
                if (!marked[i] && graph[vert, i] == 1)
                    // DFS for searching path by 
                    // decreasing length by 1 
                    DFS(graph, marked, n - 1, i, start);

            // marking vert as unvisited to make it 
            // usable again 
            marked[vert] = false;


        }

        // Count cycles of length N in an  
        // undirected and connected graph. 
        public int countCycles(int[,] graph, int n)
        {
            // all vertex are marked un-visited 
            // initially. 
            bool[] marked = new bool[V];
            count = 0;

            // Searching for cycle by using  
            // v-n+1 vertices
            
            for (int i = 0; i < V - (n - 1); i++)
            {
                //path = new List<int>();
                DFS(graph, marked, n - 1, i, i);

                //Nullable<int> prev = null;
                //foreach (int p in path)
                //{
                //    if (prev == null)
                //    {
                //        prev = p;
                //    }
                //    else
                //    {
                //        graph[prev.Value, p] = -1;
                //        prev = p;
                //    }
                //}

               //if (prev != null)
               // graph[prev.Value, i] = -1;
                                          
               // ith vertex is marked as visited 
               // and will not be visited again 
               marked[i] = true;
            }

            return count/2;
        }

        // Driver code 
        //public static void Main()
        //{
        //    int[,] graph = {{0, 1, 0, 1, 0},
        //                {1, 0, 1, 0, 1},
        //                {0, 1, 0, 1, 0},
        //                {1, 0, 1, 0, 1},
        //                {0, 1, 0, 1, 0}};

        //    int n = 4;

        //    Console.WriteLine("Total cycles of length " +
        //                    n + " are " +
        //                    countCycles(graph, n));
        //}
    }





}


    