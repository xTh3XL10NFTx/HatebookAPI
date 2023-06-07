namespace Hatebook.Common
{
    public class FriendServces : DependencyInjection
    {
        public FriendServces(IControllerConstructor dependency) : base(dependency) { }

        public async Task<IActionResult> AddFriendService(FriendsList friend)
        {
            friend.Id = Guid.NewGuid();

            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (loggedEmail == null) return BadRequest("You have already sent a friend request!");

            friend.FriendRequestSender = loggedEmail;
            friend.Status = "Pending";

            var senderUser = _dependency.Context.Users.SingleOrDefault(u => u.Email == friend.FriendRequestSender);
            var receiverUser = _dependency.Context.Users.SingleOrDefault(u => u.Email == friend.FriendRequestReceiver);

            if (senderUser == null || receiverUser == null)
                return BadRequest("Invalid user email(s).");

            friend.FriendRequestSender = senderUser.Id;
            friend.FriendRequestReceiver = receiverUser.Id;

            friend.CreatorId = friend.FriendRequestSender;

            if (friend.FriendRequestSender == friend.FriendRequestReceiver)
                return BadRequest("You cannot send a friend request to yourself.");

            var existingFriend = await _dependency.Context.friends
                .FirstOrDefaultAsync(f => (f.FriendRequestSender == friend.FriendRequestSender || f.FriendRequestReceiver == friend.FriendRequestSender) && f.Status == "Accepted" && (f.FriendRequestSender == friend.FriendRequestReceiver || f.FriendRequestReceiver == friend.FriendRequestReceiver));

            if (existingFriend != null)
                return BadRequest("You are already friends!");

            var pendingRequest = await _dependency.Context.friends
                .FirstOrDefaultAsync(f => (f.FriendRequestSender == friend.FriendRequestSender) && f.Status == "Pending" && (f.FriendRequestReceiver == friend.FriendRequestReceiver));

            if (pendingRequest != null)
                return BadRequest("You have already sent a friend request!");

            var pendingReverseRequest = await _dependency.Context.friends
                .FirstOrDefaultAsync(f => ((f.FriendRequestSender == friend.FriendRequestSender && f.FriendRequestReceiver == friend.FriendRequestReceiver) || (f.FriendRequestSender == friend.FriendRequestReceiver && f.FriendRequestReceiver == friend.FriendRequestSender)) && f.Status == "Pending");

            if (pendingReverseRequest != null)
            {
                pendingReverseRequest.Status = "Accepted";
                await _dependency.Context.SaveChangesAsync();
                return Ok(pendingReverseRequest + " Friend request " + friend.Status);
            }

            var userRepository = _dependency.UnitOfWork.GetRepository<FriendsList>();
            await userRepository.Insert(friend);
            await _dependency.UnitOfWork.Save();

            return Ok(friend);
        }
        public async Task<IActionResult> AcceptFriendRequestService(string UserRequestedFriendship)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string? userEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == UserRequestedFriendship)?.Id;

            if (loggedEmail == userEmail)
                return BadRequest("You do not have a friend request from yourself.");

            var friend = await _dependency.Context.friends
                .FirstOrDefaultAsync(f => f.Status == "Pending" && f.FriendRequestReceiver == userEmail);

            if (friend == null)
                return NotFound("You do not have friend requests from that person.");

            var friendRepository = _dependency.UnitOfWork.GetRepository<FriendsList>();
            friend.Status = "Accepted";
            await _dependency.UnitOfWork.Save();

            return Ok(friend + " Friend request " + friend.Status);
        }

        public async Task<IActionResult> DeclineFriendRequestService(string UserRequestedFriendship)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string? userEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == UserRequestedFriendship)?.Id;

            if (loggedEmail == userEmail)
                return BadRequest("You do not have a friend request from yourself.");

            var friend = await _dependency.Context.friends
                .FirstOrDefaultAsync(f => (f.FriendRequestSender == loggedEmail || f.FriendRequestReceiver == loggedEmail) && f.Status == "Pending" && (f.FriendRequestSender == userEmail || f.FriendRequestReceiver == userEmail));

            if (friend == null)
                return NotFound("You do not have friend requests from that person.");

            var friendRepository = _dependency.UnitOfWork.GetRepository<FriendsList>();
            friend.Status = "Declined";
            await friendRepository.Delete(friend.Id);
            await _dependency.UnitOfWork.Save();

            return Ok("Request declined.");
        }
        public IActionResult GetFriendsService()
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            var friendRequests = _dependency.Context.friends
                .Where(f => (f.FriendRequestSender == loggedEmail || f.FriendRequestReceiver == loggedEmail) && f.Status == "Accepted")
                .Select(f => new
                {
                    UserEmail1 = GetUserEmail(f.FriendRequestSender),
                    UserEmail2 = GetUserEmail(f.FriendRequestReceiver)
                })
                .ToList();

            return Ok(friendRequests);
        }

        public IActionResult GetFriendRequestsService()
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            var friendRequests = _dependency.Context.friends
                .Where(f => f.FriendRequestReceiver == loggedEmail && f.Status == "Pending")
                .Select(f => new
                {
                    f.Id,
                    UserEmail1 = GetUserEmail(f.FriendRequestSender),
                    UserEmail2 = GetUserEmail(f.FriendRequestReceiver),
                    f.Status
                })
                .ToList();

            return Ok(friendRequests);
        }
        public async Task<IActionResult> RemoveFriendService(string friendToRemove)
        {
            string? loggedEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string? inputEmailId = _dependency.Context.Users.SingleOrDefault(u => u.Email == friendToRemove)?.Id;

            var friendRepository = _dependency.UnitOfWork.GetRepository<FriendsList>();
            var friends = await friendRepository.Get(f => (f.FriendRequestSender == loggedEmail || f.FriendRequestReceiver == loggedEmail) && f.Status == "Accepted" && (f.FriendRequestSender == inputEmailId || f.FriendRequestReceiver == inputEmailId));

            if (friends != null)
            {
                await friendRepository.Delete(friends.Id);
                await _dependency.UnitOfWork.Save();
                return Ok(friendToRemove + " removed from friends.");
            }
            else
            {
                return BadRequest("Friendship does not exist.");
            }
        }



        private string? GetUserEmail(string? userId)
        {
            var user = _dependency.Context.Users.SingleOrDefault(u => u.Id == userId);
            return user?.Email;
        }
    }
}
