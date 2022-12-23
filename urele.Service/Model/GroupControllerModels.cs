namespace urele.Service.Model
{
    public class createGroup
    {
        public string token { get; set; }
        public string groupName { get; set; }
    }

    public class inviteToGroup
    {
        public string token { get; set; }
        public string groupName { get; set; }
        public string invitingUsername { get; set; }
    }

    public class requestToken
    {
        public string token { get; set; }
    }

}
