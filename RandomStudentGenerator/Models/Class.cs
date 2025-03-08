using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.Models
{
    public class Class
    {
        public string className { get; set; }
        public List<Student> students { get; set; }

        public Class(string className, List<Student> students)
        {
            this.className = className;
            this.students = students;
        }
    }
}
