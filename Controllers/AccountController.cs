using Azure.Core;
using Hatebook.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hatebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {


        public static ApplicationDbContext _context;
        public static IConfiguration _configuration;
        public AccountController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;

        }

        Login commonMethodsLogin = new Login(_configuration, _context);
        Register commonMethodsRegister = new Register(_configuration, _context);

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

        // POST api/<AccountController>
        [HttpPost("register")]
        public async Task<ActionResult<List<Hatebook>>> RegisterUser(Hatebook request)
        {
            return Ok(commonMethodsRegister.RegisterUser(request));
        }
        // POST api/<AccountController>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Loginnn(HatebookDTOtwo request)
        {
            return Ok(commonMethodsLogin.Loginn(request));
        }

        // POST api/<AccountController>
        [HttpPost]
        public async Task<ActionResult<List<Hatebook>>> AddUser(Hatebook user)
        {
            _context.Hatebook.Add(user);
            await _context.SaveChangesAsync();
            return Ok(await _context.Hatebook.ToListAsync());
        }

        // PUT api/<AccountController>/5
        [HttpPut]
        public async Task<ActionResult<List<Hatebook>>> UpdateUser(Hatebook request)
        {
            var dbUser = await _context.Hatebook.FindAsync(request.Id);
            if (dbUser == null)
                return BadRequest("User not found!");

            dbUser.Fname = request.Fname;
            dbUser.Lname = request.Lname;
            dbUser.Email = request.Email;
            dbUser.GenderType = request.GenderType;
            dbUser.ProfilePic = request.ProfilePic;

            await _context.SaveChangesAsync();
            return Ok(await _context.Hatebook.ToListAsync());
        }

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
    }
}
