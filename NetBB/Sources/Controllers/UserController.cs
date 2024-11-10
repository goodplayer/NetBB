using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetBB.Sources.EnhancedWeb;
using NetBB.Sources.Components;
using System.Net.Mime;

namespace NetBB.Sources.Controllers
{
    [Route("async/user/{id}")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class UserController : AsyncControllerBase
    {
        [HttpGet]
        public IActionResult Get(string id)
        {
            var result = new Dictionary<string, object>();

            result["name"] = "demo1:" + id;

            return Ok(result);
        }
    }
}
