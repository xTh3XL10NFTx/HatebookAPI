using FluentValidation;
using Hatebook.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : DependencyInjection
    {
        private readonly AccountServices _accountServices;
        private readonly IValidator<HatebookMainModel> _validator;
        public AccountController(IControllerConstructor dependency, AccountServices accountServices, IValidator<HatebookMainModel> validator) : base(dependency)
        {
            _accountServices = accountServices;
            _validator = validator;
        }

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
        [AllowAnonymous]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] HatebookMainModel request)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                // Model is invalid, handle validation errors
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                // Return appropriate response, such as BadRequest(errors)
            }
            return await _accountServices.RegisterUserService(request,ModelState);
        }

        // POST api/Account/Register/LogIn
        [HttpPost("LogIn")]
        [AllowAnonymous]
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