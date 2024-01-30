using System;
using System.ComponentModel;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace AircraftGridTraversal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //DTSE dtse = new DTSE();
            //dtse.Init();
            //foreach (SimulationModel model in dtse.models)
            //{
            //    if (model is Aircraft)
            //    {
            //        Console.WriteLine($"Aircraft start position: {model.pos.x}, {model.pos.y}");
            //        Console.WriteLine($"Aircraft end position: {((Aircraft)model).endPoint.x}, {((Aircraft)model).endPoint.y}\n");
            //    }
            //}
            //while (!((Aircraft)dtse.models[0]).reachedEndPoint)
            //{
            //    //Console.WriteLine($"\nPress Enter/Return to display next tick\n\n");
            //    //Console.ReadLine();
            //    dtse.RunSimulationEngine();
            //}

            Maze maze = new Maze();
            DFS dfs = new DFS(new Position(6, 4), new Position(18, 11), maze);
            dfs.StartDFS();
            //dfs.BatchDFS(10, dfs.startPos, dfs.endPos, dfs.inputMap);
        }
    }
}

public class Maze
{
    public string[,] mazeGraph = new string[20, 20];
    public Maze()
    {
        for (int i = 0; i < mazeGraph.GetLength(0); i++)
        {
            for (int j = 0; j < mazeGraph.GetLength(1); j++)
            {
                this.mazeGraph[i, j] = "-";
            }
        }

        for (int i = 0; i < 20; i++)
        {
            this.mazeGraph[i, 0] = "X";
            this.mazeGraph[i, 19] = "X";
        }

        for (int i = 1; i < 19; i++)
        {
            this.mazeGraph[0, i] = "X";
            this.mazeGraph[19, i] = "X";
        }

        for (int i = 2; i < 18; i++)
        {
            this.mazeGraph[i, 17] = "X";
        }

        for (int i = 2; i < 16; i++)
        {
            if (i == 8 || i == 13)
            {
                continue;
            }
            else
            {
                this.mazeGraph[i, 15] = "X";
            }
        }

        for (int i = 2; i < 19; i++)
        {
            if (i == 8 || i == 14)
            {
                continue;
            }
            else
            {
                this.mazeGraph[i, 3] = "X";
            }
        }

        for (int i = 2; i < 19; i++)
        {
            if (i == 3 || i == 5 || i == 8 || i == 14)
            {
                continue;
            }
            else
            {
                this.mazeGraph[i, 2] = "X";
            }
        }

        for (int i = 2; i < 18; i++)
        {
            if (i == 6 || i == 8 || i == 10 || i == 13 || i == 16)
            {
                continue;
            }
            else
            {
                this.mazeGraph[i, 12] = "X";
            }
        }

        for (int i = 5; i < 15; i++)
        {
            if (i == 11)
            {
                continue;
            }
            else
            {
                this.mazeGraph[2, i] = "X";
            }
        }

        for (int i = 5; i < 15; i++)
        {
            if (i == 6 || i == 9 || i == 11)
            {
                continue;
            }
            else
            {
                this.mazeGraph[5, i] = "X";
            }
        }

        for (int i = 4; i < 14; i++)
        {
            if (i == 10)
            {
                continue;
            }
            else
            {
                this.mazeGraph[7 ,i] = "X";
            }
        }

        for (int i = 5; i < 15; i++)
        {
            if (i == 10)
            {
                continue;
            }
            else
            {
                this.mazeGraph[9, i] = "X";
            }
        }

        for (int i = 5; i < 15; i++)
        {
            if (i == 7 || i == 13)
            {
                continue;
            }
            else
            {
                this.mazeGraph[11, i] = "X";
            }
        }

        for (int i = 6; i < 15; i++)
        {
            if (i == 7 || i == 13)
            {
                continue;
            }
            else
            {
                this.mazeGraph[12, i] = "X";
            }
        }

        for (int i = 6; i < 15; i++)
        {
            if (i == 7 || i == 13)
            {
                continue;
            }
            else
            {
                this.mazeGraph[14, i] = "X";
            }
        }

        for (int i = 6; i < 15; i++)
        {
            if (i == 11 || i == 12 || i == 13)
            {
                continue;
            }
            else
            {
                this.mazeGraph[16, i] = "X";
            }
        }

        for (int i = 10; i < 17; i++)
        {
            if (i == 15)
            {
                continue;
            }
            else
            {
                this.mazeGraph[17, i] = "X";
            }
        }

        for (int i = 4; i < 9; i++)
        {
            this.mazeGraph[18, i] = "X";
        }

        for (int i = 5; i < 13; i++)
        {
            if (i == 6 || i == 9 || i == 11)
            {
                continue;
            }
            else
            {
                this.mazeGraph[4, i] = "X";
            }
        }

        for (int i = 13; i < 17; i++)
        {
            this.mazeGraph[i, 4] = "X";
        }

        mazeGraph[3, 5] = "X";
        mazeGraph[3, 10] = "X";
        mazeGraph[3, 12] = "X";
        mazeGraph[6, 7] = "X";
        mazeGraph[6, 8] = "X";
        mazeGraph[8, 11] = "X";
        mazeGraph[10, 14] = "X";
        mazeGraph[9, 16] = "X";
        mazeGraph[17, 16] = "X";
        mazeGraph[14, 15] = "X";
        mazeGraph[14, 12] = "X";
        mazeGraph[10, 5] = "X";
        mazeGraph[10, 9] = "X";
        mazeGraph[13, 6] = "X";
        mazeGraph[15, 6] = "X";
        mazeGraph[15, 14] = "X";

        mazeGraph[6, 4] = "A";
        mazeGraph[18, 11] = "G";

        Console.WriteLine($"Input maze:\n");

        for (int i = this.mazeGraph.GetLength(0); i > 0; i--)
        {
            for (int j = 0; j < this.mazeGraph.GetLength(1); j++)
            {
                Console.Write($" {mazeGraph[j, i - 1]} ");
            }
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.ReadLine();
    }
}

public class DFS
{
    public Position startPos;
    public Position endPos;
    public Maze inputMap;
    public bool[,] visitedNodes;
    public bool[,] backtrackable;
    public Position[] neighbouringNodes;
    public List<Position> pathFollowed = new List<Position> ();
    public List<Position> backtrackedNodes = new List<Position> ();
    public List<int> allPathCosts = new List<int> ();
    public List<List<Position>> paths = new List<List<Position>> ();
    public int pathCost;
    public int minCostIndex;
    public List<Position> minCostPath = new List<Position> ();
    public DFS(Position startPos, Position endPos, Maze inputMap)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.inputMap = inputMap;
        this.visitedNodes = new bool[inputMap.mazeGraph.GetLength(0), inputMap.mazeGraph.GetLength(1)];
        this.backtrackable = new bool[inputMap.mazeGraph.GetLength(0), inputMap.mazeGraph.GetLength(1)];
        for(int i = 0; i < this.backtrackable.GetLength(0); i++)
        {
            for (int j = 0; j < this.backtrackable.GetLength(1); j++)
            {
                this.backtrackable[i, j] = true;
            }
        }
        for (int i = 0; i < this.visitedNodes.GetLength(0); i++)
        {
            for (int j = 0; j < this.visitedNodes.GetLength(1); j++)
            {
                this.visitedNodes[i, j] = false;
            }
        }
        this.neighbouringNodes = new Position[4];
        this.pathCost = 0;
    }

