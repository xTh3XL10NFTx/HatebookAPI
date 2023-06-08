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
            
            modelBuilder.Entity<Comment>()
        .HasOne(c => c.DbIdentityExtention)
        .WithMany()
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Like>()
        .HasOne(c => c.DbIdentityExtention)
        .WithMany()
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<DbIdentityExtention> dbIdentityExtentions { get; set; }
        public DbSet<GroupsModel> groups                       { get; set; }
        public DbSet<UsersInGroups> usersInGroups                 { get; set; }
        public DbSet<GroupAdmins> GroupAdmins                  { get; set; }
        public DbSet<FriendsList> Friends                      { get; set; }
        public DbSet<Post> Posts                              { get; set; }
        public DbSet<Like> Likes                              { get; set; }
        public DbSet<Comment> Comments                              { get; set; }
    }
}
