using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hatebook
{
    public class HatebookLogin
    {
        //login part
        [Key]
        [Required]
        [DisplayName("E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "The password must be of minimum 6 characters!")]
        public string Password { get; set; }
    }


    public class Hatebook : HatebookLogin
    {
        [Required]
        [DisplayName("First name")]
        public string Fname { get; set; }
        [DisplayName("Last name")]
        public string Lname { get; set; }
        [DisplayName("Birthday")]

        [DataType(DataType.Date)]
        public DateTime Bday { get; set; }
        public Gender GenderType { get; set; }

        //[DisplayName("Password repeat")]

        //public string PassRepeat { get; set; }

        public enum Gender
        {
            [Display(Name = "Man")]
            Man,
            [Display(Name = "Woman")]
            Woman
        }
        [DataType(DataType.ImageUrl)]
        public string? ProfilePic { get; set; }
    }
}