    public void StartDFS()
    {
        Search(startPos, endPos, inputMap);
    }

    public void BatchDFS(int batchSize, Position currentNode, Position endNode, Maze inputMap)
    {
        for (int j = 0; j < batchSize; j++)
        {
            if (currentNode.x == this.startPos.x && currentNode.y == this.startPos.y)
            {
                this.pathFollowed.Clear();
                this.pathCost = 0;
            }
            this.visitedNodes[currentNode.x, currentNode.y] = true;
            //inputMap.mazeGraph[currentNode.x, currentNode.y] = "+";

            List<Position> allowedNodes = new List<Position>();
            this.pathFollowed.Add(currentNode);
            this.pathCost++;

            //Console.WriteLine($"Current search node: {currentNode.x}, {currentNode.y}");

            this.neighbouringNodes[0] = new Position(currentNode.x, currentNode.y + 1);
            this.neighbouringNodes[1] = new Position(currentNode.x + 1, currentNode.y);
            this.neighbouringNodes[2] = new Position(currentNode.x, currentNode.y - 1);
            this.neighbouringNodes[3] = new Position(currentNode.x - 1, currentNode.y);

            for (int i = 0; i < this.neighbouringNodes.Length; i++)
            {
                bool nextNodeVisited = this.visitedNodes[neighbouringNodes[i].x, neighbouringNodes[i].y];
                bool nextNodeBacktrackable = this.backtrackable[neighbouringNodes[i].x, neighbouringNodes[i].y];
                bool obstacleAtNeighbour = inputMap.mazeGraph[neighbouringNodes[i].x, neighbouringNodes[i].y] == "X";
                bool reachedEndNode = currentNode.x == endNode.x && currentNode.y == endNode.y;
                if (!nextNodeVisited && !obstacleAtNeighbour && !reachedEndNode && nextNodeBacktrackable)
                {
                    allowedNodes.Add(neighbouringNodes[i]);
                }
            }

            if (allowedNodes.Count == 0)
            {
                this.backtrackable[currentNode.x, currentNode.y] = false;
                this.backtrackedNodes.Add(currentNode);

                for (int i = 0; i < neighbouringNodes.Length; i++)
                {
                    bool nextNodeBacktrackable = this.backtrackable[neighbouringNodes[i].x, neighbouringNodes[i].y];
                    bool obstacleAtNeighbour = inputMap.mazeGraph[neighbouringNodes[i].x, neighbouringNodes[i].y] == "X";
                    bool reachedEndNode = currentNode.x == endNode.x && currentNode.y == endNode.y;
                    if (nextNodeBacktrackable && !obstacleAtNeighbour && !reachedEndNode)
                    {
                        allowedNodes.Add(neighbouringNodes[i]);
                        //this.backtrackable[neighbouringNodes[i].x, neighbouringNodes[i].y] = false;
                        break;
                    }
                }
            }

            foreach (Position allowed in allowedNodes.OrderBy(x => Globals.randomGenerator.Next(allowedNodes.Count)))
            {
                if (batchSize > 0)
                {
                    BatchDFS(batchSize, allowed, endNode, inputMap);
                    allowedNodes.Clear();
                    break;
                }
            }


            if (currentNode.x == endNode.x && currentNode.y == endNode.y)
            {
                List<Position> path = new List<Position>();
                foreach (Position pathpos in pathFollowed)
                {
                    
                    path.Add(new Position(pathpos.x, pathpos.y));
                }
                this.paths.Add(path);
                this.allPathCosts.Add(pathCost);
            }
        }

        if (batchSize == 0)
        {
            this.minCostIndex = this.allPathCosts.IndexOf(this.allPathCosts.Min());
            this.minCostPath = paths[minCostIndex];

            Console.WriteLine("\nPath followed:\n");
            foreach (Position waypoint in minCostPath)
            {
                inputMap.mazeGraph[waypoint.x, waypoint.y] = "+";
                Console.WriteLine($"{waypoint.x}, {waypoint.y}");
            }
            Console.WriteLine($"\nPath cost: {allPathCosts[minCostIndex]}\n");

            for (int i = inputMap.mazeGraph.GetLength(0); i > 0; i--)
            {
                for (int j = 0; j < inputMap.mazeGraph.GetLength(1); j++)
                {
                    if (inputMap.mazeGraph[j, i - 1] == "+")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write($" {inputMap.mazeGraph[j, i - 1]} ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }

    void Search(Position currentNode, Position endNode, Maze inputMap)
    {
        this.pathFollowed.Add(currentNode);
        this.pathCost++;
        this.visitedNodes[currentNode.x, currentNode.y] = true;
        //inputMap.mazeGraph[currentNode.x, currentNode.y] = "+";

        List<Position> allowedNodes = new List<Position>();


        this.neighbouringNodes[0] = new Position(currentNode.x, currentNode.y + 1);
        this.neighbouringNodes[1] = new Position(currentNode.x + 1, currentNode.y);
        this.neighbouringNodes[2] = new Position(currentNode.x, currentNode.y - 1);
        this.neighbouringNodes[3] = new Position(currentNode.x - 1, currentNode.y);

        for (int i = 0; i < this.neighbouringNodes.Length; i++)
        {
            bool nextNodeVisited = this.visitedNodes[neighbouringNodes[i].x, neighbouringNodes[i].y];
            bool nextNodeBacktrackable = this.backtrackable[neighbouringNodes[i].x, neighbouringNodes[i].y];
            bool obstacleAtNeighbour = inputMap.mazeGraph[neighbouringNodes[i].x, neighbouringNodes[i].y] == "X";
            bool reachedEndNode = currentNode.x == endNode.x && currentNode.y == endNode.y;
            if (!nextNodeVisited && !obstacleAtNeighbour && !reachedEndNode && nextNodeBacktrackable)
            {
                allowedNodes.Add(neighbouringNodes[i]);
            }
        }

        if (allowedNodes.Count == 0)
        {
            this.backtrackable[currentNode.x, currentNode.y] = false;

            for (int i = 0; i < neighbouringNodes.Length; i++)
            {
                bool nextNodeBacktrackable = this.backtrackable[neighbouringNodes[i].x, neighbouringNodes[i].y];
                bool obstacleAtNeighbour = inputMap.mazeGraph[neighbouringNodes[i].x, neighbouringNodes[i].y] == "X";
                bool reachedEndNode = currentNode.x == endNode.x && currentNode.y == endNode.y;
                if (nextNodeBacktrackable && !obstacleAtNeighbour && !reachedEndNode)
                {
                    allowedNodes.Add(neighbouringNodes[i]);
                    break;
                }
            }
        }

        foreach (Position allowed in allowedNodes.OrderBy(x => Globals.randomGenerator.Next(allowedNodes.Count)))
        {
            Search(allowed, endNode, inputMap);
            allowedNodes.Clear();
            break;
        }




        if (currentNode.x == endNode.x && currentNode.y == endNode.y)
        {
            pathFollowed.RemoveAt(pathFollowed.Count - 1);
            pathFollowed.RemoveAt(0);
            Console.WriteLine("\nPath followed:\n");
            foreach (Position waypoint in pathFollowed)
            {
                inputMap.mazeGraph[waypoint.x, waypoint.y] = "+";
                Console.WriteLine($"{waypoint.x}, {waypoint.y}");
            }
            Console.WriteLine($"\nPath cost: {pathCost}\n");

            for (int i = inputMap.mazeGraph.GetLength(0); i > 0; i--)
            {
                for (int j = 0; j < inputMap.mazeGraph.GetLength(1); j++)
                {
                    if (inputMap.mazeGraph[j, i - 1] == "+")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write($" {inputMap.mazeGraph[j, i - 1]} ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }
}

public static class Globals
{
    public static int entityID = 0;
    public static Random randomGenerator = new Random();
    public static int tick = 0;
}

public class DTSE
{
    public List<SimulationModel> models = new List<SimulationModel>();
    public bool radarNeighbouringAircraft = false;
    public List<int> costs = new List<int>();
    public void Init()
    {
        Aircraft aircraft1 = new Aircraft(new Position(1, 1), new Position(8, 9));
        Aircraft aircraft2 = new Aircraft(new Position(6, 4), new Position(18, 11));
        List<Radar> maze1 = new List<Radar>()
        {
            new Radar(new Position(0, 0)),
            new Radar(new Position(1, 0)),
            new Radar(new Position(2, 0)),
            new Radar(new Position(3, 0)),
            new Radar(new Position(4, 0)),
            new Radar(new Position(5, 0)),
            new Radar(new Position(6, 0)),
            new Radar(new Position(7, 0)),
            new Radar(new Position(8, 0)),
            new Radar(new Position(9, 0)),
            new Radar(new Position(10, 0)),
            new Radar(new Position(11, 0)),

            new Radar(new Position(0, 1)),
            new Radar(new Position(11, 1)),

            new Radar(new Position(0, 2)),
            new Radar(new Position(1, 2)),
            new Radar(new Position(2, 2)),
            new Radar(new Position(4, 2)),
            new Radar(new Position(5, 2)),
            new Radar(new Position(6, 2)),
            new Radar(new Position(7, 2)),
            new Radar(new Position(8, 2)),
            new Radar(new Position(9, 2)),
            new Radar(new Position(10, 2)),
            new Radar(new Position(11, 2)),

            new Radar(new Position(2, 3)),
            new Radar(new Position(4, 3)),
            new Radar(new Position(6, 3)),
            new Radar(new Position(7, 3)),
            new Radar(new Position(8, 3)),

            new Radar(new Position(2, 4)),
            new Radar(new Position(4, 4)),
            new Radar(new Position(6, 4)),
            new Radar(new Position(8, 4)),

            new Radar(new Position(2, 5)),
            new Radar(new Position(4, 5)),
            new Radar(new Position(5, 5)),
            new Radar(new Position(6, 5)),
            new Radar(new Position(8, 5)),
            new Radar(new Position(9, 5)),
            new Radar(new Position(10, 5)),
            new Radar(new Position(11, 5)),

            new Radar(new Position(2, 6)),
            new Radar(new Position(11, 6)),

            new Radar(new Position(2, 7)),
            new Radar(new Position(4, 7)),
            new Radar(new Position(5, 7)),
            new Radar(new Position(6, 7)),
            new Radar(new Position(7, 7)),
            new Radar(new Position(8, 7)),
            new Radar(new Position(9, 7)),
            new Radar(new Position(10, 7)),
            new Radar(new Position(11, 7)),

            new Radar(new Position(2, 8)),
            new Radar(new Position(4, 8)),
            new Radar(new Position(5, 8)),
            new Radar(new Position(6, 8)),
            new Radar(new Position(7, 8)),
            new Radar(new Position(8, 8)),
            new Radar(new Position(9, 8)),

            new Radar(new Position(2, 9)),
            new Radar(new Position(9, 9)),

            new Radar(new Position(2, 10)),
            new Radar(new Position(3, 10)),
            new Radar(new Position(4, 10)),
            new Radar(new Position(5, 10)),
            new Radar(new Position(6, 10)),
            new Radar(new Position(7, 10)),
            new Radar(new Position(8, 10)),
            new Radar(new Position(9, 10))
        };

        List<Radar> maze2 = new List<Radar>();
        // Add radars to maze2 (expand to see all)
        {
            for (int i = 0; i < 20; i++)
            {
                maze2.Add(new Radar(new Position(i, 0)));
            }

            for (int i = 0; i < 20; i++)
            {
                maze2.Add(new Radar(new Position(i, 19)));
            }

            for (int i = 1; i < 19; i++)
            {
                maze2.Add(new Radar(new Position(0, i)));
            }

            for (int i = 1; i < 19; i++)
            {
                maze2.Add(new Radar(new Position(19, i)));
            }

            for (int i = 2; i < 18; i++)
            {
                maze2.Add(new Radar(new Position(i, 17)));
            }

            for (int i = 2; i < 16; i++)
            {
                if (i == 8 || i == 13)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(i, 15)));
                }
            }

            for (int i = 2; i < 19; i++)
            {
                if (i == 8 || i == 14)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(i, 3)));
                }
            }

            for (int i = 2; i < 19; i++)
            {
                if (i == 3 || i == 5 || i == 8 || i == 14)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(i, 2)));
                }
            }

            for (int i = 2; i < 18; i++)
            {
                if (i == 6 || i == 8 || i == 10 || i == 13 || i == 16)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(i, 12)));
                }
            }

