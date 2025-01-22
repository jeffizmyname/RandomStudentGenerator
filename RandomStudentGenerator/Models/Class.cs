using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.Models
{
    public class Class
    {
        string className { get; set; }
        List<Student> students { get; set; }

        public Class(string className)
        {
            this.className = className;
            students = new List<Student>();
        }
    }
}
