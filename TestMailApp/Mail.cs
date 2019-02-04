using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMailApp
{
    class Mail
    {
        private Tag[] _Tags;

        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Employee SentFromTo;

        public string Contents { get; set; }

        public void SetTags(Tag[] tagsArray)
        {
            _Tags = (Tag[])tagsArray.Clone();
        }

        public Tag[] GetTags()
        {
            return (Tag[])_Tags.Clone();
        }

    }
}
