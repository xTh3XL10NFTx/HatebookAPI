using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IControllerConstructor _dependency;
        private readonly GroupServices _groupServices;
        public GroupsController(IControllerConstructor dependency,
               GroupServices groupServices)
        {
            _dependency    = dependency;
            _groupServices = groupServices;
        }

        [HttpGet("get")]
        public async Task<ActionResult<List<GroupsModel>>> Get() => Ok(await _dependency.Context.groups.ToListAsync());

        [HttpGet("get/{Name}")]
        [ValidateModel]
        public async Task<ActionResult<List<GroupsModel>>> GetByName(string Name) => await _groupServices.GetGroupByName(Name);

        [Authorize]
        [HttpPost("CreateGroup")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGroup(GroupsModel group)
        {
            // Get the claim value by claim type
            var claimValue = User.FindFirst(ClaimTypes.Email)?.Value;
            return await _groupServices.CreateGroupService(group,claimValue);
        }

        [HttpPut("editGroup/{name}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateGroup(GroupsModel request, string name) => await _groupServices.EditGroup(request, name);

        [HttpDelete("delete/{Name}")]
        [ValidateModel]
        public async Task<ActionResult> Delete(string Name) => await _groupServices.DeleteGroup(Name);

    }
}   