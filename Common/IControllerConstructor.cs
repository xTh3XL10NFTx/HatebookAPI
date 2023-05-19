using AutoMapper;
using Hatebook.Controllers;

namespace Hatebook.Common
{
    public interface IControllerConstructor
    {
        UserManager<DbIdentityExtention> UserManager { get; }
        ILogger<AccountController> Logger            { get; }
        IMapper Mapper                               { get; }
        IConfiguration Configuration                 { get; }
        ApplicationDbContext Context                 { get; }
        IAuthManager AuthManager                     { get; }
    }
}
