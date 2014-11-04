using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class Supplier
    {
        [UniqueConstraint]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactMobileNo { get; set; }
        public long HardwareSerial { get; set; }
        public string Comments { get; set; }
    }
}