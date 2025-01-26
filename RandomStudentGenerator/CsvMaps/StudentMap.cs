using CsvHelper.Configuration;
using RandomStudentGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.CsvMaps
{
    public class StudentMap : ClassMap<Student>
    {
        public StudentMap() 
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");
            Map(m => m.Surname).Name("Surname");
        }
    }
}
