using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Mail
    {
        private List<Tag> _Tags;

        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Employee SentFromTo;

        public string Contents { get; set; }

        public void SetTags(List<Tag> tagsList)
        {
            _Tags = new List<Tag>(tagsList);
        }

        public List<Tag> GetTags()
        {
            return new List<Tag>(_Tags);
        }

        public bool Equals(Mail other)
        {
            return this.ID == other.ID &&
                this.Name.Equals(other.Name) &&
                this.RegistrationDate.Equals(other.RegistrationDate) &&
                this.SentFromTo.ID == other.SentFromTo.ID &&
                this.TagsEqual(other.GetTags()) &&
                this.Contents.Equals(other.Contents);
        }

        public bool TagsEqual(List<Tag> otherTags)
        {
            List<Tag> tags = GetTags();
            if (tags.Count != otherTags.Count)
                return false;
            foreach (var n in tags)
                if (!otherTags.Any(p => p.Name.Equals(n.Name)))
                    return false;
            return true;
        }
    }
}
