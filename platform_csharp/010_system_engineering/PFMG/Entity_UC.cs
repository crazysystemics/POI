using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG
{
    public interface Entity_UC
    {
        void display(MVCEntity entity);

        MVCEntity get_entity();
    }
}
