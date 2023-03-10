namespace documentmgr.dto
{
    public class EmailSettings
    {
        public bool AuthenticateCredentials { get; set; }
        public int Security { get; set; }
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Password { get; set; }
    }
}
