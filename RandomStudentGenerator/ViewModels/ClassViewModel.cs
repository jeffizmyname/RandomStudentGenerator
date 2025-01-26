using RandomStudentGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RandomStudentGenerator.StorageHandlers.StorageHandler;

namespace RandomStudentGenerator.ViewModels
{
    public class ClassViewModel
    {
        public Class Class { get; set; }
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        public ClassViewModel()
        {
        }

        public void newClass(string className)
        {
            CreateNewClass(className);
            setClass(className);
        }

        public void addStudent(string name, string surname)
        {
            if(Class == null) return;
            var student = new Student(0, name, surname);
            Class.students.Add(student);
            sortStudents();
            AddStudent(Class.students.Find(s => s.Name == name && s.Surname == surname), Class.className);
        }

        public void setClass(string className)
        {

            Class = ReadClass(className);
            Students.Clear();
            var sortedStudents = Class.students.OrderBy(s => s.Name).ToList();

            for (int i = 0; i < sortedStudents.Count; i++)
            {
                sortedStudents[i].Id = i + 1;
                Students.Add(sortedStudents[i]);
            }
        }

        public void sortStudents()
        {
            Students.Clear();
            var sortedStudents = Class.students.OrderBy(s => s.Name).ToList();
            for (int i = 0; i < sortedStudents.Count; i++)
            {
                sortedStudents[i].Id = i + 1;
                Students.Add(sortedStudents[i]);
            }
        }
    }
}
