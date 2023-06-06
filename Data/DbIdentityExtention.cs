using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Hatebook.Data
{
    public class DbIdentityExtention : IdentityUser
    {
        [Required]
        [DisplayName("First name")]
        public string? FirstName { get; set; }
        [DisplayName("Last name")]
        public string? LastName { get; set; }
        [DisplayName("Birthday")]
        public DateTime Birthday { get; set; }
        public Gender GenderType { get; set; }
        public enum Gender
        {
            [Display(Name = "Male", Description = "Male")]
            Male,
            [Display(Name = "Female", Description = "Female")]
            Female,
            [Display(Name = "Unknown", Description = "Unknown")]
            Unknown
        }

        public string? ProfilePicture { get; set; }
    }
}