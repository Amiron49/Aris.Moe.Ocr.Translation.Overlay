using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Aris.Moe.OverlayTranslate.Server.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : Controller
    {
        private readonly StatisticsAggregator _statisticsAggregator;

        public StatisticsController(StatisticsAggregator statisticsAggregator)
        {
            _statisticsAggregator = statisticsAggregator;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stats = await _statisticsAggregator.Create();
            
            return new JsonResult(stats);
        }
    }
}