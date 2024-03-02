using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using CodeGenerator;
using System.Collections;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Xml.Linq;

namespace CodeGenerator
{
    class Tree
    {
        public Dictionary<int, Symbol> SymbolTable = new Dictionary<int, Symbol>();
        public Node root;
        public int id_count = 0, leaf_count = 0;
        public static int node_count = 0;
        public int no_of_nodes;
        public static List<Node> leaf_nodes = new List<Node>();
        public List<Token> leafs = new List<Token>();
        public bool is_keyword = true;

        public List<Token> terminal_list = new List<Token>();

        public List<Node> TSteplist = new List<Node>();
        public Node pilist;
        public List<Node> polist = new List<Node>();
        public Node ScenarioNode;
        string id = "";
        public bool scenario_node_created = false;
        public bool all_tokens_read = false;
        public bool min_visited = false;
        //public List<string> retStrlist = new List<string>();
        public List<int> loops = new List<int>();
        public string data = string.Empty;
        public string[] retStrList;
        public int records;
        public List<int[]> Datalist = new List<int[]>();

        //Creation of Grammar
        public void createTerminalList()
        {
            terminal_list.Add(Token.LB);
            terminal_list.Add(Token.RB);
            terminal_list.Add(Token.SCENARIO);
            terminal_list.Add(Token.T_STEP);
            terminal_list.Add(Token.ID);
            terminal_list.Add(Token.INT_CONST);
            terminal_list.Add(Token.LPAREN);
            terminal_list.Add(Token.COMMA);
            terminal_list.Add(Token.RPAREN);
            terminal_list.Add(Token.PARALLEL_INPUT);
            terminal_list.Add(Token.PARALLEL_OUTPUT);
        }
        public List<Node> getTSTEPList()
        {
            //List<Node> TSteplist = new List<Node>(); 
            TSteplist.Add(getTStepNode());
            TSteplist.Add(getTStepNode());
            TSteplist.Add(getTStepNode());
            return TSteplist;
        }

        public Node getPiList()
        {
            pilist = new Node(Token.PI_LIST, 11);
            pilist.children = new List<Node>();
            pilist.children.Add(getPiNode());
            pilist.children.Add(getPiNode());
            //pilist.children.Add(getPiNode());

            return pilist;
        }

        public Node getTstepList()
        {
            Node tsteplist = new Node(Token.T_STEP_LIST, 16);
            tsteplist.children = new List<Node>();
            tsteplist.children.Add(getTStepNode());
            //tsteplist.children.Add(getTStepNode());
            //tsteplist.children.Add(getTStepNode());

            return tsteplist;
        }

        public Node getMaxNode()
        {
            Node maxnode = new Node(Token.MAX, 8);
            maxnode.children = new List<Node>();
            maxnode.children.Add(new Node(Token.INT_CONST, 9));
            return maxnode;
        }

        public Node getMinNode()
        {
            Node minnode = new Node(Token.MIN, 5);
            minnode.children = new List<Node>();
            minnode.children.Add(new Node(Token.INT_CONST, 6));
            return minnode;
        }
        public Node getStepNode()
        {
            Node stepnode = new Node(Token.STEP, 11);
            stepnode.children = new List<Node>();
            stepnode.children.Add(new Node(Token.INT_CONST, 12));
            return stepnode;
        }

        //public List<Node> getPoList()
        //{
        //polist = new List<Node>();
        //polist.Add(getPoNode());
        //polist.Add(getPoNode());
        //polist.Add(getPoNode());

        //return polist,
        //}
        public void BuildSyntaxTree(Tree tree)
        {
            tree.createTerminalList();
            tree.getNTScenarioNode();
        }

        public void getNTScenarioNode()
        {
            ScenarioNode = new Node(Token.SCENARIO, 15);
            ScenarioNode.children = new List<Node>();

            ScenarioNode.children.Add(new Node(Token.LB, 13));
            ScenarioNode.children.Add(getTstepList());
            ScenarioNode.children.Add(new Node(Token.RB, 14));

            root = ScenarioNode;
            no_of_nodes = node_count;
            scenario_node_created = true;
            foreach (Node lf in leaf_nodes)
            {
                leafs.Add(lf.item);
                leaf_count++;
            }
        }
        public Node getTStepNode()
        {
            Node TStepNode = new Node(Token.T_STEP, 12);
            TStepNode.children = new List<Node>();

            TStepNode.children.Add(new Node(Token.LB, 13));
            TStepNode.children.Add(getPiList());
            TStepNode.children.Add(new Node(Token.RB, 14));

            return TStepNode;
        }

