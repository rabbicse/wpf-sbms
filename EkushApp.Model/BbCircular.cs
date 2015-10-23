using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class BbCircular
    {
        public string Tag { get; set; }
        [Header("Department")]
        public string Department { get; set; }
        [Header("Department Key")]
        public string DepartmentKey { get; set; }
        [Header("Category")]
        public string Category { get; set; }
        [Header("Category Key")]
        public string CategoryKey { get; set; }
        [Header("Circular Title")]
        public string Title { get; set; }
        [Header("Publish Date")]
        public DateTime PublishDate { get; set; }
        [Header("File Name")]
        public string FileName { get; set; }
        [Header("File With Full Path")]
        public string FileWithFullPath { get; set; }
    }
}
