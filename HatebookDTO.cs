using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Hatebook
{
    public class HatebookDTO
    {
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}
