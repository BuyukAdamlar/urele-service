using Microsoft.AspNetCore.Mvc;

namespace urele.Service.Controllers
{
    [Route("prew")]
    [ApiController]
    public class PrewController : ControllerBase
    {
        [HttpGet("{shortLink}")]
        public async Task<ActionResult> getPreview(string shortLink)
        {
            return Ok(shortLink);
        }
    }
}
