using System;
using System.Drawing;
using System.Numerics;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aris.Moe.Ocr.Overlay.Translate.Overlay.Modes
{
    public class TargetAreaResizeOverlay : ITargetAreaResizeOverlay, IGuiMode
    {
        private const ImGuiWindowFlags NoDecoration = ImGuiWindowFlags.NoDecoration |
                                                      ImGuiWindowFlags.NoMove |
                                                      ImGuiWindowFlags.NoNav |
                                                      ImGuiWindowFlags.NoScrollbar |
                                                      ImGuiWindowFlags.NoTitleBar |
                                                      ImGuiWindowFlags.NoResize |
                                                      ImGuiWindowFlags.NoInputs;

        private readonly Rectangle _screenSize;
        private readonly ILogger _logger;


        private class ResizeOperation
        {
            private readonly Action<Rectangle?> _resultCallback;

            public ResizeOperation(Rectangle currentArea, Action<Rectangle?> resultCallback)
            {
                CurrentArea = currentArea;
                _resultCallback = resultCallback;
            }

            public bool CurrentlyDragging => DragStart != null && DragEnd == null;
            public bool FinishedDragging => DragStart != null && DragEnd != null;
            public bool OperationOver { get; private set; }
            public Point? DragStart { get; set; }
            public Point? CurrentMousePosition { get; set; }
            public Point? DragEnd { get; set; }
            public Rectangle CurrentArea { get; }

            public void Finish()
            {
                if (OperationOver)
                    return;

                OperationOver = true;

                var asRectangle = CalcRectangle();

                _resultCallback(asRectangle);
            }

            private Rectangle? CalcRectangle()
            {
                if (DragStart == null || DragEnd == null)
                    return null;

                return DragStart.Value.ToRectangleWithUnknownPointOrder(DragEnd.Value);
            }
        }

        public TargetAreaResizeOverlay(Rectangle screenSize, ILogger<TargetAreaResizeOverlay> logger)
        {
            _screenSize = screenSize;
            _logger = logger;
        }

        public void AskForResize(Rectangle current, Action<Rectangle?> resultCallback)
        {
            var io = ImGui.GetIO();

            io.WantCaptureMouse = true;
            io.WantCaptureKeyboard = true;

            _currentResizeOperation = new ResizeOperation(current, rectangle =>
            {
                io.WantCaptureMouse = false;
                io.WantCaptureKeyboard = false;

                resultCallback(rectangle);
            });
        }

        private ResizeOperation? _currentResizeOperation;

        public void Render()
        {
            if (_currentResizeOperation == null || _currentResizeOperation.OperationOver)
                return;

            RenderBackdrop(() =>
            {
                ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.2f, 0.4f, 1.0f, 1.0f));
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.8f);

                if (!_currentResizeOperation.CurrentlyDragging)
                    RenderCurrentCaptureArea();
                else
                    RenderSelectionLive();

                ImGui.End();
                ImGui.PopStyleColor();
                ImGui.PopStyleVar(0);
            });

            if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Escape)))
                _currentResizeOperation.Finish();

            HandleDragging();
        }

        private void HandleDragging()
        {
            if (_currentResizeOperation == null || _currentResizeOperation.OperationOver)
                return;

            _logger.LogDebug(JsonConvert.SerializeObject(_currentResizeOperation));

            if (!_currentResizeOperation.CurrentlyDragging)
            {
                var isMouseDown = ImGui.IsMouseDown(ImGuiMouseButton.Left);
                if (isMouseDown)
                {
                    var mousePos = ImGui.GetMousePos();
                    _currentResizeOperation.DragStart = new Point((int) mousePos.X, (int) mousePos.Y);
                }
            }
            else
            {
                var mousePos = ImGui.GetMousePos();
                _currentResizeOperation.CurrentMousePosition = new Point((int) mousePos.X, (int) mousePos.Y);

                var isMouseReleased = ImGui.IsMouseReleased(ImGuiMouseButton.Left);

                if (isMouseReleased)
                    _currentResizeOperation.DragEnd = _currentResizeOperation.CurrentMousePosition;
            }

            if (_currentResizeOperation.FinishedDragging)
                _currentResizeOperation.Finish();
        }

        private void RenderSelectionLive()
        {
            if (_currentResizeOperation?.CurrentMousePosition == null || _currentResizeOperation.DragStart == null)
                return;


            var currentMousePosition = _currentResizeOperation.CurrentMousePosition.Value;

            var selectionRectangle = currentMousePosition.ToRectangleWithUnknownPointOrder(_currentResizeOperation.DragStart.Value);

            if (selectionRectangle == null)
                return;

            ImGui.SetNextWindowSize(new Vector2(selectionRectangle.Value.Width, selectionRectangle.Value.Height));
            ImGui.SetNextWindowPos(new Vector2(selectionRectangle.Value.X, selectionRectangle.Value.Y));

            ImGui.Begin("Selection", NoDecoration);
            {
                ImGui.SetWindowFontScale(1f);
                ImGui.EndChild();
            }
        }

        private void RenderCurrentCaptureArea()
        {
            ImGui.SetNextWindowSize(new Vector2(_currentResizeOperation!.CurrentArea.Width, _currentResizeOperation.CurrentArea.Height));
            ImGui.SetNextWindowPos(new Vector2(_currentResizeOperation.CurrentArea.X, _currentResizeOperation.CurrentArea.Y));

            ImGui.Begin("CurrentSelection", NoDecoration);
            {
                ImGui.SetWindowFontScale(1f);
                ImGui.BeginChild("CurrentAreaText", new Vector2(), false, NoDecoration);
                ImGui.TextWrapped("Current active capture area. Hold and drag mouse to select new. Press Escape to cancel");
                ImGui.EndChild();
            }
        }

        private void RenderBackdrop(Action inner)
        {
            ImGui.SetNextWindowSize(new Vector2(_screenSize.Width, _screenSize.Height));
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);

            ImGui.Begin("ResizeBackground", NoDecoration);
            ImGui.PopStyleVar();

            inner();

            ImGui.End();
        }
    }
}