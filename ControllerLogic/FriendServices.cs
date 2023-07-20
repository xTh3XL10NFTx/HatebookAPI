namespace Hatebook.Common
{
    public class FriendServces
    {
        public readonly IControllerConstructor _dependency;
        public FriendServces(IControllerConstructor dependency)
        { _dependency = dependency; }

        public async Task<IActionResult> AddFriendService(FriendsList friend, string loggedEmail)
        {
            friend.Id = Guid.NewGuid();

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
                    return new BadRequestObjectResult("You cannot send a friend request to yourself.");
                }
                else
                {
                    if (_dependency.Context.Friends.SingleOrDefault(f =>
                    (f.Sender == friend.Sender || f.Reciver == friend.Sender) &&
                    f.Status == "Accepted" &&
                    (f.Sender == friend.Reciver || f.Reciver == friend.Reciver)) == null)
                    {
                        if (_dependency.Context.Friends.SingleOrDefault(f =>
                        (f.Sender == friend.Sender) &&
                        f.Status == "Pending" &&
                        (f.Reciver == friend.Reciver)) == null)
                        {
                            if (_dependency.Context.Friends.SingleOrDefault(f =>
                            ((f.Sender == friend.Sender &&
                            f.Reciver == friend.Reciver) ||
                            (f.Sender == friend.Reciver && f.Reciver == friend.Sender)) &&
                            f.Status == "Pending") == null)
                            {
                                _dependency.Context.Friends.Add(friend);
                                await _dependency.Context.SaveChangesAsync();
                                return new OkObjectResult(friend);
                            }
                            else
                            {
                                var theFriend = _dependency.Context.Friends.SingleOrDefault(f => ((f.Sender == friend.Sender && f.Reciver == friend.Reciver) || (f.Sender == friend.Reciver && f.Reciver == friend.Sender)) && f.Status == "Pending");
                                if (theFriend == null) return new BadRequestObjectResult("Error");
                                theFriend.Status = "Accepted";
                                await _dependency.Context.SaveChangesAsync();

                                return new OkObjectResult(theFriend + " Friend request " + friend.Status);
                            }
                        }
                        else
                        {
                            return new BadRequestObjectResult("You have already sent a friend request!");
                        }
                    }
                    else
                    {
                        return new BadRequestObjectResult("You are already friends!");
                    }
                }
            }
            else
            {
                return new BadRequestObjectResult("Invalid user email(s).");
            }
        }
        public async Task<IActionResult> AcceptFriendRequestService(string inputEmail, string? loggedEmail)
        {
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string? userEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            // Check if the email is different from the creator email
            if (loggedEmail == userEmail)
            {
                return new BadRequestObjectResult("You do not have a permission to accept this friend request.");
            }

            var friend = await _dependency.Context.Friends
                .FirstOrDefaultAsync(f => f.Status == "Pending" && f.Sender == userEmail);

            if (friend == null)
            {
                return new BadRequestObjectResult("You do not have friend requests from that pereson.");
            }

            friend.Status = "Accepted";
            await _dependency.Context.SaveChangesAsync();

            return new OkObjectResult(friend + " Friend request " + friend.Status);
        }

        public async Task<IActionResult> DeclineFriendRequestService(string inputEmail, string? loggedEmail)
        {
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string? userEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            // Check if the email is different from the creator email
            if (loggedEmail == userEmail)
            {
                return new BadRequestObjectResult("You do not have a friend request from yourself.");
            }

            var friend = await _dependency.Context.Friends
                .FirstOrDefaultAsync(f => f.Status == "Pending" && f.Sender.Equals(userEmail));

            if (friend == null)
            {
                return new BadRequestObjectResult("You do not have friend requests from that pereson.");
            }

            friend.Status = "Declined";

            _dependency.Context.Friends.Remove(friend);
            await _dependency.Context.SaveChangesAsync();

            return new OkObjectResult("Request declined.");
        }
        public IActionResult GetFriendsService(string? loggedEmail)
        {
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

            return new OkObjectResult(friendRequests);
        }

        public IActionResult GetFriendRequestsService(string? loggedEmail)
        {
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

            return new OkObjectResult(friendRequests);
        }
        public async Task<IActionResult> RemoveFriendService(string inputEmail, string? loggedEmail)
        {
            loggedEmail = _dependency.Context.Users.SingleOrDefault(u => u.Email == loggedEmail)?.Id;

            string? inputEmailId = _dependency.Context.Users.SingleOrDefault(u => u.Email == inputEmail)?.Id;

            FriendsList? friends = _dependency.Context.Friends.SingleOrDefault(f =>
            (f.Sender == loggedEmail || f.Reciver == loggedEmail) &&
            f.Status == "Accepted" &&
            (f.Sender == inputEmailId || f.Reciver == inputEmailId));

            // Check if the users exist and save the friend to the database
            if (friends != null)
            {
                _dependency.Context.Friends.Remove(friends);
                await _dependency.Context.SaveChangesAsync();
                return new OkObjectResult(inputEmail + " removed from friends.");
            }
            else
            {
                return new BadRequestObjectResult("Friendship does not exist.");
            }
        }
    }
}