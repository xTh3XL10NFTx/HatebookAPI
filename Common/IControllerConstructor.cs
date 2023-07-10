using AutoMapper;
using Hatebook.Controllers;
using Hatebook.IRepository;

namespace Hatebook.Common
{
    public interface IControllerConstructor
    {
        UserManager<DbIdentityExtention> UserManager { get; }
        ILogger<AccountController> Logger { get; }
        IMapper Mapper { get; }
        IConfiguration Configuration { get; }
        ApplicationDbContext Context { get; }
        IAuthManager AuthManager { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
