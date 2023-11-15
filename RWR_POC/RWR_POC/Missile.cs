class Missile : BattleSystem
{
    public Position targetPosition;
    public bool reachedTarget;
    public override bool Stopped { get; set; }

    public class Out : OutParameter
    {
        public int Ox;
        public int Oy;
        public Out(int x, int y, int id) : base(id)
        {
            this.Ox = x;
            this.Oy = y;
        }
    }

    public class In : InParameter
    {

        public int posX;
        public int posY;
        public In(int posX, int posY, int id) : base(id)
        {
            this.posX = posX;
            this.posY = posY;
            this.ID = id;
        }
    }

    public override OutParameter Get()
    {
        Out missilePosition = new(this.position.x, this.position.y, 9);
        return missilePosition;
    }

    public override void Set(List<InParameter> inParameters)
    {

    }

    public override void OnTick()
    {
        Console.WriteLine($"Missile position: {this.position.x}, {this.position.y}");
        MoveMissile();
    }

    public static int[] ComputeDistance(Position pos1, Position pos2)
    {
        int[] dist = new int[] { (int)(pos2.x - pos1.x), (int)(pos2.y - pos1.y) };
        return dist;
    }

    public void MoveMissile()
    {
        int moveRatio;
        int[] displacementArray = ComputeDistance(this.position, this.targetPosition);
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

        if (this.position.x == targetPosition.x && this.position.y == targetPosition.y)
        {
            this.reachedTarget = true;
        }
    }

    public Missile(Position missilePosition, Position targetPosition)
    {
        Console.WriteLine($"Missile launched");
        this.position = missilePosition;
        this.targetPosition = targetPosition;
    }
}