using System.ComponentModel.DataAnnotations;

namespace webapiSBIFS.Model
{
    public class User
    {
        public enum Privileges
        {
            admin,
            user
        }

        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public Privileges Privilege { get; set; } = Privileges.user;
        public string Password { get; set; } = string.Empty;

        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
