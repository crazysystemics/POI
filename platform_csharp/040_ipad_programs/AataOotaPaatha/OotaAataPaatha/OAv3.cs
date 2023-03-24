using System;
using System.Collections.Generic;
using System.Linq;


// See https://aka.ms/new-console-template for more information

using static System.Console;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

public abstract class Predicate
{
    //pvalue stands for predicate value
    //p is prefixed to  disambiguate
    //with keyword value
    public abstract bool istrue();
}
class IsSequence : Predicate
{
    public override bool istrue()
    {
        throw new NotImplementedException();
    }
}
class DoCorrespond : Predicate
{
    
    public override bool istrue()
    {   //check if there is a unique correspondence-id
        //in firstHalfStanzaList and secondHalfStanzaList
        //This id can be installed in apriori way.        
        return true;
    }
}



public class NestedString
{
    public bool isTerminal;
    public string Terminal = String.Empty;
    public List<NestedString> NonTerminal = new List<NestedString>();

    public NestedString()
    {
        //throw new NotImplementedException();
    }
    public NestedString(string Terminal)
    {
        isTerminal = true;
        this.Terminal = Terminal;
    }

    public NestedString(List<NestedString> NonTerminal)
    {
        isTerminal = false;
        this.NonTerminal = NonTerminal;
    }
    //will traverse through foreach iterator
    public string nextString()
    { throw new NotImplementedException(); }

}

class Poem
{
    public Dictionary<NestedString, List<Predicate>> architecturalAttributes
                = new Dictionary<NestedString, List<Predicate>>();

    public string text;
    public NestedString parsedPoem = new NestedString();
    public NestedString firstHalfOfStanzas = new NestedString();
    public NestedString secondHalfOfStanzas = new NestedString();
    


    public Poem(string text)
    {
        this.text = text;
    }

    //Lexical Part, Pre-Processing and Getting Next Token for Parsing
    public int token_index = 0;
    public string GetToken()
    {
        string ret_string = String.Empty;
        string[] stanzas = text.Split(':');

        if (token_index < stanzas.Length)
        {
            ret_string = stanzas[token_index / 2].Split(';')[token_index % 2];
            token_index++;
        }

        return ret_string;
    }

    //Build Architectural Constructs
    //will initialize firstHalfOfStanzas and secondHalfOfStanzas
    public string Parse()
    {
        string poemLine = GetToken();
        int lineCount = 0;
        while (poemLine != String.Empty)
        {
            if (lineCount % 2 == 0)
            {
                firstHalfOfStanzas.NonTerminal.Add(new NestedString(poemLine));
            }
            else
            {
                secondHalfOfStanzas.NonTerminal.Add(new NestedString(poemLine));
            }
            poemLine = GetToken();
        }

        return poemLine;
    }


    public string GetFirstLine(NestedString ns)
    {
        if (!String.IsNullOrEmpty(firstHalfOfStanzas.Terminal))
        {
            return firstHalfOfStanzas.Terminal;
        }
        else
        {
            return GetFirstLine(firstHalfOfStanzas.NonTerminal.First());
        }
    }

    //checks whether predicate P applies to ns by evaluating it on every element of ns
    //checks whether every (but last) element is predecessor of its next element
    //in list. Note the p should be predecessor not sequence. If every element is 
    //predecessor ns naturally is sequence
    public bool CheckAttribute(NestedString ns, Predicate p)
    {
        //check whether ns is equivalent to any of the 
        //remembered sequences

        if (ns.Terminal != null)
        {
            return false;
        }

        //checking equality with apriory sequence (genetic memory)
        foreach (NestedString apriorySequence in sglobal.apriori.sequences)
        {
            if (ns == apriorySequence)
            {
                return true;
            }
        }

        //Checking equality with any of learnt sequences

        List<Predicate> attributes = new List<Predicate>();
        attributes = architecturalAttributes[ns];
        Predicate? matchp = attributes.Find(x => x == p);
        if (matchp != null && matchp is IsSequence)
        { return true; }


        return false;
    }

    //checks attribute by correspondence between ns and referredNs over predicate P
    public bool CheckAtrribute(NestedString ns, NestedString referredString, Predicate p)
    {
        List<Predicate> first_predicates = architecturalAttributes[firstHalfOfStanzas];
        List<Predicate> second_predicates = architecturalAttributes[firstHalfOfStanzas];

        bool correspondenceExists = false;
        foreach (Predicate first_pred in first_predicates)
        {
            if (first_pred is DoCorrespond)
            {
                foreach (Predicate second_pred in second_predicates)
                {
                    if (second_pred is DoCorrespond)
                    {
                        if (((DoCorrespond)first_pred).correspondenceId
                             == ((DoCorrespond)second_pred).correspondenceId)
                        {
                            correspondenceExists = true;
                        }
                    }
                }
            }

        }

        //search for correspondence complete
        if (!correspondenceExists)
        {
            return false;
        }

        //correspondence exists, check whether referred string is sequence
        if (referredString == firstHalfOfStanzas)
        {
            if (CheckAttribute(firstHalfOfStanzas, p))
            {
                return true;
            }
        }

        if (referredString == secondHalfOfStanzas)
        {
            if (CheckAttribute(secondHalfOfStanzas, p))
            {
                return true;
            }           
        }

        return false;

    }

    public void AddAttribute(NestedString ns, Predicate p)
    {
        if (ns == firstHalfOfStanzas)
        {
            architecturalAttributes[firstHalfOfStanzas].Add(p);
        }
        else if (ns == secondHalfOfStanzas)
        {
            architecturalAttributes[secondHalfOfStanzas].Add(p);
        }
    }
}