            for (int i = 5; i < 15; i++)
            {
                if (i == 11)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(2, i)));
                }
            }

            for (int i = 5; i < 15; i++)
            {
                if (i == 6 || i == 9 || i == 11)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(5, i)));
                }
            }

            for (int i = 4; i < 14; i++)
            {
                if (i == 10)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(7, i)));
                }
            }

            for (int i = 5; i < 15; i++)
            {
                if (i == 10)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(9, i)));
                }
            }

            for (int i = 5; i < 15; i++)
            {
                if (i == 7 || i == 13)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(11, i)));
                }
            }

            for (int i = 6; i < 15; i++)
            {
                if (i == 7 || i == 13)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(12, i)));
                }
            }

            for (int i = 6; i < 15; i++)
            {
                if (i == 7 || i == 13)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(14, i)));
                }
            }

            for (int i = 6; i < 15; i++)
            {
                if (i == 11 || i == 12 || i == 13)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(16, i)));
                }
            }

            for (int i = 10; i < 17; i++)
            {
                if (i == 15)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(17, i)));
                }
            }

            for (int i = 4; i < 9; i++)
            {
                maze2.Add(new Radar(new Position(18, i)));
            }

            for (int i = 5; i < 13; i++)
            {
                if (i == 6 || i == 9 || i == 11)
                {
                    continue;
                }
                else
                {
                    maze2.Add(new Radar(new Position(4, i)));
                }
            }

            for (int i = 13; i < 17; i++)
            {
                maze2.Add(new Radar(new Position(i, 4)));
            }
        }

        {
            maze2.Add(new Radar(new Position(3, 5)));
            maze2.Add(new Radar(new Position(3, 10)));
            maze2.Add(new Radar(new Position(3, 12)));
            maze2.Add(new Radar(new Position(6, 7)));
            maze2.Add(new Radar(new Position(6, 8)));
            maze2.Add(new Radar(new Position(8, 11)));
            maze2.Add(new Radar(new Position(10, 14)));
            maze2.Add(new Radar(new Position(9, 16)));
            maze2.Add(new Radar(new Position(17, 16)));
            maze2.Add(new Radar(new Position(14, 15)));
            maze2.Add(new Radar(new Position(14, 12)));
            maze2.Add(new Radar(new Position(10, 5)));
            maze2.Add(new Radar(new Position(10, 9)));
            maze2.Add(new Radar(new Position(13, 6)));
            maze2.Add(new Radar(new Position(15, 6)));
        }

        // Added radars to maze2


        models.Add(aircraft2);
        foreach (Radar radar in maze2)
        {
            models.Add(radar);
        }
    }

    public void BuildGlobalSitaution()
    {
        foreach (SimulationModel model in models)
        {
            if (model is Aircraft)
            {
                foreach (SimulationModel model2 in models)
                {
                    if (model2 is Radar)
                    {
                        bool aircraftRadarOverlap = (Math.Abs(model2.pos.x - model.pos.x) <= 1) && (Math.Abs(model2.pos.y - model.pos.y) <= 1);
                        if (aircraftRadarOverlap)
                        {
                            ((Aircraft)model).radarInAdjacentCell = true;
                            break;
                        }
                        if (!aircraftRadarOverlap)
                        {
                            ((Aircraft)model).radarInAdjacentCell = false;
                        }
                    }
                }
            }
        }
    }

    public void RunSimulationEngine()
    {
        foreach (SimulationModel model in models)
        {
            model.Get();
        }

        BuildGlobalSitaution();

        foreach (SimulationModel model in models)
        {
            if (model is Aircraft)
            {
                List<InParameter> parameters = new List<InParameter>();
                foreach (SimulationModel model2 in models)
                {
                    if (model2 is Radar)
                    {
                        if (((Aircraft)model).radarInAdjacentCell)
                        {
                            Aircraft.In aircraftIn = new Aircraft.In(model2.pos.x, model2.pos.y, model2.id);
                            parameters.Add(aircraftIn);
                            model.Set(parameters);
                        }
                    }
                }
            }
        }

        foreach (SimulationModel model in models)
        {
            model.OnTick();
        }
    }
}

