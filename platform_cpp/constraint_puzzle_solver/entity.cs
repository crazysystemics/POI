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
        private List<string> values;
        public List<bool> isAssigned;

        public entity(string name, List<string> values)
        {
            this.name = name;
            this.values = values;

            //initialize isAssigned list to all false
            isAssigned = new List<bool>();
            for (int i = 0; i < values.Count; i++)
            {
                isAssigned.Add(false);
            }
        }

        public string getname()
        {
            return name;
        }

        public List<string> getvalue()
        {
            return values;
        }
    }

    //class entitytree contains entities(list(entity))
    //methods: constructor, addentity, getentity
    class entitytree
    {
        List<entity> entities = new List<entity>();
        int entity_index = 0;
        int value_index = 0;
        public entitytree() { }

        //modify traverse method to traverse one step at a time
        //and return the entity
        public void getLatestAssignment(ref entity e, ref string eval)
        {
            e = entities[entity_index];  
            eval = entities[entity_index].getvalue()[value_index]; 
        } 
        
        public void Assign()
        {
            entities[entity_index].isAssigned[value_index] = true;
        }
           
        public void traverse_one_step()
        {
            entities[entity_index].isAssigned[value_index] = false;
            
            entity e = entities[entity_index];            
            if (value_index < e.getvalue().Count)
            {
                value_index++;
            }
            else
            {
                entity_index++;
                if (entity_index == entities.Count)
                {
                    entity_index = -1;
                }
                value_index = 0;
            }
        }

       



        public void addentity(entity entity)
        {
            entities.Add(entity);
        }
    }
}
