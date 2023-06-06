using Hatebook.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

namespace Hatebook.Common
{
    public class AccountServices:DependencyInjection
    {
        public AccountServices(IControllerConstructor dependency) : base(dependency) { }

        public async Task<IActionResult> LogIntoUserService(HatebookLogin request, ModelStateDictionary state)
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
                    _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(LogIntoUserService)}");
                    // return new ObjectResult($"Something Went Wrong in the {nameof(LogIntoUser)}", StatusCodeResult: 500);
                }
                return new UnauthorizedObjectResult("User does not exist");
            }
        }
        public async Task<IActionResult> RegisterUserService(HatebookMainModel request, ModelStateDictionary state)
        {
            _dependency.Logger.LogInformation($"Registration Attempt for {request.Email}");
            var user = _dependency.Mapper.Map<DbIdentityExtention>(request);

            foreach (var roleName in request.Roles)
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName.Name
                };

                await _dependency.UserManager.AddToRoleAsync(user, roleName.Name);
            }
            user.UserName = request.Email;
            var result = await _dependency.UserManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    state.AddModelError(error.Code, error.Description);
                }
                return new BadRequestObjectResult(state);
            }
            // Assign the "User" role to the user by default
            await _dependency.UserManager.AddToRoleAsync(user, "User");
            return new AcceptedResult("", "User registered successfully!");
        }
        public async Task<IActionResult> DeleteUserService(string email)
        {
            GroupsModel? user = await GetModelByNameService(email);
            if (user == null) return new BadRequestObjectResult("User not found.");

            _dependency.Context.Remove(user);
            await _dependency.Context.SaveChangesAsync();

            return new OkObjectResult("User " + email + " deleted successfully!");
        }
        public async Task<IActionResult> GetUser(string email)
        {
            GroupsModel? user = await GetModelByNameService(email);
            if (user == null) return new BadRequestObjectResult("User not found.");

            return new OkObjectResult(user);
        }
        public async Task<GroupsModel?> GetModelByNameService(string name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == name);
            return dbUser;
        }
    }
}
