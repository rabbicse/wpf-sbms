using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public class BbCircular
    {
        [Header("Search Term")]
        public string SearchTerm { get; set; }
        [Header("Search Term Key")]
        public string SearchTermKey { get; set; }
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
