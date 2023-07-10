using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : DependencyInjection
    {
        private readonly AccountServices _accountServices;
        public AccountController(IControllerConstructor dependency, AccountServices accountServices) : base(dependency) => _accountServices = accountServices;

        // Get api/Account/get
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() => Ok(_dependency.Mapper.Map<IList<HatebookMainModel>>(await _dependency.Context.Users.ToListAsync()));

        // Get api/Account/get/5
        [HttpGet("get/{email}")]
        public async Task<IActionResult> Get(string email) => await _accountServices.GetUser(email);

        // POST api/Account/Register
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] HatebookMainModel request) => await _accountServices.RegisterUserService(request, ModelState);

        // POST api/Account/Register/LogIn
        [HttpPost("log-in")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogIn([FromBody] HatebookLogin request) => await _accountServices.LogIntoUserService(request);

        // DELETE api/Account/delete/5
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete()
        {
            string? email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            if (email != null)
            {
                return await _accountServices.DeleteUserService(email);
            }
            else return NotFound();
        }

        // EDIT api/Account/edit/
        [HttpPut("edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromBody] HatebookMainModel updatedUser)
        {
            string? email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            if (email != null)
            {
                return await _accountServices.EditUserService(updatedUser, email);
            }
            else return NotFound();
        }
    }
}