using System.Collections.Generic;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface INeedConfiguration
    {
        public string Name { get; }
        IEnumerable<string> GetConfigurationIssues();
    }
}