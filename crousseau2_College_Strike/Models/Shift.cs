using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace crousseau2_College_Strike.Models
{
    public class Shift : Auditable
    {
        public int id { get; set; }

        [Display(Name = "Shift Date")]
        [Required(ErrorMessage = "You must specify the date for the shift.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Index("IX_Unique_Shift", Order = 2, IsUnique = true)]
        public DateTime ShiftDate { get; set; }

        [Required(ErrorMessage = "You must specify the member.")]
        [Index("IX_Unique_Shift", Order = 1, IsUnique = true)]
        public int MemberID { get; set; }

        public virtual Member Member { get; set; }

        [Required(ErrorMessage = "You must specify the assignment.")]
        public int AssignmentID { get; set; }

        public virtual Assignment Assignment { get; set; }
    }
}