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
        private string _Name;
        private string _Description;

        public Tag() { }

        public Tag(string name, string desciption)
        {
            Name = name;
            Description = desciption;
        }

        [DataMember]
        public string Name
        {
            get { return _Name; }
            set { _Name = value.Trim(); }
        }

        [DataMember]
        public string Description
        {
            get { return _Description; }
            set { _Description = value.Trim(); }
        }
    }
}
