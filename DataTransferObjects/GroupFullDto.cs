namespace webapiSBIFS.DataTransferObjects
{
    public class GroupFullDto
    {
        public int GroupID { get; set; }
        public int? OwnerID { get; set; }
        public List<User>? Participants { get; set; } = new List<User>();
        public List<Activity>? Activities { get; set; } = new List<Activity>();
    }
}
