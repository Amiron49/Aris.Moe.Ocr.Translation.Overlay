using System.Collections.Generic;

namespace Aris.Moe.Configuration
{
    public interface INeedConfiguration
    {
        public string Name { get; }
        IEnumerable<string> GetConfigurationIssues();
    }
}