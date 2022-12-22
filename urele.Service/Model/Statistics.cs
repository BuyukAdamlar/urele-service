namespace urele.Service.Model
{
    public class Statistics
    {
        public long shortLinkCount { get; set; }
        public long userCount { get; set; }
        public List<topUser> topUsers { get; set; }
        public List<topLink> topLinks { get; set; }
    }

    public class topUser
    {
        public string username { get; set; }
        public long linkCount { get; set; }
    }
    public class topLink
    {
        public string shortLink { get; set; }
        public string username { get; set; }
        public long clickCount { get; set; }
    }
}
