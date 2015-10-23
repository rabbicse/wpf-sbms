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
        public string Tag { get; set; }
        [Header("Search Key")]
        public string SearchKey { get; set; }
        [Header("Search Name")]
        public string SearchName { get; set; }
    }
    public class BbSearchBy
    {
        [UniqueConstraint]
        [Header("Search By Key")]
        public string SearchKey { get; set; }
        [Header("Search Name")]
        public string SearchName { get; set; }
    }

    public class BbDepartment
    {
        public string Tag { get; set; }
        [Header("Department")]
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class BbCategory
    {
        public string Tag { get; set; }
        [Header("Category")]
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
