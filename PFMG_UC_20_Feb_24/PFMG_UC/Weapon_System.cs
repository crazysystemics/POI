using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG_UC
{
    public class Weapon_System : MVCEntity
    {
        public override Header GetHeader(int count)
        {
            throw new NotImplementedException();
        }

        public override MVCEntity get_entity(string[] data, ref StreamReader reader)
        {
            throw new NotImplementedException();
        }

        public override Identifier get_Identifier()
        {
            throw new NotImplementedException();
        }

        public override bool Match(MVCEntity entity_list, MVCEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    //class Active_Sensors : Weapon_System
    //{

    //}
    //class Passive_Sensors : Weapon_System
    //{

    //}
    //class Weapons : Weapon_System
    //{

    //}
}
