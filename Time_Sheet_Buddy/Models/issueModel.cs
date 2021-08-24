using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Time_Sheet_Buddy.Models
{
    public class issueModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Duration { get; set; }

        public string AssignedTo { get; set; }

        public string State { get; set; }
    }
}