public class Position
{
    public int x;
    public int y;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class InParameter
{
    public int id;
    public InParameter(int id)
    {
        this.id = id;
    }
}

public class OutParameter
{
    public int id;
    public OutParameter(int id)
    {
        this.id = id;
    }
}

public abstract class SimulationModel
{
    public abstract int id { get; set; }
    public Position pos = new Position(-1, -1);
    public abstract OutParameter Get();
    public abstract void Set(List<InParameter> inParameters);
    public abstract void OnTick();
}

public abstract class BattleSystem : SimulationModel
{

}

// Add BattleSystem class, add Get and Set methods in SimulationModel

public class Aircraft : BattleSystem
{
    public override int id { get; set; }
    public Position startPoint;
    public Position endPoint;
    public Position selectedMove;
    public Position lastVisitedCell = new Position(0, 0);
    public List<Position> adjacentRadars = new List<Position>();
    public List<Position> waypointsFollowed = new List<Position>();
    public List<Position> possibleMoves = new List<Position>();
    public List<Position> allowedMoves = new List<Position>();
    public List<Position> unBacktrackable = new List<Position>();
    public List<Position> intersections = new List<Position>();
    public List<Position> foundPaths = new List<Position>();
    public bool radarInAdjacentCell = false;
    public bool foundDeadend;
    public bool reachedEndPoint = false;
    public int costFactor;

