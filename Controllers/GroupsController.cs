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
            _dependency = dependency;
            _groupServices = groupServices;
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupsModel>>> Get()
        {
            return Ok(await _dependency.Context.groups.ToListAsync());
        }
        /*
        [HttpGet("{Name}")]
        public async Task<ActionResult<List<GroupsModel>>> GetByName(string Name)
        {

            var result = await _dependency.Context.groups.FindAsync(Name);
            if (result == null)
            {
                return BadRequest("Group not found");
            }

            return Ok(result);
        }
        */
        [Authorize]
        [HttpPost("CreateGroup")]
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

        /*

        [HttpPut]
        public async Task<IActionResult> UpdateGroup(GroupsModel request, string Name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == Name);
            if (dbUser == null)
                return BadRequest("User not found!");

            dbUser.Name = request.Name;
            dbUser.Description = request.Description;
            dbUser.CreatedDate = request.CreatedDate;
            dbUser.CreatorId = request.CreatorId;

            await _dependency.Context.SaveChangesAsync();
            return Ok(await _dependency.Context.groups.ToListAsync());
        }

        [HttpDelete("{Name}")]
        public async Task<ActionResult> Delete(string Name)
        {
            var dbUser = await _dependency.Context.groups.FindAsync(Name);

            if (dbUser == null)
                return BadRequest("Group not found");

            _dependency.Context.groups.Remove(dbUser);

            await _dependency.Context.SaveChangesAsync();
            return Ok(await _dependency.Context.groups.ToListAsync());
        }
        */
    }
}   