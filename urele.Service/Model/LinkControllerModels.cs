namespace urele.Service.Model
{
	public class updateLinkOnCreating
	{
		public string shortLink { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public long waitTime { get; set; }
		public DateTime expiresOn { get; set; }
	}
	public class CreateLinkModel
	{
		public string url { get; set; }
		public string? token { get; set; }
	}
	public class GetUserLinks
	{
		public string token { get; set; }
	}
}