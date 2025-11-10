using System.Data;
using System.Security;

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

            //Maze maze = new Maze(new Position(6, 4), new Position(18, 11));
            //DFS dfs = new DFS(new Position(6, 4), new Position(18, 11), maze);
            //dfs.StartDFS(10);
            //dfs.BatchDFS(10, dfs.startPos, dfs.endPos, dfs.inputMap);

            //DTSE dtse = new DTSE();
            //dtse.Init();
            //while (Globals.simulationActive)
            //{
            //    dtse.RunSimulationEngine();
            //    Console.ReadLine();
            //}

            Maze maze = new Maze(new Position(0, 0), new Position(4, 4));
            QState QLearner = new QState(new Position(0, 0), new Position(4, 4), maze);
            QLearner.QLearning(QLearner.startPos);
            QLearner.followFinalPolicy(new Position(2, 2));
        }
    }
}

//public class Maze
//{
//    public string[,] mazeGraph = new string[20, 20];
//    public Maze(Position startPos, Position endPos)
//    {
//        for (int i = 0; i < mazeGraph.GetLength(0); i++)
//        {
//            for (int j = 0; j < mazeGraph.GetLength(1); j++)
//            {
//                this.mazeGraph[i, j] = "-";
//            }
//        }

//        for (int i = 0; i < 20; i++)
//        {
//            this.mazeGraph[i, 0] = ".";
//            this.mazeGraph[i, 19] = ".";
//        }

//        for (int i = 1; i < 19; i++)
//        {
//            this.mazeGraph[0, i] = ".";
//            this.mazeGraph[19, i] = ".";
//        }

//        for (int i = 2; i < 18; i++)
//        {
//            this.mazeGraph[i, 17] = ".";
//        }

//        for (int i = 2; i < 16; i++)
//        {
//            if (i == 8 || i == 13)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[i, 15] = ".";
//            }
//        }

//        for (int i = 2; i < 19; i++)
//        {
//            if (i == 8 || i == 14)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[i, 3] = ".";
//            }
//        }

//        for (int i = 2; i < 19; i++)
//        {
//            if (i == 3 || i == 5 || i == 8 || i == 14)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[i, 2] = ".";
//            }
//        }

//        for (int i = 2; i < 18; i++)
//        {
//            if (i == 6 || i == 8 || i == 10 || i == 13 || i == 16)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[i, 12] = ".";
//            }
//        }

//        for (int i = 5; i < 15; i++)
//        {
//            if (i == 11)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[2, i] = ".";
//            }
//        }

//        for (int i = 5; i < 15; i++)
//        {
//            if (i == 6 || i == 9 || i == 11)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[5, i] = ".";
//            }
//        }

//        for (int i = 4; i < 14; i++)
//        {
//            if (i == 10)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[7, i] = ".";
//            }
//        }

//        for (int i = 5; i < 15; i++)
//        {
//            if (i == 10)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[9, i] = ".";
//            }
//        }

//        for (int i = 5; i < 15; i++)
//        {
//            if (i == 7 || i == 13)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[11, i] = ".";
//            }
//        }

//        for (int i = 6; i < 15; i++)
//        {
//            if (i == 7 || i == 13)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[12, i] = ".";
//            }
//        }

//        for (int i = 6; i < 15; i++)
//        {
//            if (i == 7 || i == 13)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[14, i] = ".";
//            }
//        }

//        for (int i = 6; i < 15; i++)
//        {
//            if (i == 11 || i == 12 || i == 13)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[16, i] = ".";
//            }
//        }

//        for (int i = 10; i < 17; i++)
//        {
//            if (i == 15)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[17, i] = ".";
//            }
//        }

//        for (int i = 4; i < 9; i++)
//        {
//            this.mazeGraph[18, i] = ".";
//        }

//        for (int i = 5; i < 13; i++)
//        {
//            if (i == 6 || i == 9 || i == 11)
//            {
//                continue;
//            }
//            else
//            {
//                this.mazeGraph[4, i] = ".";
//            }
//        }

//        for (int i = 13; i < 17; i++)
//        {
//            this.mazeGraph[i, 4] = ".";
//        }

//        mazeGraph[3, 5] = ".";
//        mazeGraph[3, 10] = ".";
//        mazeGraph[3, 12] = ".";
//        mazeGraph[6, 7] = ".";
//        mazeGraph[6, 8] = ".";
//        mazeGraph[8, 11] = ".";
//        mazeGraph[10, 14] = ".";
//        mazeGraph[9, 16] = ".";
//        mazeGraph[17, 16] = ".";
//        mazeGraph[14, 15] = ".";
//        mazeGraph[14, 12] = ".";
//        mazeGraph[10, 5] = ".";
//        mazeGraph[10, 9] = ".";
//        mazeGraph[13, 6] = ".";
//        mazeGraph[15, 6] = ".";
//        mazeGraph[15, 14] = ".";

