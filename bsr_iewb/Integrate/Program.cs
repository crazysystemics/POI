//C# program to illustrate the 
//Integration of different subsystems of BSR
using System;
namespace project
{
    public class BSRModel
    {
        public double td, ta;

        public virtual double TargetDistance
        {
            set
            {
                td = value;
            }

            get
            {
                return td;
            }
        }

        public virtual double TargetAzimuth
        {
            set
            {
                ta = value;
            }

            get
            {
                return ta;
            }
        }
    }

    public class Target : BSRModel
    {
        public double ttd, tta;
        Antenna antenna = new Antenna();

        public void BindRadar(ref Antenna lt)
        {
            antenna = lt;
        }

        public override double TargetDistance
        {
            set
            {
                ttd = value;
            }

            get
            {
                return ttd;
            }
        }

        public override double TargetAzimuth
        {
            set
            {
                tta = value;
            }

            get
            {
                return tta;
            }
        }
    }

    public class Antenna : BSRModel
    {
        public double atd, ata;
        Target target = new Target();
        RSP rsp = new RSP();

        public void BindRadar(ref Target rt)
        {
            Target = rt;
        }

        public void BindRadar(ref RSP lt)
        {
            rsp = lt;
        }
        public override double TargetDistance
        {
            set
            {
                Target.ttd = value;
                atd = Target.ttd;
            }

            get
            {
                return atd;
            }
        }

        public override double TargetAzimuth
        {
            set
            {
                Target.tta = value;
                ata = Target.tta;
            }

            get
            {
                return ata;
            }
        }

        public Target Target { get => target; set => target = value; }
    }

    public class RSP : BSRModel
    {
        public double rstd, rsta;
        Antenna antenna = new Antenna();
        RDP rdp = new RDP();

        public void BindRadar(ref Antenna rt)
        {
            antenna = rt;
        }

        public void BindRadar(ref RDP lt)
        {
            rdp = lt;
        }

        public override double TargetDistance
        {
            set
            {
                antenna.atd = value;
                rstd = antenna.atd;
            }

            get
            {
                return rstd;
            }
        }

        public override double TargetAzimuth
        {
            set
            {
                antenna.ata = value;
                rsta = antenna.ata;
            }

            get
            {
                return rsta;
            }
        }
    }

    public class RDP : BSRModel
    {
        public double rdtd, rdta;
        RSP rsp = new RSP();
        ERC erc = new ERC();

        public void BindRadar(ref RSP rt)
        {
            rsp = rt;
        }

        public void BindRadar(ref ERC lt)
        {
            erc = lt;
        }

        public override double TargetDistance
        {
            set
            {
                rsp.rstd = value;
                rdtd = rsp.rstd;
            }

            get
            {
                return rdtd;
            }
        }

        public override double TargetAzimuth
        {
            set
            {
                rsp.rsta = value;
                rdta = rsp.rsta;
            }

            get
            {
                return rdta;
            }
        }
    }

    public class ERC : BSRModel
    {
        public double ertd, erta;
        RDP rdp = new RDP();
        RCDS rcds = new RCDS();

        public void BindRadar(ref RDP rt)
        {
            rdp = rt;
        }

        public void BindRadar(ref RCDS lt)
        {
            rcds = lt;
        }

        public override double TargetDistance
        {
            set
            {
                rdp.rdtd = value;
                ertd = rdp.rdtd;
            }

            get
            {
                return ertd;
            }
        }

        public override double TargetAzimuth
        {
            set
            {
                rdp.rdta = value;
                erta = rdp.rdta;
            }

            get
            {
                return erta;
            }
        }
    }

    public class RCDS : BSRModel
    {
        public double rctd, rcta;
        ERC erc = new ERC();

        public void BindRadar(ref ERC rt)
        {
            erc = rt;
        }

        public override double TargetDistance
        {
            set
            {
                erc.ertd = value;
                rctd = erc.ertd;
            }

            get
            {
                return rctd;
            }
        }

        public override double TargetAzimuth
        {
            set
            {
                erc.erta = value;
                rcta = erc.erta;
            }

            get
            {
                return rcta;
            }
        }
    }

    public class IEWB
    {
        public Target target = new Target();
        public Antenna antenna = new Antenna();
        public RSP rsp = new RSP();
        public RDP rdp = new RDP();
        public ERC erc = new ERC();
        public RCDS rcds = new RCDS();

        public IEWB()
        {
            antenna.BindRadar(ref target);
            rsp.BindRadar(ref antenna);
            rdp.BindRadar(ref rsp);
            erc.BindRadar(ref rdp);
            rcds.BindRadar(ref erc);
        }
    }

    static class program
    {
        public static void Main()
        {
            IEWB iewb = new IEWB();
            Console.WriteLine("Enter the Target Distance and Target Azimuth");
            double arg1 = Convert.ToDouble(Console.ReadLine());
            double arg2 = Convert.ToDouble(Console.ReadLine());

            iewb.target.ttd = arg1;
            iewb.target.tta = arg2;

            Console.WriteLine("\nAt Antenna: \nThe Target Distance is " + iewb.antenna.atd.ToString() + ".");
            Console.WriteLine("The Target Azimuth is " + iewb.antenna.ata.ToString() + ".");

            Console.WriteLine("\nFor RSP: \nThe Target Distance is " + iewb.rsp.rstd.ToString() + ".");
            Console.WriteLine("The Target Azimuth is " + iewb.rsp.rsta.ToString() + ".");

            Console.WriteLine("\nFor RDP: \nThe Target Distance is " + iewb.rdp.rdtd.ToString() + ".");
            Console.WriteLine("The Target Azimuth is " + iewb.rdp.rdta.ToString() + ".");

            Console.WriteLine("\nFor ERC: \nThe Target Distance is " + iewb.erc.ertd.ToString() + ".");
            Console.WriteLine("The Target Azimuth is " + iewb.erc.erta.ToString() + ".");

            Console.WriteLine("\nFor RCDS: \nThe Target Distance is " + iewb.rcds.rctd.ToString() + ".");
            Console.WriteLine("The Target Azimuth is " + iewb.rcds.rcta.ToString() + ".");
        }
    }
}
