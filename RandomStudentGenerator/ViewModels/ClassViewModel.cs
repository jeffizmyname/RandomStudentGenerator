using RandomStudentGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.ViewModels
{
    public class ClassViewModel
    {
        public Class Class { get; set; }
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        public ClassViewModel()
        {
        }

        public void setClass(Class c)
        {
            Class = c;
            c.students.ToList().ForEach(Students.Add);
        }
    }
}
