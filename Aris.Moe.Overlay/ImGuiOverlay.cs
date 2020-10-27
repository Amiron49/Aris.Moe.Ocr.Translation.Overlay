using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Aris.Moe.Overlay
{
    public abstract class ImGuiOverlay : IDisposable
    {
        private volatile Sdl2Window _window;
        private GraphicsDevice _gd;
        private CommandList _cl;
        private ImGuiController _controller;
        private volatile CancellationTokenSource _cancellationTokenSource;
        private Thread _renderThread;
        protected volatile bool ThreadIsReady = false;

        private readonly Vector3 _clearColor = new Vector3(0f, 0f, 0f);

        protected volatile bool Visible = false;

        public async Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            _renderThread = new Thread(() =>
            {
                // _clearColor = new Vector3(0.00f, 0.00f, 0.00f);
                //loadedImages = new Dictionary<string, Texture>();
                _window = new Sdl2Window(
                    "Overlay",
                    0,
                    0,
                    1920,
                    1080,
                    SDL_WindowFlags.Borderless |
                     SDL_WindowFlags.AlwaysOnTop |
                    SDL_WindowFlags.SkipTaskbar,
                    false);
                _gd = VeldridStartup.CreateDefaultD3D11GraphicsDevice(
                    new GraphicsDeviceOptions(false, null, true),
                    _window);
                _cl = _gd.ResourceFactory.CreateCommandList();
                _controller = new ImGuiController(
                    _gd,
                    _gd.MainSwapchain.Framebuffer.OutputDescription,
                    _window.Width,
                    _window.Height);
                _window.Resized += () =>
                {
                    _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                    _controller.WindowResized(_window.Width, _window.Height);
                };
                

                //_window.BorderVisible = false;
                //_window.WindowState = WindowState.Normal;
                Visible = true;
                _window.Closing += () => { };

                _cl = _gd.ResourceFactory.CreateCommandList();
                _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

                WindowsNativeMethods.InitTransparency(_window.Handle);
                SetClickAbility(false);

                ThreadIsReady = true;
                
                MainLoop(_cancellationTokenSource.Token);
            })
            {
                IsBackground = true
            };
            
            _renderThread.Start();

            for (int i = 0; i < 100; i++)
            { 
               await Task.Delay(TimeSpan.FromMilliseconds(10));

               if (ThreadIsReady)
                   break;
            }
        }

        private void MainLoop(CancellationToken cancellationToken)
        {
            while (_window.Exists && !cancellationToken.IsCancellationRequested)
            {
                var snapshot = _window.PumpEvents();
                if (!_window.Exists)
                {
                    break;
                }

                _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                Render();
                
                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 0.0f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
            }

            if (_window.Exists)
                _window.Close();
        }

        protected void SetVisibility(bool visible)
        {
            WindowsNativeMethods.SetWindowVisibility(_window.Handle, visible); 
        }

        protected IntPtr AddImageTexture(Bitmap bitmap)
        {
            return _controller.AddImageTexture(_gd, bitmap);
        }

        public async Task Stop()
        {
            _cancellationTokenSource.Cancel();

            for (int i = 0; i < 10; i++)
            {
                if (_window?.Exists ?? false)
                    return;

                await Task.Delay(TimeSpan.FromSeconds(1 / 10d));
            }
        }

        protected virtual void Render()
        {
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(1.0f, 0.1f, 0.1f, 1.0f));

            ImGui.SetNextWindowSize(new Vector2(300, 300));
            ImGui.SetNextWindowPos(new Vector2(300, 300));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.8f);
            ImGui.Begin("DebugTextBox2", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove);
            ImGui.PopStyleVar();
            ImGui.PopStyleColor();

            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 1.0f);
            ImGui.BeginChild("DebugText2");
            ImGui.Text("DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK DEBUG BACK");
            ImGui.EndChild();
            ImGui.PopStyleVar();

            ImGui.End();
            
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(1.0f, 0.1f, 0.1f, 1.0f));

            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.SetNextWindowPos(new Vector2(350, 250));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.8f);
            ImGui.Begin("DebugTextBox", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove);
            ImGui.PopStyleVar();
            ImGui.PopStyleColor();

            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 1.0f);
            ImGui.BeginChild("DebugText");
            ImGui.TextWrapped("Debug  Japanese: こんにちは！テスト test ' ");
            ImGui.EndChild();
            ImGui.PopStyleVar();

            ImGui.End();
        }

        protected void BringToForeground()
        {
            WindowsNativeMethods.BringToForeground(_window.Handle);
        }

        protected void SetClickAbility(bool clickable)
        {
            WindowsNativeMethods.SetOverlayClickable(_window.Handle, clickable);
        }

        public void Dispose()
        {
            if (_renderThread != null)
                Stop().Wait();
            
            _gd?.WaitForIdle();
            _controller?.Dispose();
            _cl?.Dispose();
            _gd?.Dispose();
        }
    }
}