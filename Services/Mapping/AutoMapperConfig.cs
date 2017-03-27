using AutoMapper;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;

namespace DocumentManager.Services.Mapping
{
	public static class AutoMapperConfig
	{
        static bool _registered = false;

		public static void RegisterMappings()
		{
            if (_registered) return;

			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<User, UserDto>().ReverseMap();
                cfg.CreateMap<Session, SessionDto>();
			});
		}
	}
}
