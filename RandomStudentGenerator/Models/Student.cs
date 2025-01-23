using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.Models
{
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<Presence> Presences { get; set; }
        public string FullName => $"{Name} {Surname}";

        public Student(string id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
        }   
    }
}
