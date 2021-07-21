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

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public double Duration { get; set; }

        [Required]
        public string Assignee { get; set; }

        [Required]
        public string AssignedTo { get; set; }


        public DateTime Date { get; set; }

        [Required]

        public string State { get; set; }

        [Required]
        public string Project { get; set; }
    }
}
