using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Hatebook
{
    public class Hatebook
    {
        //login part
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("E-mail")]
        [MinLength(6, ErrorMessage = "Must enter valid e-mail address!")]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "The password must be of minimum 6 characters!")]
        public string Password { get; set; }

        //register part
        [Required]
        [DisplayName("First name")]
        public string Fname { get; set; }
        [DisplayName("Last name")]
        public string Lname { get; set; }
        [DisplayName("Birthday")]
        public DateTime Bday { get; set; }
        public Gender GenderType { get; set; }

        [DisplayName("Password repeat")]
            public string PassRepeat { get; set; }

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
