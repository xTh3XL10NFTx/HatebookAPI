using System.ComponentModel.DataAnnotations;

namespace Hatebook.Models
{
    public class GroupPosts
    {
        [Key]
        public string GroupPostsId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorId { get; set; }
    }
}