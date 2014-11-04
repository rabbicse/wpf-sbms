using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class HardwareReport
    {
        public string Category { get; set; }
        public string Model { get; set; }
        public string Count { get; set; }
    }
    public class UserReport 
    {
        public string UserName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public IEnumerable<Hardware> Hardwares { get; set; }
    }
}
