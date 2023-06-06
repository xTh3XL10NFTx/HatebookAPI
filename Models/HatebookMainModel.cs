using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Hatebook.Models
{
    public class HatebookLogin
    {
        //login part
        [Key]
        [Required]
        [DisplayName("E-mail")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        //[MinLength(6, ErrorMessage = "The password must be of minimum 6 characters!")]
        public string? Password { get; set; }
    }

    public class HatebookMainModel : HatebookLogin
    {
        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("Birthday")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [JsonConverter(typeof(GenderConverter))]
        public Gender GenderType { get; set; }
        public enum Gender
        {
            [Display(Name = "Male", Description = "Male")]
            MainModelGenderMale,
            [Display(Name = "Female", Description = "Female")]
            MainModelGenderFemale,
            [Display(Name = "Unknown", Description = "Unknown")]
            MainModelGenderUnknown
        }

        [DataType(DataType.ImageUrl)]
        public string? ProfilePicture { get; set; }

        public ICollection<Role> Roles { get; set; }
        public HatebookMainModel()
        {
            Roles = new List<Role>();
        }
    }
    public class Role
    {
        public string? Name { get; set; }
    }
}
