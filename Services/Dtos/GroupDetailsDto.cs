namespace FileSharing.Services.Dtos
{
    public class GroupDetailsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int NumOfMembers { get; set; }
        public bool IsAdministrable { get; set; }
    }
}
