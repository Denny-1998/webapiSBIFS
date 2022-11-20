using System.ComponentModel.DataAnnotations.Schema;

namespace webapiSBIFS.Model
{
    public class Group
    {
        public int GroupID { get; set; }
        public int OwnerID { get; set; }
        public List<User>? Participants { get; set; }
        public List<Bill>? Bills { get; set; }
    }
}
