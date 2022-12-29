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
            Guid activation = Guid.NewGuid();
            usr.password = SpassEnc.Encrypt(usr.password);
            string query = $"CREATE (n:User {{username: '{usr.username}', name: '{usr.name}', surname: '{usr.surname}', email: '{usr.email}', password: '{usr.password}', activation: '{activation}'}})";
            await Executor.executeReturnless(query);
            string mailBody = SendMail.mailText.Replace("azzxsdara5612661", URL.baseUrl + "/User/activ ate/" + activation).Replace("http://urele.azurewebsites.net", URL.mainUrl);
            await SendMail.sendMail(usr.email, "URELE HESAP AKTİVASYONU", mailBody);
            return Ok();

        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenEntity>> Login(LoginModel lm)
        {
            string id = lm.id;
            string password = SpassEnc.Encrypt(lm.password);
            string query = $"MATCH (n:User) WHERE n.password = '{password}' AND (n.username ='{id}' OR n.email = '{id}') RETURN COUNT(n) AS c";
            var res = (long)(await Executor.executeOneNode(query))["c"];
            if (res == 1)
            {
                string getUserQuery = $"MATCH (n:User) WHERE n.password = '{password}' AND (n.username ='{id}' OR n.email = '{id}') RETURN n.username AS un, n.email AS e, n.password AS p, n.name AS n, n.surname AS s, n.activation AS act";
                var getRes = await Executor.executeOneNode(getUserQuery);
                if (getRes["act"] != null)
                {

                    return Conflict("Lütfen mailinize gönderilen link ile hesabınızı aktifleştiriniz!");
                }
                User usr = new User
                {
                    username = (string)getRes["un"],
                    email = (string)getRes["e"],
                    password = lm.password,
                    name = (string)getRes["n"],
                    surname = (string)getRes["s"]
                };
                TokenEntity tkent = tk.encrypt(usr);
                return Ok(tkent);
            }
            else
            {
                return Unauthorized("Giriş bilgileri hatalı...");
            }
        }

        [HttpPost("login/token")]
        public async Task<ActionResult<TokenEntity>> Login(requestToken rt)
        {
            var tkn = tk.decrypt(rt.token);
            return await Login(new LoginModel
            {
                id = tkn.username,
                password = tkn.password
            });
        }


        [HttpPost("edit")]
        public async Task<ActionResult> Edit(EditUser usr)
        {
            var user = tk.decrypt(usr.token);
            if (usr.username != user.username)
            {
                return BadRequest();
            }
            var isRegistered = await Executor.executeOneNode($"MATCH (u:User) WHERE u.username <> '{usr.username}' AND u.email = '{usr.email}' RETURN COUNT(u) AS ct");
            if ((long)isRegistered["ct"] != 0)
            {
                //Eğer email varsa
                return Conflict("This email is existing");
            }
            string passEnc = SpassEnc.Encrypt(usr.password);
            string query = $"MATCH(u:User) WHERE u.username = '{user.username}' SET u.name = '{usr.name}', " +
                $"u.surname = '{usr.surname}', u.email = '{usr.email}', u.password = '{passEnc}' ";
            if (usr.email != user.email)
            {
                Guid activation = Guid.NewGuid();
                query += $", u.activation= '{activation}'";
                string mailBody = SendMail.mailText.Replace("azzxsdara5612661", URL.baseUrl + "/user/activate/" + activation).Replace("http://urele.azurewebsites.net", URL.mainUrl)
                    .Replace("hesabınız oluşturuldu", "hesabınızın mail adresi değiştirildi");
                await SendMail.sendMail(usr.email, "URELE HESAP AKTİVASYONU", mailBody);
            }
            await Executor.executeReturnless(query);
            return Ok();
        }

        [HttpGet("activate/{activation}")]
        public async Task<RedirectResult> activate(string activation)
        {
            string query = $"MATCH (n:User) WHERE n.activation = '{activation}' REMOVE n.activation";
            await Executor.executeReturnless(query);
            return RedirectPermanent(URL.mainUrl + "/login");
        }

    }
}
