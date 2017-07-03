using System;

namespace FileSharing.Persistence.Models.Filters
{
    public class UserGroupFilter
    {
        public long? IdUser { get; set; }
        public long? IdGroup { get; set; }
        public DateTime? DateInclusionRequestFrom { get; set; }
        public DateTime? DateInclusionRequestTo { get; set; }
        public DateTime? DateInclusionApprovalFrom { get; set; }
        public DateTime? DateInclusionApprovalTo { get; set; }
    }
}
