using Hatebook.ControllerLogic;
using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersInGroupsController : ControllerBase
    {
        private readonly UsersInGroupsServces _usersInGroupsServices;
        public UsersInGroupsController(UsersInGroupsServces usersInGroupsServices) => _usersInGroupsServices = usersInGroupsServices;

        [HttpGet("allUsersInGroups")]
        public async Task<ActionResult<List<GroupsModel>>> Get()=> await _usersInGroupsServices.GetService();

        [HttpPost("moveUserToGroup")]
        public async Task<IActionResult> MoveUserToGroup(string email, string groupName) => await _usersInGroupsServices.MoveUserToGroupService(email, groupName);


        [HttpPost("createAdmin")]
        public async Task<IActionResult> MoveUserToAdmin(string email, string groupName) => await _usersInGroupsServices.MoveUserToAdminService(email, groupName);

        [HttpDelete("removeUserFromGroup")]
        [GroupAdminAuthorization]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveUserFromGroup(string email, string groupName) => await _usersInGroupsServices.RemoveUserFromGroupService(email, groupName);
    }
}