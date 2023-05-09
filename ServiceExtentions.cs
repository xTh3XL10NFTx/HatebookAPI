using Microsoft.AspNetCore.Identity;

namespace Hatebook
{
    public static class ServiceExtentions
    {
        public static void ConfigureIdentity(this IServiceCollection service)
        {
            var builder = service.AddIdentityCore<DbIdentityExtention>(q => q.User.RequireUniqueEmail = true);

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), service);
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        }
    }
}
