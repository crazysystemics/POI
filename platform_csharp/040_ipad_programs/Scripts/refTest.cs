using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//OO_OSI
namespace OutOfScope
{

    class Receiver
    {
        public Queue<string> queue = new Queue<string>();

        public  Queue<string> getQ()
        { return queue; }

        public string getsQ()
        {
            string s = queue.Dequeue();
            return s;
        }

    }
    class Sender
    {
        public Queue<string> toQ = new Queue<string>();   
        public void Connect(Queue<string> rqs)
        {
            toQ = rqs; 
            //this assignment is reference
        }

        public void putQ(string s)
        {
            toQ.Enqueue(s);
        }      
    }
}
