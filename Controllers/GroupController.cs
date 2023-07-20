using Hatebook.ControllerLogic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupsController : DependencyInjection
    {
        private readonly GroupServices _groupServices;
        public GroupsController(IControllerConstructor dependency, GroupServices groupServices) : base(dependency) => _groupServices = groupServices;

        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<GroupsModel>>> Get() => Ok(await _dependency.Context.groups.ToListAsync());

        [HttpGet("get/{Name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<GroupsModel>>> GetByName(string Name) => await _groupServices.GetGroupByNameService(Name);

        [HttpPost("CreateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGroup(GroupsModel group)
        {
            // Get the claim value by claim type
            var claimValue = User.FindFirst(ClaimTypes.Email)?.Value;
            if(claimValue != null) return await _groupServices.CreateGroupService(group, claimValue);
            else return BadRequest("You log in first.");
        }

        [HttpPut("editGroup/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateGroup(GroupsModel request, string name) => await _groupServices.EditGroupService(request, name);

        [HttpDelete("delete/{Name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string Name) => await _groupServices.DeleteGroupService(Name);
    }
}