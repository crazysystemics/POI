using System.Diagnostics;
using System.IO;
using static System.Console;
using System.Collections.Generic;
using System;


partial class Program
{
    private static void Main(string[] args)
    {
        WriteLine("Ootada Aata...");
        // See https://aka.ms/new-console-template for more information
        WriteLine(sglobal.poem_text);
        
        //Poem is Application Model
        //ಊಟದ_ಆಟ is Application Instance With Persitence Instance (sglobal.poem_txt)
        Poem Ootada_Aata = new Poem(sglobal.poem_text);
        //Lexical and Syntax Analysis Phase (semantic also??)
        //Architectural Components are Created
        Ootada_Aata.Parse();
        WriteLine("First Line after parsing...");
        WriteLine(Ootada_Aata.GetFirstLine(Ootada_Aata.firstHalfOfStanzas));
        sglobal.apriori.sequences.Add(Ootada_Aata.secondHalfOfStanzas);

        List<string> first_half_list  = Ootada_Aata.firstHalfOfStanzas.ToStringList();
        List<string> second_half_list = Ootada_Aata.secondHalfOfStanzas.ToStringList();
        WriteLine("\n\nList of First Half of Stanzas...");
        WriteLine(Ootada_Aata.firstHalfOfStanzas);        
        WriteLine("List of Second Half of Stanzas...");
        WriteLine(Ootada_Aata.secondHalfOfStanzas);
        Ootada_Aata.UpdateArchitecturalRelations();

        //Discover Properties of Architectural Components
        //"ಬಾಳೆಲೆ ಹರಡು, ಅನ್ನ ಹಾಕು..
        NestedString secondLinesNs = Ootada_Aata.secondHalfOfStanzas;
        bool isSecondHalfSequence  = Ootada_Aata.CheckAttribute(secondLinesNs, new IsSequence());
        if (isSecondHalfSequence)
        {
            //WriteLine("[ಬಾಳೆಲೆ ಹರಡು, ಅನ್ನ ಹಾಕು..] is a Sequence");
            WriteLine("\n\n[BaaLele haradu, anna haaku..] is a Sequence");
            WriteLine("based on its equivalence to apriori sequence");
            WriteLine("It is like knowing by experience...");
            Ootada_Aata.AddAttribute(secondLinesNs, new IsSequence());
        }
        //Discover Properties of Architectural Components
        //"ಒಂದು ಎರಡು, ಮೂರು ನಾಕು...
        NestedString firstLinesNs = Ootada_Aata.firstHalfOfStanzas;
        bool isFirstHalfSequence = Ootada_Aata.CheckAttribute(firstLinesNs, new IsSequence());
        if (isFirstHalfSequence)
        {
                    
            Ootada_Aata.AddAttribute(firstLinesNs, new IsSequence());
        }
        else
        {
            //WriteLine("No memory of [ಒಂದು ಎರಡು, ಮೂರು ನಾಕು...] being a sequence");
            if (isSecondHalfSequence)
            {
                WriteLine("\n\nNo memory of [Ondu Eradu, Mooru Naaku...] being a sequence");
                WriteLine("There is a rule in poetry. Second half of stanza correspnds");
                WriteLine("to First half of stanza. If one half list has a property");
                WriteLine("other one will have same attribute/property. This poem obeys ");
                WriteLine("this rule of correspondence. Sencce Second Half Stanza List has ");
                WriteLine("Sequence property First Half will have too...");
                isFirstHalfSequence = Ootada_Aata.CheckAttribute(firstLinesNs, secondLinesNs, new IsSequence());
                if (isFirstHalfSequence)
                { Ootada_Aata.AddAttribute(firstLinesNs, new IsSequence()); }
            }
        }

        //Lesson Learnt
        //Relatively complex insight discovered, learnt!!
        //"ಒಂದು ಎರಡು, ಮೂರು ನಾಕು...is a Sequence !!!        
        Debug.Assert(Ootada_Aata.architecturalAttributes[firstLinesNs].Find(x => x is IsSequence) != null);
        WriteLine("\nCode reached here means First Half of Stanzas has been added Sequence Property");
    }
}