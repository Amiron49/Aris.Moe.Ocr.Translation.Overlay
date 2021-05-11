using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.TestServer.Pages
{
    public class ScalingModel : PageModel
    {
        private readonly ILogger<ScalingModel> _logger;

        public ScalingModel(ILogger<ScalingModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}