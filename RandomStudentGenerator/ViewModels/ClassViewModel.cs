using CommunityToolkit.Mvvm.ComponentModel;
using RandomStudentGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static RandomStudentGenerator.StorageHandlers.StorageHandler;

namespace RandomStudentGenerator.ViewModels
{
    public class ClassViewModel
    {
        public Class Class { get; set; }
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                UpdatePresencesForSelectedDate();
            }
        }

        public ICommand deleteStudentCommand { get; }

        public ClassViewModel()
        {
            deleteStudentCommand = new Command<Student>(deleteStudent);
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
            AddStudent(Students.ToList(), Class.className);
        }

        private void deleteStudent(Student student)
        {
            Class.students.Remove(student);
            sortStudents();
            AddStudent(Students.ToList(), Class.className);
        }

        public void setClass(string className)
        {

            Class = ReadClass(className, SelectedDate);
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

        public void setStudentPresence(Student student, bool isPresent)
        {
            if (student == null) return;

            var presence = student.GetPresenceForDate(SelectedDate);
            presence.isPresent = isPresent;

            if (!student.Presences.Contains(presence))
                student.Presences.Add(presence);
        }

        private void UpdatePresencesForSelectedDate()
        {
            foreach (var student in Students)
            {
                student.CurrentPresence = student.GetPresenceForDate(SelectedDate);
            }
        }
    }
}
