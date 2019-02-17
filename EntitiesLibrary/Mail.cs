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
        private string _Name;
        private string _Contents;

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
        public string Name
        {
            get { return _Name; }
            set { _Name = value.Trim(); }
        }

        [DataMember]
        public DateTime RegistrationDate { get; set; }

        [DataMember]
        public Employee SentFrom;

        [DataMember]
        public Employee SentTo;

        [DataMember]
        public string Contents
        {
            get { return _Contents; }
            set { _Contents = value.Trim(); }
        }

        [DataMember]
        public List<Tag> Tags { get; set; } = new List<Tag>();

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
