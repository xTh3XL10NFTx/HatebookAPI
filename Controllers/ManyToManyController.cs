using Hatebook.Common;
using Hatebook.Filters;
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

        [HttpGet("allUsersInGroups")]
        public async Task<ActionResult<List<GroupsModel>>> Get()
        {
            var usersInGroups = await _dependency.Context.manyToMany
                .Include(u => u.DbIdentityExtention)
                .Include(u => u.GroupsModel)
                .ToListAsync();

            var result = usersInGroups.Select(u => $"{u.DbIdentityExtention?.Email}, {u.GroupsModel?.Name}").ToList();
            return Ok(result);
        }

        [HttpPost("addUser")]
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
                UserId              = user.Email,
                DbIdentityExtention = user,
                GroupId             = group.Id,
                GroupsModel         = group
            };

            _dependency.Context.manyToMany.Add(userInGroup);
            await _dependency.Context.SaveChangesAsync();

            return Ok("User added to group.");

        }
        
        
        [HttpPost("createAdmin")]
        public async Task<IActionResult> MoveUserToAdmin(string email, string groupName)
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

            var groupAdmins = new GroupAdmins
            {
                UserId              = user.Email,
                DbIdentityExtention = user,
                GroupId             = group.Id,
                GroupsModel         = group
            };

            _dependency.Context.GroupAdmins.Add(groupAdmins);
            await _dependency.Context.SaveChangesAsync();

            return Ok("User changed to admin.");

        }


        [HttpDelete("removeUser")]
        [GroupAdminAuthorization]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUserFromGroup(string email, string groupName)
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
    }
}