    public class Out : OutParameter
    {
        public int pos_x;
        public int pos_y;
        public Out(int x, int y, int id) : base(id)
        {
            this.pos_x = x;
            this.pos_y = y;
        }
    }

    public class In : InParameter
    {
        public int radar_x;
        public int radar_y;
        public In(int radar_x, int radar_y, int id) : base(id)
        {
            this.radar_x = radar_x;
            this.radar_y = radar_y;
        }
    }

    public Aircraft(Position pos, Position targetPos)
    {
        this.startPoint = pos;
        this.pos = pos;
        this.selectedMove = pos;
        this.endPoint = targetPos;
        this.id = Globals.entityID;
        Globals.entityID++;
    }

    public override OutParameter Get()
    {
        Out aircraftPos = new Out(this.pos.x, this.pos.y, this.id);
        return aircraftPos;
    }

    public override void Set(List<InParameter> inParameters)
    {
        this.radarInAdjacentCell = true;
        this.adjacentRadars.Clear();
        foreach (InParameter inParameter in  inParameters)
        {
            Position newRadarPos = new Position(0, 0);
            newRadarPos.x = ((Aircraft.In)inParameter).radar_x;
            newRadarPos.y = ((Aircraft.In)inParameter).radar_y;
            if (Math.Abs(this.pos.x - newRadarPos.x) <= 1 && Math.Abs(this.pos.y - newRadarPos.y) <= 1)
            {
                this.adjacentRadars.Add(newRadarPos);
            }
        }
    }

