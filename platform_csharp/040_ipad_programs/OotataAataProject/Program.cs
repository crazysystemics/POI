// See https://aka.ms/new-console-template for more information

using static System.Console;
using System.Collections.Generic;
using System;
using System.Diagnostics;

Poem ಊಟದ_ಆಟ = new Poem();
ಊಟದ_ಆಟ.Parse();
WriteLine(ಊಟದ_ಆಟ.GetFirstLine());

List<string> firstLines = ಊಟದ_ಆಟ.GetFirstHalfOfStanzas();
foreach (string line in firstLines)
{
    WriteLine(line);
}

List<string> secondLines = ಊಟದ_ಆಟ.GetSecondHalfOfStanzas();
foreach (string line in firstLines)
{
    WriteLine(line);
}

//"ಬಾಳೆಲೆ ಹರಡು, ಅನ್ನ ಹಾಕು..
NestedString secondLinesNs = new NestedString();
bool isSecondHalfSequence = ಊಟದ_ಆಟ.CheckAttribute(secondLinesNs,new IsSequence());
if (isSecondHalfSequence)
{
    ಊಟದ_ಆಟ.AddAttribute(secondLinesNs, new IsSequence());
}

//"ಒಂದು ಎರಡು, ಮೂರು ನಾಕು...
NestedString firstLinesNs = new NestedString();
bool isFirstHalfSequence = ಊಟದ_ಆಟ.CheckAttribute(firstLinesNs, secondLinesNs, new DoCorrespond());
if (isFirstHalfSequence)
{
    ಊಟದ_ಆಟ.AddAttribute(firstLinesNs, new IsSequence());
}

//Lesson Learnt
//"ಒಂದು ಎರಡು, ಮೂರು ನಾಕು...is a Sequence !!!
Debug.Assert(ಊಟದ_ಆಟ.architecturalAttributes[firstLinesNs].Find(x => x == new IsSequence()) != null);