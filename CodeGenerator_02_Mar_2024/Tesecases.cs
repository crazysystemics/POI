using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using CodeGenerator;

namespace CodeGenerator
{
    //enum Token (ID, SCENARIO, TSTEP, LPAREN, LB, PARALLEL_INPUT, INT_CONST, COMMA, RPAREN, PARALLEL OUTPUT, RB, NULL, SKIP }

    class TestHarness
    {

        //variables
        //
        public string Name = "Tree Creation Test";
        public List<TerminalToken> terminal_list = new List<TerminalToken>();
        public List<Token> tokens = new List<Token>();
        public static StreamReader input = new StreamReader("Scenario.txt");
        public static StreamReader input2 = new StreamReader("Scenario.txt");

        int i = 0;
        bool end = false;

        public string IdentifierReader(char ch)
        {
            string id = "";
            while (Regex.IsMatch(ch.ToString(), "[a-zA-Z0-9_]"))
            {
                id += ch;
                ch = Convert.ToChar(Globals.input1.Peek());
                if (Regex.IsMatch(ch.ToString(), "[a-zA-Z0-9_]"))
                {
                    ch = Convert.ToChar(Globals.input1.Read());
                }
            }
            return id;
        }

        public string NumberReader(char ch)
        {
            string num = "";
            while (Regex.IsMatch(ch.ToString(), "[-0-9]"))
            {
                num += ch;

                ch = Convert.ToChar(Globals.input1.Peek());
                if (Regex.IsMatch(ch.ToString(), "[-0-9]"))
                {
                    ch = Convert.ToChar(Globals.input1.Read());
                }
            }
            return num;
        }

        public string GetString(string str, int count)
        {
            for (int i = 0; i < count; i++)
            {
                char c = Convert.ToChar(Globals.input1.Read());
                if (c == ' ')
                    break;
                if (Globals.pushBackQ.Count != 0)
                    str += Globals.pushBackQ.Dequeue();
                else
                    str += c;
                if (str == "PARALLEL_INPUTS")
                {
                    return str;
                }
            }
            return str;
        }
        public void AddToQueue(string str, int count)
        {
            for (int i = 1; i < count; i++)
            {
                Globals.pushBackQ.Enqueue(str[i]);
            }
        }

        public void DriveTest_01()
        {
            Tree AST = new Tree();

            Debug.Assert(AST != null);
            Debug.Assert(AST.ScenarioNode == null);

            AST.BuildSyntaxTree(AST);
            Debug.Assert(AST.no_of_nodes == 151);
        }

        public void DriverTest_02()
        {
            //Build Concrete Tree 
            //By fitting .r Tokens into Abstract Syntax Tree 
            Tree AST = new Tree();
            AST.BuildSyntaxTree(AST);
            AST.fit_tokens();

            //foreach (Token token in tokens)
            while (AST.end != true)
            {
                Token token = AST.GetToken(input);
                if (token != Token.SKIP)
                {
                    Debug.Assert(AST.leafs[i] == token);
                    i++;
                }
            }
        }

        public void DriverTest_03()
        {
            Tree AST = new Tree();

            //Build Abstract Syntax Tree 
            AST.BuildSyntaxTree(AST);

            //Build Concrete Syntax Tree
            //By fitting.r Tokens into Abstract Syntax Tree
            AST.fit_tokens();

            //DataGenerate
            //List<string> GeneratedDataList = new List<string>();

            //GeneratedDatalist AST. GenerateData(AST.ScenarioNode); 
            AST.GenerateData(AST.ScenarioNode);

            foreach (string row in AST.retStrList)
                Console.WriteLine(row);
        }

        public void DriverTest_04()
        {
            Tree AST = new Tree();
            data_record[] data_table = { };
            data datawrite = new data(5, 100, 5);

            //Build Abstract Syntax Tree 
            AST.BuildSyntaxTree(AST);

            //Build Concrete Syntax Tree
            //By fitting .r Tokens into Abstract Syntax Tree
            AST.fit_tokens();

            //List<string> GeneratedDataList = new List<string>();
            //DataGenerate

            //GeneratedDataList = AST.GenerateData(AST.ScenarioNode); 
            AST.GenerateData(AST.ScenarioNode);
            AST.InputDataList();
            datawrite.GenerateDataTable(AST);
            datawrite.Write_to_csv(AST);
            datawrite.Write_to_bin(AST);
            // datawrite.Read_from_bin();
            foreach (string row in AST.retStrList)
                Console.WriteLine(row);
        }
    }
}
