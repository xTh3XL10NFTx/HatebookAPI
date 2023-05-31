using System.ComponentModel.DataAnnotations.Schema;

namespace Hatebook.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Timestamp { get; set; }
        [ForeignKey(nameof(DbIdentityExtention))]
        public string UserId { get; set; }
        public DbIdentityExtention DbIdentityExtention { get; set; }
        public ICollection<Like> Likes { get; set; } // Navigation property to the post's likes
        public ICollection<Comment> Comments { get; set; } // Navigation property to the post's comments
    }

    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; } // Assuming string type for user ID, change as per your setup

        public Post Post { get; set; } // Navigation property to the liked post
        [ForeignKey("UserId")]
        public HatebookMainModel User { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; } // Assuming string type for user ID, change as per your setup

        public Post Post { get; set; } // Navigation property to the commented post
        [ForeignKey("UserId")]
        public HatebookMainModel User { get; set; }
    }
}
