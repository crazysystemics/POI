using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG
{
    public class WeaponSystem
    {
       public int id;
       public string name;
       public string type;

        public List<Active_Sensor> active_sensors;

        public WeaponSystem(int pid, string pname, string ptype, List<Active_Sensor> pactive_sensors)
        {
            this.id = pid;
            this.name = pname;
            this.type = ptype;
            this.active_sensors = pactive_sensors;
        }
    }

    public class Active_Sensor
    {

    }

    public class Radar : Active_Sensor
    {

    }

    public class Passive_Sensor
    {

    }

    public class Weapon
    {

    }
}
