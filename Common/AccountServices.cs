using Hatebook.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;
using System.Net;

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
                var userRepository = _dependency.UnitOfWork.GetRepository<DbIdentityExtention>();

                var user = await userRepository.Get(u => u.Email == request.Email);
                if (user == null || !await _dependency.AuthManager.ValidateUser(request))
                {
                    return Unauthorized("Wrong email or password");
                }
                await _dependency.UnitOfWork.Save();

                return Accepted("User logged in", new { Token = await _dependency.AuthManager.CreateToken() });
            }
            catch (Exception ex)
            {
                if (!await _dependency.AuthManager.ValidateUser(request))
                {
                    _dependency.Logger.LogError(ex, $"Something Went Wrong in the {nameof(LogIntoUserService)}");
                    // return new ObjectResult($"Something Went Wrong in the {nameof(LogIntoUser)}", StatusCodeResult: 500);
                }
                return Unauthorized("User does not exist");
            }
        }
        public async Task<IActionResult> RegisterUserService(HatebookMainModel request, ModelStateDictionary state)
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

            var userRepository = _dependency.UnitOfWork.GetRepository<DbIdentityExtention>();
            await userRepository.Insert(user);

            try
            {
                await _dependency.UnitOfWork.Save();
            }
            catch (Exception ex)
            {
                _dependency.Logger.LogError(ex, $"Error occurred while registering user: {request.Email}");
                return new ObjectResult($"An error occurred while registering the user: {request.Email}") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return Accepted("", "User registered successfully!");
        }

        public async Task<IActionResult> DeleteUserService(string email)
        {
            GroupsModel? user = await GetModelByNameService(email);
            if (user == null) return new BadRequestObjectResult("User not found.");

            var userRepository = _dependency.UnitOfWork.GetRepository<DbIdentityExtention>();
            await userRepository.Delete(user.Id);

            try
            {
                await _dependency.UnitOfWork.Save();
            }
            catch (Exception ex)
            {
                _dependency.Logger.LogError(ex, $"Error occurred while deleting user: {email}");
                return new ObjectResult($"An error occurred while deleting the user: {email}") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return Ok($"User {email} deleted successfully!");
        }

        public async Task<IActionResult> GetUser(string email)
        {
            GroupsModel? user = await GetModelByNameService(email);
            if (user == null) return new BadRequestObjectResult("User not found.");

            return Ok(user);
        }
        public async Task<GroupsModel?> GetModelByNameService(string name)
        {
            var dbUser = await _dependency.Context.groups.FirstOrDefaultAsync(u => u.Name == name);
            return dbUser;
        }
    }
}
