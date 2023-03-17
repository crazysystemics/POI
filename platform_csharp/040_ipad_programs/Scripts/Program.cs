// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, Script World!");

Predicate p;

p = new Predicate();
p=true;
bool q = p;
p.Value = true;



abstract class Predicate
{
    public bool Value { get; set; }     
}
