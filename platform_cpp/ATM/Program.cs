using System.Diagnostics;

enum Transaction { DEPOSIT, WITHDRAWAL };

namespace ATM
{
    public static class QueueExtension
    {

        public static void  Enqueue2(this Queue<QItem> qitemQ, QItem qval)
        {
           qitemQ.Enqueue(qval);
           Console.WriteLine(qval.Name + ";" + qval.Value + ";;");
           return; 
        }
    }

    public class QItem
    {
        public string Name = String.Empty;
        public string Value = String.Empty;

        public QItem(string n, string v)
        {
            Name = n; Value = v; 
        }
    }

    public abstract class AbstractSystem
    {
        abstract public bool done { get; set; }
        abstract public string get();
        abstract public void set(string s);
        abstract public void OnTick();

    }

    //To be designed
    class DTSE
    {
        public List<AbstractSystem> SystemList = new List<AbstractSystem>();
        public bool done = false;

        public void init()
        {
            done = false;
            foreach (AbstractSystem system in SystemList)
                system.done = false;
        }
        public void OnTick()
        {
            bool tempdone = true;
            foreach(AbstractSystem absys in SystemList)
            {
                //communication will happen on 
                //structured channels. So no get, set
                //required.
                tempdone &= absys.done;
                if (!absys.done)
                    absys.OnTick();
            }
            
            if (tempdone)
            {
                done = true; 
            }

        }
    }

    //Systems
    class BankATM:AbstractSystem
    {
        public override bool done { get; set; }
        public int Balance = 0;


        public Queue<QItem> requestQ = new Queue<QItem>();
        public Queue<QItem> responseQ = new Queue<QItem>();

        public BankATM(int Balance = 0)
        {
            this.Balance = Balance;
        }

        public ref Queue<QItem> getRequestQRef()
        {
            return ref requestQ;
        }

        public ref Queue<QItem> getResponseQRef()
        {
            return ref responseQ; ;
        }

        public override string get()
        {
            throw new NotImplementedException();
        }

        public override void set(string s)
        {
            throw new NotImplementedException();
        }

        public override void OnTick()
        {
            
        }
    }

    class ATMUser:AbstractSystem
    {
        public override bool done { get; set; }
        public Queue<QItem> requestQ = new Queue<QItem>();
        public Queue<QItem> responseQ = new Queue<QItem>();
        public int pocketBalance = 0;

        public ATMUser(int pocketBalance=0)
        {
            this.pocketBalance = pocketBalance;            
        }

        public ATMUser(BankATM atm)
        {
            requestQ = atm.getRequestQRef();
            responseQ = atm.getResponseQRef();
        }

        public override string get()
        {
            throw new NotImplementedException();
        }

        public override void set(string s)
        {
            throw new NotImplementedException();
        }

        public override void OnTick()
        {
            
        }
    }

    //Scenarios or Test Cases
    class Test01
    {
        public BankATM atm01 = new BankATM(1000);
        public ATMUser atmUser01 = new ATMUser(100);
        public DTSE dtse = new DTSE();
        public Test01()
        {
           
        }
        public void Run()
        {
            //Initialization of the result
            dtse.SystemList.Add(atm01);
            dtse.SystemList.Add(atmUser01);
            dtse.init();

            //atm01.doTransaction(atmUser01,Transaction.WITHDRAWAL, 100 );
            while (!dtse.done)
            {
                dtse.OnTick(); 
            }

            //Verification of the results
            Debug.Assert(atm01.Balance == 900);
            Debug.Assert(atmUser01.pocketBalance == 200);          
        }
    }   


    internal class Program
    {       

        static void Main(string[] args)
        {
            BankATM axis_atm = new BankATM();
            ATMUser ravij = new ATMUser(axis_atm);

            axis_atm.requestQ.Enqueue2(new QItem("INSERT", "CARD"));
            ravij.responseQ.Enqueue(new QItem("INSERT", "CARD"));
            axis_atm.requestQ.Enqueue(new QItem("PIN", ""));
            ravij.responseQ.Enqueue(new QItem("PIN", "1234"));
            axis_atm.requestQ.Enqueue(new QItem("AMOUNT", ""));
            ravij.responseQ.Enqueue(new QItem("AMOUNT", "3000"));
            axis_atm.requestQ.Enqueue(new QItem("GIVE", "CASH"));
            ravij.responseQ.Enqueue(new QItem("COLLECT", "CASH"));
            axis_atm.requestQ.Enqueue(new QItem("GIVE", "CARD"));
            ravij.responseQ.Enqueue(new QItem("COLLECT", "COLLECT"));



            Console.WriteLine("Hello, World!");
            Console.ReadKey();
        }
    }
}