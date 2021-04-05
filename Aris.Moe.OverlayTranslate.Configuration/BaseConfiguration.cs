namespace Aris.Moe.OverlayTranslate.Configuration
{
    public class BaseConfiguration
    {
        public GoogleConfiguration Google { get; set; } = new GoogleConfiguration();
        public DeeplConfiguration Deepl { get; set; } = new DeeplConfiguration();
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
    }
}