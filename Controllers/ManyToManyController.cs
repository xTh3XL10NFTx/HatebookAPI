using Hatebook.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManyToManyController : ControllerBase
    {
        private readonly IControllerConstructor _dependency;
        public ManyToManyController(IControllerConstructor dependency)
        {
            _dependency = dependency;
        }

        [HttpGet("get")]
        public async Task<ActionResult<List<GroupsModel>>> Get()
        {
            var usersInGroups = await _dependency.Context.manyToMany
                .Include(u => u.DbIdentityExtention)
                .Include(u => u.GroupsModel)
                .ToListAsync();

            var result = usersInGroups.Select(u => $"{u.DbIdentityExtention?.Email}, {u.GroupsModel?.Name}").ToList();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> MoveUserToGroup(string email, string groupName)
        {

            var user = await _dependency.Context.dbIdentityExtentions.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var group = await _dependency.Context.groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null)
            {
                return BadRequest("Group not found");
            }

            var userInGroup = new UsersInGroups
            {
                UserId = user.Email,
                DbIdentityExtention = user,
                GroupId = group.Id,
                GroupsModel = group
            };

            _dependency.Context.manyToMany.Add(userInGroup);
            await _dependency.Context.SaveChangesAsync();

            return Ok("User added to group.");

        }


        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUserFromGroup(string email, string groupName)
        {
            var claimValue = User.FindFirst(ClaimTypes.Email)?.Value;
            if (claimValue == "IvelinTanev@gmail.com")
            {


            var user = await _dependency.Context.dbIdentityExtentions.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var group = await _dependency.Context.groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null)
            {
                return BadRequest("Group not found");
            }

            var userInGroup = await _dependency.Context.manyToMany.FirstOrDefaultAsync(u => u.UserId == user.Id && u.GroupsModel.Id == group.Id);
            if (userInGroup == null)
            {
                return BadRequest("User is not a member of the group");
            }

            _dependency.Context.manyToMany.Remove(userInGroup);
            await _dependency.Context.SaveChangesAsync();
            return Ok("User " + email + " deleted from group " + groupName + " successfully!");
            }
            else
            {
                return BadRequest("User is not authorized");
            }
        }
    }
}
