using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system_engineering
{
    class SystemEngineer
    {
        Domain d;
        Technology[] technologies;
        SOS sos;
        System[] systems;
        Operation[] operations;
        Evaluation evaluation;


    }

    class LifeCycle
    {
        public SystemSpecification CreateSystemSpecification(object state_of_art,
                                                      object user_customer_requirements,
                                                      object user_customer_pain_points,
                                                      object market_analysis,
                                                      object competitive_analysis)
        { return null; }

        public object executeNextLifeCycle(object prev_slc_artifact,
                                 SystemSpecification spec)

        {
            //current_slc_artifact = 
            //  prev_slc_artifact.Translate(spec, prev_slc_artifact)
            //return currelnt_slc_artifact

            //slc_artifact ==> SystemLifeCycle artifact
            //slc.Realize(spec)
            //slc.Operate() will be called during operational phase

            return new Object();
        }
    }
}
