namespace webapiSBIFS.Model
{
    public class UserBalance
    {
        public string userEmail { get; set; }
        public double balance { get; set; }



        public UserBalance(string userEmail, double balance)
        {
            this.userEmail = userEmail;
            this.balance = balance;
        }
    }
}
