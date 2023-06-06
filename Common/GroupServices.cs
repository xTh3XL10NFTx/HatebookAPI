namespace Hatebook.Common
{
    public class GroupServices : DependencyInjection
    {
        private readonly UsersInGroupsServces _usersInGroupsServices;
        public GroupServices(IControllerConstructor dependency, UsersInGroupsServces usersInGroupsServices) : base(dependency) => _usersInGroupsServices = usersInGroupsServices;
        public async Task<IActionResult> CreateGroupService(GroupsModel group, string claimsvalue)
        {

            var ifExists = await _unitOfWork.Groups.Get(g => g.Name == group.Name);

            if (ifExists == null)
            {
                group.Id = Guid.NewGuid();

                if (claimsvalue != null)
                {
                    _dependency.Logger.LogInformation($"Registration Attempt for {claimsvalue} ");

                    try
                    {
                        group.CreatorId = claimsvalue;
                        await _dependency.UnitOfWork.Groups.Insert(group);
                        await _dependency.UnitOfWork.Save();

                        await _usersInGroupsServices.MoveUserToGroupService(claimsvalue, group.Name);
                        await _usersInGroupsServices.MoveUserToAdminService(claimsvalue, group.Name);

                        return new OkObjectResult(group);
                    }
                    catch (Exception ex)
                    {
                        _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(CreateGroupService)}: Such group already exists.");
                        // return Problem($"Something Went Wrong in the {nameof(CreateGroup)}: Such group already exists.", statusCode: 500);
                    }
                }

                return new BadRequestObjectResult("Claim not found.");
            }

            return new BadRequestObjectResult("A group with this name already exists.");
        }

        public async Task<ActionResult> GetGroupByNameService(string Name)
        {
            var group = await _dependency.UnitOfWork.Groups.Get(g => g.Name == Name);

            if (group == null)
            {
                return new BadRequestObjectResult("Group not found");
            }

            return new OkObjectResult(group);
        }

        public async Task<ActionResult> DeleteGroupService(string Name)
        {

            var group = await _dependency.UnitOfWork.Groups.Get(g => g.Name == Name);

            if (group == null)
            {
                return new BadRequestObjectResult("Group not found");
            }

            await _dependency.UnitOfWork.Groups.Delete(group.Id);
            await _dependency.UnitOfWork.Save();

            return new OkObjectResult("Group " + Name + " deleted successfully!");
        }
        public async Task<IActionResult> EditGroupService(GroupsModel request, string name)
        {
            var group = await _dependency.UnitOfWork.Groups.Get(g => g.Name == name);

            if (group == null)
            {
                return new BadRequestObjectResult("Group not found!");
            }
            group.Name = request.Name;
            group.Description = request.Description;
            group.CreatedDate = request.CreatedDate;
            group.CreatorId = request.CreatorId;

            _dependency.UnitOfWork.Groups.Update(group);
            await _dependency.UnitOfWork.Save();
            return new OkObjectResult(group);
        }
    }
}