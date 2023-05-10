using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Hatebook.Data
{
    public class ApplicationDbContext : IdentityDbContext<DbIdentityExtention>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Hatebook> Hatebook { get; set; }
    }
}
