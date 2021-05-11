using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.TestServer.Pages
{
    public class DefaultModel : PageModel
    {
        private readonly ILogger<DefaultModel> _logger;

        public DefaultModel(ILogger<DefaultModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}