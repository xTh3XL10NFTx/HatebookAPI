using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : DependencyInjection
    {

        public PostController(IControllerConstructor dependency) : base(dependency) { }

        [Authorize]
        [HttpPost("createPost")]
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
            _dependency.Context.posts.Add(post);
            _dependency.Context.SaveChanges();

            // Return the created post entity
            return Ok(post);
        }

        /* 
        [Authorize]
        [HttpPost("createPost/{groupName}")]
        public async Task<IActionResult> CreatePostInGroupAsync(string groupName, PostDto model)
        {
            GroupsModel group = await GetModelByNameService(groupName);
            // Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the user information from the request
            var userId = GetUserIdFromRequest();

            if (group == null)
            {
                return NotFound("Group not found");
            }

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
        */

        [Authorize]
        [HttpPost("{postId}/like")]
        public IActionResult LikePost(int postId)
        {
            // Retrieve the post from the database using the postId
            var post = _dependency.Context.posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return NotFound();
            }

            // Get the user information from the request
            var userId = GetUserIdFromRequest();
            // Create a new like entity and save it to the database

            // Check if the user has already liked the post
            var existingLike = _dependency.Context.likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);




            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };
            if (existingLike != null)
            {        // Remove the existing like from the database
                _dependency.Context.likes.Remove(existingLike);
                _dependency.Context.SaveChanges();
                return BadRequest("Unliked");
            }

            _dependency.Context.likes.Add(like);
            _dependency.Context.SaveChanges();

            return Ok("Liked");
        }

        [Authorize]
        [HttpPost("{postId}/comment")]
        public IActionResult AddComment(int postId, CommentDto model)
        {
            // Retrieve the post from the database using the postId
            var post = _dependency.Context.posts.FirstOrDefault(p => p.Id == postId);

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

            _dependency.Context.comments.Add(comment);
            _dependency.Context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpGet("posts")]
        public IActionResult GetPosts()
        {
            // Retrieve the posts from the database including the Likes navigation property
            var posts = _dependency.Context.posts.Include(p => p.Likes).Include(p => p.Comments).ToList();

            // Configure the JSON serializer options
            var serializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            // Serialize the posts to JSON using the configured options
            var json = JsonSerializer.Serialize(posts, serializerOptions);

            // Return the JSON response
            return Ok(json);
        }

        [Authorize]
        [HttpGet("likes")]
        public IActionResult GetLikes()
        {
            // Retrieve all posts from the database
            var likes = _dependency.Context.likes.ToList();

            return Ok(likes);
        }
        [HttpGet("comments")]
        public IActionResult GetComments()
        {
            // Retrieve all posts from the database
            var comments = _dependency.Context.comments.ToList();

            return Ok(comments);
        }

        // Other controller methods for updating and deleting posts

        // Helper method to get the user ID from the request
        private string? GetUserIdFromRequest()
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            return _dependency.Context.Users.SingleOrDefault(u => u.Email == userEmail)?.Id;
            // Implement your logic to retrieve the user ID from the request
        }



        [HttpGet]
        public async Task<GroupsModel?> GetModelByNameService(string name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == name);
            return dbUser;
        }
    }


}
