namespace webapiSBIFS.Model
{
    public class Activity
    {
        public int ActivityID { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int OwnerID { get; set; }
        public List<User>? Participants { get; set; }
        public Group? Group { get; set; }
    }
}
