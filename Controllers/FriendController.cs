using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : DependencyInjection
    {
        public FriendController(IControllerConstructor dependency) : base(dependency) { }

        //[HttpGet("allFriends")]
        //public async Task<ActionResult<List<GroupsModel>>> Get()=> await _usersInGroupsServices.GetService();

        [Authorize]
        [HttpPost("addFriend")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFriend([FromBody] FriendsList friend)
        {
            friend.Id = Guid.NewGuid();

            string loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            friend.Sender = loggedEmail;

            friend.Status = "Pending";

            // Set the UserEmail1 and UserEmail2 properties using the provided email values
            friend.Sender = _dependency.Context.Users.SingleOrDefault(u => u.Email == friend.Sender)?.Id;
            friend.Reciver = _dependency.Context.Users.SingleOrDefault(u => u.Email == friend.Reciver)?.Id;

            friend.CreatorId = friend.Sender;

            // Check if the users exist and save the friend to the database
            if (friend.Sender != null && friend.Reciver != null)
            {
                // Check if the user is making a friend request to themselves
                if (friend.Sender == friend.Reciver)
                {
                    return BadRequest("You cannot send a friend request to yourself.");
                }
                else
                {


                    if (_dependency.Context.Friends.SingleOrDefault(f => (f.Sender == friend.Sender || f.Reciver == friend.Sender) && f.Status == "Accepted" && (f.Sender == friend.Reciver || f.Reciver == friend.Reciver)) == null)
                    {
                        if (_dependency.Context.Friends.SingleOrDefault(f => (f.Sender == friend.Sender) && f.Status == "Pending" && (f.Reciver == friend.Reciver)) == null)
                        {
                            if (_dependency.Context.Friends.SingleOrDefault(f => ((f.Sender == friend.Sender && f.Reciver == friend.Reciver) || (f.Sender == friend.Reciver && f.Reciver == friend.Sender)) && f.Status == "Pending") == null)
                            {
                                _dependency.Context.Friends.Add(friend);
                                await _dependency.Context.SaveChangesAsync();
                                return Ok(friend);
                            }
                            else
                            {
                                var theFriend = _dependency.Context.Friends.SingleOrDefault(f => ((f.Sender == friend.Sender && f.Reciver == friend.Reciver) || (f.Sender == friend.Reciver && f.Reciver == friend.Sender)) && f.Status == "Pending");
                                theFriend.Status = "Accepted";
                                await _dependency.Context.SaveChangesAsync();

                                return Ok(theFriend + " Friend request " + friend.Status);
                            }
                        }
                        else
                        {
                            return BadRequest("You have already sent a friend request!");
                        }
                    }
                    else
                    {
                        return BadRequest("You are already friends!");
                    }
                }
            }
            else
            {
                return BadRequest("Invalid user email(s).");
            }
        }

        [Authorize]
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] string inputEmail)
        {
            // Retrieve the email from the authenticated user and his friend requests
            string loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string userEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            // Check if the email is different from the creator email

            if (loggedEmail == userEmail)
            {
                return BadRequest("You do not have a permission to accept this friend request.");
            }

            var friend = await _dependency.Context.Friends
                .FirstOrDefaultAsync(f => f.Status == "Pending" && f.Sender == userEmail);

            if (friend == null)
            {
                return NotFound("You do not have friend requests from that pereson.");
            }

            friend.Status = "Accepted";
            await _dependency.Context.SaveChangesAsync();

            return Ok(friend + " Friend request " + friend.Status);
        }

        [Authorize]
        [HttpPost("decline")]
        public async Task<IActionResult> DeclineFriendRequest([FromBody] string inputEmail)
        {
            // Retrieve the email from the authenticated user and his friend requests
            string loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string userEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            // Check if the email is different from the creator email

            if (loggedEmail == userEmail)
            {
                return BadRequest("You do not have a friend request from yourself.");
            }

            var friend = await _dependency.Context.Friends
    .FirstOrDefaultAsync(f => f.Status == "Pending" && f.Sender.Equals(userEmail));

            if (friend == null)
            {
                return NotFound("You do not have friend requests from that pereson.");
            }

            friend.Status = "Declined";

            _dependency.Context.Friends.Remove(friend);
            await _dependency.Context.SaveChangesAsync();

            return Ok("Request declined.");
        }

        [Authorize]
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            string loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            var friendRequests = _dependency.Context.Friends
    .Where(f => (f.Sender == loggedEmail || f.Reciver == loggedEmail) && f.Status == "Accepted")
    .Select(f => new
    {
        sender = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.Sender).Email,
        reciver = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.Reciver).Email,
        f.Status
    })
    .ToList();

            return Ok(friendRequests);
        }

        [Authorize]
        [HttpGet("friend-requests")]
        public async Task<IActionResult> GetFriendRequests()
        {
            string loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            var friendRequests = _dependency.Context.Friends
                .Where(f => (f.Reciver == loggedEmail) && f.Status == "Pending")
                .Select(f => new
                {
                    f.Id,
                    sender = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.Sender).Email,
                    reciver = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.Reciver).Email,
                    f.Status
                })
                .ToList();

            return Ok(friendRequests);
        }




        [Authorize]
        [HttpPost("removeFriend")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveFriend([FromBody] string inputEmail)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string inputEmailId = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            FriendsList friends = _dependency.Context.Friends.SingleOrDefault(f => (f.Sender == loggedEmail || f.Reciver == loggedEmail) && f.Status == "Accepted" && (f.Sender == inputEmailId || f.Reciver == inputEmailId));

            // Check if the users exist and save the friend to the database
            if (friends != null)
            {
                _dependency.Context.Friends.Remove(friends);
                await _dependency.Context.SaveChangesAsync();
                return Ok(inputEmail + " removed from friends.");
            }
            else
            {
                return BadRequest("Friendship does not exist.");
            }
        }


        //[HttpDelete("removeFriend")]
        //[GroupAdminAuthorization]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<IActionResult> RemoveUserFromGroup(string email, string groupName) => await _usersInGroupsServices.RemoveUserFromGroupService(email, groupName);
    }
}
