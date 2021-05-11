namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    /// <summary>
    /// 2D positioned text boxes
    /// </summary>
    public class SpatialTextViewModel
    {
        public string Text { get; set; }
        public RectangleModel Position { get; set; }
        
        public SpatialTextViewModel(string text, RectangleModel position)
        {
            Text = text;
            Position = position;
        }
    }
}