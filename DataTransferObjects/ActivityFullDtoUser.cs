namespace webapiSBIFS.Model
{
    public class ActivityFullDtoUser
    {
        public int ActivityID { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> ParticipantsEmail { get; set; } = new List<string>(); 
        public int GroupID { get; set; }
    }
}
