using Jose;
using Newtonsoft.Json;
using urele.Service.Model;

namespace urele.Service.Helper
{
	public class Token
	{

		private readonly byte[] secretKey = new byte[] { 3, 234, 131, 182, 25, 29, 145, 80, 73, 196, 31, 218, 82, 59, 105, 110, 3, 2, 90, 147, 100, 103, 156, 208, 86, 236, 187, 141, 94, 98, 59, 190 };

		public TokenEntity encrypt(User user)
		{
			DateTime expire = DateTime.Now.AddHours(2);
			var payload = new Dictionary<string, object>
			{
				{ "username", user.username },
				{ "name", user.name },
				{ "surname", user.surname },
				{ "email", user.email },
				{ "password", user.password },
				{ "tokenExpiresOn", expire}
			};
			string token = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
			return new TokenEntity
			{
				username = user.username,
				email = user.email,
				token = token,
				tokenExpiresOn = expire
			};
		}
		public string encrypt(Dictionary<string, object> payload)
		{
			if (payload.ContainsKey("tokenExpiresOn"))
			{
				payload.Remove("tokenExpiresOn");
			}
			payload.Add("tokenExpiresOn", DateTime.Now.AddHours(2));
			return JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
		}

		public User decrypt(string token)
		{
			var payload = JWT.Decode(token, secretKey, JwsAlgorithm.HS256);
			var result = JsonConvert.DeserializeObject<User>(payload);
			if (result.tokenExpiresOn! > DateTime.Now)
			{
				return result;
			}
			else
			{
				throw new Exception("Token geçerlilik süresi dolmuş!");
			}
		}
		public User decrypt(TokenEntity token)
		{
			return decrypt(token.token);
		}


	}
}
