using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    class Globals
    {
        public static int paramCount = 1;
        public static Queue<char> pushBackQ = new Queue<char>();
        public static List<object> tokens = new List<object>();
        public static StreamReader input1 = new StreamReader("Scenario.txt");
        public static StreamReader input2 = new StreamReader("Scenario.txt");
        public static StreamWriter writer = new StreamWriter("play.csv");
        public static FileStream fs = new FileStream("play.bin", FileMode.Create);
        public static BinaryWriter binwriter = new BinaryWriter(fs);

        public static void AddTokenToList(object token)
        {
            if (token != null)
            {
                if (token.ToString() != Token.SKIP.ToString())
                    Globals.tokens.Add(token);
            }
        }
    }
}