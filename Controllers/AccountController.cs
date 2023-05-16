using Microsoft.AspNetCore.Authorization;
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
        public AccountController(IControllerConstructor dependency) => _dependency = dependency;

        // Get api/Account/get
        [HttpGet]
        [Route("get")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var requestParams = await _dependency.Context.Users.ToListAsync();
            var results = _dependency.Mapper.Map<IList<HatebookMainModel>>(requestParams);
            return Ok(results);
        }

        // Get api/Account/get/5
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<HatebookMainModel>> Get(int id)
        {
            var user = await _dependency.Context.Users.FindAsync(id);
            if (user == null)
                return BadRequest("User not found.");
            return Ok(user);
        }

        // POST api/Account/Register
        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] HatebookMainModel request)
        {
            _dependency.Logger.LogInformation($"Registration Attempt for {request.Email} ");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = _dependency.Mapper.Map<DbIdentityExtention>(request);
                user.UserName = request.Email;
                var result = await _dependency.UserManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Accepted();
            }
            catch (Exception ex)
            {
                _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        // POST api/Account/Register/LogIn
        [HttpPost("LogIn")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogIn([FromBody] HatebookLogin request)
        {
            AccountServices login = new AccountServices(_dependency);
            return await login.LogIntoUser(request);
        }

        // DELETE api/Account/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(string email)
        {
            var dbUser = await _dependency.Context.Users.FindAsync(email);
            if (dbUser == null)
                return BadRequest("User not found.");

            _dependency.Context.Users.Remove(dbUser);

            await _dependency.Context.SaveChangesAsync();
            return Ok(await _dependency.Context.Users.ToListAsync());
        }
    }
}