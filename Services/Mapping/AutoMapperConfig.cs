using AutoMapper;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Filters;
using System.Linq;

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
                cfg.CreateMap<Group, GroupDetailsDto>()
                    .ForMember(dest => dest.NumOfFiles, opt => opt.MapFrom(src => src.Files.Count))
                    .ForMember(dest => dest.NumOfMembers, opt => opt.MapFrom(src => src.Users.Where(x => x.DateInclusionApproval.HasValue).ToList().Count));
                cfg.CreateMap<Group, GroupDetailsExtendedDto>()
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Users.Where(x => x.DateInclusionApproval.HasValue).Select(x => x.User)))
                    .ForMember(dest => dest.Requests, opt => opt.MapFrom(src => src.Users.Where(x => !x.DateInclusionApproval.HasValue).Select(x => x.User)));

                cfg.CreateMap<UserGroup, UserGroupDto>().ReverseMap();
                cfg.CreateMap<UserGroup, UserGroupDetailsDto>()
                    .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

                cfg.CreateMap<Folder, FolderDto>().ReverseMap();
                cfg.CreateMap<Folder, FolderDetailsDto>();

				cfg.CreateMap<File, FileDto>().ReverseMap();

				cfg.CreateMap<Audit, AuditDto>().ReverseMap();
				cfg.CreateMap<AuditFilterDto, AuditFilterDto>();
			});
		}
	}
}
