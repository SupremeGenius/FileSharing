using AutoMapper;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;

namespace DocumentManager.Services.Mapping
{
	public static class AutoMapperConfig
	{
		public static void RegisterMappings()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<User, UserDto>().ReverseMap();
			});
		}
	}
}
