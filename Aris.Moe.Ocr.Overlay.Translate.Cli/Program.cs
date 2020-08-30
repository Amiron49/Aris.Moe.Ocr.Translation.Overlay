using System;
using System.Threading.Tasks;
using Aris.Moe.Overlay;

namespace Aris.Moe.Ocr.Overlay.Translate.Cli
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using var render = new Moe.Overlay.Overlay();

            await render.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            await render.Stop();
        }
    }
}