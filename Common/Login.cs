using Hatebook.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Hatebook.Common
{
    public class Login
    {
        public readonly ApplicationDbContext _context;
        public readonly IConfiguration _configuration;
        public Login(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<ActionResult<string>> Loginn(HatebookDTOtwo request)
        {
            string notoken = "User not found!";
            foreach (var useers in _context.Hatebook)
            {
                if (VerifyPasswordHash(request.Password, useers.PasswordHash, useers.PasswordSalt) && useers.Email == request.Email)
                {
                    string token = CreateTokenn(useers);
                    return token;
                }
            }
            return notoken;//    _context.Hatebook.Add(user);//    await _context.SaveChangesAsync();
        }

        public string CreateTokenn(Hatebook hatebook)
        {
        List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, hatebook.Email)
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
                var computerHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computerHash.SequenceEqual(passwordHash);
            }
        }
    }
}
