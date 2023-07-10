using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Hatebook.Models
{
    public class EditUserModel
    {
        [DisplayName("First name")]
        public string? FirstName { get; set; }

        [DisplayName("Last name")]
        public string? LastName { get; set; }

        [DisplayName("Birthday")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [JsonConverter(typeof(GenderConverter))]
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

        [DataType(DataType.ImageUrl)]
        public string? ProfilePicture { get; set; }

        public ICollection<Role> Roles { get; set; }
        public EditUserModel()
        {
            Roles = new List<Role>();
        }

        public class Role
        {
            public string? Name { get; set; }
        }
    }
}
