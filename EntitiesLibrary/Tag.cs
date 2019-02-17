using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EntitiesLibrary
{
    [DataContract]
    public class Tag
    {
        public Tag() { }

        public Tag(string name, string desciption)
        {
            Name = name;
            Description = desciption;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
