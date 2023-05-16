using Hatebook.Controllers;

namespace Hatebook.Common
{
    public class AccountServices : AccountController
    {
        private readonly IControllerConstructor _dependency;
        public AccountServices(IControllerConstructor dependency) : base(dependency)
        {
            _dependency = dependency;
        }
        [HttpPost]
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
    }
}
