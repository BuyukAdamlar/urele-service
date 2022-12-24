using Microsoft.AspNetCore.Mvc;
using urele.Service.Helper;
using urele.Service.Model;

namespace urele.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Statistics>> Get()
        {
            string q1 = "MATCH (l:Link) WITH COUNT(l) AS ls " +
                " MATCH (u:User) RETURN ls, COUNT(u) AS us ";
            string q2 = " MATCH(l:Link)-[:CREATED_BY]->(u:User) RETURN u.username AS u, COUNT(l) AS ct ORDER BY ct DESC LIMIT 10";
            string q3 = " MATCH(l:Link)-[:CREATED_BY]->(u:User) RETURN u.username AS un, l.shortLink AS sl, l.clickCount AS cc ORDER BY cc DESC LIMIT 10";
            var qres1 = await Executor.executeOneNode(q1);
            var qres2 = await Executor.execute(q2);
            var qres3 = await Executor.execute(q3);
            Statistics stats = new Statistics();
            stats.shortLinkCount = (long)qres1["ls"];
            stats.userCount = (long)qres1["us"];
            var topUserNames = qres2["u"];
            var topUserCounts = qres2["ct"];
            stats.topUsers = new List<topUser>();
            for (int i = 0; i < topUserNames.Count; i++)
            {
                stats.topUsers.Add(
                    new topUser
                    {
                        linkCount = (long)topUserCounts[i],
                        username = (string)topUserNames[i]

                    });
            }
            var topShortLinks = qres3["sl"];
            var topShortLinkCounts = qres3["cc"];
            var topShortLinkNames = qres3["un"];
            stats.topLinks = new List<topLink>();
            for (int i = 0; i < topShortLinks.Count; i++)
            {
                stats.topLinks.Add(
                    new topLink
                    {
                        clickCount = (long)topShortLinkCounts[i],
                        shortLink = (string)topShortLinks[i],
                        username = (string)topShortLinkNames[i]
                    });
            }
            return stats;
        }
    }
}
