using Microsoft.AspNetCore.Mvc;
using urele.Service.Helper;
using urele.Service.Model;

namespace urele.Service.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		Token tk { get; } = new Token();
		[HttpPut]
		public async Task<ActionResult> Register(User usr)
		{
			//neo4j query for checking username or email exists
			var isRegistered = await Executor.executeOneNode($"MATCH (u:User) WHERE u.username = '{usr.username}' OR u.email = '{usr.email}' RETURN COUNT(u) AS ct");
			if ((long)isRegistered["ct"] != 0)
			{
				//Eğer kullanıcı adı veya email varsa
				return Conflict("This username or email is existing");
			}
			usr.password = SpassEnc.Encrypt(usr.password);
			string query = $"CREATE (n:User {{username: '{usr.username}', name: '{usr.name}', surname: '{usr.surname}', email: '{usr.email}', password: '{usr.password}'}})";
			await Executor.executeReturnless(query);

			return Ok();

		}
		[Route("login")]
		[HttpPost]
		public async Task<ActionResult<TokenEntity>> Login(LoginModel lm)
		{
			string id = lm.id;
			string password = SpassEnc.Encrypt(lm.password);
			string query = $"MATCH (n:User) WHERE n.password = '{password}' AND (n.username ='{id}' OR n.email = '{id}') RETURN COUNT(n) AS c";
			var res = (long)(await Executor.executeOneNode(query))["c"];
			if (res == 1)
			{
				string getUserQuery = $"MATCH (n:User) WHERE n.password = '{password}' AND (n.username ='{id}' OR n.email = '{id}') RETURN n.username AS un, n.email AS e, n.password AS p, n.name AS n, n.surname AS s";
				var getRes = await Executor.executeOneNode(getUserQuery);
				User usr = new User
				{
					username = (string)getRes["un"],
					email = (string)getRes["e"],
					password = password,
					name = (string)getRes["n"],
					surname = (string)getRes["s"]
				};
				TokenEntity tkent = tk.encrypt(usr);
				return Ok(tkent);
			}
			else
			{
				throw new Exception("Giriş bilgileri hatalı...");
			}
		}



	}
}
