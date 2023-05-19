namespace Hatebook.Common
{
    public class GroupServices
    {
        private readonly IControllerConstructor _dependency;
        public GroupServices(IControllerConstructor dependency) => _dependency = dependency;
        public async Task<ActionResult> CreateGroupService(GroupsModel group, string claimsvalue )
        {
            group.Id = Guid.NewGuid();
            // Use the claim value as needed
            if (claimsvalue != null)
            {
                _dependency.Logger.LogInformation($"Registration Attempt for {claimsvalue} ");

                try
                {
                    group.CreatorId = claimsvalue;
                    _dependency.Context.groups.Add(group);
                    await _dependency.Context.SaveChangesAsync();
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

        [HttpGet("GetGroupByName")]
        public async Task<ActionResult> GetGroupByName(string Name)
        {
            GroupsModel user = await GetModelByName(Name);
            if (user == null) return new BadRequestObjectResult("Group not found");

            return new OkObjectResult(user);
        }
        [HttpDelete("DeleteGroup")]
        public async Task<ActionResult> DeleteGroup(string Name)
        {
            GroupsModel user = await GetModelByName(Name);
            if (user == null) return new BadRequestObjectResult("Group not found");

            _dependency.Context.groups.Remove(user);

            await _dependency.Context.SaveChangesAsync();
            return new OkObjectResult("Group " + Name + " deleted successfully!");
        }
        public async Task<IActionResult> EditGroup(GroupsModel request, string name)
        {
            GroupsModel user =  await GetModelByName(name);
            if (user == null) return new BadRequestObjectResult("User not found!");

            user.Name        = request.Name;
            user.Description = request.Description;
            user.CreatedDate = request.CreatedDate;
            user.CreatorId   = request.CreatorId;

            await _dependency.Context.SaveChangesAsync();
            return new OkObjectResult(user);
        }

        public async Task<GroupsModel> GetModelByName(string name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == name);
            return dbUser;
        }
    }
}