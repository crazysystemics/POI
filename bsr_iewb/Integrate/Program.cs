//C# program to illustrate the 
//Integration of different subsystems of BSR
using System;
namespace project
{
    // Class Declaration 
    public class TargetAttributes
    {
        //Instance Variables
        double SigAmp;
        double ToTxn;
        double ToRxn;

        //Constructor Declaration of Class
        public TargetAttributes (double SigAmp, double ToTxn, double ToRxn)
        {
            this.SigAmp = SigAmp;
            this.ToTxn = ToTxn;
            this.ToRxn = ToRxn;
        }

        //method 1 
		public double getSignalAmplitude()
        {
            return SigAmp;
        }

        // method 2 
        public double getTimeofTransmission()
        {
            return ToTxn;
        }

        // method 3 
        public double getTimeofReception()
        {
            return ToRxn;
        }

        public string threedouble()
        {
            return ("\nTarget Attributes: \nThe Signal Amplitude of the Target is " + this.getSignalAmplitude()
                    + ".\nThe Time of Transmission of the Target is " + this.getTimeofTransmission()
                    + ".\nThe Time of Reception of the Target is " + this.getTimeofReception()
                    + ".");
        }
    }

    public class Target
    {
        // Instance Variables 
        public double td;
        public double ta;

        // Constructor Declaration of Class 
        public Target (double td, double ta)
        {
            this.td = td;
            this.ta = ta;
        }

        // method 1
        public double getTargetDistance()
        {
            return td;
        }

        // method 2
        public double getTargetAzimuth()
        {
            return ta;
        }

        public string todouble()
        {
            return ("\nAt the Target: \nThe Target Distance is " + this.getTargetDistance() + ".\nThe Target Azimuth is " + this.getTargetAzimuth() + ".");
        }
    }

    public class Antenna : Target
    {
        public double atd; public double ata;
        public Antenna (double td, double ta) : base (td, ta)
        {
            atd = td * 1000;
            ata = ta * 1000;
            Console.WriteLine("At the Antenna:");
            Console.WriteLine("The Target Distance is " + atd + " metres.");
            Console.WriteLine("The Target Azimuth is " + ata + " metres.");
        }
    }

    public class RSP : Antenna
    {
        public double rstd; public double rsta;
        public RSP (double td, double ta) : base (td, ta)
        {
            rstd = td / 1000;
            rsta = ta / 1000;
            Console.WriteLine("\nAt the RSP:");
            Console.WriteLine("The Target Distance is " + rstd + " kilometres.");
            Console.WriteLine("The Target Azimuth is " + rsta + " kilometres.");
        }
    }

    public class RDP : RSP
    {
        public double rdtd; public double rdta;
        public RDP (double td, double ta) : base (td, ta)
        {
            rdtd = td;
            rdta = ta;
            Console.WriteLine("\nAt the RDP:");
            Console.WriteLine("The Target Distance is " + rdtd + ".");
            Console.WriteLine("The Target Azimuth is " + rdta + ".");
        }
    }

    public class ERC : RDP
    {
        public double ertd; public double erta;
        public ERC (double td, double ta) : base (td, ta)
        {
            ertd = td;
            erta = ta;
            Console.WriteLine("\nAt the ERC:");
            Console.WriteLine("The Target Distance is " + ertd + ".");
            Console.WriteLine("The Target Azimuth is " + erta + ".");
        }
    }

    public class RCDS : ERC
    {
        public double rctd; public double rcta;
        public RCDS (double td, double ta) : base (td, ta)
        {
            rctd = td;
            rcta = ta;
            Console.WriteLine("\nAt the RCDS:");
            Console.WriteLine("The Target Distance is " + rctd + ".");
            Console.WriteLine("The Target Azimuth is " + rcta + ".");
        }
    }

    // Main Method 
    public class multilevel
    {
        public static void Main()
        {
            TargetAttributes attributes = new TargetAttributes(2.3, 4.3, 1.1);
            Target target = new Target (3.4, 2.8);
            Antenna antenna = new Antenna (3.4, 2.8);
            RSP rsp = new RSP (3.4, 2.8);
            RDP rdp = new RDP (3.4, 2.8);
            ERC erc = new ERC (3.4, 2.8);
            RCDS rcds = new RCDS (3.4, 2.8);
            Console.WriteLine (attributes.threedouble());
            Console.WriteLine (target.todouble());
        }
    }
}
            
        
    
    