    public void RecursiveDFS(Position currentNode, Position previousNode)
    {
        List<Position> paths = new List<Position>();
        this.possibleMoves.Add(new Position(currentNode.x + 1, currentNode.y));
        this.possibleMoves.Add(new Position(currentNode.x - 1, currentNode.y));
        this.possibleMoves.Add(new Position(currentNode.x, currentNode.y + 1));
        this.possibleMoves.Add(new Position(currentNode.x, currentNode.y - 1));

        foreach (Position possibleMove in this.possibleMoves)
        {
            bool forbidden = false;
            if (possibleMove.x == previousNode.x && possibleMove.y == previousNode.y)
            {
                forbidden = true;
            }

            foreach (Position unback in this.unBacktrackable)
            {
                if (possibleMove.x == lastVisitedCell.x && possibleMove.y == lastVisitedCell.y)
                {
                    forbidden = true;
                }
            }

            foreach (Position radar in this.adjacentRadars)
            {
                if (possibleMove.x == radar.x && possibleMove.y == radar.y)
                {
                    forbidden = true;
                }
            }

            if (!forbidden)
            {
                this.allowedMoves.Add(possibleMove);
            }
        }

        bool startingInCorner = allowedMoves.Count == 1 && this.pos == this.startPoint;
        bool cellIsDeadEnd = allowedMoves.Count == 0 && this.pos != this.startPoint;

        if (allowedMoves.Count == 0)
        {
            previousNode = this.startPoint;
        }

        if (cellIsDeadEnd || startingInCorner)
        {
            this.unBacktrackable.Add(this.pos);
            foundDeadend = true;
        }

        if (allowedMoves.Count == 1 && foundDeadend)
        {
            this.unBacktrackable.Add(this.pos);
        }

        if (allowedMoves.Count >= 2 && foundDeadend)
        {
            foundDeadend = false;
        }

        if (!reachedEndPoint)
        {
            this.lastVisitedCell = currentNode;
            currentNode = allowedMoves[Globals.randomGenerator.Next(allowedMoves.Count)];
            RecursiveDFS(currentNode, this.lastVisitedCell);
        }
        else
        {
            return;
        }

        this.possibleMoves.Clear();
        this.allowedMoves.Clear();
    }

