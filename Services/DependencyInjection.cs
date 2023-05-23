namespace Hatebook.Services
{
    public class DependencyInjection : ControllerBase
    {
        public DependencyInjection(IControllerConstructor dependency) => _dependency = dependency;

        public IControllerConstructor _dependency { get; }
}
}
