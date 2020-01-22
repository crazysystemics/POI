//C# program to illustrate the 
//Integration of different subsystems of BSR
using System;
using System.IO;
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
        public TargetAttributes(double SigAmp, double ToTxn, double ToRxn)
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
        public Target(double td, double ta)
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

    public class BSR
    {
        public class Antenna
        {
            public Target UpTarget;
            public RSP DownRSP;
            
            double targetDist;
            double targetAzim;

            public Antenna(ref Target target)
            {
                UpTarget = target;
            }

            public Antenna(ref RSP rsp)
            {
                DownRSP = rsp;
            }

            public double atd; public double ata;
            public double getTargetDistance()
            {
                targetDist = UpTarget.getTargetDistance();
                Console.WriteLine("\nFor Antenna : \nThe Target Distance is " + targetDist + ".");
                return targetDist;
            }

            public double getTargetAzimuth()
            {
                targetAzim = UpTarget.getTargetAzimuth();
                Console.WriteLine("The Target Azimuth is " + targetAzim + ".");
                return targetAzim;
            }
        }

        public class RSP
        {
            public Antenna UpAnt;
            public RDP DownRDP;

            double targetDist;
            double targetAzim;

            public RSP(ref Antenna antenna)
            {
                UpAnt = antenna;
            }

            public RSP(ref RDP rdp)
            {
                DownRDP = rdp;
            }

            public double rstd; public double rsta;
            public double getTargetDistance()
            {
                targetDist = UpAnt.getTargetDistance();
                Console.WriteLine("\nFor RSP : \nThe Target Distance is " + targetDist + ".");
                return targetDist;
            }

            public double getTargetAzimuth()
            {
                targetAzim = UpAnt.getTargetAzimuth();
                Console.WriteLine("The Target Distance is " + targetAzim + ".");
                return targetAzim;
            }
        }

        public class RDP
        {
            public RSP UpRSP;
            public ERC DownERC;

            double targetDist;
            double targetAzim;

            public RDP(ref RSP rsp)
            {
                UpRSP = rsp;
            }

            public RDP(ref ERC erc)
            {
                DownERC = erc;
            }

            public double rdtd; public double rdta;
            public double getTargetDistance()
            {
                targetDist = UpRSP.getTargetDistance();
                Console.WriteLine("\nFor RDP :\nThe Target Distance is " + targetDist + ".");
                return targetDist;
            }

            public double getTargetAzimuth()
            {
                targetAzim = UpRSP.getTargetAzimuth(); 
                Console.WriteLine("The Target Azimuth is " + targetAzim + ".");
                return targetAzim;
            }
        }

        public class ERC
        {
            public RDP UpRDP;
            public RCDS DownRCDS;

            double targetDist;
            double targetAzim;

            public ERC(ref RDP rdp)
            {
                UpRDP = rdp;
            }

            public ERC(ref RCDS rcds)
            {
                DownRCDS = rcds;
            }

            public double ertd; public double erta;
            
            public double getTargetDistance()
            {
                targetDist = UpRDP.getTargetDistance();
                Console.WriteLine("\nFor ERC : \nThe Target Distance is " + targetDist + ".");
                return targetDist;
            }

            public double getTargetAzimuth()
            {
                targetAzim = UpRDP.getTargetAzimuth();
                Console.WriteLine("The Target Azimuth is " + targetAzim + ".");
                return targetAzim;
            }
        }

        public class RCDS
        {
            public ERC UpERC;

            double targetDist;
            double targetAzim;

            public RCDS(ref ERC erc)
            {
                UpERC = erc;
            }

            public double rctd; public double rcta;
           
            public double getTargetDistance()
            {
                targetDist = UpERC.getTargetDistance();
                Console.WriteLine("\nFor RCDS : \nThe Target Distance is " + targetDist + ".");
                return targetDist;
            }

            public double getTargetAzimuth()
            {
                targetAzim = UpERC.getTargetAzimuth();
                Console.WriteLine("The Target Azimuth is " + targetAzim + ".");
                return targetAzim;
            }
        }

        public static void Main()
        {
            double arg1 = 10;
            double arg2 = 50;
            TargetAttributes attributes = new TargetAttributes(2.3, 4.3, 1.1);
            Target target = new Target(arg1, arg2);
            Antenna antenna = new Antenna(ref target);
            RSP rsp = new RSP(ref antenna);
            RDP rdp = new RDP(ref rsp);
            ERC erc = new ERC(ref rdp);
            RCDS rcds = new RCDS(ref erc);

            Console.WriteLine(attributes.threedouble());
            Console.WriteLine(target.todouble());

            antenna.atd = antenna.getTargetDistance();
            antenna.ata = antenna.getTargetAzimuth();

            rsp.rstd = rsp.getTargetDistance();
            rsp.rsta = rsp.getTargetAzimuth();

            rdp.rdtd = rdp.getTargetDistance();
            rdp.rdta = rdp.getTargetAzimuth();

            erc.ertd = erc.getTargetDistance();
            erc.erta = erc.getTargetAzimuth();

            rcds.rctd = rcds.getTargetDistance();
            rcds.rcta = rcds.getTargetAzimuth();
        }
    }
}
