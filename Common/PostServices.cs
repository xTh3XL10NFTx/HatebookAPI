using System.Text.Json.Serialization;
using System.Text.Json;

namespace Hatebook.Common
{
    public class PostServices
    {
        public readonly IControllerConstructor _dependency;
        public PostServices(IControllerConstructor dependency)
        { _dependency = dependency; }

        public IActionResult CreatePostService(PostDto model, string userIdFromRequest)
        {
            // Get the user information from the request
            var userId = GetUserIdFromRequest(userIdFromRequest);

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
            return new OkObjectResult(post);
        }

        public async Task<IActionResult> CreatePostInGroupService(string groupName, PostDto model, string userIdFromRequest)
        {
            GroupsModel group = await GetModelByNameService(groupName);

            // Get the user information from the request
            var userId = GetUserIdFromRequest(userIdFromRequest);

            if (group == null)
            {
                return new BadRequestObjectResult("Group not found");
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
            return new OkObjectResult(post);
        }

        public IActionResult LikePostService(int postId, string userIdFromRequest)
        {
            // Retrieve the post from the database using the postId
            var post = _dependency.Context.Posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return new BadRequestObjectResult("Error liking the post.");
            }

            // Get the user information from the request
            var userId = GetUserIdFromRequest(userIdFromRequest);
            // Create a new like entity and save it to the database

            // Check if the user has already liked the post
            var existingLike = _dependency.Context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };
            if (existingLike != null)
            {        // Remove the existing like from the database
                _dependency.Context.Likes.Remove(existingLike);
                _dependency.Context.SaveChanges();
                return new BadRequestObjectResult("Unliked");
            }

            _dependency.Context.Likes.Add(like);
            _dependency.Context.SaveChanges();

            return new OkObjectResult("Liked");
        }

        public IActionResult AddCommentService(int postId, CommentDto model, string userIdFromRequest)
        {
            // Retrieve the post from the database using the postId
            var post = _dependency.Context.Posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return new BadRequestObjectResult("Error in finding the post.");
            }

            // Get the user information from the request
            var userId = GetUserIdFromRequest(userIdFromRequest);

            // Create a new comment entity and save it to the database
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = model.Content
            };

            _dependency.Context.Comments.Add(comment);
            _dependency.Context.SaveChanges();

            return new OkObjectResult("Post commented successfully.");
        }

        public IActionResult GetPostsService()
        {
            // Retrieve the posts from the database including the Likes navigation property
            var posts = _dependency.Context.Posts.Include(p => p.Likes).Include(p => p.Comments).ToList();

            // Configure the JSON serializer options
            var serializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            // Serialize the posts to JSON using the configured options
            var json = JsonSerializer.Serialize(posts, serializerOptions);

            // Return the JSON response
            return new OkObjectResult(json);
        }

        public IActionResult GetLikesService()
        {
            // Retrieve all posts from the database
            var likes = _dependency.Context.Likes.ToList();

            return new OkObjectResult(likes);
        }

        public IActionResult GetCommentsService()
        {
            // Retrieve all posts from the database
            var comments = _dependency.Context.Comments.ToList();

            return new OkObjectResult(comments);
        }

        // Helper method to get the user ID from the request
        private string? GetUserIdFromRequest(string userIdFromRequest)
        {
            return _dependency.Context.Users.SingleOrDefault(u => u.Email == userIdFromRequest)?.Id;
        }

        public async Task<GroupsModel?> GetModelByNameService(string name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == name);
            return dbUser;
        }
    }
}
