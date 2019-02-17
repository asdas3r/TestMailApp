using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EntitiesLibrary
{
    [DataContract]
    public class Mail
    {
        private List<Tag> _Tags = new List<Tag>();

        public Mail() { }

        public Mail(int id, string name, DateTime registrationdate, Employee sender, Employee reciever, string content, List<Tag> tags)
        {
            ID = id;
            Name = name;
            RegistrationDate = registrationdate;
            SentFrom = sender;
            SentTo = reciever;
            Contents = content;
            Tags = tags;
        } 

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime RegistrationDate { get; set; }

        [DataMember]
        public Employee SentFrom;

        [DataMember]
        public Employee SentTo;

        [DataMember]
        public string Contents { get; set; }

        [DataMember]
        public List<Tag> Tags
        {
            get { return _Tags; }
            set { _Tags = value; }
        }

        public bool Equals(Mail other)
        {
            return this.ID == other.ID &&
                this.Name.Equals(other.Name) &&
                this.RegistrationDate.Equals(other.RegistrationDate) &&
                this.SentFrom.ID == other.SentFrom.ID &&
                this.SentTo.ID == other.SentTo.ID &&
                this.TagsEqual(other.Tags) &&
                this.Contents.Equals(other.Contents);
        }

        public bool TagsEqual(List<Tag> otherTags)
        {
            List<Tag> tags = Tags;
            if (tags.Count != otherTags.Count)
                return false;
            foreach (var n in tags)
                if (!otherTags.Any(p => p.Name.Equals(n.Name)))
                    return false;
            return true;
        }
    }
}
