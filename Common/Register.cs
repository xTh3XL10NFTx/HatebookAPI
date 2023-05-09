using System.Security.Cryptography;

namespace Hatebook.Common
{
    public class Register
    {
        private IConfiguration _configuration;
        private ApplicationDbContext _context;

        public Register(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public Hatebook RegisterUser(Hatebook request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            request.PasswordHash = passwordHash;
            request.PasswordSalt = passwordSalt;
            return request;
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
