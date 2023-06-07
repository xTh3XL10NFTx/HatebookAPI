using Hatebook.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : DependencyInjection
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext, IControllerConstructor dependency) : base(dependency)
        {
            _hubContext = hubContext;
        }

        [HttpPost("allChat")]
        public async Task<IActionResult> SendMessage(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
            return Ok();
        }

        [Authorize]
        [HttpPost("group/{groupName}")]
        public async Task<IActionResult> SendMessageToGroup(string groupName, string message)
        {
            string? user = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;


            string? userId = _dependency.Context.Users.SingleOrDefault(u => u.Email == user)?.Id;
            Guid? groupId = _dependency.Context.groups.SingleOrDefault(u => u.Name == groupName)?.Id;

            if (groupId == null)
            {
                return BadRequest("Group does not exist.");
            }


            if (!_dependency.Context.usersInGroups
        .Any(ug => ug.UserId == userId && ug.GroupId == groupId))
            {
                return BadRequest("User is not in this group. You need to enter the group first.");
            }

            await _hubContext.Clients.Group(groupId?.ToString() ?? "").SendAsync("ReceiveMessage", user, message);
            return Ok();
        }

        [Authorize]
        [HttpPost("friend/{friendName}")]
        public async Task<IActionResult> SendMessageToFriend(string friendName, string message)
        {
            string? user = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            string? userId = _dependency.Context.Users.SingleOrDefault(u => u.Email == user)?.Id;

            if (userId == null)
            {
                return BadRequest("User does not exist.");
            }

            string? friendId = _dependency.Context.Users.SingleOrDefault(u => u.Email == friendName)?.Id;

            if (friendId == null)
            {
                return BadRequest("User does not exist.");
            }

            // Check if the sender and receiver are friends
            bool areFriends = _dependency.Context.friends.Any(f =>
                (f.FriendRequestSender == userId && f.FriendRequestReceiver == friendId) ||
                (f.FriendRequestSender == friendId && f.FriendRequestReceiver == userId));

            if (!areFriends)
            {
                // Sender and receiver are not authorized to send messages to each other
                return BadRequest("You need to be friedns in order to send messages to eachother.");
            }

            // Create a unique group ID for the friend chat
            string groupId = GetFriendGroupId(userId, friendId);

            // Send the received message to the friend's group
            await _hubContext.Clients.Group(groupId).SendAsync("ReceiveMessage", user, message);

            return Ok();
        }

        // Helper method to generate a unique group ID for friend chat
        private string GetFriendGroupId(string senderId, string receiverId)
        {
            // Sort the user IDs to ensure consistency in generating the group ID
            string[] sortedIds = new[] { senderId, receiverId }.OrderBy(id => id).ToArray();

            return $"FriendChat_{sortedIds[0]}_{sortedIds[1]}";
        }
    }
}
