using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.TestServer.Pages
{
    public class SpawningModel : PageModel
    {
        private readonly ILogger<SpawningModel> _logger;

        public SpawningModel(ILogger<SpawningModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}