static class sglobal
{

    public static string poem_text = "ondu eradu;balele haradu:" +
        "mooru naaku;anna haaku:" +
        "aidu aaru;bele saaru:" +
        "elu entu;palyake dantu:" +
        "ombattu hattu;ele mudirettu:" +
        "ondarinda hattu heegittu;ootada aata mugidittu:";

    public static class apriori
    {
        public static List<NestedString> sequences = new List<NestedString>();


        public static void print_first_two_words(string text)
        {
            Console.WriteLine(text.Split(':')[0]);
        }


    }

    class OotadaAata
    {
        public NestedString first_line_list = new NestedString();
        public NestedString second_line_list = new NestedString();

        
        public OotadaAata()
        {

        }

        public void Parse(ref NestedString first_line_list,
                          ref NestedString second_line_list)
        {

        }
    }
}

/*
 
//=================================================================

//NS -> Nested String
//Attrib->Attribute
//CO->Consecutive
//Validator

//First of all this string overrides by adding a NS parameter
//Then this method checks each adjacent elements of a list are CONSECUTIVE
//i.e., every element's next element is also it's successor in domain.
//this also means they are ORDERED. By checking the integrity of  the list an ATTRIBUTE of SEQUENCE is
//assigned to list.
abstract class AttributeValidator
{    
    public abstract bool isAttributeValid();
}


//public List<PoeticProperty> 
/poetic_properties=new List<PoeticProperty()

class NsSequenceValidatorByCO : AttributeValidator
{
    public NestedString ns;

    public override bool isAttributeValid()
    {
        return true;
    }

    public bool AreElementsConsecutive(NestedString ns)
    {
        //@here
        return true;
    }

}


List<AttributeValidator> attributeValidators = new List<AttributeValidator>();
static class Utilities
{
    //procedure
    public static bool isOrdered(NestedString ns)
    {
        // true if, every ns[i] <= ns[i+1] (but last ns[i])
        return true;
    }

    public static bool isSequence(NestedString ns)
    {
        //true if ns is consecutively ordered
        return true;
    }

    public static bool isSequence(NestedString ns, NestedString reference_ns)
    {
        //sequence by analogy
        //true if reference_ns is sequence and
        //ns and reference_correspond to each other
        return true;
    }

    public static bool correspond(NestedString ns1, NestedString ns2)
    {
        //ns1 consists of first line of stanza
        //ns2 consists of second line of stanza
        //if corresponding items in ns1 and ns2 refer to
        //first and second lines of a stanza (or vice-versa)
        return true;
    }
}

class NestedStringSequenceIterator
{
    private List<NestedString> NestedStringSequence;
    private int pos;

    public NestedStringSequenceIterator(List<NestedString> NestedStringSequence)
    {
        pos = 0;
        this.NestedStringSequence = NestedStringSequence;
    }

    public ref NestedString begin()
    {
        pos = 0;
        return ref NestedStringSequence.ToArray()[pos];
    }

    public ref NestedString end()
    {
        pos = NestedStringSequence.Count - 1;
        return ref NestedStringSequence.ToArray()[pos]; ;
    }

    public ref NestedString prev()
    {
        Debug.Assert(pos > 0);
        pos--;
        return ref NestedStringSequence.ToArray()[pos];
    }

    public ref NestedString next()
    {
        Debug.Assert(pos < NestedStringSequence.Count - 1);
        pos++;
        return ref NestedStringSequence.ToArray()[pos];
    }

    public ref NestedString here()
    {
        return ref NestedStringSequence.ToArray()[pos];
    }
}


class AttribNameVal
{
    public delegate bool TestAttribute(NestedString ns, NestedString rs = null);

    string attribute = "";
    string value = "";
    TestAttribute fn_isAttribNameValid;

    public AttribNameVal(string attribute, string value, TestAttribute fn_is_tattrib_valid)
    {
        this.attribute = attribute;
        this.value = value;
        this.fn_isAttribNameValid = fn_is_tattrib_valid;
    }

    poetic_properties.Add(first_line_list,
                  second_line_list,
                              "correspondence",
                              true);
        poetic_properties.Add(first_line_list,
                                  "sequence",
                                  false,
                                  Utilities.isSequence);

        poetic_properties.Add(second_line_list,
                               "sequence",
                               false,
                               Utilities.isSequence);

         public bool isFirstLineListSequence()
    {
        bool do_first_second_list_correspond = false;
        bool is_second_line_list_ordered = false;
        bool is_second_line_list_sequence = false;
        bool is_first_line_list_sequence = false;

        is_second_line_list_ordered =
                Utilities.isOrdered(second_line_list);
        if (is_second_line_list_ordered)
        {
            is_second_line_list_sequence =
                Utilities.isSequence(second_line_list);
        }

        if (is_second_line_list_sequence)
        {
            do_first_second_list_correspond =
                Utilities.correspond(first_line_list, second_line_list);
        }

        is_first_line_list_sequence = Utilities.isSequence(first_line_list, second_line_list);

        Debug.Assert(is_first_line_list_sequence ==
                     do_first_second_list_correspond &&
                     is_second_line_list_sequence);

        return is_first_line_list_sequence;
    }

        public class PoeticProperty
    {
        private NestedString entity;
        private List<string> attributes;
        public PoeticProperty(NestedString entity, List<AttribNameVal> attributes)
        {
            this.entity = entity;
            this.attributes = attributes;
        }
    }
}

*/






