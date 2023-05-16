using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IControllerConstructor _dependency;
        private readonly AccountServices _accountServices;
        public AccountController(IControllerConstructor dependency,
               AccountServices accountServices)
        {
            _dependency = dependency;
            _accountServices = accountServices;
        }

        // Get api/Account/get
        [HttpGet("get")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get() => Ok(_dependency.Mapper.Map<IList<HatebookMainModel>>(await _dependency.Context.Users.ToListAsync()));

        // Get api/Account/get/5
        [HttpGet("get/{email}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string email) => await _accountServices.GetUser(email);

        // POST api/Account/Register
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] HatebookMainModel request) => await _accountServices.RegisterUser(request);

        // POST api/Account/Register/LogIn
        [HttpPost("LogIn")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogIn([FromBody] HatebookLogin request) => await _accountServices.LogIntoUser(request);

        // DELETE api/Account/delete/5
        [HttpDelete("delete/{email}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(string email) => await _accountServices.DeleteUser(email);
    }
}