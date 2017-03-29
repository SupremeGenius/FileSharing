﻿using FileStorage.Services.Dtos;
using System;
namespace FileStorage.Services.Filters
{
	public class AuditFilterDto
	{
		public long? IdUser { get; set; }
		public string Object { get; set; }
		public string IdObject { get; set; }
        public ActionDto? Action { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
	}
}
