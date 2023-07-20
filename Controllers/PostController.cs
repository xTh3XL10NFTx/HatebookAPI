using Hatebook.ControllerLogic;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/posts")]
    public class PostController : DependencyInjection
    {
        private readonly PostServices _postServices;
        public PostController(IControllerConstructor dependency, PostServices postServices) : base(dependency) => _postServices = postServices;

        [HttpPost("createPost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreatePost(PostDto model)
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (userEmail != null) return _postServices.CreatePostService(model, userEmail);
            else return BadRequest("You log in first.");
        }

        [HttpPost("createPost/{groupName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePostInGroupAsync(string groupName, PostDto model)
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (userEmail != null) return await _postServices.CreatePostInGroupService(groupName, model, userEmail);
            else return BadRequest("You log in first.");
        }

        [HttpPost("{postId}/like")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult LikePost(int postId)
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (userEmail != null) return _postServices.LikePostService(postId, userEmail);
            else return BadRequest("You log in first.");
        }

        [HttpPost("{postId}/comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddComment(int postId, CommentDto model)
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (userEmail != null) return _postServices.AddCommentService(postId, model, userEmail);
            else return BadRequest("You log in first.");
        }

        [HttpGet("posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPosts() => _postServices.GetPostsService();

        [HttpGet("likes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetLikes() => _postServices.GetLikesService();

        [HttpGet("comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetComments() => _postServices.GetCommentsService();
    }
}
