using Hatebook.Hubs;
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

        [HttpPost]
        public async Task<IActionResult> SendMessage(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
            return Ok();
        }

        [HttpPost("group")]
        public async Task<IActionResult> SendMessageToGroup(Guid groupId, string user, string message)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", user, message);
            return Ok();
        }

        [HttpPost("friend")]
        public async Task<IActionResult> SendMessageToFriend(string userId, string friendId, string user, string message)
        {
            string connectionId = friendId; // Implement your logic to retrieve the friend's connection ID
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message);
            return Ok();
        }
    }
}
