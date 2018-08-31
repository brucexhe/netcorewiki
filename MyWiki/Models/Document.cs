using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWiki.Models
{
    public class Document
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }

        public List<string> ChangeLogs { get; set; }

    }
}
