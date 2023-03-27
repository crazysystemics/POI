using System.Diagnostics;
using System.IO;
using static System.Console;
using System.Collections.Generic;
using System;


partial class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("hello");

        // See https://aka.ms/new-console-template for more information

        
        //Poem is Application Model
        //ಊಟದ_ಆಟ is Application Instance With Persitence Instance (sglobal.poem_txt)
        Poem ಊಟದ_ಆಟ = new Poem(sglobal.poem_text);

        //Lexical and Syntax Analysis Phase (semantic also??)
        //Architectural Components are Created
        ಊಟದ_ಆಟ.Parse();
        WriteLine(ಊಟದ_ಆಟ.GetFirstLine(ಊಟದ_ಆಟ.firstHalfOfStanzas));
        sglobal.apriori.sequences.Add(ಊಟದ_ಆಟ.secondHalfOfStanzas);

        List<string> first_half_list  = ಊಟದ_ಆಟ.firstHalfOfStanzas.ToStringList();
        List<string> second_half_list = ಊಟದ_ಆಟ.secondHalfOfStanzas.ToStringList();

        //Discover Properties of Architectural Components
        //"ಬಾಳೆಲೆ ಹರಡು, ಅನ್ನ ಹಾಕು..
        NestedString secondLinesNs = ಊಟದ_ಆಟ.secondHalfOfStanzas;
        bool isSecondHalfSequence = ಊಟದ_ಆಟ.CheckAttribute(secondLinesNs, new IsSequence());
        if (isSecondHalfSequence)
        {
            ಊಟದ_ಆಟ.AddAttribute(secondLinesNs, new IsSequence());
        }
        //Discover Properties of Architectural Components
        //"ಒಂದು ಎರಡು, ಮೂರು ನಾಕು...
        NestedString firstLinesNs = ಊಟದ_ಆಟ.firstHalfOfStanzas;
        bool isFirstHalfSequence = ಊಟದ_ಆಟ.CheckAttribute(firstLinesNs, new IsSequence());
        if (isFirstHalfSequence)
        {
            ಊಟದ_ಆಟ.AddAttribute(firstLinesNs, new IsSequence());
        }

        //Lesson Learnt
        //Relatively complex insight discovered, learnt!!
        //"ಒಂದು ಎರಡು, ಮೂರು ನಾಕು...is a Sequence !!!
        Debug.Assert(ಊಟದ_ಆಟ.architecturalAttributes[firstLinesNs].Find(x => x == new IsSequence()) != null);
    }
}