using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace crousseau2_College_Strike.Models
{
    public class Position
    {
        public int id { get; set; }

        [Display(Name = "Position")]
        [Required(ErrorMessage = "You cannot leave the name of the position blank.")]
        [StringLength(50, ErrorMessage = "Position name is too long.")]
        [Index("IX_Unique_Position", IsUnique = true)]
        public string PositionTitle { get; set; }

        public virtual ICollection<Member> Members { get; set; }
    }
}