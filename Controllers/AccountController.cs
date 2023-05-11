using AutoMapper;
using Hatebook.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<DbIdentityExtention> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public static ApplicationDbContext _context;
        public static IConfiguration _configuration;

        public AccountController(IConfiguration configuration,
            ApplicationDbContext context,
            UserManager<DbIdentityExtention> userManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            IAuthManager authManager)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
            _authManager = authManager;

        }

        [HttpGet]
        public async Task<ActionResult<List<Hatebook>>> Get()
        {
            return Ok(await _context.Hatebook.ToListAsync());
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hatebook>> Get(int id)
        {
            var user = await _context.Hatebook.FindAsync(id);
            if (user == null)
                return BadRequest("User not found.");
            return Ok(user);
        }

        //// POST api/<AccountController>
        //[HttpPost("register")]
        //public async Task<ActionResult<List<Hatebook>>> RegisterUser(Hatebook request)
        //{
        //    Register commonMethodsRegister = new Register(_configuration, _context);
        //    commonMethodsRegister.RegisterUser(request);
        //    _context.Hatebook.Add(request);
        //    await _context.SaveChangesAsync();
        //    return Ok("Successfully registered!");
        //}



        // POST api/<AccountController>
        [HttpPost("registerNew")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RegisterUserNEW([FromBody] Hatebook request)
        {
            _logger.LogInformation($"Registration Attempt for {request.Email} ");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = _mapper.Map<DbIdentityExtention>(request);
                user.UserName = request.Email;
                var result = await _userManager.CreateAsync(user, request.Password);

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
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(RegisterUserNEW)}");
                return Problem($"Something Went Wrong in the {nameof(RegisterUserNEW)}", statusCode: 500);
            }
        }

        // POST api/<AccountController>
        [HttpPost("loginNew")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUserNEW([FromBody] HatebookLogin request)
        {
            _logger.LogInformation($"Login Attempt for {request.Email} ");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _authManager.ValidateUser(request))
            {
                return Unauthorized();
            }
            return Accepted(new { Token = await _authManager.CreateToken() });
        }




        //// POST api/<AccountController>
        //[HttpPost("registerNew")]
        //public async Task<ActionResult> RegisterUserNEW(Hatebook model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);

        //        UserManagerExtensions.CreateIdentity(user, model.Password);

        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false);
        //            return RedirectToAction("index", "home");
        //        }

        //        foreach(var error in result.Errors)
        //        {
        //            ModelState.AddModelError("",error.Description);
        //        }
        //    }

        //}




        //// POST api/<AccountController>
        //[HttpPost("login")]
        //public async Task<ActionResult<string>> LoginUser(HatebookDTOtwo request)
        //{
        //    Login commonMethodsLogin = new Login(_configuration, _context);
        //    return Ok(commonMethodsLogin.LoginReturn(request));
        //}

        //// PUT api/<AccountController>/5
        //[HttpPut]
        //public async Task<ActionResult<List<Hatebook>>> UpdateUser(Hatebook request)
        //{
        //    var dbUser = await _context.Hatebook.FindAsync(request.Id);
        //    if (dbUser == null)
        //        return BadRequest("User not found!");

        //    dbUser.Fname = request.Fname;
        //    dbUser.Lname = request.Lname;
        //    dbUser.Email = request.Email;
        //    dbUser.GenderType = request.GenderType;
        //    dbUser.ProfilePic = request.ProfilePic;

        //    await _context.SaveChangesAsync();
        //    return Ok(await _context.Hatebook.ToListAsync());
        //}

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Hatebook>>> Delete(int id)
        {
            var dbUser = await _context.Hatebook.FindAsync(id);
            if (dbUser == null)
                return BadRequest("Hero not found.");

            _context.Hatebook.Remove(dbUser);

            await _context.SaveChangesAsync();
            return Ok(await _context.Hatebook.ToListAsync());
        }






        //// POST api/<AccountController>
        //[HttpPost]
        //public async Task<ActionResult<List<Hatebook>>> AddUser(Hatebook user)
        //{
        //    _context.Hatebook.Add(user);
        //    await _context.SaveChangesAsync();
        //    return Ok(await _context.Hatebook.ToListAsync());
        //}
    }
}