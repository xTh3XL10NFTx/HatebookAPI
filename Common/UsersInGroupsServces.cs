namespace Hatebook.Common
{
    public class UsersInGroupsServces : DependencyInjection
    {
        public UsersInGroupsServces(IControllerConstructor dependency) : base(dependency) { }

        public async Task<IActionResult> MoveUserToGroupService(string email, string groupName)
        {
            var user = await _dependency.UnitOfWork.GetRepository<DbIdentityExtention>().Get(u => u.Email == email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var group = await _dependency.UnitOfWork.GetRepository<GroupsModel>().Get(g => g.Name == groupName);

            if (group == null)
            {
                return BadRequest("Group not found");
            }

            var userInGroup = new UsersInGroups
            {
                UserId = user.Email,
                DbIdentityExtention = user,
                GroupId = group.Id,
                GroupsModel = group
            };

            await _dependency.UnitOfWork.GetRepository<UsersInGroups>().Insert(userInGroup);
            await _dependency.UnitOfWork.Save();

            return Ok("User added to group.");
        }

        public async Task<IActionResult> MoveUserToAdminService(string email, string groupName)
        {
            var user = await _dependency.UnitOfWork.GetRepository<DbIdentityExtention>().Get(u => u.Email == email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var group = await _dependency.UnitOfWork.GetRepository<GroupsModel>().Get(g => g.Name == groupName);

            if (group == null)
            {
                return BadRequest("Group not found");
            }

            var groupAdmins = new GroupAdmins
            {
                UserId = user.Email,
                DbIdentityExtention = user,
                GroupId = group.Id,
                GroupsModel = group
            };

            await _dependency.UnitOfWork.GetRepository<GroupAdmins>().Insert(groupAdmins);
            await _dependency.UnitOfWork.Save();

            return Ok("User changed to admin.");
        }

        public async Task<IActionResult> RemoveUserFromGroupService(string email, string groupName)
        {
            var user = await _dependency.UnitOfWork.GetRepository<DbIdentityExtention>().Get(u => u.Email == email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var group = await _dependency.UnitOfWork.GetRepository<GroupsModel>().Get(g => g.Name == groupName);

            if (group == null)
            {
                return BadRequest("Group not found");
            }

            var userInGroup = await _dependency.UnitOfWork.GetRepository<UsersInGroups>().Get(u => u.UserId == user.Id && u.GroupsModel != null && u.GroupsModel.Id == group.Id);

            if (userInGroup == null)
            {
                return BadRequest("User is not a member of the group");
            }

            await _dependency.UnitOfWork.GetRepository<UsersInGroups>().Delete(userInGroup.Id);
            await _dependency.UnitOfWork.Save();

            return Ok("User " + email + " deleted from group " + groupName + " successfully!");
        }

        public async Task<ActionResult<List<GroupsModel>>> GetService()
        {
            var usersInGroups = await _dependency.UnitOfWork.GetRepository<UsersInGroups>().GetAll(
                includes: new List<string> { "DbIdentityExtention", "GroupsModel" });

            var result = usersInGroups.Select(u => $"{u.DbIdentityExtention?.Email}, {u.GroupsModel?.Name}").ToList();

            return Ok(result);
        }
    }
}