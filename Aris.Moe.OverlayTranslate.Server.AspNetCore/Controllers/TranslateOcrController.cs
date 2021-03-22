using System;
using System.Threading.Tasks;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Server;
using Aris.Moe.OverlayTranslate.Server.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Aris.Moe.OverlayTranslate.Server.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranslateOcrController : ControllerBase
    {
        private readonly IOverlayTranslateServer _overlayTranslateServer;

        public TranslateOcrController(IOverlayTranslateServer overlayTranslateServer)
        {
            _overlayTranslateServer = overlayTranslateServer;
        }

        [HttpPost("public")]
        public async Task<ActionResult<ResultResponse<OcrTranslateResponse>>> Public(PublicOcrTranslationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return new ResultResponse<OcrTranslateResponse>(await _overlayTranslateServer.TranslatePublic(request));
        }
    }
}