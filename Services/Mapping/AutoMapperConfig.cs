﻿using AutoMapper;
using FileStorage.Persistence.Models;
using FileStorage.Services.Dtos;
using FileStorage.Services.Filters;

namespace FileStorage.Services.Mapping
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
				cfg.CreateMap<UserGroup, UserGroupDto>().ReverseMap();
				cfg.CreateMap<Folder, FolderDto>().ReverseMap();
				cfg.CreateMap<Document, DocumentDto>().ReverseMap();
				cfg.CreateMap<Audit, AuditDto>().ReverseMap();

				cfg.CreateMap<AuditFilterDto, AuditFilterDto>();
			});
		}
	}
}
