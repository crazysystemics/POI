using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace constraint_puzzle_solver
{
    //class entity contains
    //attributes: name(string) and value(list(string))
    //methods: constructor, getname, getvalue   
    class entity
    {
        private string name;
        private List<string> value;

        public entity(string name, List<string> value)
        {
            this.name = name;
            this.value = value;
        }

        public string getname()
        {
            return name;
        }

        public List<string> getvalue()
        {
            return value;
        }
    }

    //class entitytree contains entities(list(entity))
    //methods: constructor, addentity, getentity
    class entitytree
    {
        List<entity> entities = new List<entity>();
        public entitytree() { }

        //traverse entities in depth first order
        public void traverse()
        {
            foreach (entity e in entities)
            {
                //Console.WriteLine(e.getname());
                foreach (string s in e.getvalue())
                {
                    Console.WriteLine(e.getname() + ":" + s);
                }
            }
        }

        public void addentity(entity entity)
        {
            entities.Add(entity);
        }
    }
}
