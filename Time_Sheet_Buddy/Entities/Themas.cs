using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Time_Sheet_Buddy.Entities
{
    public class Themas
    {
        [Key]
        public int Id { get; set; }

        public string Color { get; set; }

        public byte[] ThemesPicture { get; set; }
    }
}
