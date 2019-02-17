using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EntitiesLibrary
{
    [DataContract]
    public class Employee
    {
        public Employee() { }

        public Employee(int id, string surname, string name, string patronymic, string email)
        {
            ID = id;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Email = email;
        }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Patronymic { get; set; }

        [DataMember]
        public string Email { get; set; }

        public string Info
        {
            get
            {
                string initials;
                if (Patronymic.Equals(string.Empty))
                    initials = string.Format("{0}.", Name[0]);
                else
                    initials = string.Format("{0}.{1}.", Name[0], Patronymic[0]);
                return string.Format("{0} {1} ({2})", Surname, initials, Email);
            }
        }

        public string FullInfo
        {
            get
            {
                if (Patronymic.Equals(string.Empty))
                    return string.Format("{0} {1} ({2})", Surname, Name, Email);
                return string.Format("{0} {1} {2} ({3})", Surname, Name, Patronymic, Email);
            }
        }
    }
}
