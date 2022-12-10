namespace webapiSBIFS.Model
{
    public class GroupActivityDto
    {
        public double Amount { get; set; }

        public string Description { get; set; }

        public string OwnerEmail { get; set; }

        public List<string> ParticipantsEmail { get; set; } = new List<string>();

        public int GroupID { get; set; }
    }
}
