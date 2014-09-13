using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class Supplier
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactMobileNo { get; set; }
        public Hardware SupplyHardware { get; set; }
        public string Comments { get; set; }
    }
}