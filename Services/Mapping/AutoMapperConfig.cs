using AutoMapper;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Filters;

namespace FileSharing.Services.Mapping
{
	public static class AutoMapperConfig
	{
        static bool _registered;

		public static void RegisterMappings()
		{
            if (_registered) return;

			_registered = true;

			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<User, UserDto>().ReverseMap();

				cfg.CreateMap<Session, SessionDto>().ReverseMap();

				cfg.CreateMap<Group, GroupDto>().ReverseMap();
                cfg.CreateMap<Group, GroupDetailsDto>();
                cfg.CreateMap<Group, GroupDetailsExtendedDto>();

                cfg.CreateMap<UserGroup, UserGroupDto>().ReverseMap();

				cfg.CreateMap<Folder, FolderDto>().ReverseMap();
                cfg.CreateMap<FolderDto, FolderDetailsDto>();

				cfg.CreateMap<Document, DocumentDto>().ReverseMap();

				cfg.CreateMap<Audit, AuditDto>().ReverseMap();
				cfg.CreateMap<AuditFilterDto, AuditFilterDto>();
			});
		}
	}
}
