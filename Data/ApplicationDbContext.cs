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

        public DbSet<DbIdentityExtention> dbIdentityExtentions { get; set; }
        public DbSet<GroupsModel> groups { get; set; }
    }
}
