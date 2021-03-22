using System;
using System.Linq;

namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public class DomainStatistic : IDomainStatistic
    {
        public DomainStatistic(string domain)
        {
            Domain = domain;
        }

        private Maybe30Bag<DateTime> Requests { get; } = new();

        public string Domain { get; }

        public double RequestsPerSecond()
        {
            var maxTime = Requests.Contents.Max();
            var minTime = Requests.Contents.Min();
            var count = Requests.Contents.Count;

            var timeSpan = maxTime - minTime;
            var seconds = timeSpan.TotalSeconds;

            return seconds / count;
        }

        public void AddRequest()
        {
            Requests.Add(DateTime.UtcNow);
        }
    }
}