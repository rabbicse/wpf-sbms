using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public enum HardwareCategory
    {
        CPU,
        MONITOR,
        PRINTER,
        UPS,
        SCANNER,
        MODEM,
        PEN_DRIVE
    }
    public enum HardwareStatus
    {
        RUNNING,
        UN_USED,
        REPAIR_TO_TLD
    }
    public class Hardware
    {
        [UniqueConstraint]
        public long SerialNo { get; set; }
        public HardwareCategory Category { get; set; }
        [UniqueConstraint]
        public string HardwareTagNo { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }        
        public DateTime? ReceiveDate { get; set; }
        public HardwareStatus Status { get; set; }
        public string Comments { get; set; }
    }
}
