using Bgt.Ocean.Service.Mapping;

namespace Bgt.Ocean.WebAPI
{
    public static class MapperConfig
    {
        public static void Register()
        {
            ServiceMapperBootstrapper.Configure();
        }
    }
}