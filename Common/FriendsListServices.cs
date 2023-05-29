namespace Hatebook.Common
{
    public class FriendsListServces: DependencyInjection
    {
        public FriendsListServces(IControllerConstructor dependency) : base(dependency) { }



        public async Task<IActionResult> MoveUserToGroupService(string email, string groupName)
        {
            var user = await _dependency.Context.dbIdentityExtentions.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found");
            }

            var group = await _dependency.Context.groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null)
            {
                return new BadRequestObjectResult("Group not found");
            }

            var userInGroup = new UsersInGroups
            {
                UserId              = user.Email,
                DbIdentityExtention = user,
                GroupId             = group.Id,
                GroupsModel         = group
            };

            _dependency.Context.manyToMany.Add(userInGroup);
            await _dependency.Context.SaveChangesAsync();

            return new OkObjectResult("User added to group.");
        }
        
        public async Task<IActionResult> MoveUserToAdminService(string email, string groupName)
        {
            var user = await _dependency.Context.dbIdentityExtentions.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return new BadRequestObjectResult("User not found");

            var group = await _dependency.Context.groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null) return new BadRequestObjectResult("Group not found");

            var groupAdmins = new GroupAdmins
            {
                UserId              = user.Email,
                DbIdentityExtention = user,
                GroupId             = group.Id,
                GroupsModel         = group
            };

            _dependency.Context.GroupAdmins.Add(groupAdmins);
            await _dependency.Context.SaveChangesAsync();

            return new OkObjectResult("User changed to admin.");
        }
        public async Task<IActionResult> RemoveUserFromGroupService(string email, string groupName)
        {
            var user = await _dependency.Context.dbIdentityExtentions.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found");
            }

            var group = await _dependency.Context.groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null)
            {
                return new BadRequestObjectResult("Group not found");
            }

            var userInGroup = await _dependency.Context.manyToMany.FirstOrDefaultAsync(u => u.UserId == user.Id && u.GroupsModel.Id == group.Id);
            if (userInGroup == null)
            {
                return new BadRequestObjectResult("User is not a member of the group");
            }

            _dependency.Context.manyToMany.Remove(userInGroup);
            await _dependency.Context.SaveChangesAsync();
            return new OkObjectResult("User " + email + " deleted from group " + groupName + " successfully!");
        }
        public async Task<ActionResult<List<GroupsModel>>> GetService()
        {
            var usersInGroups = await _dependency.Context.manyToMany
                .Include(u => u.DbIdentityExtention)
                .Include(u => u.GroupsModel)
                .ToListAsync();

            var result = usersInGroups.Select(u => $"{u.DbIdentityExtention?.Email}, {u.GroupsModel?.Name}").ToList();
            return new OkObjectResult(result);
        }
    }
}
