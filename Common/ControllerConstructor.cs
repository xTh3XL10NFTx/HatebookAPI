using AutoMapper;
using Hatebook.Controllers;

namespace Hatebook.Common
{
    public class ControllerConstructor : IControllerConstructor
    {
        public ControllerConstructor(
            UserManager<DbIdentityExtention> userManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            IConfiguration configuration,
            ApplicationDbContext context,
            IAuthManager authManager)
        {
            UserManager = userManager;
            Logger = logger;
            Mapper = mapper;
            Configuration = configuration;
            Context = context;
            AuthManager = authManager;
        }

        public UserManager<DbIdentityExtention> UserManager { get; }
        public ILogger<AccountController> Logger { get; }
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }
        public ApplicationDbContext Context { get; }
        public IAuthManager AuthManager { get; }
    }
}
