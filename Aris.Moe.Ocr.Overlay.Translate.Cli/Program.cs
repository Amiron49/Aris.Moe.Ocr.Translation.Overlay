using System;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Lamar;

namespace Aris.Moe.Ocr.Overlay.Translate.Cli
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var config = new Config();

            var container = new Container(new OverlayTranslateCliRegistry(config));

            var ocrTranslateOverlay = container.GetInstance<IOcrTranslateOverlay>();

            ocrTranslateOverlay.ShowOverlay();

            var result = "";
            do
            {
                Console.WriteLine("t: translate");
                Console.WriteLine("o: toggle overlay");
                Console.WriteLine("r: ocr the screen");
                Console.WriteLine("x: exit");
                Console.WriteLine("Press any of the above keys: ");
                result = Console.ReadKey().KeyChar.ToString();

                Console.WriteLine();

                if (result == "t")
                {
                    await ocrTranslateOverlay.TranslateScreen();
                }
                else if (result == "o")
                {
                    ocrTranslateOverlay.ToggleOverlay();
                }
                else if (result == "r")
                {
                    await ocrTranslateOverlay.OcrScreen();
                }
            } while (result != "x");
        }
    }
}