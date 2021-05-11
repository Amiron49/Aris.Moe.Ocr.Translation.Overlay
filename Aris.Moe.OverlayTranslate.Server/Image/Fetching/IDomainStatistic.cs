namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public interface IDomainStatistic
    {
        public string Domain { get; }
        public double RequestsPerSecond();
        public void AddRequest();
    }
}