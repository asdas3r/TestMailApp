using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMailApp
{
    public class Employee
    {
        public Employee() { }

        public Employee(int id, string S, string N, string P, string E)
        {
            Surname = S;
            Name = N;
            Patronymic = P;
            Email = E;
            ID = id;
        }

        public int ID { get; set; }

        public string Surname { get; set; }

        public string Name { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string Info
        {
            get
            {
                string initials;
                if (Patronymic == "")
                    initials = string.Concat(Name[0] + ".");
                else
                    initials = string.Concat(Name[0] + "." + Patronymic[0] + ".");
                return string.Concat(Surname + " " + initials + "(" + Email + ")");
            }
        }

    }
}
