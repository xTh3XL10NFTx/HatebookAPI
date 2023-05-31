using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Hatebook.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController : DependencyInjection
    {

        public PostsController(IControllerConstructor dependency) : base(dependency) { }

        [HttpPost]
        public IActionResult CreatePost(PostDto model)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the user information from the request
            var userId = GetUserIdFromRequest();

            // Create a new post entity
            Post post = new Post
            {
                Content = model.Content,
                ImageUrl = model.ImageUrl,
                Timestamp = DateTime.Now,
                UserId = userId
            };

            // Save the new post to the database
            _dependency.Context.Posts.Add(post);
            _dependency.Context.SaveChanges();

            // Return the created post entity
            return Ok(post);
        }


        [HttpPost("{postId}/like")]
        public IActionResult LikePost(int postId)
        {
            // Retrieve the post from the database using the postId
            var post = _dependency.Context.Posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return NotFound();
            }

            // Get the user information from the request
            var userId = GetUserIdFromRequest();

            // Check if the user has already liked the post
            var existingLike = _dependency.Context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                return BadRequest("User has already liked this post.");
            }

            // Create a new like entity and save it to the database
            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };

            _dependency.Context.Likes.Add(like);
            _dependency.Context.SaveChanges();

            return Ok();
        }

        [HttpPost("{postId}/comment")]
        public IActionResult AddComment(int postId, CommentDto model)
        {
            // Retrieve the post from the database using the postId
            var post = _dependency.Context.Posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return NotFound();
            }

            // Get the user information from the request
            var userId = GetUserIdFromRequest();

            // Create a new comment entity and save it to the database
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = model.Content
            };

            _dependency.Context.Comments.Add(comment);
            _dependency.Context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetPosts()
        {
            // Retrieve all posts from the database
            var posts = _dependency.Context.Posts.ToList();

            return Ok(posts);
        }

        // Other controller methods for updating and deleting posts

        // Helper method to get the user ID from the request
        private string GetUserIdFromRequest()
        {
            string userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            //return _dependency.Context.Users.SingleOrDefault(u => u.Email == userEmail)?.Id;
            return userEmail;
            // Implement your logic to retrieve the user ID from the request
        }
    }


}
