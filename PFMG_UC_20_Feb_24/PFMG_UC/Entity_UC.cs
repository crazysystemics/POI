using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG_UC
{
    public interface Entity_UC
    {
        MVCEntity get_user_input_entity();
        void display(MVCEntity data);
        void Clear_all();
        void Enable();
    }
}
