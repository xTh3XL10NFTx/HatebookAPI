using Hatebook.Controllers;

namespace Hatebook.Common
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountServices : AccountController
    {
        private static readonly AccountServices? accountServices;
        private readonly IControllerConstructor _dependency;
        public AccountServices(IControllerConstructor dependency) : base(dependency, accountServices)
        {
            _dependency = dependency;
        }
        [HttpPost("LogInUser")]
        public async Task<IActionResult> LogIntoUser(HatebookLogin request)
        {
            _dependency.Logger.LogInformation($"Login Attempt for {request.Email} ");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                if (!await _dependency.AuthManager.ValidateUser(request))
                {
                    return Unauthorized();
                }
                return Accepted(new { Token = await _dependency.AuthManager.CreateToken() });
            }
            catch (Exception ex)
            {

                if (!await _dependency.AuthManager.ValidateUser(request))
                {
                    _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(LogIn)}");
                    return Problem($"Something Went Wrong in the {nameof(LogIn)}", statusCode: 500);
                }
                return Unauthorized();
            }
        }
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(HatebookMainModel request)
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
                _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(RegisterUser)}");
                return Problem($"Something Went Wrong in the {nameof(RegisterUser)}", statusCode: 500);
            }
        }
        [HttpPost("DeleteUser")]
        public async Task<ActionResult> DeleteUser(string email)
        {
            var dbUser = await _dependency.Context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (dbUser == null)
                return BadRequest("User not found.");
            _dependency.Context.Remove(dbUser);
            await _dependency.Context.SaveChangesAsync();
            return Ok("User " + email + " deleted successfully!");
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetUser(string email)
        {
            var user = await _dependency.Context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return BadRequest("User not found.");
            return Ok(user);
        }
    }
}
