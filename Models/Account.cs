using Hatebook.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HatebookUX.Models
{
    public class Account
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
}
