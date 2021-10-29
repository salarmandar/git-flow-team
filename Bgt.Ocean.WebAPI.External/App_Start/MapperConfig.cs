using Bgt.Ocean.Service.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bgt.Ocean.WebAPI.External
{
    public class MapperConfig
    {
        public static void Register()
        {
            ServiceMapperBootstrapper.Configure();
        }
    }
}