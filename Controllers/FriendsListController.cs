using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsListController : DependencyInjection
    {
        public FriendsListController(IControllerConstructor dependency) : base(dependency) { }

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

            friend.UserId1 = loggedEmail;

            friend.Status = "Pending";

            // Set the UserEmail1 and UserEmail2 properties using the provided email values
            friend.UserId1 = _dependency.Context.Users.SingleOrDefault(u => u.Email == friend.UserId1)?.Id;
            friend.UserId2 = _dependency.Context.Users.SingleOrDefault(u => u.Email == friend.UserId2)?.Id;

            friend.CreatorId = friend.UserId1;

            // Check if the users exist and save the friend to the database
            if (friend.UserId1 != null && friend.UserId2 != null)
            {
                // Check if the user is making a friend request to themselves
                if (friend.UserId1 == friend.UserId2)
                {
                    return BadRequest("You cannot send a friend request to yourself.");
                }
                else {


                if (_dependency.Context.friends.SingleOrDefault(f => (f.UserId1 == friend.UserId1 || f.UserId2 == friend.UserId1) && f.Status == "Accepted" && (f.UserId1 == friend.UserId2 || f.UserId2 == friend.UserId2)) == null)
                {
                    if (_dependency.Context.friends.SingleOrDefault(f => (f.UserId1 == friend.UserId1) && f.Status == "Pending" && (f.UserId2 == friend.UserId2)) == null)
                    {
                        if (_dependency.Context.friends.SingleOrDefault(f => ((f.UserId1 == friend.UserId1 && f.UserId2 == friend.UserId2) || (f.UserId1 == friend.UserId2 && f.UserId2 == friend.UserId1)) && f.Status == "Pending") == null)
                        {
                            _dependency.Context.friends.Add(friend);
                            await _dependency.Context.SaveChangesAsync();
                            return Ok(friend);
                        }
                        else {
                            var theFriend = _dependency.Context.friends.SingleOrDefault(f => ((f.UserId1 == friend.UserId1 && f.UserId2 == friend.UserId2) || (f.UserId1 == friend.UserId2 && f.UserId2 == friend.UserId1)) && f.Status == "Pending");
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
        public async Task<IActionResult> AcceptFriendRequest(string inputEmail)
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

            var friend = await _dependency.Context.friends
                .FirstOrDefaultAsync(f => f.Status == "Pending" && f.UserId2 == userEmail);

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
        public async Task<IActionResult> DeclineFriendRequest(string inputEmail)
        {
            // Retrieve the email from the authenticated user and his friend requests
            string loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string userEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            if (loggedEmail == userEmail)
            {
                return BadRequest("You do not have a friend request from yourself.");
            }

            var friend = await _dependency.Context.friends
                .FirstOrDefaultAsync(f => (f.UserId1 == loggedEmail || f.UserId2 == loggedEmail) && f.Status == "Pending" && (f.UserId1 == userEmail || f.UserId2 == userEmail));

            if (friend == null)
            {
                return NotFound("You do not have friend requests from that pereson.");
            }

            friend.Status = "Declined";

            _dependency.Context.friends.Remove(friend);
            await _dependency.Context.SaveChangesAsync();

            return Ok("Request declined.");
        }

        [Authorize]
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            string loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            var friendRequests = _dependency.Context.friends
    .Where(f => (f.UserId1 == loggedEmail || f.UserId2 == loggedEmail) && f.Status == "Accepted")
    .Select(f => new
    {
        UserEmail1 = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.UserId1).Email,
        UserEmail2 = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.UserId2).Email
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

            var friendRequests = _dependency.Context.friends
                .Where(f => (f.UserId2 == loggedEmail) && f.Status == "Pending")
                .Select(f => new
                {
                    f.Id,
                    UserEmail1 = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.UserId1).Email,
                    UserEmail2 = _dependency.Context.Users.SingleOrDefault(u => u.Id == f.UserId2).Email,
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
        public async Task<IActionResult> RemoveFriend(string inputEmail)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string inputEmailId = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            FriendsList friends = _dependency.Context.friends.SingleOrDefault(f => (f.UserId1 == loggedEmail || f.UserId2 == loggedEmail) && f.Status == "Accepted" && (f.UserId1 == inputEmailId || f.UserId2 == inputEmailId));

            // Check if the users exist and save the friend to the database
            if (friends != null)
            {
                _dependency.Context.friends.Remove(friends);
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




