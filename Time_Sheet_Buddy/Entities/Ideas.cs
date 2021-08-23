using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Time_Sheet_Buddy.Entities
{
    public class Ideas
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string LeftStyle { get; set; }
        public string TopStyle { get; set; }

        public byte[] IdeaPicture { get; set; }
    }
}
