namespace webapiSBIFS.Model
{
    public class ActivityReceipt
    {
        public Activity activity { get; set; }
        public List<UserBalance> balances { get; set; }

        public ActivityReceipt(Activity activity, List<UserBalance> balances)
        {
            this.activity = activity;
            this.balances = balances;
        }
    }
}
