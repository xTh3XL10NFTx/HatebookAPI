using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Hatebook.Data
{
    public class DbIdentityExtention : IdentityUser
    {
        [Required]
        [DisplayName("First name")]
        public string Fname { get; set; }
        [DisplayName("Last name")]
        public string Lname { get; set; }
        [DisplayName("Birthday")]
        public DateTime Bday { get; set; }
        public Gender GenderType { get; set; }

        public enum Gender
        {
            [Display(Name = "Man")]
            Man,
            [Display(Name = "Woman")]
            Woman
        }
        public string? ProfilePic { get; set; }
    }
}