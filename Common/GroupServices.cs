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
    }
}
