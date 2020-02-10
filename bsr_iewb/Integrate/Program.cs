//C# program to illustrate the 
//Integration of different subsystems of BSR
using System;
namespace project
{
    // Virtual auto-implemented property. Overrides can only
    // provide specialized behavior if they implement get and set accessors.
    public class BSRModel
    {
        public int td, ta;

        public virtual double TargetDistance
        {
            set
            {
                td = (int)value;
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

    // Override auto-implemented property with ordinary property
    // to provide specialized accessor behavior.
    public class Target : BSRModel
    {
        public double ttd, tta;
        BSRModel bsr = new BSRModel();
        public override double TargetDistance
        {
            set
            {
                ttd = bsr.td;
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
                tta = bsr.ta;
            }

            get
            {
                return tta;
            }
        }
    }

    public class Antenna : BSRModel
    {
        //public double atd, ata;
        Target target;

        public void BindRadar(ref Target rt)
        {
            target = rt;
        }
        public override double TargetDistance
        {
            set
            {
                target.td = value;
            }

            get
            {
                return target.td;
            }
        }

        public override double TargetAzimuth
        {
            set
            {
                target.ta = value;
            }

            get
            {
                return target.azimuth;
            }
        }
    }

    public class IEWB
    {
        public Target tgt;
        public Antenna ant;

        public IEWB()
        {
            ant.BindRadar(ref tgt);
        }
    }

    static class Program
    {
        public static void Main()
        {
            IEWB iewb = new IEWB();

            Console.WriteLine("Enter the Target Distance and Target Azimuth");
            double arg1 = Convert.ToDouble(Console.ReadLine());
            double arg2 = Convert.ToDouble(Console.ReadLine());



            Console.WriteLine("dist: ", ant.TargetDistance);
            Console.WriteLine("azim: ", ant.TargetAzimuth);

            //BSRModel bsr = new BSRModel();
            //bsr.td = arg1;
            //bsr.ta = arg2;
            ////bsr  = new Target();

            //Antenna antenna = new Antenna();

            //Console.WriteLine("\nThe Target Distance is " + bsr.getTargetDistance + ".");
            //Console.WriteLine("The Target Azimuth is " + bsr.getTargetAzimuth + ".");

            //bsr = new Target();
            //Console.WriteLine("\nThe Target Distance is " + bsr.getTargetDistance + ".");
            //Console.WriteLine("The Target Azimuth is " + bsr.getTargetAzimuth + ".");
        }

    }
    
}
