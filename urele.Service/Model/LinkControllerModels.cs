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

	public class CreateSpecialModel
	{
		public string url { get; set; }
		public string token { get; set; }
		public string shortUrl { get; set; }
	}

	public class CheckSpecialModel
	{
		public string shortUrl { get; set; }
		public string token { get; set; }
	}

	public class SharedLinksUser
	{
		public string shortLink { get; set; }
		public string username { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public DateTime expiresOn { get; set; }

	}
	public class SharedLinksGroup
	{
		public string shortLink { get; set; }
		public string username { get; set; }
		public string groupname { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public DateTime expiresOn { get; set; }
	}

	public class GenerateOtherModel
	{
		public string value { get; set; }
		public string? token { get; set; }
		public OtherTypes type { get; set; }
	}
	public enum OtherTypes
	{
		Mail, Sms, Telefon, Whatsapp
	}
}


