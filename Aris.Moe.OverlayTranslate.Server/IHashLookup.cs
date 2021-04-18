using Aris.Moe.OverlayTranslate.Server.ViewModel;

namespace Aris.Moe.OverlayTranslate.Server
{
    public interface IHashLookup: IApiRequest
    {
        public byte[] ImageHash { get; set; }
    }
    
    public interface IUrlLookup: IApiRequest
    {
        public string ImageUrl { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
    }
}