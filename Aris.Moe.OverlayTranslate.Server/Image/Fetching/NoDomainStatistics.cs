namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public class NoDomainStatistics : IDomainStatistic
    {
        public string Domain { get; } = "NoDomain";

        public double RequestsPerSecond()
        {
            return 0;
        }

        public void AddRequest()
        {
        }
    }
}