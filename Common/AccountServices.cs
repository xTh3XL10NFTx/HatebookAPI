using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace Hatebook.Common
{
    public class AccountServices
    {
        public readonly IControllerConstructor _dependency;
        public AccountServices(IControllerConstructor dependency)
        { _dependency = dependency; }

        public async Task<IActionResult> LogIntoUserService(HatebookLogin request)
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
                }
                return new UnauthorizedObjectResult("User does not exist");
            }
        }
        public async Task<IActionResult> RegisterUserService(HatebookMainModel request, ModelStateDictionary modelState)
        {
            _dependency.Logger.LogInformation($"Registration Attempt for {request.Email}");

            var user = _dependency.Mapper.Map<DbIdentityExtention>(request);

            foreach (var roleName in request.Roles)
            {
                if (roleName.Name != "Administrator")
                {
                    roleName.Name = "User";
                }

                await _dependency.UserManager.AddToRoleAsync(user, roleName.Name);
            }

            user.UserName = request.Email;
            var result = await _dependency.UserManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.Code, error.Description);
                }
                return new BadRequestObjectResult(modelState);
            }
            // Assign the "User" role to the user by default
            await _dependency.UserManager.AddToRoleAsync(user, "User");
            await _dependency.UnitOfWork.SaveAsync();
            return new AcceptedResult("", "User registered successfully!");
        }

        public async Task<IActionResult> DeleteUserService(string email)
        {
            DbIdentityExtention? user = await GetModelByNameService(email);
            if (user == null) return new BadRequestObjectResult("User not found.");

            _dependency.UnitOfWork.GetRepository<DbIdentityExtention>().Remove(user);

            try
            {
                await _dependency.UnitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                _dependency.Logger.LogError(ex, $"Error occurred while deleting user: {email}");
                return new ObjectResult($"An error occurred while deleting the user: {email}") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return new OkObjectResult($"User {email} deleted successfully!");
        }


        public async Task<IActionResult> EditUserService(HatebookMainModel updatedUserr, string email)
        {
            var user = await _dependency.Context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found.");
            }

            user.Email = email;
            if (updatedUserr != null)
            {
                user.FirstName = updatedUserr.FirstName;
                user.LastName = updatedUserr.LastName;
                user.Birthday = updatedUserr.Birthday;
                user.GenderType = (DbIdentityExtention.Gender)updatedUserr.GenderType;
                user.ProfilePicture = updatedUserr.ProfilePicture;

                _dependency.UnitOfWork.GetRepository<DbIdentityExtention>().Update(user);

                return new OkObjectResult("User " + email + " updated successfully!");
            }
            else return new BadRequestObjectResult("Error in editing the user");
        }

        public async Task<IActionResult> GetUser(string email)
        {
            DbIdentityExtention? user = await GetModelByNameService(email);
            if (user == null) return new BadRequestObjectResult("User not found.");

            return new OkObjectResult(user);
        }
        public async Task<DbIdentityExtention?> GetModelByNameService(string name)
        {
            var dbUser = await _dependency.Context.Users.FirstOrDefaultAsync(u => u.Email == name);
            return dbUser;
        }
    }
}