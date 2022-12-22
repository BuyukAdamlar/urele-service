namespace urele.Service.Model
{
	public class Link
	{
		public string created_by { get; set; }
		public string url { get; set; }
		public string shortLink { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public long clickCount { get; set; }
		public DateTime createdOn { get; set; }
		public DateTime expiresOn { get; set; }
		public long waitTime { get; set; }
	}
}
