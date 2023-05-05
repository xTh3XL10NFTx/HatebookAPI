using Hatebook.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Hatebook.Common
{
    public class Register
    {
        public readonly ApplicationDbContext _context;
        public readonly IConfiguration _configuration;
        public Register(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public static HatebookDTO userr = new HatebookDTO();
        public async Task<ActionResult<List<Hatebook>>> RegisterUser(Hatebook request)
        {
            List<Hatebook> done = new List<Hatebook>();
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            userr.PasswordHash = passwordHash;
            userr.PasswordSalt = passwordSalt;

            request.PasswordHash = passwordHash;
            request.PasswordSalt = passwordSalt;

            _context.Hatebook.Add(request);
            await _context.SaveChangesAsync();
            return done;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
