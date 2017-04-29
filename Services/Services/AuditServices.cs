using System;
using System.Collections.Generic;
using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Persistence.Models.Filters;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using FileSharing.Services.Filters;

namespace FileSharing.Services
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
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
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
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}
