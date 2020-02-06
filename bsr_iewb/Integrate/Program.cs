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

        public virtual double getTargetDistance
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

        public virtual double getTargetAzimuth
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
        public override double getTargetDistance
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

        public override double getTargetAzimuth
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
        public double atd, ata;
        Target target;

        public void BindRadar(ref Target rt)
        {
            target = rt;
        }
        public override double getTargetDistance
        {
            set
            {
                atd = target.ttd;
            }

            get
            {
                return atd;
            }
        }

        public override double getTargetAzimuth
        {
            set
            {
                ata = target.tta;
            }

            get
            {
                return ata;
            }
        }
    }

    public class IEWB
    {

        public Target tgt;
        public Antenna ant;

        public IEWB(ref Target rt)
        {
            ant.BindRadar(ref rt);
        }


        public static void Main()
        {
            Console.WriteLine("Enter the Target Distance and Target Azimuth");
            double arg1 = Convert.ToDouble(Console.ReadLine());
            double arg2 = Convert.ToDouble(Console.ReadLine());

            ant.getTargetAzimuth();
            ant.getTargetDistance();



            BSRModel bsr = new BSRModel();
            bsr.td = arg1;
            bsr.ta = arg2;
            //bsr  = new Target();

            Antenna antenna = new Antenna();

            Console.WriteLine("\nThe Target Distance is " + bsr.getTargetDistance + ".");
            Console.WriteLine("The Target Azimuth is " + bsr.getTargetAzimuth + ".");

            bsr = new Target();
            Console.WriteLine("\nThe Target Distance is " + bsr.getTargetDistance + ".");
            Console.WriteLine("The Target Azimuth is " + bsr.getTargetAzimuth + ".");
        }
    }
}
