using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hatebook.Models
{
    public class UsersInGroups
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(DbIdentityExtention))]
        public string? UserId { get; set; }
        public DbIdentityExtention? DbIdentityExtention { get; set; }

        [ForeignKey(nameof(GroupsModel))]
        public Guid GroupId { get; set; }
        public GroupsModel? GroupsModel { get; set; }
        public UsersInGroups()
        {
            Id = Guid.NewGuid();
        }
    }
}
