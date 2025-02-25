namespace constraint_puzzle_solver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            entitytree et = new entitytree();
            et.addentity(new entity("room1", new List<string> { "1", "2", "3" }));
            et.addentity(new entity("room2", new List<string> { "4", "5", "6" }));  
            et.addentity(new entity("room3", new List<string> { "7", "8", "9" }));
            et.traverse();
        }
    }
}