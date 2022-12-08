namespace webapiSBIFS.Model
{
    public class ActivityFullDto
    {
        public int ActivityID { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int OwnerID { get; set; }
        public List<string> ParticipantsEmail { get; set; } = new List<string>(); 
        public int GroupID { get; set; }
    }
}
