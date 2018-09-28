using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POI_XCS
{
    class BattleEngine
    {

        public void UpdateRwrRxBuf(Radar r, Aircraft ac, Rwr rwr)
        {

            int min_azim, max_azim;
            min_azim = (int)r.mb_azim - (int)r.beam_width;
            max_azim = (int)r.mb_azim + (int)r.beam_width;

            if (min_azim < 0)
                min_azim = 360 + min_azim; //it is subtraction as min_azim is -ve
            else
                if (max_azim > 360)
                {
                    max_azim = max_azim - 360;
               }

            double azim_rad;
            if (r.posx == ac.x)
            {
                azim_rad = Math.PI / 2.0;
            }
            else
            {
                azim_rad = Math.Atan2((r.posy - ac.y), (r.posx - ac.x));
            }
            double azim_deg = (180 / Math.PI) * azim_rad;



            if (azim_deg >= min_azim && azim_deg <= max_azim  && r.freq == rwr.band)
            {
                //if (rwr.rxunit.rxBufCount < 32)
                if (rwr.rxunit.rxBufCount == 0)
                    rwr.rxunit.rxbuf[rwr.rxunit.rxBufCount++] = r;
                Console.WriteLine(globals.tick.ToString() + " " + r.mb_azim.ToString() + " " +  r.beam_width.ToString() + " " + min_azim.ToString() + " " +
                                                        max_azim.ToString() + " " + azim_deg.ToString());
            }

            Console.WriteLine("out " + globals.tick.ToString() + " " +
                              r.mb_azim.ToString() + " " + r.beam_width.ToString() + " " +
                              min_azim.ToString() + " "  + max_azim.ToString() + " " + 
                              azim_deg.ToString() + " " + r.freq.ToString() + " " + rwr.band.ToString() );

        }

    }
}
