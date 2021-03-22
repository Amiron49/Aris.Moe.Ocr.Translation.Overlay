using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public class DomainStatistics
    {
        private readonly ILogger<DomainStatistics> _logger;
        private readonly ConcurrentDictionary<string, DomainStatistic> _domainStatistics = new();

        public DomainStatistics(ILogger<DomainStatistics> logger)
        {
            _logger = logger;
        }

        public IDomainStatistic GetStats(string url)
        {
            var domain = GetMainDomain(url);
            var hasEntry = _domainStatistics.ContainsKey(domain);

            if (!hasEntry)
            {
                var newDomain = new DomainStatistic(domain);
                newDomain = _domainStatistics.GetOrAdd(domain, newDomain);
                return newDomain;
            }

            var success = _domainStatistics.TryGetValue(domain, out var existingDomain);

            if (success)
                return existingDomain!;

            _logger.LogWarning("Failed to get DomainStatistics");
            return new NoDomainStatistics();
        }

        private static string GetMainDomain(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }
    }
}