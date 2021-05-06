using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.TestServer.Pages
{
    public class MovingModel : PageModel
    {
        private readonly ILogger<MovingModel> _logger;

        public MovingModel(ILogger<MovingModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}