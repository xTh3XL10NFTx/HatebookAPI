using AutoMapper;

namespace Hatebook.Configurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<DbIdentityExtention, Hatebook>().ReverseMap();
        }
    }
}
