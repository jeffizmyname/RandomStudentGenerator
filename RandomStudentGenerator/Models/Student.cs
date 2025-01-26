using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.Models
{
    public partial class Student : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string surname;
        public List<Presence> Presences { get; set; }

        [ObservableProperty]
        private Presence currentPresence;
        public string FullName => $"{Name} {Surname}";

        public Student(int id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
            currentPresence = new Presence(DateTime.Now, false);
        }

        public void savePresence()
        {
            currentPresence.date = DateTime.Now;
            Presences.Add(currentPresence);
            currentPresence = new Presence(DateTime.Now, false); // ??
        }
    }
}
