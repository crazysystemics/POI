using CodeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    class Node
    {
        public Token item;
        public int sym_id;
        public int action_id;
        public string token;
        public List<Node> children;
        public string keyword = null;
        //RAVIJ-Move it in appropriate class

        public Node(Token item, int action_id, List<Node> children = null)
        {
            this.item = item;
            this.action_id = action_id;
            if (children == null)
            {
                this.children = new List<Node>();
            }
            else
            {
                this.children = children;
            }
            if (this.item == Token.ID || this.item == Token.SCENARIO || this.item == Token.T_STEP
                || this.item == Token.LB || this.item == Token.RB || this.item == Token.PARALLEL_INPUT
                || this.item == Token.LPAREN || this.item == Token.RPAREN || this.item == Token.COMMA
                || this.item == Token.INT_CONST || this.item == Token.PARALLEL_OUTPUT)
                Tree.leaf_nodes.Add(this);
            Tree.node_count++;
        }

        public Node(string token, int action_id, List<Node> children = null)
        {
            this.token = token;
            this.action_id = action_id;
            if (children == null)
            {
                this.children = new List<Node>();
            }
            else
            {
                this.children = children;
            }
        }
    }
}