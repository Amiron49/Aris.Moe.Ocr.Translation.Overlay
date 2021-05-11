using Microsoft.AspNetCore.Mvc;

namespace Aris.Moe.TestServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult Public(string id, [FromQuery] ImageQuery query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stream = typeof(ImageController).Assembly.GetManifestResourceStream($"Aris.Moe.TestServer.{id}.jpg")!;
            
            return new FileStreamResult(stream, "image/jpeg");
        }
    }

    public class ImageQuery
    {
        public int? Time { get; set; }
        public int? StatusCode { get; set; }
    }
}