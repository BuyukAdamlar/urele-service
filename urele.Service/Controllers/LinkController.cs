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
        private static string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
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
            await Executor.executeReturnless(query);
            return Ok(shortLink);
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
        public async Task<ActionResult> updateLink(updateLinkOnCreating uloc)
        {
            ZonedDateTime zdt = new ZonedDateTime(uloc.expiresOn);
            string query = $"MATCH (n:Link) WHERE n.shortLink = '{uloc.shortLink}' SET n.title = '{uloc.title}', n.description = '{uloc.description}', " +
                $"n.waitTime = {uloc.waitTime}, n.expiresOn = datetime('{zdt}')";
            await Executor.executeReturnless(query);
            return Ok();
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


        [HttpPost("special/check")]
        public async Task<ActionResult<bool>> checkForSpecial(CheckSpecialModel csm)
        {
            var res = tk.decrypt(csm.token);
            long count = (long)(await Executor.executeOneNode($"MATCH (n:Link) WHERE n.shortLink = '{csm.shortUrl}' RETURN COUNT(n) AS c"))["c"];
            if (count == 0)
            {
                return Ok(true);
            }
            else
            {
                return Conflict(false);
            }
        }

        [HttpPost("special")]
        public async Task<ActionResult<string>> generateSpecial(CreateSpecialModel csm)
        {
            long count = (long)(await Executor.executeOneNode($"MATCH (n:Link) WHERE n.shortLink = '{csm.shortUrl}' RETURN COUNT(n) AS c"))["c"];
            if (count != 0)
            {
                return Conflict("Bu kısa link zaten mevcut");
            }
            var tokenParsed = tk.decrypt(csm.token);
            ZonedDateTime creation = new ZonedDateTime(DateTime.Now);
            ZonedDateTime expire = new ZonedDateTime(DateTime.Now.AddMonths(6));
            string query = $"MATCH(u:User) WHERE u.username = '{tokenParsed.username}' WITH u CREATE (n:Link {{url: " +
                $"'{csm.url}', shortLink:'{csm.shortUrl}', title:'', description: '', clickCount: 0, " +
                $"createdOn: datetime('{creation}'), expiresOn: datetime('{expire}'), waitTime: 0 }})-[:CREATED_BY]->(u)";
            await Executor.executeReturnless(query);
            return Ok(csm.shortUrl);
        }

        [HttpPost("generateOther")]
        public async Task<ActionResult<string>> generateOtherLink(GenerateOtherModel gom)
        {
            string val = gom.value.Trim().Replace(" ", "");
            switch (gom.type)
            {
                case OtherTypes.Mail:
                    val = "mailto:" + val;
                    break;
                case OtherTypes.Sms:
                    val = "sms:" + val;
                    break;
                case OtherTypes.Telefon:
                    val = "tel:" + val;
                    break;
                case OtherTypes.Whatsapp:
                    val = "wa.me/" + val;
                    break;
            }
            if (gom.token == null || gom.token.Trim() == "")
            {
                return await CreateLink(new CreateLinkModel
                {
                    url = val
                });
            }
            else
            {
                return await CreateLink(new CreateLinkModel
                {
                    token = gom.token,
                    url = val
                });

            }

        }


        //Kullanıcılar tarafından paylaşılmış linkler için kullanılır
        [HttpPost("getShared")]
        public async Task<ActionResult<List<SharedLinksUser>>> getSharedLinks(requestToken rt)
        {
            var user = tk.decrypt(rt.token);
            string query = $"MATCH (l:Link)-[:SHARED]->(u:User) WHERE u.username = '{user.username}' WITH l " +
                $" MATCH (l)-[:CREATED_BY]->(u:User) RETURN l.shortLink AS sl, l.title AS t, l.description AS d, u.username AS u, l.expiresOn AS e";
            var qres = await Executor.execute(query);
            List<SharedLinksUser> res = new List<SharedLinksUser>();
            foreach (var obj in qres["sl"])
            {
                SharedLinksUser slu = new SharedLinksUser();
                slu.shortLink = (string)obj;
                slu.title = (string)qres["t"][res.Count];
                slu.description = (string)qres["d"][res.Count];
                slu.username = (string)qres["u"][res.Count];
                slu.expiresOn = (DateTime)(qres["e"][res.Count]);
                res.Add(slu);
            }
            return Ok(res);
        }

        [HttpPost("getGroupShared")]
        public async Task<ActionResult<List<SharedLinksGroup>>> getGroupSharedLinks(requestToken rt)
        {
            var user = tk.decrypt(rt.token);
            string query = $"MATCH(u:User)-[:MEMBER]->(g:Group) WHERE u.username = '{user.username}' WITH g " +
                $"MATCH (l:Link)-[:SHARED]->(g) WITH g, l " +
                $"MATCH (l)-[:CREATED_BY]->(u:User) RETURN l.shortLink AS sl, l.title AS t, l.description AS d, u.username AS u, l.expiresOn AS ex, g.name AS gn";
            var qres = await Executor.execute(query);
            List<SharedLinksGroup> res = new List<SharedLinksGroup>();
            foreach (var obj in qres["sl"])
            {
                SharedLinksGroup slg = new SharedLinksGroup();
                slg.shortLink = (string)obj;
                slg.title = (string)qres["t"][res.Count];
                slg.description = (string)qres["d"][res.Count];
                slg.username = (string)qres["u"][res.Count];
                slg.expiresOn = (DateTime)(qres["ex"][res.Count]);
                slg.groupname = (string)qres["gn"][res.Count];
                res.Add(slg);
            }
            return Ok(res);
        }


        [HttpPost("click")]
        public async Task<ActionResult> addClick(requestToken shortLink)
        {
            string query = $"MATCH (n:Link) WHERE n.shortLink = '{shortLink.token}' SET n.clickCount = n.clickCount + 1";
            await Executor.executeReturnless(query);
            return Ok();
        }
    }




    [ApiController]
    public class OpenLinkController : ControllerBase
    {
        [HttpGet("{shortLink}")]
        public async Task<ActionResult<GoShortLink>> goShortLink(string shortLink)
        {
            bool second = false;
            string query = $"MATCH(l:Link)-[:CREATED_BY]->(u:User) WHERE l.shortLink = '{shortLink}' RETURN l, u.username AS u";
            var qres = await Executor.executeOneNode(query);
            if (qres.Count == 0)
            {
                second = true;
                query = $"MATCH(l:Link) WHERE l.shortLink = '{shortLink}' RETURN l";
                qres = await Executor.executeOneNode(query);
            }
            var result = new GoShortLink();
            var linkRes = ((NodeEntity)qres["l"]).Properties;
            result.title = (string)linkRes["title"];
            result.description = (string)linkRes["description"];
            if (!second)
            {
                result.creator = (string)qres["u"];
            }
            else
            {
                result.creator = "Anonim";
            }
            result.waitTime = (long)linkRes["waitTime"];
            result.url = (string)linkRes["url"];
            result.expiresOn = Executor.NeoDateTimeDecrypt(linkRes["expiresOn"]);
            return Ok(result);
        }
    }
}
