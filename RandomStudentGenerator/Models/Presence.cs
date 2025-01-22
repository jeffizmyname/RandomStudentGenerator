using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.Models
{
    public class Presence
    {
        DateTime date { get; set; }
        bool isPresent { get; set; }

        public Presence(DateTime date, bool isPresent)
        {
            this.date = date;
            this.isPresent = isPresent;
        }
    }
}
