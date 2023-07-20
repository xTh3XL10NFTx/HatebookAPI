using Hatebook.ControllerLogic;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FriendController : DependencyInjection
    {
        private readonly FriendServces _friendServices;
        public FriendController(IControllerConstructor dependency, FriendServces friendServices) : base(dependency) => _friendServices = friendServices;

        [HttpPost("addFriend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFriend([FromBody] FriendsList friend)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (loggedEmail != null) return await _friendServices.AddFriendService(friend, loggedEmail);
            else return BadRequest("You log in first.");
        }

        [HttpPost("accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] string inputEmail)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (loggedEmail != null) return await _friendServices.AcceptFriendRequestService(inputEmail, loggedEmail);
            else return BadRequest("You log in first.");
        }

        [HttpPost("decline")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeclineFriendRequest([FromBody] string inputEmail)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (loggedEmail != null) return await _friendServices.DeclineFriendRequestService(inputEmail, loggedEmail);
            else return BadRequest("You log in first.");
        }

        [HttpGet("friends")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetFriends()
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            return _friendServices.GetFriendsService(loggedEmail);
        }

        [HttpGet("friend-requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetFriendRequests()
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            return _friendServices.GetFriendRequestsService(loggedEmail);
        }

        [HttpPost("removeFriend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveFriend([FromBody] string inputEmail)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (loggedEmail != null) return await _friendServices.RemoveFriendService(inputEmail, loggedEmail);
            else return BadRequest("You log in first.");
        }
    }
}