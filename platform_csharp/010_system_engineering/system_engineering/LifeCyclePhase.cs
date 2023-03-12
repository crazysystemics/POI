using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system_engineering
{
    internal class LifeCyclePhase
    {
        //Specification:
        //Definition
        //Overview => Interfaces + Functions
        //List of Specifications [boolean expressions, predicates]
        //functional, non-functional, design-constraints

        public SystemSpecification specify(Mode m)
        {
            return null;
        }

        public SystemArchitecture define_architecture(SystemSpecification spec)
        { return null; }

        public List<Instruction> Realize(SystemSpecification specification,
                                         SystemArchitecture architecture)
        {
            return null;
        }

        public void UnitTest()
        { }

        public void IntegrationTest()
        { }

        public void SystemTest()
        { }

        //here lab prototype is done.
        //engineering models and 
        public void QualificationTest()
        { }

        public void ESS()
        { }

        public void FlightTrials()
        { }

        //production is assumed.
        //It is very big topic and needs to be treated 
        //separately
        public void Deploy()
        { }

        public void Operate()
        { }

        public void Maintain()
        { }

        public void Retire()
        { }
    }
}
