using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace crousseau2_College_Strike.Models
{
    public class Member : Auditable 
    {
        public Member()
        {
            this.Positions = new HashSet<Position>();
            this.Shift = new HashSet<Shift>();
        }

        [Display(Name = "Member")]
        public string FullName
        {
            get
            {
                return FirstName                   
                    + ' ' + LastName;
            }
        }

        public int id { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Enter your first name.")]
        [StringLength(50, ErrorMessage = "First Name is too long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Enter your last name.")]
        [StringLength(50, ErrorMessage = "Last Name is too long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:(###) ###-####}", ApplyFormatInEditMode = false)]
        public Int64 Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Index("IX_Unique_Member_email", IsUnique = true)]
        public string eMail { get; set; }

        [Required(ErrorMessage = "You must specify the assignment for the member.")]
        public int AssignmentID { get; set; }

        public virtual Assignment Assignment { get; set; }

        public virtual ICollection<Shift> Shift { get; set; }

        public virtual ICollection<Position> Positions { get; set; }
    }
}