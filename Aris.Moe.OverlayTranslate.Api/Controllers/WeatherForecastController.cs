using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Aris.Moe.OverlayTranslate.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranslateOcrController : ControllerBase
    {
        public TranslateOcrController()
        {
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Public(PublicOcrTranslationRequest request)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }

    /// <summary>
    /// Request for translating a publicly reachable image
    /// </summary>
    public class PublicOcrTranslationRequest
    {
        /// <summary>
        /// Original publicly reachable image url
        /// </summary>
        [Required]
        public string ImageUrl { get; set; }
        public string? ImageHash { get; set; }
    }

    public class OcrTranslateResponse
    {
        public string? ImageUrl { get; init; }
    }

    public class ImageInfo
    {
        public string Hash { get; set; }
        public string VisualHash { get; set; }
    }
}