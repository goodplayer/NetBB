using Microsoft.AspNetCore.Mvc;
using NetBB.Sources.EnhancedWeb;
using System.Net.Mime;

namespace NetBB.Sources.Apis
{
    [Route("api/user/{id}")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class UserApiController : ApiControllerBase
    {
        [HttpGet]
        public IActionResult Get(string id)
        {
            return Ok("id:"+id);
        }
    }
}
