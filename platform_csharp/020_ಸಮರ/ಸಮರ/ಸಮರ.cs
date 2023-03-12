using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system_engineering
{
    class RWREngineer:SystemEngineer
    {
        public Domain Battle;
        public TutorialLifeCycle slc = new RWRlifeCycle();

    }

    internal class RWR:System
    {
    }

    class RWRSpecification:SystemSpecification
    {
        //In case System needs to be created, constructed or engineered
    }

    class RWRlifeCycle:TutorialLifeCycle
    {
       
    }

    
    class EW:Technology
    { }

    class Battlefield:SOS
    { }

    

    class Antenna:SubSystem
    { }

    class EWAttack:Operation
    { }

    class POI:Evaluation
    {
        double benchmark;
    }

}
