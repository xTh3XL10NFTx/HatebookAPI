namespace Hatebook.Common
{
    public class GroupServices
    {
        private readonly IControllerConstructor _dependency;
        public GroupServices(IControllerConstructor dependency)
        {
            _dependency = dependency;
        }
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
            var result = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == Name);
            if (result == null)
            {
                return new BadRequestObjectResult("Group not found");
            }

            return new OkObjectResult(result);
        }
        [HttpDelete("DeleteGroup")]
        public async Task<ActionResult> DeleteGroup(string Name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == Name);

            if (dbUser == null)
                return new BadRequestObjectResult("Group not found");

            _dependency.Context.groups.Remove(dbUser);

            await _dependency.Context.SaveChangesAsync();
            return new OkObjectResult("Group " + Name + " deleted successfully!");
        }
        public async Task<IActionResult> EditGroup(GroupsModel request, string name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == name);
            if (dbUser == null)
                return new BadRequestObjectResult("User not found!");

            dbUser.Name = request.Name;
            dbUser.Description = request.Description;
            dbUser.CreatedDate = request.CreatedDate;
            dbUser.CreatorId = request.CreatorId;

            await _dependency.Context.SaveChangesAsync();
            return new OkObjectResult(dbUser);
        }
    }
}