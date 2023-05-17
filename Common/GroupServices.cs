using Hatebook.Controllers;
namespace Hatebook.Common
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GroupServices : GroupsController
    {
        private static readonly GroupServices? groupServices;
        private readonly IControllerConstructor _dependency;
        public GroupServices(IControllerConstructor dependency) : base(dependency, groupServices)
        {
            _dependency = dependency;
        }

        [HttpPost("CreateGroupService")]
        public async Task<ActionResult> CreateGroupService(GroupsModel group, string claimsvalue )
        {
            group.Id = Guid.NewGuid();
            // Use the claim value as needed
            if (claimsvalue != null)
            {
                _dependency.Logger.LogInformation($"Registration Attempt for {claimsvalue} ");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                try
                {
                    group.CreatorId = claimsvalue;
                    _dependency.Context.groups.Add(group);
                    await _dependency.Context.SaveChangesAsync();
                    return Ok(group);
                }
                catch (Exception ex)
                {
                    _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(CreateGroup)}: Such group already exists.");
                    return Problem($"Something Went Wrong in the {nameof(CreateGroup)}: Such group already exists.", statusCode: 500);
                }
            }
            return BadRequest("Claim not found.");
        }

        [HttpGet("GetGroupByName")]
        public async Task<ActionResult> GetGroupByName(string Name)
        {
            var result = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == Name);
            if (result == null)
            {
                return BadRequest("Group not found");
            }

            return Ok(result);
        }
        [HttpDelete("DeleteGroup")]
        public async Task<ActionResult> DeleteGroup(string Name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == Name);

            if (dbUser == null)
                return BadRequest("Group not found");

            _dependency.Context.groups.Remove(dbUser);

            await _dependency.Context.SaveChangesAsync();
            return Ok("Group " + Name + " deleted successfully!");
        }
        [HttpPut("EditGroup")]
        public async Task<IActionResult> EditGroup(GroupsModel request, string name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == name);
            if (dbUser == null)
                return BadRequest("User not found!");

            dbUser.Name = request.Name;
            dbUser.Description = request.Description;
            dbUser.CreatedDate = request.CreatedDate;
            dbUser.CreatorId = request.CreatorId;

            await _dependency.Context.SaveChangesAsync();
            return Ok(dbUser);
        }
    }
}