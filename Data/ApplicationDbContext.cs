using System.Collections.Generic;

namespace Hatebook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Hatebook> Hatebook { get; set; }
    }
}
