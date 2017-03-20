using System;
using System.Collections.Generic;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Persistence.Models.Filters;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;
using DocumentManager.Services.Filters;

namespace DocumentManager.Services
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
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
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
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}
