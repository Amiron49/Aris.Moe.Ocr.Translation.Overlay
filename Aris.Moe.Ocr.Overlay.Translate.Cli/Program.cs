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

            string result;
            do
            {
                Console.WriteLine("t: translate");
                Console.WriteLine("o: toggle overlay");
                Console.WriteLine("r: ocr the screen");
                Console.WriteLine("x: exit");
                Console.WriteLine("Press any of the above keys: ");
                result = Console.ReadKey().KeyChar.ToString();

                Console.WriteLine();

                switch (result)
                {
                    case "t":
                        await ocrTranslateOverlay.TranslateScreen();
                        break;
                    case "o":
                        ocrTranslateOverlay.ToggleOverlay();
                        break;
                    case "r":
                        await ocrTranslateOverlay.OcrScreen();
                        break;
                }
            } while (result != "x");
        }
    }
}