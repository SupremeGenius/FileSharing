namespace FileSharing.Persistence.Models.Filters
{
    public class UserGroupFilter
    {
        public long? IdUser { get; set; }
        public long? IdGroup { get; set; }
        public bool Accepted { get; set; }
    }
}
