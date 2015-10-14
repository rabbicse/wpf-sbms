using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class Header : Attribute
    {
        public string Name { get; private set; }
        public Header(string name)
        {
            Name = name;
        }
    }
}
