using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Hatebook.Data
{
    public class ApplicationDbContext : IdentityDbContext<DbIdentityExtention>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        internal Task FindAsync(string email)
        {
            throw new NotImplementedException();
        }

        public DbSet<DbIdentityExtention> dbIdentityExtentions { get; set; }
    }
}
