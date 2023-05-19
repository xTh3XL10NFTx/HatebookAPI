using Hatebook.Configurations;
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

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }

        public DbSet<DbIdentityExtention> dbIdentityExtentions { get; set; }
        public DbSet<GroupsModel> groups                       { get; set; }
        public DbSet<UsersInGroups> manyToMany                 { get; set; }
        public DbSet<GroupAdmins> GroupAdmins                  { get; set; }
    }
}
