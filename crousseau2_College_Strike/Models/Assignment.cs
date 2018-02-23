using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace crousseau2_College_Strike.Models
{
    public class Assignment : Auditable
    {
        public Assignment()
        {
            this.Shift = new HashSet<Shift>();
            this.Members = new HashSet<Member>();
        }

        public int id { get; set; }



        [Display(Name = "Assignment")]
        [Required(ErrorMessage = "You cannot leave the name of the assignment blank.")]
        [StringLength(500, MinimumLength =5, ErrorMessage = "Assignment name is too long.")]
        [Index("IX_Unique_Assignment", IsUnique = true)]
        public string AssignmentName { get; set; }

        public virtual ICollection<Shift> Shift { get; set; }

        public virtual ICollection<Member> Members { get; set; }
    }
}