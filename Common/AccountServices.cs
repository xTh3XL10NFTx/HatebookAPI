namespace Hatebook.Common
{
    public class AccountServices
    {
        private readonly IControllerConstructor _dependency;
        public AccountServices(IControllerConstructor dependency)
        {
            _dependency = dependency;
        }
        public async Task<IActionResult> LogIntoUser(HatebookLogin request)
        {
            _dependency.Logger.LogInformation($"Login Attempt for {request.Email} ");
            try
            {
                if (!await _dependency.AuthManager.ValidateUser(request))
                {
                    return new UnauthorizedObjectResult("Wrong email or password");
                }
                return new AcceptedResult("User logged in", new { Token = await _dependency.AuthManager.CreateToken() });
            }
            catch (Exception ex)
            {

                if (!await _dependency.AuthManager.ValidateUser(request))
                {
                    _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(LogIntoUser)}");
                   // return new ObjectResult($"Something Went Wrong in the {nameof(LogIntoUser)}", StatusCodeResult: 500);
                }
                return new UnauthorizedObjectResult("User does not exist");
            }
        }
        public async Task<IActionResult> RegisterUser(HatebookMainModel request)
        {
            _dependency.Logger.LogInformation($"Registration Attempt for {request.Email}");
            var user = _dependency.Mapper.Map<DbIdentityExtention>(request);
                user.UserName = request.Email;
                var result = await _dependency.UserManager.CreateAsync(user, request.Password);

                await _dependency.UserManager.AddToRolesAsync(user, request.Roles);

                return new AcceptedResult("", "User registered successfully!");
        }
        public async Task<IActionResult> DeleteUser(string email)
        {
            var dbUser = await _dependency.Context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (dbUser == null)
                return new BadRequestObjectResult("User not found.");
            _dependency.Context.Remove(dbUser);
            await _dependency.Context.SaveChangesAsync();
            return new OkObjectResult("User " + email + " deleted successfully!");
        }
        public async Task<IActionResult> GetUser(string email)
        {
            var user = await _dependency.Context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return new BadRequestObjectResult("User not found.");
            return new OkObjectResult(user);
        }
    }
}
