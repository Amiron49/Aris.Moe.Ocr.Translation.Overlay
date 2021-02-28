using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aris.Moe.Configuration;

namespace Aris.Moe.OverlayTranslate.Gui.Qt5
{
    public class ControlsModel : IOcrTranslateOverlay
    {
        private readonly IOcrTranslateOverlay _translateOverlay;

        public ControlsModel()
        {
            _translateOverlay = Program.Services.GetInstance<IOcrTranslateOverlay>();
        }

        public async Task TranslateScreen()
        {
            await _translateOverlay.TranslateScreen();
        }

        public void HideOverlay()
        {
            _translateOverlay.HideOverlay();
        }

        public void ToggleOverlay()
        {
            _translateOverlay.ToggleOverlay();
        }

        public void ShowOverlay()
        {
            _translateOverlay.ShowOverlay();
        }

        public async Task OcrScreen()
        {
            await _translateOverlay.OcrScreen();
        }

        public void AskForTargetResize()
        {
            _translateOverlay.AskForTargetResize();
        }
        
        public void DisplayProgress()
        {
            _translateOverlay.DisplayProgress();
        }

        public void Dispose()
        {
        }
        public string GetErrors()
        {
            var needConfigurations = Program.Services.GetAllInstances<INeedConfiguration>();

            var withIssues = needConfigurations
                .Select(x => (Name: x.Name, Issues: x.GetConfigurationIssues().ToList())).Where(x => x.Issues.Any()).ToList();

            if (!withIssues.Any())
                return "Configuration seems fine";

            var builder = new StringBuilder();

            builder.AppendLine($"Configuration issues:");

            foreach (var (name, issues) in withIssues)
            {
                builder.AppendLine($"### {name} ###");

                foreach (var issue in issues)
                {
                    builder.AppendLine("- " + issue);
                }
            }

            return builder.ToString();
        }
    }
}