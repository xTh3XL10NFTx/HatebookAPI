using System.ComponentModel.DataAnnotations;

namespace Hatebook.Models
{
    public class GroupsModel
    {
        [Key]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatorId { get; set; }
    }
}