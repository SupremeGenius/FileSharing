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
                cfg.CreateMap<Folder, FolderDetailsDto>()
                    .ForMember(dest => dest.Folders, opt => opt.MapFrom(src => src.Folders))
                    .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));

				cfg.CreateMap<Document, DocumentDto>().ReverseMap();

				cfg.CreateMap<Audit, AuditDto>().ReverseMap();
				cfg.CreateMap<AuditFilterDto, AuditFilterDto>();
			});
		}
	}
}
