using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.Models
{
    public class Student
    {
        string Id { get; set; }
        string Name { get; set; }
        string Surname { get; set; }
        List<Presence> Presences { get; set; }

        public Student(string id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
        }   
    }
}
