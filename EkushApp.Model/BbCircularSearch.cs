using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class BbCircularSearch
    {
        [Header("Search By Key")]
        public string SearchKey { get; set; }
        [Header("Search By")]
        public string SearchByName { get; set; }
        [Header("Search Term Key")]
        public string SearchTermKey { get; set; }
        [Header("Search Term")]
        public string SearchTerm { get; set; }
    }
    public class BbSearchBy 
    {
        [UniqueConstraint]
        [Header("Search By Key")]
        public string SearchKey { get; set; }
        [Header("Search By")]
        public string SearchByName { get; set; }
    }
}
