namespace urele.Service.Model
{
	public class TokenEntity
	{
		public string username { get; set; } = String.Empty;
		public string email { get; set; } = String.Empty;
		public string token { get; set; } = String.Empty;
		public DateTime tokenExpiresOn { get; set; }
	}
}
