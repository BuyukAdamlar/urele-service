namespace urele.Service.Model
{
    public class requestForShareToUsers
    {
        public List<string> usernames { get; set; }
        public string token { get; set; }
        public string shortLink { get; set; }
    }

    public class requestForShareToGroups
    {
        public List<string> groupNames { get; set; }
        public string token { get; set; }
        public string shortLink { get; set; }
    }

    public class checkUsernameForShare
    {
        public string username { get; set; }
        public string token { get; set; }
    }

}
