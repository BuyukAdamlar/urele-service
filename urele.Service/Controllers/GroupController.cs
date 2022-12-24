using Microsoft.AspNetCore.Mvc;
using urele.Service.Helper;
using urele.Service.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace urele.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        Token tk = new Token();

        //kullanıcının üyesi olduğu grupları listelemek için kullanılır
        [HttpPost]
        public async Task<ActionResult<List<string>>> getGroupNames(requestToken rt)
        {
            string user = tk.decrypt(rt.token).username;
            string query = $"MATCH (n:User) WHERE n.username = '{user}' MATCH (n)-[:MEMBER]->(g:Group) RETURN g.name AS name";
            var qres = (await Executor.execute(query))["name"].Select(x => x.ToString()).ToList();
            return Ok(qres);
        }

        [HttpPut]
        public async Task<ActionResult> createGroup(createGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            long check = (long)(await Executor.executeOneNode($"MATCH(g:Group) WHERE g.name = '{cg.groupName}' RETURN COUNT(g) AS c"))["c"];
            if (check != 0)
            {
                return Conflict("Grup adı zaten mevcut!");
            }
            string query = $"MATCH(u:User) WHERE u.username='{user}' CREATE(u)-[:MEMBER]->(g:Group {{name:'{cg.groupName}'}})";
            await Executor.executeReturnless(query);
            return Ok();
        }

        [HttpPost("invite")]
        public async Task<ActionResult> inviteToGroup(inviteToGroup itg)
        {
            string user = tk.decrypt(itg.token).username;
            string query = $"MATCH(u:User)-[:MEMBER]->(g:Group) WHERE u.username = '{user}' AND g.name = '{itg.groupName}' WITH g " +
                $"MATCH(u:User) WHERE u.username = '{itg.invitingUsername}' CREATE (u)-[:INVITED]->(g)";
            await Executor.executeReturnless(query);
            return Ok();
        }

        [HttpPost("request")]
        public async Task<ActionResult> requestToJoin(createGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User) WHERE u.username = '{user}' WITH u " +
                $"MATCH(g:Group) WHERE g.name = '{cg.groupName}' CREATE (u)-[:REQUESTED]->(g)";
            await Executor.executeReturnless(query);
            return Ok();
        }

        //Kullanıcının istek yollamadığı, davet edilmediği ve üyesi olmaayan tüm grupları listeler bu alttaki metot
        [HttpPost("all")]
        public async Task<ActionResult<List<string>>> getAllGroups(requestToken rt)
        {
            string user = tk.decrypt(rt.token).username;
            string query = $"MATCH(u:User) WHERE u.username='{user}' MATCH(g:Group) WHERE NOT (u)-->(g) RETURN g.name AS n";
            var res = (await Executor.execute(query))["n"].Select(e => e.ToString()).ToList();
            return Ok(res);
        }

        [HttpPost("accept/invite")]
        public async Task<ActionResult> acceptInvite(createGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[r:INVITED]->(g:Group) WHERE u.username = '{user}' AND g.name = '{cg.groupName}' " +
                $"DELETE r WITH u, g CREATE(u)-[:MEMBER]->(g)";
            await Executor.executeReturnless(query);
            return Ok();
        }

        [HttpPost("accept/request")]
        public async Task<ActionResult> acceptRequest(inviteToGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[:MEMBER]->(g:Group) WHERE u.username = '{user}' AND g.name = '{cg.groupName}' WITH g" +
                $" MATCH(u:User)-[r:REQUESTED]->(g) WHERE u.username = '{cg.invitingUsername}'  " +
                $"DELETE r WITH u, g CREATE(u)-[:MEMBER]->(g)";
            await Executor.executeReturnless(query);
            return Ok();
        }

        //Gruptan çıkmak amacıyla kullanılabilir.
        [HttpPost("leave")]
        public async Task<ActionResult> leave(createGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[r:MEMBER]->(g:Group) WHERE u.username = '{user}' AND g.name = '{cg.groupName}' " +
                $" DELETE r";
            await Executor.executeReturnless(query);
            return Ok();
        }

        //grup davetini reddetmek amacıyla kullanılabilir.
        [HttpPost("denny/invite")]
        public async Task<ActionResult> dennyInvite(createGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[r:INVITED]->(g:Group) WHERE u.username = '{user}' AND g.name = '{cg.groupName}' " +
                $" DELETE r";
            await Executor.executeReturnless(query);
            return Ok();
        }

        //grup talebini reddetmek amacıyla kullanılabilir
        [HttpPost("denny/request")]
        public async Task<ActionResult> dennyRequest(inviteToGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[:MEMBER]->(g:Group) WHERE u.username = '{user}' AND g.name = '{cg.groupName}' WITH g " +
                $" MATCH(u:User)-[r:REQUESTED]->(g) WHERE u.username = '{cg.invitingUsername}' " +
                $" DELETE r";
            await Executor.executeReturnless(query);
            return Ok();
        }

        [HttpPost("members")]
        public async Task<ActionResult<List<string>>> getGroupMembers(createGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[:MEMBER]->(g:Group) WHERE u.username = '{user}' AND g.name = '{cg.groupName}' WITH g " +
                $" MATCH(u:User)-[:MEMBER]->(g) RETURN u.username AS n";
            var res = (await Executor.execute(query))["n"].Select(e => e.ToString()).ToList();
            return Ok(res);
        }

        //invite yollayan grup isimlerini döndürür
        [HttpPost("invites")]
        public async Task<ActionResult<List<string>>> getInvites(requestToken cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[:INVITED]->(g:Group) WHERE u.username = '{user}' RETURN g.name AS n";
            var res = (await Executor.execute(query))["n"].Select(e => e.ToString()).ToList();
            return Ok(res);
        }


        //request yollayan kullanıcı isimlerini döndürür
        [HttpPost("requests")]
        public async Task<ActionResult<List<string>>> getGroupRequests(createGroup cg)
        {
            string user = tk.decrypt(cg.token).username;
            string query = $"MATCH(u:User)-[:MEMBER]->(g:Group) WHERE u.username = '{user}' AND g.name = '{cg.groupName}' WITH g " +
                $" MATCH(u:User)-[:REQUESTED]->(g) RETURN u.username AS n";
            var res = (await Executor.execute(query))["n"].Select(e => e.ToString()).ToList();
            return Ok(res);
        }
    }
}
