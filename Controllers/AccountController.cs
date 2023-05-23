﻿using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : DependencyInjection
    {
        private readonly AccountServices _accountServices;
        public AccountController(IControllerConstructor dependency, AccountServices accountServices) : base(dependency) => _accountServices = accountServices;

        // Get api/Account/get
        [HttpGet("get")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            return Ok(_dependency.Mapper.Map<IList<HatebookMainModel>>(await _dependency.Context.Users.ToListAsync()));
        }

        // Get api/Account/get/5
        [HttpGet("get/{email}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string email)
        {
            return await _accountServices.GetUser(email);
        }

        // POST api/Account/Register
        [HttpPost("Register")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] HatebookMainModel request)
        {
            return await _accountServices.RegisterUserService(request,ModelState);
        }

        // POST api/Account/Register/LogIn
        [HttpPost("LogIn")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogIn([FromBody] HatebookLogin request)
        {
            return await _accountServices.LogIntoUserService(request, ModelState);
        }

        // DELETE api/Account/delete/5
        [HttpDelete("delete/{email}")]
        [Authorize]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string email) => await _accountServices.DeleteUserService(email);
    }
}