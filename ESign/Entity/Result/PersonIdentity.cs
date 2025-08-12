namespace ESign.Entity.Result
{
    public class PersonIdentity
    {
        public int realnameStatus { get; set; }
        public bool authorizeUserInfo { get; set; }
        public string psnId { get; set; }

    }

    public class PsnAccount
    {
        public string accountMobile { get; set; }
        public string accountEmail { get; set; }
    }

    public class PsnInfo
    {
        public string psnName { get; set; }
        public string psnNationality { get; set; }
        public string psnIDCardNum { get; set; }
        public string psnIDCardType { get; set; }
        public string bankCardNum { get; set; }
        public string psnMobile { get; set; }
    }
}
