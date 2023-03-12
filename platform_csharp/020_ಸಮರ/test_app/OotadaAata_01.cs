using static System.Console;
using System.Collections.Generic; 
using System;
using System.Diagnostics;

string poem = "ondu eradu;balele haradu:mooru naaku;anna haaku: aidu aaru;bele saaru:elu entu;palyake dantu:ombattu hattu;ele mudirettu;ondarinda hattu heegittu;ootada aata mugidittu";

//balele haradu, anna haaku, bele saaru, palyake dantu, ele mudirettu
//ondu eradu, mooru naku, aidu aaru, elu entu, ombattu hattu

class NestedString
{
    public bool isTerminal;
    public string Terminal=String.Empty;
    public List<NestedString> NonTerminal=new  List<NestedString>();
    
    public NestedString(string Terminal)
    {
        isTerminal = true;
        this.Terminal   = Terminal; 
    }
    
    public NestedString(List<NestedString> NonTerminal)
    {
        isTerminal = false;
        this.NonTerminal = NonTerminal;
    }
    
    //will traverse through foreach iterator
    
}            
      
    
    

class OotadaAata
{
    public NestedString first_line_list = new NestedString();
    public NestedString second_line_list = new NestedString();
    public List<PoeticProperty> poetic_properties;
    
    public class PoeticProperty
    {
        public NestedString entity;
        public string attribute_name;
        public bool attribute_present;
        public TestAttribute checkAttribute;
        
        
        public PoeticProperty(NestedString entity, 
                              string attribute_name, 
                              bool   attribute_present, 
                              TestAttribute checkAttribute)
        {
            this.entity = entity;
            this.attribute_name = attribute_name;
            this.attribute_present = attribute_present;
            this.checkAttribute = checkAttribute;
        }
    }
    
    public OotadaAata()
    {
        poetic_properties.Add(first_line_list,
                              second_line_list,
                              "correspondence",
                              true,
                              Utilities.apriori);
    poetic_properties.Add(first_line_list,
                              "sequence",
                              false,
                              Utilities.isSequence);
                              
       poetic_properties.Add(second_line_list,
                              "sequence",
                              false,
                              Utilities.isSequence); 
    }
    
    public void Parse(ref NestedString first_line_list, 
                      ref NestedString second_line_list)
    {
     AttributeChecks.Add(Utilities.isSequence);   
    }
    
    public delegate bool TestAttribute(NestedString ns, 
    NestedString rs = null);
    
    List<TestAttribute> AttributeChecks = new List<TestAttribute>();
    
    public bool HasAttribute(NestedString first_list,
                             NestedString second_list,
                             TestAttribute testAttribute)
    {
        if (testAttribute(second_list) &&                testAttribute(first_list,second_list))
        {
            
        }
    }
    
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
    
        Debug.Assert(is_first_line_list_sequence==
                     do_first_second_list_correspond &&
                     is_second_line_list_sequence);
                     
        return is_first_line_list_sequence;               
        }    
                  
             
    
    
}


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
	    pos=0;
        return ref NestedStringSequence.ToArray()[pos];
	}
	
	public ref NestedString end()
	{
	    pos = NestedStringSequence.Count - 1; 
        return ref NestedStringSequence.ToArray()[pos];;
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






WriteLine ("Hello World!");
