namespace Aris.Moe.Translate
{
    public interface ITranslateConfig
    {
        public bool Cache { get; set; }
        public string? CacheFolderRoot { get; set; }
    }
}