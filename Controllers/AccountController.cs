using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public static HatebookDTO userr = new HatebookDTO();
        public static HatebookDTOtwo userrr = new HatebookDTOtwo();
        public static Hatebook userrrr = new Hatebook();

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AccountController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
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

        // POST api/<AccountController>
        [HttpPost("register")]
        public async Task<ActionResult<List<Hatebook>>> RegisterUser(Hatebook request, string Pass)
        {

            request.Password= Pass;

            CreatePasswordHash(Pass, out byte[] passwordHash, out byte[] passwordSalt);

            userr.PasswordHash = passwordHash; 
            userr.PasswordSalt = passwordSalt;

            request.PasswordHash = passwordHash;
            request.PasswordSalt = passwordSalt;

            _context.Hatebook.Add(request);
            await _context.SaveChangesAsync();
            return Ok(userr);
        }
        // POST api/<AccountController>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(HatebookDTOtwo request)
        {
            foreach (var useers in _context.Hatebook)
            {
                if (VerifyPasswordHash(request.Password, useers.PasswordHash, useers.PasswordSalt) && useers.Email == request.Email)
                {
                    string token = CreateToken(useers);
                    return Ok(token);
                }
            }
            return BadRequest("User not found!");

            //    return Ok("My Crazy Token");

            //    _context.Hatebook.Add(user);
            //    await _context.SaveChangesAsync();

        }


        private string CreateToken(Hatebook userrrr)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userrrr.Email)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computerHash= hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computerHash.SequenceEqual(passwordHash);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
            }
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
            if(dbUser == null)
                return BadRequest("User not found!");

            dbUser.Fname = request.Fname;
            dbUser.Lname  = request.Lname;
            dbUser.Email = request.Email;
            dbUser.GenderType = request.GenderType;
            dbUser.ProfilePic   = request.ProfilePic;

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