    public void DFS()
    {

        
        this.possibleMoves.Add(new Position(this.pos.x + 1, this.pos.y));
        this.possibleMoves.Add(new Position(this.pos.x - 1, this.pos.y));
        this.possibleMoves.Add(new Position(this.pos.x, this.pos.y + 1));
        this.possibleMoves.Add(new Position(this.pos.x, this.pos.y - 1));

        foreach (Position possibleMove in possibleMoves)
        {
            bool moveForbidden = false;

            if (possibleMove.x == this.lastVisitedCell.x && possibleMove.y == this.lastVisitedCell.y)
            {
                moveForbidden = true;
            }

            foreach (Position unback in this.unBacktrackable)
            {
                if (possibleMove.x == unback.x && possibleMove.y == unback.y)
                {
                    moveForbidden = true;
                }
            }

            foreach (Position radarPos in adjacentRadars)
            {
                if (radarPos.x == possibleMove.x && radarPos.y == possibleMove.y)
                {
                    moveForbidden = true;
                }
            }

            if (!moveForbidden)
            {
                allowedMoves.Add(possibleMove);
            }
        }

        bool startingInCorner = allowedMoves.Count == 1 && this.pos == this.startPoint;
        bool cellIsDeadEnd = allowedMoves.Count == 0 && this.pos != this.startPoint;

        if (allowedMoves.Count == 0)
        {
            this.lastVisitedCell = this.startPoint;
        }

        if (cellIsDeadEnd || startingInCorner)
        {
            this.unBacktrackable.Add(this.pos);
            foundDeadend = true;
        }

        if (allowedMoves.Count == 1 && foundDeadend)
        {
            this.unBacktrackable.Add(this.pos);
        }

        if (allowedMoves.Count >= 2 && foundDeadend)
        {
            foundDeadend = false;
        }

        if (allowedMoves.Count > 2)
        {
            this.intersections.Add(this.pos);
        }

        if (!reachedEndPoint)
        {
            this.lastVisitedCell = this.pos;
            if (allowedMoves.Count != 0)
            {
                //List<int> distances = new List<int>();

                if (allowedMoves.Count > 2)
                {
                    this.unBacktrackable.Add(allowedMoves[Globals.randomGenerator.Next(allowedMoves.Count)]);
                    //foreach (Position unback in this.unBacktrackable)
                    //{
                    //    foreach (Position allowed in this.allowedMoves.ToList())
                    //    {
                    //        if (unback.x == allowed.x && unback.y == allowed.y)
                    //        {
                    //            this.allowedMoves.Remove(allowed);
                    //        }
                    //    }
                    //}
                }

                //foreach (Position inter in this.intersections)
                //{
                //    if (this.pos.x == inter.x && this.pos.y == inter.y)
                //    {
                //        for (int i = 0; i < allowedMoves.Count; i++)
                //        {
                //            int x_dist = Math.Abs(this.endPoint.x - allowedMoves[i].x);
                //            int y_dist = Math.Abs(this.endPoint.y - allowedMoves[i].y);
                //            int manhattan_dist = x_dist + y_dist;
                //            distances.Add(manhattan_dist);
                //        }
                //        this.selectedMove = allowedMoves[distances.IndexOf(distances.Min())];
                //        return;
                //    }
                //}

                //foreach (Position inter in this.intersections)
                //{
                //    if (this.pos.x == inter.x && this.pos.y == inter.y)
                //    {
                //        this.unBacktrackable.Add(this.allowedMoves[Globals.randomGenerator.Next(this.allowedMoves.Count)]);
                //    }
                //}

                else
                {
                    this.selectedMove = allowedMoves[Globals.randomGenerator.Next(this.allowedMoves.Count)];
                }
                
            }
        }
    }

