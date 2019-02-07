using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMailApp
{
    public class Tag
    {
        public Tag() { }

        public Tag(string nm, string desc)
        {
            Name = nm;
            Description = desc;
        }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
