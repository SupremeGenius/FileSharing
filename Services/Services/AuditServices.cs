using System;
using System.Collections.Generic;
using AutoMapper;
using FileStorage.Persistence.Daos;
using FileStorage.Persistence.Models;
using FileStorage.Persistence.Models.Filters;
using FileStorage.Services.Dtos;
using FileStorage.Services.Exceptions;
using FileStorage.Services.Filters;

namespace FileStorage.Services
{
	public class AuditServices : AbstractServices<AuditDao>
	{
		public AuditServices() : base(new AuditDao()) { }

		public void Create(AuditDto audit)
		{
			try
			{
				var auditDom = Mapper.Map<Audit>(audit);
				auditDom.Date = DateTime.Now;
				_dao.Create(auditDom);
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
		public List<AuditDto> Query(AuditFilterDto filter)
		{
			try
			{
				var filterDom = Mapper.Map<AuditFilter>(filter);
				var result = _dao.Query(filterDom);
				return Mapper.Map<List<AuditDto>>(result);
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}