    public override void OnTick()
    {
        //Console.WriteLine($"Ticks : {Globals.tick}");
        DFS();
        this.pos = selectedMove;

        if (this.pos.x == this.endPoint.x && this.pos.y == this.endPoint.y)
        {
            reachedEndPoint = true;
        }

        this.waypointsFollowed.Add(this.pos);

        //Console.WriteLine($"\nLast visited cell: {this.lastVisitedCell.x}, {this.lastVisitedCell.y}");

        if (this.unBacktrackable != null)
        {
            //Console.WriteLine($"\nUnbacktrackable cells:\n");
            foreach (Position unback in this.unBacktrackable.ToList())
            {
                //Console.WriteLine($"{unback.x}, {unback.y}");
            }
        }

        //Console.WriteLine($"\nCurrent Aircraft position = {this.pos.x}, {this.pos.y}");
        //Console.WriteLine($"Target position = {this.endPoint.x}, {this.endPoint.y}\n");

        if (reachedEndPoint)
        {
            costFactor = Globals.tick;
            Console.WriteLine($"\nPath:\n");
            foreach (Position waypoints in this.waypointsFollowed)
            {
                Console.WriteLine($"{waypoints.x}, {waypoints.y}");
            }
            Console.WriteLine($"Path cost: {costFactor}");
        }
        else
        {
            Globals.tick++;
        }
        
        this.allowedMoves.Clear();
        this.possibleMoves.Clear();
    }
}



public class Radar : BattleSystem
{
    public override int id { get; set; }
    public class Out : OutParameter
    {
        public int pos_x;
        public int pos_y;
        public Out(int x, int y, int id) : base(id)
        {
            this.pos_x = x;
            this.pos_y = y;
        }
    }

    public override OutParameter Get()
    {
        Out radarPos = new Out(this.pos.x, this.pos.y, this.id);
        return radarPos;
    }

    public override void Set(List<InParameter> inParameters)
    {

    }
    public override void OnTick()
    {
        //Console.WriteLine($"Radar at {this.pos.x}, {this.pos.y}");
    }

    public Radar(Position pos)
    {
        this.pos = pos;
        this.id = Globals.entityID;
        Globals.entityID++;
    }
}