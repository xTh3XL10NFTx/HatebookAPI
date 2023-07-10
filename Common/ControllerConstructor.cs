using AutoMapper;
using Hatebook.Controllers;
using Hatebook.IRepository;

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
            IAuthManager authManager,
            IUnitOfWork unitOfWork)
        {
            UserManager = userManager;
            Logger = logger;
            Mapper = mapper;
            Configuration = configuration;
            Context = context;
            AuthManager = authManager;
            UnitOfWork = unitOfWork;
        }

        public UserManager<DbIdentityExtention> UserManager { get; }
        public ILogger<AccountController> Logger { get; }
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }
        public ApplicationDbContext Context { get; }
        public IAuthManager AuthManager { get; }
        public IUnitOfWork UnitOfWork { get; }
    }
}
