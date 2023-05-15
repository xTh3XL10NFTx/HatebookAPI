using AutoMapper;
using Hatebook.Services;
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


        [HttpGet("get")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var requestParams = await _context.Users.ToListAsync();
            var results = _mapper.Map<IList<Hatebook>>(requestParams);
            return Ok(results);
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hatebook>> Get(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return BadRequest("User not found.");
            return Ok(user);
        }

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

            var user = await _userManager.FindByNameAsync(request.Email);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            // Retrieve the secret from appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var secret = configuration["Jwt:Key"];

            // Create a SymmetricSecurityKey using the secret
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            // Create a SigningCredentials object using the key
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            

            var jwtSettings = _configuration.GetSection("Jwt");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);


            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string email)
        {
            var dbUser = await _context.Users.FindAsync(email);
            if (dbUser == null)
                return BadRequest("User not found.");

            _context.Users.Remove(dbUser);

            await _context.SaveChangesAsync();
            return Ok(await _context.Users.ToListAsync());
        }
    }
}