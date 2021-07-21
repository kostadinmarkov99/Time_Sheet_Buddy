using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Time_Sheet_Buddy.Entities
{
    public class Issue
    {
        public Issue()
        {
            this.Date = DateTime.Now;
        }

        [Key]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public double Duration { get; set; }

        public string Assignee { get; set; }

        public string AssignedTo { get; set; }


        public DateTime Date { get; set; }

        [Required]

        public string State { get; set; }

        public string Project { get; set; }
    }
}
