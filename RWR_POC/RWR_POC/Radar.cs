class Radar : BattleSystem
{

    public override bool Stopped { get; set; }
    public Pulse txPulse;
    public Pulse activePulse;
    public Pulse zeroPulse = new Pulse(0, 0, 0, 0, "zero");
    public Pulse echoPulse;
    public int pulseRepetitionInterval;
    //public int targetX;
    //public int targetY;
    public int radius;
    public int txTick;
    public int rxTick;
    public bool hasReceivedEcho = false;
    public int echoTimeOfArrival;
    public string pulseSymbol;
    //public bool hasPulseReachedTarget = false;

    public class Out : OutParameter
    {
        public Pulse p;
        public Position pos;
        public int firstTxTick;
        public int txTick;
        public Out(Pulse p, Position pos, int tcTick, int id) : base(id)
        {
            this.pos = pos;
            this.p = p;
            this.txTick = tcTick;
        }
    }

    public class In : InParameter
    {
        public Pulse echoPulse;
        public In(Pulse echoPulse, int id) : base(id)
        {
            this.echoPulse = echoPulse;
        }
    }

    public override OutParameter Get()
    {
        if (txPulse.amplitude > 0)
        {
            Globals.debugPrint = true;
        }
        Out radarOutput = new Out(txPulse, position, txTick, 1);
        return radarOutput;
    }

    public override void OnTick()
    {
        if (Globals.Tick % this.pulseRepetitionInterval == 0)
        {
            txPulse = activePulse;
            txTick = Globals.Tick;
            //if (Globals.debugPrint)
            //{
            //    Console.WriteLine($"Pulse emitted by {this} {id}");
            //}
            Console.WriteLine($"Pulse emitted by {this} {id}\n");

            if (Globals.debugPrint)
            {
                Console.WriteLine($"Radar {id}:\n\ttxPulse:\n\t\tPulse width: {txPulse.pulseWidth}\n\t\tPRI: {pulseRepetitionInterval}" +
        $"\n\t\tTime of transmission: {txPulse.timeOfTraversal}\n\t\tAngle of transmission: {txPulse.angleOfTraversal}\n\t\tSymbol: {txPulse.symbol}" +
        $"\n\t\tAmplitude: {txPulse.amplitude}\n\t\ttxTick = {txTick}\n");
            }

            //Globals.debugPrint = true;
        }
        else
        {
            txPulse = zeroPulse;
            //Globals.debugPrint = false;
        }



        if (Globals.debugPrint)
        {
            if (hasReceivedEcho)
            {
                Console.WriteLine($"Radar {id}:\n\techoPulse:\n\t\tPulse width: {echoPulse.pulseWidth}\n\t\tPRI: {pulseRepetitionInterval}" +
                $"\n\t\tTime of arrival: {echoTimeOfArrival}\n\t\tAngle of arrival: {echoPulse.angleOfTraversal}\n\t\tSymbol: {echoPulse.symbol}" +
                $"\n\t\tAmplitude: {echoPulse.amplitude}\n\t\ttxTick = {txTick}\n");
            }
        }


    }

    public override void Set(List<InParameter> inParameters)
    {
        if (this.echoPulse.symbol == this.pulseSymbol)
        {
            this.echoPulse = ((In)(inParameters[0])).echoPulse;
        }
    }


    public Radar(Pulse initPulse, Position position, int pulseRepetitionInterval, string pulseSymbol, int txTick, int radius, int id)
    {
        this.txPulse = zeroPulse;
        this.activePulse = initPulse;
        this.echoPulse = activePulse;
        this.pulseSymbol = pulseSymbol;
        this.position = position;
        this.id = id;
        this.radius = radius;
        this.txTick = txTick;
        this.pulseRepetitionInterval = pulseRepetitionInterval;
    }
}