//        //mazeGraph[startPos.x, startPos.y] = "A";
//        //mazeGraph[endPos.x, endPos.y] = "G";

//        Console.WriteLine($"Input maze:\n");

//        for (int i = this.mazeGraph.GetLength(0); i > 0; i--)
//        {
//            for (int j = 0; j < this.mazeGraph.GetLength(1); j++)
//            {
//                if (mazeGraph[j, i - 1] == ".")
//                {
//                    Console.ForegroundColor = ConsoleColor.DarkYellow;
//                }
//                if (mazeGraph[j, i - 1] == "A")
//                {
//                    Console.ForegroundColor = ConsoleColor.Blue;
//                }
//                if (mazeGraph[j, i - 1] == "G")
//                {
//                    Console.ForegroundColor = ConsoleColor.Green;
//                }
//                if (mazeGraph[j, i - 1] == "-")
//                {
//                    Console.ForegroundColor = ConsoleColor.White;
//                }
//                Console.Write($" {mazeGraph[j, i - 1]} ");
//                Console.ResetColor();
//            }

//            Console.WriteLine();
//        }

//        Console.WriteLine();
//        Console.ReadLine();
//    }
//}

public class Maze
{
    public string[,] mazeGraph = new string[5, 5];

    public Maze(Position startPos, Position endPos)
    {

        for (int i = 0; i < mazeGraph.GetLength(0); i++)
        {
            for (int j = 0; j < mazeGraph.GetLength(1); j++)
            {
                this.mazeGraph[i, j] = "-";
            }
        }

        this.mazeGraph[1, 1] = ".";
        this.mazeGraph[2, 1] = ".";
        this.mazeGraph[3, 1] = ".";

        this.mazeGraph[1, 2] = ".";
        this.mazeGraph[3, 2] = ".";

        this.mazeGraph[3, 3] = ".";

        this.mazeGraph[1, 4] = ".";

        Console.WriteLine($"Input maze:\n");

        for (int j = 0; j < this.mazeGraph.GetLength(0); j++)
        {
            for (int i = 0; i < this.mazeGraph.GetLength(1); i++)
            {
                if (mazeGraph[i, j] == ".")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                if (mazeGraph[i, j] == "A")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                if (mazeGraph[i, j] == "G")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (mazeGraph[i, j] == "-")
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write($" {mazeGraph[i, j]} ");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.ReadLine();
    }
}

public class QState
{
    public Position startPos;
    public Position endPos;

    int[,] rewardTable = new int[5, 5];
    double[,][] QTable = new double[5, 5][];
    int[,] finalPolicy = new int[5, 5];

    // Initialize Q-table and Reward Value table
    public QState(Position startPos, Position endPos, Maze inputMaze)
    {
        this.startPos = startPos;
        this.endPos = endPos;

        for (int i = 0; i < inputMaze.mazeGraph.GetLength(0); i++)
        {
            for (int j = 0; j < inputMaze.mazeGraph.GetLength(1); j++)
            {
                QTable[i, j] = new double[4];
            }
        }

        for (int i = 0; i < inputMaze.mazeGraph.GetLength(0); i++)
        {
            for (int j = 0; j < inputMaze.mazeGraph.GetLength(1); j++)
            {
                if (inputMaze.mazeGraph[i, j] == "-")
                {
                    QTable[i, j][0] = 0;
                    QTable[i, j][1] = 0;
                    QTable[i, j][2] = 0;
                    QTable[i, j][3] = 0;
                }
            }
        }

        for (int i = 0; i < inputMaze.mazeGraph.GetLength(0); i++)
        {
            for (int j = 0; j < inputMaze.mazeGraph.GetLength(1); j++)
            {
                if (inputMaze.mazeGraph[i, j] == ".")
                {
                    rewardTable[i, j] = -9999;
                }
                else if (inputMaze.mazeGraph[i, j] == "-")
                {
                    if (endPos.x == i && endPos.y == j)
                    {
                        rewardTable[i, j] = 99;
                    }
                    else
                    {
                        rewardTable[i, j] = 0;
                    }
                }
            }
        }

        //for (int i = inputMaze.mazeGraph.GetLength(0); i > 0; i--)
        //{
        //    for (int j = 0; j < inputMaze.mazeGraph.GetLength(1); j++)
        //    {
        //        if (rewardTable[j, i - 1] > 0)
        //        {
        //            Console.Write($" {rewardTable[j, i - 1]} ".ToString());
        //        }
        //        else
        //        {
        //            Console.Write("  X  ");
        //        }
        //    }
        //    Console.WriteLine();
        //}

        for (int i = 1; i < inputMaze.mazeGraph.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < inputMaze.mazeGraph.GetLength(1) - 1; j++)
            {
                if (inputMaze.mazeGraph[i, j + 1] == ".")
                {
                    QTable[i, j][0] = -9999;
                }
                if (inputMaze.mazeGraph[i + 1, j] == ".")
                {
                    QTable[i, j][1] = -9999;
                }
                if (inputMaze.mazeGraph[i, j - 1] == ".")
                {
                    QTable[i, j][2] = -9999;
                }
                if (inputMaze.mazeGraph[i - 1, j] == ".")
                {
                    QTable[i, j][3] = -9999;
                }
            }
        }
    }

    public int ExploratoryPolicy(Position state)
    {
        return Globals.randomGenerator.Next(4);
    }

    public int TargetPolicy(double[,][] Qtable, Position state)
    {
        int selectedAction = 0;

        double epsilon = 0.2;

        //for (int i = 0; i < 5; i++)
        //{
        //    for (int j = 0; j < 5; j++)
        //    {
        //        if (state.x == i && state.y == j)
        //        {
        //            selectedAction = Array.IndexOf(Qtable[i, j], Qtable[i, j].Max());
        //        }
        //    }
        //}

        if (Globals.randomGenerator.NextDouble() > epsilon)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (state.x == i && state.y == j)
                    {
                        List<int> maxActions = new List<int>();
                        for (int k = 0; k < 4; k++)
                        {
                            if (QTable[i, j][k] == QTable[i, j].Max())
                            {
                                maxActions.Add(k);
                            }
                        }

                        maxActions = maxActions.OrderBy(x => Globals.randomGenerator.Next(maxActions.Count)).ToList();
                        selectedAction = maxActions[0];
                        break;
                    }
                }
            }
            return selectedAction;
        }

        else
        {
            return Globals.randomGenerator.Next(4);
        }
    }

    public Position Step(Position currentState, int action)
    {
        Position nextState = new Position(currentState.x, currentState.y);
        if (action == 0 && currentState.y != this.rewardTable.GetLength(1) - 1)
        {
            if (rewardTable[currentState.x, currentState.y + 1] != -9999)
            {
                nextState.y = currentState.y + 1;
            }
        }
        else if (action == 1 && currentState.x != this.rewardTable.GetLength(0) - 1)
        {
            if (rewardTable[currentState.x + 1, currentState.y] != -9999)
            {
                nextState.x = currentState.x + 1;
            }
        }
        else if (action == 2 && currentState.y != 0)
        {
            if (rewardTable[currentState.x, currentState.y - 1] != -9999)
            {
                nextState.y = currentState.y - 1;
            }
        }
        else if (action == 3 && currentState.x != 0)
        {
            if (rewardTable[currentState.x - 1, currentState.y] != -9999)
            {
                nextState.x = currentState.x - 1;
            }
        }
        return nextState;
    }

    public void QLearning(Position currentState, int episodes = 10000, double alpha = 0.1, double gamma = 0.99)
    {
        bool done = false;
        for (int i = 0; i < episodes; i++)
        {
            currentState = startPos;
            done = false;
            int a = 0;
            while (!done)
            {
                int action = ExploratoryPolicy(currentState);
                Position nextState = Step(currentState, action);
                int nextAction = TargetPolicy(QTable, nextState);
                int reward = rewardTable[nextState.x, nextState.y];

                double qsa = QTable[currentState.x, currentState.y][action];
                double next_qsa = QTable[nextState.x, nextState.y][nextAction];
                QTable[currentState.x, currentState.y][action] = qsa + alpha * (reward + gamma * next_qsa - qsa);

                //rewardTable[currentState.x, currentState.y] = -1;

                if (currentState.x == this.endPos.x && currentState.y == this.endPos.y)
                {
                    done = true;
                    break;
                }
                a++;
                currentState = nextState;
                //Console.WriteLine($"{currentState.x}, {currentState.y}");
            }
        }

        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                double[] QValues = new double[4];
                int maxQSA = 0;
                for (int k = 0; k < 4; k++)
                {
                    QValues[k] = QTable[i, j][k];
                    maxQSA = Array.IndexOf(QValues, QValues.Max());
                }
                Console.Write($" {maxQSA} ");
                finalPolicy[i, j] = maxQSA;
            }
            Console.WriteLine();
        }
    }

    public void followFinalPolicy(Position startPos)
    {
        bool reachedEndNode = false;
        Position currentPos = startPos;
        Position[] neighbours = new Position[4];

        while (!reachedEndNode)
        {
            if (currentPos.x == this.endPos.x && currentPos.y == this.endPos.y)
            {
                reachedEndNode = true; break;
            }
            neighbours[0] = new Position(currentPos.x, currentPos.y + 1);
            neighbours[1] = new Position(currentPos.x + 1, currentPos.y);
            neighbours[2] = new Position(currentPos.x, currentPos.y - 1);
            neighbours[3] = new Position(currentPos.x - 1, currentPos.y);
            Console.WriteLine($"Position before action: {currentPos.x}, {currentPos.y}");
            int action = finalPolicy[currentPos.x, currentPos.y];
            currentPos = neighbours[action];
            Console.WriteLine($"Position after action: {currentPos.x}, {currentPos.y}");
            Console.ReadLine();
        }
    }
}

