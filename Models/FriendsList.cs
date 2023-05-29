using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hatebook.Models
{
    public class FriendsList
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
        public string Status { get; set; } // Pending, Accepted, Declined
        public string CreatorId { get; set; }

        public FriendsList()
        {
            Id = Guid.NewGuid();
        }
    }
}
