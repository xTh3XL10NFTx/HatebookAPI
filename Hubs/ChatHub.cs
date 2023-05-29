using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Hatebook.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IControllerConstructor _dependency;
        public ChatHub(IControllerConstructor dependency) => _dependency = dependency;

        public async Task SendMessageToGroup(Guid groupId, string user, string message)
        {
            // Check if the user is part of the group
            bool isMember = _dependency.Context.manyToMany.Any(ug => ug.UserId == user && ug.GroupId == groupId);
            if (!isMember)
            {
                // User is not authorized to send messages in this group
                return;
            }

            // Send the received message to the group
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToFriend(string senderId, string receiverId, string user, string message)
        {
            // Check if the sender and receiver are friends
            bool areFriends = _dependency.Context.Friends.Any(f =>
                (f.UserId1 == senderId && f.UserId2 == receiverId) ||
                (f.UserId1 == receiverId && f.UserId2 == senderId));

            if (!areFriends)
            {
                // Sender and receiver are not authorized to send messages to each other
                return;
            }

            // Create a unique group ID for the friend chat
            string groupId = GetFriendGroupId(senderId, receiverId);

            // Send the received message to the friend's group
            await Clients.Group(groupId).SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.UserIdentifier;

            // Get the groups the user belongs to
            var groupIds = _dependency.Context.manyToMany
                .Where(ug => ug.UserId == userId)
                .Select(ug => ug.GroupId)
                .ToList();

            foreach (var groupId in groupIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromAllGroups(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
        private async Task RemoveFromAllGroups(string connectionId)
        {
            var groupList = await GetGroupsForConnection(connectionId);

            foreach (var group in groupList)
            {
                await Groups.RemoveFromGroupAsync(connectionId, group);
            }
        }
        private async Task<List<string>> GetGroupsForConnection(string connectionId)
        {
            return _dependency.Context.manyToMany
                .Where(ug => ug.DbIdentityExtention.Id == connectionId)
                .Select(ug => ug.GroupsModel.Id.ToString())
                .ToList();
        }

        // Helper method to generate a unique group ID for friend chat
        private string GetFriendGroupId(string senderId, string receiverId)
        {
            // Sort the user IDs to ensure consistency in generating the group ID
            string[] sortedIds = new[] { senderId, receiverId }.OrderBy(id => id).ToArray();

            return $"FriendChat_{sortedIds[0]}_{sortedIds[1]}";
        }

        // Helper method to get all available groups
        private string[] GetAllGroups()
        {
            return _dependency.Context.groups.Select(g => g.Id.ToString()).ToArray();
        }
    }
}