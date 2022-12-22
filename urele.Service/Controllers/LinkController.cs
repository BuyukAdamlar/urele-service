using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using urele.Service.Helper;
using urele.Service.Model;

namespace urele.Service.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class LinkController : ControllerBase
	{
		private static string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$&()-_+=[]'";
		Token tk = new Token();

		//Yeni link kısaltmak için kullanılır
		[HttpPut]
		public async Task<ActionResult<string>> CreateLink(CreateLinkModel clm)
		{
			ZonedDateTime creation = new ZonedDateTime(DateTime.Now);
			ZonedDateTime expire = new ZonedDateTime(DateTime.Now.AddMonths(6));
			string shortLink = await generateShortLink();
			string query = $" CREATE (n:Link {{url: '{clm.url}', shortLink:'{shortLink}', title:'', description: '', clickCount: 0, " +
					$"createdOn: datetime('{creation}'), expiresOn: datetime('{expire}'), waitTime: 0 }})";
			if (clm.token != null)
			{
				var tokenParsed = tk.decrypt(clm.token);
				query = $"MATCH(u:User) WHERE u.username = '{tokenParsed.username}' WITH u " + query + $"-[:CREATED_BY]->(u)";
			}
			var res = await Executor.executeReturnless(query);
			if (res == true)
			{
				return Ok(shortLink);
			}
			else
			{
				return BadRequest();
			}
		}
		private async Task<string> generateShortLink()
		{
			long count = 1;
			string res = "";
			do
			{
				Random rnd = new Random();
				for (int i = 0; i < 5; i++)
				{
					res += charset[rnd.Next(0, charset.Length)];
				}
				count = (long)(await Executor.executeOneNode($"MATCH (n:Link) WHERE n.shortLink = '{res}' RETURN COUNT(n) AS c"))["c"];
			} while (count != 0);
			return res;
		}


		//Link oluşturduktan sonra açılacak olan kutucuktan linkin ayarlarını yapmak için kullanılır
		[HttpPost]
		public async Task<ActionResult> updateWhileCreating(updateLinkOnCreating uloc)
		{
			ZonedDateTime zdt = new ZonedDateTime(uloc.expiresOn);
			string query = $"MATCH (n:Link) WHERE n.shortLink = '{uloc.shortLink}' SET n.title = '{uloc.title}', n.description = '{uloc.description}', " +
				$"n.waitTime = {uloc.waitTime}, n.expiresOn = datetime('{zdt}')";
			bool res = await Executor.executeReturnless(query);
			if (res)
			{
				return Ok();
			}
			else
			{
				return BadRequest();
			}
		}


		//Kullanıcının linklerini listelemek için kullanılır
		[Route("user")]
		[HttpPost]
		public async Task<ActionResult<List<Link>>> getUserLinks(GetUserLinks gul)
		{
			var tokenParsed = tk.decrypt(gul.token);
			string query = $"MATCH(l:Link)-[:CREATED_BY]->(u:User) WHERE u.username = '{tokenParsed.username}' RETURN l";
			var res = await Executor.execute(query);
			List<Link> links = new List<Link>();
			foreach (var obj in res["l"])
			{
				Link link = new Link();
				var props = ((NodeEntity)obj).Properties;
				link.description = (string)props["description"];
				link.created_by = "";
				link.title = (string)props["title"];
				link.shortLink = (string)props["shortLink"];
				link.clickCount = (long)props["clickCount"];
				link.createdOn = Executor.NeoDateTimeDecrypt(props["createdOn"]);
				link.expiresOn = Executor.NeoDateTimeDecrypt(props["expiresOn"]);
				link.url = (string)props["url"];
				link.waitTime = (long)props["waitTime"];
				link.created_by = tokenParsed.username;
				links.Add(link);
			}
			return Ok(links);
		}
	}
}
