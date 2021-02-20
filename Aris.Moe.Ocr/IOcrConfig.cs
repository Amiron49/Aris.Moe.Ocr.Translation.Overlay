namespace Aris.Moe.Ocr
{
    public interface IOcrConfig
    {
        public bool Cache { get; set; }
        public string CacheFolderRoot { get; set; }
    }
}