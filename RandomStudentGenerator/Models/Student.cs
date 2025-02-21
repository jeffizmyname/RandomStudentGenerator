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
            Presences = new List<Presence>();
            currentPresence = new Presence(DateTime.Now, false);
        }

        public void savePresence(bool isPresent)
        {
            currentPresence = new Presence(DateTime.Now, isPresent);
            Presences.Add(currentPresence);
        }

        public Presence GetPresenceForDate(DateTime selectedDate)
        {
            return Presences.FirstOrDefault(p => p.date.Date == selectedDate.Date)
                   ?? new Presence(selectedDate, false);
        }

    }
}