        public Node getPiNode()
        {
            Node pinode = new Node(Token.PI, 0);

            pinode.children.Add(new Node(Token.PARALLEL_INPUT, 1));
            pinode.children.Add(new Node(Token.LB, 2));
            pinode.children.Add(new Node(Token.ID, 3));
            pinode.children.Add(new Node(Token.LB, 4));
            pinode.children.Add(getMinNode());
            pinode.children.Add(new Node(Token.COMMA, 7));
            pinode.children.Add(getMaxNode());
            pinode.children.Add(new Node(Token.COMMA, 10));
            pinode.children.Add(getStepNode());
            pinode.children.Add(new Node(Token.RB, 13));
            pinode.children.Add(new Node(Token.RB, 14));

            return pinode;
        }
        public Node getPoNode()
        {
            //Node ponode = new Node(Token.PO);

            //ponode.children.Add(new Node(Token.PARALLEL_OUTPUT)); 
            //ponode.children.Add(new Node (Token. ID));
            return null;
        }

        //public TerminalToken GetToken() { return null; }

        //Parsing
        public static List<string> identifiers = new List<string>();
        public static List<string> numbers = new List<string>();
        public static int lb = 0, rb = 0;
        public bool end = false;
        int c = 0;

        public void GetToken(Node leaf_node)
        {
            TerminalToken token = TerminalToken.NULL;
            bool token_fit = false;
            string str;
            char ch;

            while (end != true && token_fit == false)
            {
                ch = Convert.ToChar(Globals.input1.Read());
                switch (ch)
                {
                    case '{':
                        str = "{";
                        leaf_node.keyword = str;
                        lb++;
                        token_fit = true;
                        break;
                    case 'S':
                        str = ch.ToString();
                        str = GetString(str, 7);

                        if (str == "SCENARIO")
                            leaf_node.keyword = str;

                        else
                            AddToQueue(str, str.Length);
                        token_fit = true;
                        break;

                    case 'T':
                        str = ch.ToString();
                        str = GetString(str, 13);

                        if (str == "TSTEP_SCENARIO")
                            leaf_node.keyword = str;
                        else
                            AddToQueue(str, str.Length);
                        token_fit = true;
                        break;

                    case '}':
                        str = "}";
                        leaf_node.keyword = str;
                        rb++;
                        if ((lb == rb) && (lb > 0 && rb > 0))
                            end = true;
                        token_fit = true;
                        break;

                    case ',':

                        str = ",";
                        leaf_node.keyword = str;
                        token_fit = true;
                        break;

                    //case 'N':
                    //str = ch.ToString();
                    //str = GetString(str, 12);

                    // if (str == "NESTED_INPUTS")
                    // token = ;
                    // else//AddToQueue(str, str.Length);
                    //break;

                    case 'P':
                        str = ch.ToString();
                        str = GetString(str, 15);

                        if (str == "PARALLEL_INPUTS")
                            leaf_node.keyword = str;
                        else if (str == "PARALLEL_OUTPUTS")
                            leaf_node.keyword = str;
                        else
                            AddToQueue(str, str.Length);
                        token_fit = true;
                        break;

                    default:

                        //if (ch == '\r' | ch == '\n')
                        if (Regex.IsMatch(ch.ToString(), "[a-zA-Z]"))
                        {
                            string identifier_name = IdentifierReader(ch);
                            id_count++;
                            Symbol identifier = new Symbol(id_count, identifier_name, DataType.IDENTIFIER, "0");
                            SymbolTable.Add(id_count, identifier);
                            leaf_node.sym_id = id_count;
                            token_fit = true;
                        }

                        else if (Regex.IsMatch(ch.ToString(), "[-0-9]"))
                        {
                            string num = NumberReader(ch);
                            id_count++;
                            Symbol constant = new Symbol(id_count, num, DataType.INTEGER_CONSTRAINT, num);
                            SymbolTable.Add(id_count, constant);
                            leaf_node.sym_id = id_count;
                            token_fit = true;
                        }

                        else
                            token = TerminalToken.SKIP;
                        break;
                }
            }
            all_tokens_read = true;
        }

        public Token GetToken(StreamReader input)
        {
            Token token = Token.NULL;
            string str;
            string token_char;
            char ch;

            ch = Convert.ToChar(input.Read());

            switch (ch)
            {
                case '{':
                    str = "{";
                    token_char = str;
                    lb++;
                    token = Token.LB;
                    break;
                case 'S':
                    str = ch.ToString();
                    str = GetString(str, 7);

                    if (str == "SCENARIO")
                        token = Token.SCENARIO;
                    else
                        AddToQueue(str, str.Length);
                    break;

                case 'T':
                    str = ch.ToString();
                    str = GetString(str, 13);

                    if (str == "TSTEP_SCENARIO")
                        token = Token.T_STEP;
                    else
                        AddToQueue(str, str.Length);
                    break;

                case '}':
                    str = "}";
                    rb++;
                    token = Token.RB;
                    if ((lb == rb) && (lb > 0 && rb > 0))
                        end = true;
                    break;

                case ',':
                    str = ",";
                    token = Token.COMMA;
                    token_char = str;
                    break;

                //case 'N':
                //str = ch.ToString();
                //str = GetString(str, 12);

                // if (str == "NESTED_INPUTS")
                // token = ;
                // else
                //AddToQueue(str, str.Length);
                //break;

                case 'P':
                    str = ch.ToString();
                    str = GetString(str, 15);

                    if (str == "PARALLEL_INPUTS")
                        token = Token.PARALLEL_INPUT;
                    else if (str == "PARALLEL_OUTPUTS")
                        token_char = str;
                    else
                        AddToQueue(str, str.Length);
                    break;

                default:

                    //if (ch == '\r' | ch == '\n')
                    if (Regex.IsMatch(ch.ToString(), "[a-zA-Z]"))
                    {
                        token_char = IdentifierReader(ch, input);
                        token = Token.ID;
                    }
                    else if (Regex.IsMatch(ch.ToString(), "[-0-9]"))
                    {
                        token_char = NumberReader(ch, input);
                        token = Token.INT_CONST;
                    }

                    else
                        token = Token.SKIP;
                    break;
            }
            return token;
        }