public class DFS
{
    public Position initState;
    public Position startPos;
    public Position endPos;
    public Maze inputMap;
    public bool[,] visitedNodes;
    public double[,] rewards;
    public double[,][] QTable = new double[20, 20][];
    public Position[] neighbouringNodes;
    public List<Position> waypointsFollowed = new List<Position>();
    public int pathCost;
    public bool reachedEndNode = false;

    public int episodeCount = 0;
    public DFS(Position startPos, Position endPos, Maze inputMap)
    {
        this.initState = startPos;
        this.startPos = startPos;
        this.endPos = endPos;
        this.inputMap = inputMap;
        this.visitedNodes = new bool[inputMap.mazeGraph.GetLength(0), inputMap.mazeGraph.GetLength(1)];
        this.rewards = new double[inputMap.mazeGraph.GetLength(0), inputMap.mazeGraph.GetLength(1)];

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                QTable[i, j] = new double[4];
            }
        }

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (inputMap.mazeGraph[i, j] == "-")
                {
                    QTable[i, j][0] = 0;
                    QTable[i, j][1] = 0;
                    QTable[i, j][2] = 0;
                    QTable[i, j][3] = 0;
                }
            }
        }


        for (int i = 0; i < this.visitedNodes.GetLength(0); i++)
        {
            for (int j = 0; j < this.visitedNodes.GetLength(1); j++)
            {
                this.visitedNodes[i, j] = false;
            }
        }

        for (int i = 0; i < this.rewards.GetLength(0); i++)
        {
            for (int j = 0; j < this.rewards.GetLength(1); j++)
            {
                if (this.inputMap.mazeGraph[i, j] == ".")
                {
                    this.rewards[i, j] = -99;
                }
                else if (i == this.endPos.x && j == this.endPos.y)
                {
                    this.rewards[i, j] = 99;
                }
                else
                {
                    this.rewards[i, j] = 99 - (Math.Abs(this.endPos.x - i) + Math.Abs(this.endPos.y - j));
                }
            }
        }

        this.neighbouringNodes = new Position[4];
        this.pathCost = 0;
    }

    public void StartDFS(int episodes)
    {
        List<Position> tempPaths = new List<Position>();

        while (episodeCount < episodes)
        {
            startPos = initState;
            reachedEndNode = false;
            Search(startPos, endPos, inputMap, new List<Position>(tempPaths));
            episodeCount++;
            PrintPath(waypointsFollowed);
        }
        //int turns = CountTurns(waypointsFollowed);
        //Console.WriteLine($"\nTurns = {turns}");
    }

    public void CalculateRewards(Position currentNode, Position endNode, Maze inputMap, int nodeCost)
    {

    }

    public void Search(Position currentNode, Position endNode, Maze inputMap, List<Position> pathFollowed)
    {
        List<Position> allowedNodes = new List<Position>();
        double[] nodeRewards = new double[4];
        double Qvalue;

        if (!reachedEndNode)
        {
            reachedEndNode = currentNode.x == endNode.x && currentNode.y == endNode.y;
            this.visitedNodes[currentNode.x, currentNode.y] = true;
            //rewards[currentNode.x, currentNode.y] = -99;
            if (!reachedEndNode)
            {
                pathFollowed.Add(currentNode);
            }
            else
            {
                this.waypointsFollowed = pathFollowed;
            }
        }

        if (reachedEndNode)
        {
            return;
        }

        this.neighbouringNodes[0] = new Position(currentNode.x, currentNode.y + 1);
        this.neighbouringNodes[1] = new Position(currentNode.x + 1, currentNode.y);
        this.neighbouringNodes[2] = new Position(currentNode.x, currentNode.y - 1);
        this.neighbouringNodes[3] = new Position(currentNode.x - 1, currentNode.y);

        nodeRewards[0] = rewards[this.neighbouringNodes[0].x, this.neighbouringNodes[0].y];
        nodeRewards[1] = rewards[this.neighbouringNodes[1].x, this.neighbouringNodes[1].y];
        nodeRewards[2] = rewards[this.neighbouringNodes[2].x, this.neighbouringNodes[2].y];
        nodeRewards[3] = rewards[this.neighbouringNodes[3].x, this.neighbouringNodes[3].y];

        int bestNode = Array.IndexOf(nodeRewards, nodeRewards.Max());
        List<int> maxIndices = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            if (nodeRewards[i] == nodeRewards.Max())
            {
                maxIndices.Add(i);
            }
        }

        foreach (Position node in this.neighbouringNodes.OrderBy(x => Globals.randomGenerator.Next(4)))
        {
            bool nextNodeVisited = this.visitedNodes[node.x, node.y];
            bool obstacleAtNeighbour = inputMap.mazeGraph[node.x, node.y] == ".";

            if (!nextNodeVisited && !obstacleAtNeighbour)
            {
                Position allowed = new Position(node.x, node.y);
                int allowedIndex = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (neighbouringNodes[i].x == allowed.x && neighbouringNodes[i].y == allowed.y)
                    {
                        foreach (int index in maxIndices.OrderBy(x => Globals.randomGenerator.Next(maxIndices.Count())))
                        {
                            bestNode = index;
                        }
                        allowedIndex = bestNode;
                    }
                }

                Qvalue = QTable[currentNode.x, currentNode.y][allowedIndex];
                double reward = this.rewards[allowed.x, allowed.y];

                double nextQvalue = QTable[allowed.x, allowed.y][allowedIndex];
                QTable[currentNode.x, currentNode.y][allowedIndex] = Qvalue + 0.1 * (reward + 0.99 * nextQvalue - Qvalue);

                Search(allowed, endNode, inputMap, new List<Position>(pathFollowed));
            }
        }

        //foreach (Position node in this.neighbouringNodes.OrderBy(x => Globals.randomGenerator.Next(4)))
        //{
        //    bool nextNodeVisited = this.visitedNodes[node.x, node.y];
        //    bool obstacleAtNeighbour = inputMap.mazeGraph[node.x, node.y] == "X";

        //    if (!nextNodeVisited && !obstacleAtNeighbour)
        //    {
        //        Position allowed = new Position(node.x, node.y);
        //        int allowedIndex = 0;

        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (neighbouringNodes[i].x == allowed.x && neighbouringNodes[i].y == allowed.y)
        //            {
        //                allowedIndex = i;
        //            }
        //        }

        //        Qvalue = QTable[currentNode.x, currentNode.y][allowedIndex];
        //        double reward = this.rewards[allowed.x, allowed.y];

        //        double nextQvalue = QTable[allowed.x, allowed.y][Array.IndexOf(QTable[allowed.x, allowed.y], QTable[allowed.x, allowed.y].Max())];
        //        QTable[currentNode.x, currentNode.y][allowedIndex] = Qvalue + 0.1 * (reward + 0.99 * nextQvalue - Qvalue);

        //        Search(allowed, endNode, inputMap, new List<Position>(pathFollowed));
        //    }
        //}

        //nodeRewards[0] = rewards[currentNode.x, currentNode.y + 1];
        //nodeRewards[1] = rewards[currentNode.x + 1, currentNode.y];
        //nodeRewards[2] = rewards[currentNode.x, currentNode.y - 1];
        //nodeRewards[3] = rewards[currentNode.x - 1, currentNode.y];

        //int bestNode = Array.IndexOf(nodeRewards, nodeRewards.Max());
        //List<int> maxIndices = new List<int>();

        //for (int i = 0; i < 4; i++)
        //{
        //    if (nodeRewards[i] == nodeRewards.Max())
        //    {
        //        maxIndices.Add(i);
        //    }
        //}

        //foreach (int index in maxIndices.OrderBy(x => Globals.randomGenerator.Next(maxIndices.Count())))
        //{
        //    bestNode = index;
        //    Position nextNode;
        //    if (Globals.randomGenerator.NextDouble() < 0.33)
        //    {
        //        nextNode = neighbouringNodes[Globals.randomGenerator.Next(neighbouringNodes.Count())];
        //    }
        //    else
        //    {
        //        nextNode = neighbouringNodes[bestNode];
        //    }
        //    Qvalue = QTable[currentNode.x, currentNode.y][bestNode];
        //    double reward = nodeRewards[bestNode];

        //    double nextQvalue = QTable[nextNode.x, nextNode.y][Array.IndexOf(QTable[nextNode.x, nextNode.y], QTable[nextNode.x, nextNode.y].Max())];
        //    QTable[currentNode.x, currentNode.y][bestNode] = Qvalue + 0.1 * (reward + 0.99 * nextQvalue - Qvalue);
        //    rewards[nextNode.x, nextNode.y] = QTable[currentNode.x, currentNode.y][bestNode];
        //    Search(this.neighbouringNodes[bestNode], endNode, inputMap, new List<Position>(pathFollowed));
        //}
    }

    public int CountTurns(List<Position> finalPath)
    {
        int turns = 0;
        string currentDirection = "";

        for (int i = 1; i < finalPath.Count - 1; i++)
        {
            int xDiff = finalPath[i].x - finalPath[i - 1].x;
            int yDiff = finalPath[i].y - finalPath[i - 1].y;

            int xDiffNew = finalPath[i + 1].x - finalPath[i].x;
            int yDiffNew = finalPath[i + 1].y - finalPath[i].y;

            if (xDiff != 0 && yDiff == 0)
            {
                currentDirection = "X";
            }
            else if (yDiff != 0 && xDiff == 0)
            {
                currentDirection = "Y";
            }

            if (currentDirection == "Y" && xDiffNew != 0 && yDiffNew == 0)
            {
                turns++;
                currentDirection = "X";
            }

            else if (currentDirection == "X" & yDiffNew != 0 && xDiffNew == 0)
            {
                turns++;
                currentDirection = "Y";
            }
        }
        return turns;
    }

    public void PrintPath(List<Position> path)
    {
        //this.waypointsFollowed = path;
        Console.WriteLine("\nPath followed:\n");
        foreach (Position waypoint in path)
        {
            if (waypoint != startPos)
            {
                inputMap.mazeGraph[waypoint.x, waypoint.y] = "+";
            }
            else if (waypoint == startPos)
            {
                inputMap.mazeGraph[waypoint.x, waypoint.y] = "A";
            }
            else if (waypoint == endPos)
            {
                inputMap.mazeGraph[waypoint.x, waypoint.y] = "G";
            }
            Console.WriteLine($"{waypoint.x}, {waypoint.y}");
        }
        Console.WriteLine($"\nPath cost: {path.Count}\n");

        for (int i = inputMap.mazeGraph.GetLength(0); i > 0; i--)
        {
            for (int j = 0; j < inputMap.mazeGraph.GetLength(1); j++)
            {
                if (inputMap.mazeGraph[j, i - 1] == ".")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                if (inputMap.mazeGraph[j, i - 1] == "A")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                if (inputMap.mazeGraph[j, i - 1] == "G")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (inputMap.mazeGraph[j, i - 1] == "-")
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (inputMap.mazeGraph[j, i - 1] == "+")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write($" {inputMap.mazeGraph[j, i - 1]} ");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        Console.WriteLine($"\nTurns = {CountTurns(path)}\n\n");
    }
}

public static class Globals
{
    public static int entityID = 0;
    public static Random randomGenerator = new Random();
    public static int tick = 0;
    public static bool simulationActive = true;
}

public class DTSE
{
    public List<SimulationModel> models = new List<SimulationModel>();
    public bool radarNeighbouringAircraft = false;
    public List<int> costs = new List<int>();
    public void Init()
    {
        //Aircraft aircraft1 = new Aircraft(new Position(1, 1), new Position(8, 9));
        Maze maze = new Maze(new Position(6, 4), new Position(18, 11));

        List<Position> tempPaths = new List<Position>();

        DFS dfs = new DFS(new Position(6, 4), new Position(18, 11), maze);
        dfs.StartDFS(1000);
        Aircraft aircraft = new Aircraft(dfs.waypointsFollowed, 0);

        dfs.PrintPath(dfs.waypointsFollowed);


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


        models.Add(aircraft);
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
                        bool aircraftRadarOverlap = (Math.Abs(model2.position.x - model.position.x) <= 1) && (Math.Abs(model2.position.y - model.position.y) <= 1);
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
                            Aircraft.In aircraftIn = new Aircraft.In(model2.position.x, model2.position.y, model2.id);
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

    public override string ToString()
    { return x.ToString() + "," + y.ToString(); }
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
    public Position position = new Position(-1, -1);
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

    List<Position> waypoints { get; set; }
    public Position currentWaypoint;
    public Position nextWaypoint;
    public int currentWaypointID;
    public int nextWaypointID;


    //public Position startPoint;
    //public Position endPoint;
    //public Position selectedMove;
    //public Position lastVisitedCell = new Position(0, 0);
    public List<Position> adjacentRadars = new List<Position>();
    //public List<Position> waypointsFollowed = new List<Position>();
    //public List<Position> possibleMoves = new List<Position>();
    //public List<Position> allowedMoves = new List<Position>();
    //public List<Position> unBacktrackable = new List<Position>();
    //public List<Position> intersections = new List<Position>();
    //public List<Position> foundPaths = new List<Position>();
    public bool radarInAdjacentCell = false;
    //public bool foundDeadend;
    //public bool reachedEndPoint = false;
    //public int costFactor;

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

    public Aircraft(List<Position> waypoints, int id)
    {
        this.position = waypoints.ToList()[0];
        this.waypoints = waypoints;
        this.currentWaypoint = waypoints[0];
        this.nextWaypoint = waypoints[1];
        this.currentWaypointID = 0;
        this.nextWaypointID = 1;
        this.id = Globals.entityID;
        Globals.entityID++;
    }

    public override OutParameter Get()
    {
        Out aircraftPos = new Out(this.position.x, this.position.y, this.id);
        return aircraftPos;
    }

    public override void Set(List<InParameter> inParameters)
    {
        this.radarInAdjacentCell = true;
        this.adjacentRadars.Clear();
        foreach (InParameter inParameter in inParameters)
        {
            Position newRadarPos = new Position(0, 0);
            newRadarPos.x = ((Aircraft.In)inParameter).radar_x;
            newRadarPos.y = ((Aircraft.In)inParameter).radar_y;
            if (Math.Abs(this.position.x - newRadarPos.x) <= 1 && Math.Abs(this.position.y - newRadarPos.y) <= 1)
            {
                this.adjacentRadars.Add(newRadarPos);
            }
        }
    }

    //public void DFS()
    //{
    //    this.possibleMoves.Add(new Position(this.pos.x + 1, this.pos.y));
    //    this.possibleMoves.Add(new Position(this.pos.x - 1, this.pos.y));
    //    this.possibleMoves.Add(new Position(this.pos.x, this.pos.y + 1));
    //    this.possibleMoves.Add(new Position(this.pos.x, this.pos.y - 1));

    //    foreach (Position possibleMove in possibleMoves)
    //    {
    //        bool moveForbidden = false;

    //        if (possibleMove.x == this.lastVisitedCell.x && possibleMove.y == this.lastVisitedCell.y)
    //        {
    //            moveForbidden = true;
    //        }

    //        foreach (Position unback in this.unBacktrackable)
    //        {
    //            if (possibleMove.x == unback.x && possibleMove.y == unback.y)
    //            {
    //                moveForbidden = true;
    //            }
    //        }

    //        foreach (Position radarPos in adjacentRadars)
    //        {
    //            if (radarPos.x == possibleMove.x && radarPos.y == possibleMove.y)
    //            {
    //                moveForbidden = true;
    //            }
    //        }

    //        if (!moveForbidden)
    //        {
    //            allowedMoves.Add(possibleMove);
    //        }
    //    }

    //    bool startingInCorner = allowedMoves.Count == 1 && this.pos == this.startPoint;
    //    bool cellIsDeadEnd = allowedMoves.Count == 0 && this.pos != this.startPoint;

    //    if (allowedMoves.Count == 0)
    //    {
    //        this.lastVisitedCell = this.startPoint;
    //    }

    //    if (cellIsDeadEnd || startingInCorner)
    //    {
    //        this.unBacktrackable.Add(this.pos);
    //        foundDeadend = true;
    //    }

    //    if (allowedMoves.Count == 1 && foundDeadend)
    //    {
    //        this.unBacktrackable.Add(this.pos);
    //    }

    //    if (allowedMoves.Count >= 2 && foundDeadend)
    //    {
    //        foundDeadend = false;
    //    }

    //    if (allowedMoves.Count > 2)
    //    {
    //        this.intersections.Add(this.pos);
    //    }

    //    if (!reachedEndPoint)
    //    {
    //        this.lastVisitedCell = this.pos;
    //        if (allowedMoves.Count != 0)
    //        {
    //            //List<int> distances = new List<int>();

    //            if (allowedMoves.Count > 2)
    //            {
    //                this.unBacktrackable.Add(allowedMoves[Globals.randomGenerator.Next(allowedMoves.Count)]);
    //                //foreach (Position unback in this.unBacktrackable)
    //                //{
    //                //    foreach (Position allowed in this.allowedMoves.ToList())
    //                //    {
    //                //        if (unback.x == allowed.x && unback.y == allowed.y)
    //                //        {
    //                //            this.allowedMoves.Remove(allowed);
    //                //        }
    //                //    }
    //                //}
    //            }

    //            //foreach (Position inter in this.intersections)
    //            //{
    //            //    if (this.pos.x == inter.x && this.pos.y == inter.y)
    //            //    {
    //            //        for (int i = 0; i < allowedMoves.Count; i++)
    //            //        {
    //            //            int x_dist = Math.Abs(this.endPoint.x - allowedMoves[i].x);
    //            //            int y_dist = Math.Abs(this.endPoint.y - allowedMoves[i].y);
    //            //            int manhattan_dist = x_dist + y_dist;
    //            //            distances.Add(manhattan_dist);
    //            //        }
    //            //        this.selectedMove = allowedMoves[distances.IndexOf(distances.Min())];
    //            //        return;
    //            //    }
    //            //}

    //            //foreach (Position inter in this.intersections)
    //            //{
    //            //    if (this.pos.x == inter.x && this.pos.y == inter.y)
    //            //    {
    //            //        this.unBacktrackable.Add(this.allowedMoves[Globals.randomGenerator.Next(this.allowedMoves.Count)]);
    //            //    }
    //            //}

    //            else
    //            {
    //                this.selectedMove = allowedMoves[Globals.randomGenerator.Next(this.allowedMoves.Count)];
    //            }

    //        }
    //    }
    //}

    public static int[] ComputeDistance(Position pos1, Position pos2)
    {
        int[] dist = new int[] { (int)(pos2.x - pos1.x), (int)(pos2.y - pos1.y) };
        return dist;
    }

    void MoveAircraft()
    {
        int moveRatio;
        int[] displacementArray = ComputeDistance(this.currentWaypoint, this.nextWaypoint);
        int[] distanceToNextWaypoint = new int[2];
        distanceToNextWaypoint[0] = Math.Abs(displacementArray[0]);
        distanceToNextWaypoint[1] = Math.Abs(displacementArray[1]);

        bool minIsZero = distanceToNextWaypoint.Min() == 0;

        // Case: neither of the displacements are zero

        if (!minIsZero)
        {
            moveRatio = (int)(distanceToNextWaypoint.Max() / distanceToNextWaypoint.Min());

            // Case 1: Both x-displacment and y-displacement are positive

            if (displacementArray[0] > 0 && displacementArray[1] > 0)
            {

                // If x_distance > y-distance

                if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                {
                    // x = distanceToNextWaypoint[0] * moveRatio
                    this.position.x += moveRatio;
                    this.position.y += 1;
                }

                // If y-distance > x-distance

                else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                {
                    this.position.x += 1;
                    this.position.y += moveRatio;
                }

                // If x-distance = y-distance

                else
                {
                    this.position.x += 1;
                    this.position.y += 1;
                }
            }

            // Case 2: Both x-displacement and y-displacement are negative

            if (displacementArray[0] < 0 && displacementArray[1] < 0)
            {

                // If x-distance > y-distance

                if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                {
                    this.position.x += -moveRatio;
                    this.position.y += -1;
                }

                // If y-distance > x-distance

                else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                {
                    this.position.x += -1;
                    this.position.y += -moveRatio;
                }

                // If x-distance = y-distance

                else
                {
                    this.position.x += -1;
                    this.position.y += -1;
                }
            }

            // Case 3: x-displacement is positive, y-displacement is negative

            if (displacementArray[0] > 0 && displacementArray[1] < 0)
            {

                // If x-distance > y-distance

                if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                {
                    this.position.x += moveRatio;
                    this.position.y += -1;
                }

                // If y-distance > x-distance

                else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                {
                    this.position.x += 1;
                    this.position.y += -moveRatio;
                }

                // If x-distance = y-distance

                else
                {
                    this.position.x += 1;
                    this.position.y += -1;
                }
            }

            // Case 4: x-displacement is negative, y-displacement is positive

            if (displacementArray[0] < 0 && displacementArray[1] > 0)
            {

                // If x-distance > y-distance

                if (distanceToNextWaypoint[0] > distanceToNextWaypoint[1])
                {
                    this.position.x += -moveRatio;
                    this.position.y += 1;
                }

                // If y-distance > x-distance

                else if (distanceToNextWaypoint[1] > distanceToNextWaypoint[0])
                {
                    this.position.x += -1;
                    this.position.y += moveRatio;
                }

                // If x-distance = y-distance

                else
                {
                    this.position.x += -1;
                    this.position.y += 1;
                }
            }
        }

        // Case: At least one of the displacements is zero

        else
        {
            // Case 1: If x-displacement is zero

            if (displacementArray[0] == 0)
            {

                // If y-displacement is positive

                if (displacementArray[1] > 0)
                {
                    this.position.y += 1;
                }

                // If y-displacement is negative

                else if (displacementArray[1] < 0)
                {
                    this.position.y += -1;
                }
            }

            // Case 2: If y-displacement is zero

            if (displacementArray[1] == 0)
            {

                // If x-displacement is positive

                if (displacementArray[0] > 0)
                {
                    this.position.x += 1;
                }

                // If x-displacement is negative

                else if (displacementArray[0] < 0)
                {
                    this.position.x += -1;
                }
            }
        }

        Console.WriteLine($"Aircraft position after moving: {this.position.x}, {this.position.y}\n");

        if (this.position.x == nextWaypoint.x && this.position.y == nextWaypoint.y)
        {
            if (nextWaypointID < waypoints.Count - 1)
            {
                nextWaypointID++;
                nextWaypoint = waypoints[nextWaypointID];
            }
            else
            {
                Globals.simulationActive = false;
            }
        }
    }

    public override void OnTick()
    {
        MoveAircraft();
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
        Out radarPos = new Out(this.position.x, this.position.y, this.id);
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
        this.position = pos;
        this.id = Globals.entityID;
        Globals.entityID++;
    }
}