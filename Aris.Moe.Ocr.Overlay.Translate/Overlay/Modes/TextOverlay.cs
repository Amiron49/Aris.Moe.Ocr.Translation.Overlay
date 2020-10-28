using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Numerics;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using ImGuiNET;

namespace Aris.Moe.Ocr.Overlay.Translate.Overlay.Modes
{
    public class TextOverlay : ITextOverlay, IGuiMode
    {
        private volatile ConcurrentBag<ISpatialText> _activeTexts = new ConcurrentBag<ISpatialText>();

        public void Render()
        {
            foreach (var activeText in _activeTexts)
            {
                var (areaCompensated, compensatedFontScale) = CompensateForRender(activeText);

                var size = new Vector2(areaCompensated.Width, areaCompensated.Height);
                ImGui.SetNextWindowSize(size);
                ImGui.SetNextWindowPos(new Vector2(areaCompensated.X, areaCompensated.Y));
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.8f);
                var name = activeText.GetHashCode().ToString();
                const ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.NoDecoration |
                                                          ImGuiWindowFlags.NoMove |
                                                          ImGuiWindowFlags.NoNav |
                                                          ImGuiWindowFlags.NoScrollbar |
                                                          ImGuiWindowFlags.NoTitleBar |
                                                          ImGuiWindowFlags.NoResize |
                                                          ImGuiWindowFlags.NoInputs;
                ImGui.Begin(name, imGuiWindowFlags);
                ImGui.PopStyleVar();

                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 1.0f);
                ImGui.SetWindowFontScale(compensatedFontScale);
                ImGui.BeginChild(name + "text", new Vector2(), false, imGuiWindowFlags);
                ImGui.TextWrapped(activeText.Text);
                ImGui.EndChild();
                ImGui.PopStyleVar(0);

                ImGui.End();
            }

            //base.Render();
        }

        public void Add(params ISpatialText[] texts)
        {
            foreach (var text in texts) _activeTexts.Add(text);
        }
        

        private static (Rectangle compensatedArea, float compensatedFontScale) CompensateForRender(ISpatialText text)
        {
            const float defaultFontScale = 0.5f;
            const int imGuiPadding = 8;

            var calculatedSize = ImGui.CalcTextSize(text.Text);

            var areaHeight = Math.Abs(text.Area.Height);
            var areaWidth = Math.Abs(text.Area.Width);

            var estimatedTextArea = calculatedSize.X * calculatedSize.Y * defaultFontScale;
            var additionalPaddingArea = areaWidth * imGuiPadding * 2 + areaHeight * imGuiPadding * 2 - imGuiPadding * imGuiPadding * 4;


            var neededArea = estimatedTextArea + additionalPaddingArea;
            var availableArea = areaHeight * areaWidth;

            var missingArea = neededArea - availableArea;

            if (missingArea < 0)
            {
                var extraAreaFactor = availableArea / neededArea;

                return (text.Area, extraAreaFactor * defaultFontScale);
            }

            var scaleFactor = Math.Sqrt(neededArea / availableArea);

            var missingHeight = scaleFactor * text.Area.Height - text.Area.Height;
            var missingWidth = scaleFactor * text.Area.Width - text.Area.Width;

            var compensatedRectangle = Rectangle.Inflate(text.Area, (int) (missingWidth / 2), (int) (missingHeight / 2));

            return (compensatedRectangle, defaultFontScale);
        }

        public void ClearAll()
        {
            _activeTexts = new ConcurrentBag<ISpatialText>();
        }
    }
}