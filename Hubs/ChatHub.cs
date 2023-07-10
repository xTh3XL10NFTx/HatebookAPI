using AutoMapper.Execution;
using Microsoft.AspNetCore.SignalR;

namespace Hatebook.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IControllerConstructor _dependency;
        public ChatHub(IControllerConstructor dependency) => _dependency = dependency;

        public string userName = "";
        public async Task JoinGroupChat(string groupNameVar, string iuserName)
        {
            Guid? groupId = _dependency.Context.groups.SingleOrDefault(u => u.Name == groupNameVar)?.Id;

            // Check if the group exists
            if (groupId==null)
            {
                new BadRequestObjectResult("Error");
                return;
            }

            userName = iuserName;
            string? groupName = groupId.ToString();

            if (groupName == null)
            {
                new BadRequestObjectResult("Error");
                return;
            }

            // Add the connection to the group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Send a welcome message to the user
            await Clients.Caller.SendAsync("ReceiveSystemMessage", $"You have joined the group chat ({groupName}).");

            // Send a notification to the group about the new member
            await Clients.Group(groupName).SendAsync("ReceiveSystemMessage", $"{userName} has joined the group chat ({groupName}).");
        }

        public async Task JoinFriendChat(string friendNameVar, string iuserName)
        {
            string? friendId = _dependency.Context.Users.SingleOrDefault(u => u.Email == friendNameVar)?.Id;
            string? userNameId = _dependency.Context.Users.SingleOrDefault(u => u.Email == iuserName)?.Id;
            if (userNameId == null || friendId == null)
            {
                new BadRequestObjectResult("Error");
                return;
            }
            // Implement your logic to check if the user is friends with the given friendId
            bool areFriends = _dependency.Context.Friends.Any(f =>
                (f.Sender == userNameId && f.Reciver == friendId) ||
                (f.Sender == friendId && f.Reciver == userNameId));

            if (!areFriends)
            {
                // Users are not friends
                return;
            }

            string groupId = GetFriendGroupId(userNameId, friendId);

            // Add the connection to the group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

            // Send a welcome message to the user
            await Clients.Caller.SendAsync("ReceiveSystemMessage", $"You have joined the friend chat with {friendNameVar}.");

            // Send a notification to the group about the new member
            await Clients.Group(groupId).SendAsync("ReceiveSystemMessage", $"{userName} has joined the friend chat.");
        }

        public async Task SendMessage(string user, string message)
        {
            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToGroup(string message, Guid groupId)
        {
            // Check if the user is part of the group
            bool isMember = _dependency.Context.usersInGroups.Any(ug => ug.UserId == userName && ug.GroupId == groupId);
            if (!isMember)
            {
                new BadRequestObjectResult("Error");
                return;
            }

            // Send the received message to the group
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", userName, message);
        }

        public async Task SendMessageToFriend(string message, string friendName)
        {
            string? userNameId = _dependency.Context.Users.SingleOrDefault(u => u.Email == userName)?.Id;
            string? friendId = _dependency.Context.Users.SingleOrDefault(u => u.Email == friendName)?.Id;
            if (userNameId == null || friendId == null)
            {
                new BadRequestObjectResult("Error");
                return;
            }
            // Check if the sender and receiver are friends
            bool areFriends = _dependency.Context.Friends.Any(f =>
                (f.Sender == userNameId && f.Reciver == friendId) ||
                (f.Sender == friendId && f.Reciver == userNameId));

            if (!areFriends)
            {
                // Sender and receiver are not authorized to send messages to each other
                return;
            }

            string groupId = GetFriendGroupId(userNameId, friendId);

            // Send the received message to the friend's group
            await Clients.Group(groupId).SendAsync("ReceiveFriendMessage", userName, message);
        }

        public override async Task OnConnectedAsync()
        {
            // Get the groups the user belongs to
            var groupIds = _dependency.Context.usersInGroups
                .Where(ug => ug.UserId == userName)
                .Select(ug => ug.GroupId)
                .ToList();

            foreach (var groupId in groupIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveFromAllGroups(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
        private async Task RemoveFromAllGroups(string connectionId)
        {
            var groupList = GetGroupsForConnection(connectionId);

            foreach (var group in groupList)
            {
                await Groups.RemoveFromGroupAsync(connectionId, group);
            }
        }
        private List<string> GetGroupsForConnection(string connectionId)
        {
            var groupIds = _dependency.Context.usersInGroups
                .Where(ug => ug.DbIdentityExtention != null && ug.DbIdentityExtention.Id == connectionId && ug.GroupsModel != null)
                .Select(ug => ug.GroupsModel!.Id.ToString())
                .ToList();

            return groupIds;
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