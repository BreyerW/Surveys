using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Surveys.Model
{
    public partial class User
    {
        public User()
        {
            Surveys = new HashSet<Survey>();
        }

        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required(ErrorMessage = "Hasło jest wymagane", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Potwierdź hasło")]
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare(nameof(Password), ErrorMessage = "Hasła się nie zgadzają")]
        public string ConfirmPassword { get; set; }
        public int? IdRole { get; set; }
        [Required]
        public string BirthYear { get; set; }
        [Required]
        public string Sex { get; set; }

        public virtual Role IdRoleNavigation { get; set; }
        public virtual ICollection<Survey> Surveys { get; set; }
    }
}
