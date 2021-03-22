using System;
using Aris.Moe.OverlayTranslate.Configuration;
using Aris.Moe.Translate;

namespace Aris.Moe.OverlayTranslate.Server.AspNetCore
{
    public class ApiConfiguration : BaseConfiguration, ITranslateConfig
    {
        public bool Cache { get; set; } = false;
        public string CacheFolderRoot { get; set; } 
    }
}