        public string IdentifierReader(char ch)
        {
            string Id_name = String.Empty;
            while (Regex.IsMatch(ch.ToString(), "[a-zA-Z0-9_]"))
            {
                Id_name += ch;
                ch = Convert.ToChar(Globals.input1.Peek());
                if (Regex.IsMatch(ch.ToString(), "[a-zA-Z0-9_]"))
                {
                    ch = Convert.ToChar(Globals.input1.Read());
                }
            }
            return Id_name;
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

        public string IdentifierReader(char ch, StreamReader input)
        {
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
        public string NumberReader(char ch, StreamReader input)
        {
            string num = "";
            while (Regex.IsMatch(ch.ToString(), "[-0-9]"))
            {
                num += ch;

                ch = Convert.ToChar(Globals.input1.Peek());
                if (Regex.IsMatch(ch.ToString(), "[-0-9]"))
                {
                    ch  = Convert.ToChar(Globals.input1.Read());
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

        public void fit_tokens()
        {
            foreach (Node leaf in Tree.leaf_nodes)
            {
                GetToken(leaf);
            }
        }


        public void print_SymbolTable()
        {
            foreach (KeyValuePair<int, Symbol> sym in SymbolTable)
            {
                Console.WriteLine(sym.Value);
            }
        }

        //Data Generation
        public void GenerateData(Node node)
        { 
            if ((node.item == Token.MIN || min_visited == true) && node.item != Token.PI) 
                semantic_action_executive(node);

            foreach (Node child in node.children)
                GenerateData(child);

            if (node.item == Token.PI)
                semantic_action_executive(node);

            if (node.item == Token.PI_LIST) //retStrList.Add(DataListGenerate());
                data += DataListGenerate();
        }
        public void InputDataList()
        {
            retStrList = data.Split("\n");
        }

        range pi_range = new range();
        List<range> piRangelist = new List<range>();
        public List<int> InputList = new List<int>();
        public void semantic_action_executive(Node n)
        {
            switch (n.item)
            {
                case Token.PI: //PI
                    piRangelist.Add(new range(pi_range.min, pi_range.max, pi_range.step));
                    break;

                case Token.MIN://MIN
                    pi_range.min = Convert.ToInt32(SymbolTable[n.children[0].sym_id].value);
                    min_visited = true;
                    //SymbolTable[n.sym_id].value[3]
                    break;

                case Token.MAX: //MAX
                    pi_range.max = Convert.ToInt32(SymbolTable[n.children[0].sym_id].value);
                    break;

                case Token.STEP: //STEP
                    pi_range.step = Convert.ToInt32(SymbolTable[n.children[0].sym_id].value);
                    break;
            }
            //return debugStr;
        }

        public string DataListGenerate()
        {

            string debugStr = String.Empty;
            int[] pivalues = new int[piRangelist.Count];

            //public int[] DataRow;
            for (int piindex = 0; piindex < pivalues.Length; piindex++)
            {
                pivalues[piindex] = piRangelist[piindex].min;
            }

            int num_loops = (piRangelist[0].max - piRangelist[0].min) / piRangelist[0].step;
            int step = piRangelist[0].step;

            for (int index = 0; index < num_loops; index++)
            {
                int paramIndex = 0;
                while (paramIndex < pivalues.Length)
                {
                    int i = 0;
                    //print each parameter 
                    //Console.Write(pivalues[paramIndex]);\

                    int paramLoops = (piRangelist[paramIndex].max - piRangelist[paramIndex].min) / piRangelist[paramIndex].step;

                    Debug.Assert(paramLoops == num_loops);

                    if (paramIndex > 0 && !String.IsNullOrEmpty(debugStr))
                    {
                        debugStr += ",";
                    }
                    debugStr += pivalues[paramIndex]; InputList.Add(pivalues[paramIndex]);
                    pivalues[paramIndex] += piRangelist[paramIndex].step;
                    if (paramIndex == pivalues.Length - 1 && index != num_loops - 1)
                    {
                        debugStr += "\n";
                    }

                    //records = paramIndex;

                    paramIndex++;

                }
                Datalist.Add(pivalues);
            }
            piRangelist.Clear();
            return debugStr;
        }
    }
}


