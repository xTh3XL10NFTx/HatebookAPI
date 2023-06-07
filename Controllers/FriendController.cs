using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : DependencyInjection
    {
        private readonly FriendServces _friendServces;
        public FriendController(IControllerConstructor dependency, FriendServces friendServces) : base(dependency) { _friendServces = friendServces; }

        [Authorize]
        [HttpPost("addFriend")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFriend([FromBody] FriendsList friend) => await _friendServces.AddFriendService(friend);

        [Authorize]
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest(string inputEmail) => await _friendServces.AcceptFriendRequestService(inputEmail);

        [Authorize]
        [HttpPost("decline")]
        public async Task<IActionResult> DeclineFriendRequest(string inputEmail) => await _friendServces.DeclineFriendRequestService(inputEmail);


        [Authorize]
        [HttpGet("friend-list")]
        public IActionResult GetFriendsList() => _friendServces.GetFriendsService();

        [Authorize]
        [HttpGet("friend-requests")]
        public IActionResult GetFriendRequests() => _friendServces.GetFriendRequestsService();

        [Authorize]
        [HttpPost("removeFriend")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveFriend(string inputEmail) => await _friendServces.RemoveFriendService(inputEmail);
    }
}
