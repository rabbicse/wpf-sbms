using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class HardwareCountReport
    {
        public string Category { get; set; }
        public string Model { get; set; }
        public string Count { get; set; }
    }
    public class HardwareReport
    {
        public string Category { get; set; }
        public long SerialNo { get; set; }
        public string HardwareTagNo { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string HardwareSerialNo { get; set; }
        public string ReceiveDate { get; set; }
        public string Status { get; set; }
        public string ComputerUserName { get; set; }
        public string Comments { get; set; }
    }
    public class UserReport
    {
        public string UserName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public IEnumerable<Hardware> Hardwares { get; set; }
    }
}
