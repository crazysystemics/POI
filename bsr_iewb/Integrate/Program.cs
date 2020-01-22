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
            public double atd; public double ata;
            public Antenna(Target target)
            {
                this.atd = target.td;
                this.ata = target.ta;
            }

            public double getTargetDistance(Target target)
            {
                target.td = target.td * 100;
                Console.WriteLine("\nFor Antenna : \nThe Target Distance is " + target.td + ".");
                return target.td;
            }

            public double getTargetAzimuth(Target target)
            {
                target.ta = target.ta * 100;
                Console.WriteLine("The Target Azimuth is " + target.ta + ".");
                return target.ta;
            }
        }

        public class RSP
        {
            public double rstd; public double rsta;
            public RSP(Antenna antenna)
            {
                this.rstd = antenna.atd;
                this.rsta = antenna.ata;
            }
            public double getTargetDistance(Antenna antenna)
            {
                antenna.atd = (antenna.atd / 100) + 2;
                Console.WriteLine("\nFor RSP : \nThe Target Distance is " + antenna.atd + ".");
                return antenna.atd;
            }

            public double getTargetAzimuth(Antenna antenna)
            {
                antenna.ata = (antenna.ata / 100) + 2;
                Console.WriteLine("The Target Distance is " + antenna.ata + ".");
                return antenna.ata;
            }
        }

        public class RDP
        {
            public double rdtd; public double rdta;
            public RDP(RSP rsp)
            {
                this.rdtd = rsp.rstd;
                this.rdta = rsp.rsta;
            }

            public double getTargetDistance(RSP rsp)
            {
                rsp.rstd = rsp.rstd + 10;
                Console.WriteLine("\nFor RDP :\nThe Target Distance is " + rsp.rstd + ".");
                return rsp.rstd;
            }

            public double getTargetAzimuth(RSP rsp)
            {
                rsp.rsta = rsp.rsta + 10;
                Console.WriteLine("The Target Azimuth is " + rsp.rsta + ".");
                return rsp.rsta;
            }
        }

        public class ERC
        {
            public double ertd; public double erta;
            public ERC(RDP rdp)
            {
                this.ertd = rdp.rdtd;
                this.erta = rdp.rdta;
            }
            public double getTargetDistance(RDP rdp)
            {
                rdp.rdtd = rdp.rdtd - 10;
                Console.WriteLine("\nFor ERC : \nThe Target Distance is " + rdp.rdtd + ".");
                return rdp.rdtd;
            }

            public double getTargetAzimuth(RDP rdp)
            {
                rdp.rdta = rdp.rdta - 10;
                Console.WriteLine("The Target Azimuth is " + rdp.rdta + ".");
                return rdp.rdta;
            }
        }

        public class RCDS
        {
            public double rctd; public double rcta;
            public RCDS(ERC erc)
            {
                this.rctd = erc.ertd;
                this.rcta = erc.erta;
            }
            public double getTargetDistance(ERC erc)
            {
                erc.ertd = erc.ertd * 10;
                Console.WriteLine("\nFor RCDS : \nThe Target Distance is " + erc.ertd + ".");
                return erc.ertd;
            }

            public double getTargetAzimuth(ERC erc)
            {
                erc.erta = erc.erta * 10;
                Console.WriteLine("The Target Azimuth is " + erc.erta + ".");
                return erc.erta;
            }
        }

        public static void Main()
        {
            double arg1 = 10;
            double arg2 = 50;
            TargetAttributes attributes = new TargetAttributes(2.3, 4.3, 1.1);
            Target target = new Target(arg1, arg2);
            Antenna antenna = new Antenna(target);
            RSP rsp = new RSP(antenna);
            RDP rdp = new RDP(rsp);
            ERC erc = new ERC(rdp);
            RCDS rcds = new RCDS(erc);

            Console.WriteLine(attributes.threedouble());
            Console.WriteLine(target.todouble());

            antenna.atd = antenna.getTargetDistance(target);
            antenna.ata = antenna.getTargetAzimuth(target);

            rsp.rstd = rsp.getTargetDistance(antenna);
            rsp.rsta = rsp.getTargetAzimuth(antenna);

            rdp.rdtd = rdp.getTargetDistance(rsp);
            rdp.rdta = rdp.getTargetAzimuth(rsp);

            erc.ertd = erc.getTargetDistance(rdp);
            erc.erta = erc.getTargetAzimuth(rdp);

            rcds.rctd = rcds.getTargetDistance(erc);
            rcds.rcta = rcds.getTargetAzimuth(erc);
        }
    }
}
