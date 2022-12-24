using Microsoft.AspNetCore.Mvc;
using urele.Service.Helper;
using urele.Service.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace urele.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShareController : ControllerBase
    {
        Token tk = new Token();
        [HttpPost("check")]
        public async Task<ActionResult<bool>> CheckUsername(checkUsernameForShare cufs)
        {
            var usr = tk.decrypt(cufs.token);
            var result = (long)(await Executor.executeOneNode($"MATCH(n:User) WHERE n.username = '{cufs.username}' RETURN COUNT(n) AS c"))["c"];
            if (result == 1)
            {
                return Ok(true);

            }
            else
            {
                return NotFound(false);
            }
        }

        [HttpPost]
        public async Task<ActionResult> shareToUsers([FromBody] requestForShareToUsers rfs)
        {

            string user = tk.decrypt(rfs.token).username;
            string query = $"MATCH(l:Link)-[:CREATED_BY]->(u:User) WHERE l.shortLink = '{rfs.shortLink}' AND u.username = '{user}' WITH l ";
            string with = "WITH l";
            for (int j = 0; j < rfs.usernames.Count; j++)
            {
                if (j != rfs.usernames.Count - 1)
                    query += $" MATCH(u:User) WHERE u.username = '{rfs.usernames[j]}' CREATE(l)-[:SHARED]->(u) {with} ";
                else
                    query += $" MATCH(u:User) WHERE u.username = '{rfs.usernames[j]}' CREATE(l)-[:SHARED]->(u); ";
            }
            await Executor.executeReturnless(query);
            return Ok();
        }

        [HttpPost("group")]
        public async Task<ActionResult> shareToGroups([FromBody] requestForShareToGroups rfsg)
        {
            string user = tk.decrypt(rfsg.token).username;
            string query = $"MATCH(l:Link)-[:CREATED_BY]->(u:User) WHERE l.shortLink = '{rfsg.shortLink}' AND u.username = '{user}' WITH l ";
            string with = "WITH l";
            for (int j = 0; j < rfsg.groupNames.Count; j++)
            {
                if (j != rfsg.groupNames.Count - 1)
                    query += $" MATCH(g:Group) WHERE g.name = '{rfsg.groupNames[j]}' CREATE(l)-[:SHARED]->(g) {with} ";
                else
                    query += $" MATCH(g:Group) WHERE g.name = '{rfsg.groupNames[j]}' CREATE(l)-[:SHARED]->(g); ";
            }
            await Executor.executeReturnless(query);
            return Ok();
        }
    }
}
