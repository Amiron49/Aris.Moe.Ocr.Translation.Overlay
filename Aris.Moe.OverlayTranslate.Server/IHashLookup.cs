using Aris.Moe.OverlayTranslate.Server.ViewModel;

namespace Aris.Moe.OverlayTranslate.Server
{
    public interface IHashLookup: IApiRequest
    {
        public byte[] ImageHash { get; set; }
    }
}