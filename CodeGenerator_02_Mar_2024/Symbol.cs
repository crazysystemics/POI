using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    class Symbol
    {
        public int id;
        public string name;

        public DataType Type;
        public string value;

        public Symbol(int id, string name, DataType Type, string value)
        {
            this.id = id;
            this.name = name;
            this.Type = Type;
            this.value = value;
        }
    }
}