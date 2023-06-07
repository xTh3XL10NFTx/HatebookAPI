using AutoMapper.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Hatebook.Common
{
    public class GroupServices : DependencyInjection
    {
        private readonly UsersInGroupsServces _usersInGroupsServices;
        public GroupServices(IControllerConstructor dependency, UsersInGroupsServces usersInGroupsServices) : base(dependency) => _usersInGroupsServices = usersInGroupsServices;
        public async Task<IActionResult> CreateGroupService(GroupsModel group, string claimsvalue)
        {

            var ifExists = await _dependency.UnitOfWork.GetRepository<GroupsModel>().Get(g => g.Name == group.Name);

            if (ifExists == null)
            {
                group.Id = Guid.NewGuid();

                if (claimsvalue != null)
                {
                    _dependency.Logger.LogInformation($"Registration Attempt for {claimsvalue} ");

                    try
                    {
                        group.CreatorId = claimsvalue;
                        await _dependency.UnitOfWork.GetRepository<GroupsModel>().Insert(group);
                        await _dependency.UnitOfWork.Save();

                        if (group.Name == null)
                        {
                            return BadRequest("Error: Group.name");
                        }

                        await _usersInGroupsServices.MoveUserToGroupService(claimsvalue, group.Name);
                        await _usersInGroupsServices.MoveUserToAdminService(claimsvalue, group.Name);

                        return Ok(group);
                    }
                    catch (Exception ex)
                    {
                        _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(CreateGroupService)}: Such group already exists.");
                        // return Problem($"Something Went Wrong in the {nameof(CreateGroup)}: Such group already exists.", statusCode: 500);
                    }
                }

                return BadRequest("Claim not found.");
            }

            return BadRequest("A group with this name already exists.");
        }

        public async Task<ActionResult> GetGroupByNameService(string Name)
        {
            var group = await _dependency.UnitOfWork.GetRepository<GroupsModel>().Get(g => g.Name == Name);

            if (group == null)
            {
                return BadRequest("Group not found");
            }

            return Ok(group);
        }

        public async Task<ActionResult> DeleteGroupService(string Name)
        {

            var group = await _dependency.UnitOfWork.GetRepository<GroupsModel>().Get(g => g.Name == Name);

            if (group == null)
            {
                return BadRequest("Group not found");
            }

            await _dependency.UnitOfWork.GetRepository<GroupsModel>().Delete(group.Id);
            await _dependency.UnitOfWork.Save();

            return Ok("Group " + Name + " deleted successfully!");
        }
        public async Task<IActionResult> EditGroupService(GroupsModel request, string name)
        {
            var group = await _dependency.UnitOfWork.GetRepository<GroupsModel>().Get(g => g.Name == name);

            if (group == null)
            {
                return BadRequest("Group not found!");
            }
            group.Name = request.Name;
            group.Description = request.Description;
            group.CreatedDate = request.CreatedDate;
            group.CreatorId = request.CreatorId;

            _dependency.UnitOfWork.GetRepository<GroupsModel>().Update(group);
            await _dependency.UnitOfWork.Save();
            return Ok(group);
        }
    }
}