using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace constraint_puzzle_solver
{
    public class ExpressionBase
    {
       

        public ExpressionBase()
        {
            
        }
    }    

    public class Expression<T>:ExpressionBase
    {
        string parameter;
        public List<T> ValueList { get; set; }

        public Expression(string parameter,
                                List<T> ValueList)
        {
            this.parameter = parameter;
            this.ValueList = ValueList;            
        }
    }

    public class ExpressionContainer
    {
        public List<ExpressionBase> ExpressionList { get; set; }

        public ExpressionContainer()
        {
            ExpressionList = new List<ExpressionBase>();
        }
    }

    public class Program1
    {
        public  void Main1()
        {
            // Example usage
            List<int> roomlist = new List<int>();
            roomlist.Add(1);
            roomlist.Add(2);
            roomlist.Add(3);
            roomlist.Add(4);
            string[] sa = new string[] { "hello", "world" };
            Expression<int> e1 = new Expression<int>("room1", roomlist);
            Expression<int> e2 = new Expression<int>("room2", roomlist);
            Expression<string> e3s = new Expression<string>
                                ("room3",
                                sa.ToList());

            ExpressionContainer ec = new ExpressionContainer();
            ec.ExpressionList.Add(e1);
            ec.ExpressionList.Add(e2);
            ec.ExpressionList.Add(e3s);

            //Console.WriteLine("Expression list count: " + intExpressionContainer.ExpressionList.Count);
        }
    }


    
}
