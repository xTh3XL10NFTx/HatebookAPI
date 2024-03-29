﻿using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : DependencyInjection
    {
        private readonly GroupServices _groupServices;
        public GroupController(IControllerConstructor dependency, GroupServices groupServices) : base(dependency) => _groupServices = groupServices;

        [HttpGet("get")]
        public async Task<ActionResult<List<GroupsModel>>> Get() => Ok(await _dependency.Context.groups.ToListAsync());

        [HttpGet("get/{Name}")]
        [ValidateModel]
        public async Task<ActionResult<List<GroupsModel>>> GetByName(string Name) => await _groupServices.GetGroupByNameService(Name);

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
            if (claimValue != null)
            {
                return await _groupServices.CreateGroupService(group, claimValue);
            } else return Unauthorized("Unauthorized");
        }

        [HttpPut("editGroup/{name}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateGroup(GroupsModel request, string name) => await _groupServices.EditGroupService(request, name);

        [HttpDelete("delete/{Name}")]
        [ValidateModel]
        public async Task<ActionResult> Delete(string Name) => await _groupServices.DeleteGroupService(Name);

    }
}   