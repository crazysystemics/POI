using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static system_engineering.ಪಾಠಶಾಲಾ;

namespace system_engineering
{
    public abstract class concept
    {
        abstract public bool istrue(string lhs, string concept_operator,string rhs,
                                  List<string> lhs_substitutions,
                                  List<string> rhs_substitutions);


    }

    class atomic_constant_concept : concept
    {
        //atomic-concept is a single relational expression with no variables
        //logical-proposition
        public override bool istrue(string lhs, string concept_operator, string rhs,
                           List<string> lhs_substitutions = null,
                           List<string> rhs_substitutions = null)
        {
            return lhs == rhs;
        }
    }

    class atomic_single_variable_concept : concept
    {
        public string lhs;
        public string concept_operator;
        public string rhs;
        public bool lhs_is_constant;

        public atomic_single_variable_concept(string plhs, string pco, string prhs,
                                              bool plic)
        {
            lhs = plhs;
            concept_operator = pco;
            rhs = prhs;
            lhs_is_constant = plic;
        }

        public override bool istrue(string lhs, string concept_operator, string rhs,
                                         List<string> lhs_substitutions = null,
                                         List<string> rhs_substitutions = null)
        {
            return false;
        }
    }


    class atomic_double_variable_concept : concept
    {
        public override bool istrue(string lhs, string concept_operator, string rhs,
                           List<string> lhs_substitutions = null,
                           List<string> rhs_substitutions = null)
        {
            //resolution or unification algorithm
            return false;
        }
    }

    class Query
    {
        atomic_single_variable_concept query_concept =
               new atomic_single_variable_concept("","","",false);
        public System query(atomic_single_variable_concept iq)
        {
            return null;
        }

        public void set(atomic_single_variable_concept query_asvc)
        {
            query_concept = query_asvc;
        }

        public System execute()
        {
            //return null
            //execute query_concept
            return null;
        }

    }

    class predicate_concept : concept
    {
        public override bool istrue(string lhs, string concept_operator, string rhs,
                                           List<string> lhs_substitutions = null,
                                           List<string> rhs_substitutions = null)
        {
            return false;
        }

        public string resolve(string lhs, string concept_operator, string rhs,
                                           ref List<string> lhs_substitutions,
                                           ref List<string> rhs_substitutions)

        {
            string unified_string = string.Empty;
            lhs_substitutions.Add("temp");
            rhs_substitutions.Add("temp");
            return unified_string;
        }     
    }
}
