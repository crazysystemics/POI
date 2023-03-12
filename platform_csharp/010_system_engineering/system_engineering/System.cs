using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system_engineering
{
    public abstract class System
    {
        public int id;
        public List<string> outbox = new List<string>();
        public List<string> inbox = new List<string>();

        //abstract public System query(atomic_single_variable_concept asvc);
        //discovery, vision 
        List<concept> Definition = new List<concept>();

        public abstract System get();
        public abstract void set(System sys);
        public abstract void update();

        public List<concept> define(Mode m)
        { return null; }
    }

    class Domain
    { }

    class EcoSystem
    { }

    abstract public class SOS:System
    { }    

    abstract class SubSystem:System
    {
       
    }

    abstract class Unit : SubSystem
    { }

    //Units are also Subsystems - just Atomic SubSystems
    public class SystemDefinition
    {
        //In case system is a entity/concept/phenomenon
        //that is discovered, perceived or understood
    }

    class SystemSpecification
    {
        //In case System needs to be created, constructed or engineered
    }

    class SystemArchitecture
    {
        
        public List<SubSystem> SubSystems = new List<SubSystem>();
        public SubSystem[,] SubSystemRelations;        
    }

    class Instruction
    { public string code;  } 

    class Technology
    { }
   
    class Operation
    { }

    class Evaluation
    {
        double benchmark;
    }

}
