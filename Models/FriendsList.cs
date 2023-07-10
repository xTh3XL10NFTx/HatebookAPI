using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Hatebook.Models
{
    public class FriendsList
    {
        [Key]
        public Guid Id { get; set; }

        public string? Sender { get; set; }
        public string? Reciver { get; set; }
        public string? Status { get; set; } // Pending, Accepted, Declined
        public string? CreatorId { get; set; }

        public FriendsList()
        {
            Id = Guid.NewGuid();
        }
    }
}
