using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Aris.Moe.Overlay
{
    public class Overlay : IDisposable
    {
        private Sdl2Window _window;
        private GraphicsDevice _gd;
        private CommandList _cl;
        private ImGuiController _controller;
        private MemoryEditor _memoryEditor;
        private volatile CancellationTokenSource _cancellationTokenSource;
        private Thread _renderThread;

        // UI state
        private float _f = 0.0f;
        private int _counter = 0;
        private int _dragInt = 0;
        private Vector3 _clearColor = new Vector3(0f, 0f, 0f);
        private bool _showDemoWindow = true;
        private bool _showAnotherWindow = false;
        private bool _showMemoryEditor = false;
        private byte[] _memoryEditorData;
        private uint s_tab_bar_flags = (uint) ImGuiTabBarFlags.Reorderable;
        private bool[] s_opened = {true, true, true, true}; // Persistent user state

        static void SetThing(out float i, float val)
        {
            i = val;
        }

        public async Task Start()
        {
           
            var random = new Random();
            _memoryEditorData = Enumerable.Range(0, 1024).Select(i => (byte) random.Next(255)).ToArray();

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
                

                _window.BorderVisible = false;

                _window.Closing += () => { };    
                
                _cl = _gd.ResourceFactory.CreateCommandList();
                _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);
                WindowsNativeMethods.InitTransparency(_window.Handle);
                MainLoop(_cancellationTokenSource.Token);
            })
            {
                IsBackground = true
            };

            _renderThread.Start();
        }

        public void MainLoop(CancellationToken cancellationToken)
        {
            
            while (_window.Exists && !cancellationToken.IsCancellationRequested)
            {
                var snapshot = _window.PumpEvents();
                if (!_window.Exists)
                {
                    break;
                }

                _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                
                SubmitUI();

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

        public async Task Stop()
        {
            _cancellationTokenSource.Cancel();

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        private void SubmitUI()
        {
            // Demo code adapted from the official Dear ImGui demo program:
            // https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L172

            // 1. Show a simple window.
            // Tip: if we don't call ImGui.BeginWindow()/ImGui.EndWindow() the widgets automatically appears in a window called "Debug".

            {
      
                ImGui.PushStyleColor(ImGuiCol.WindowBg,new Vector4(1.0f, 0.1f, 0.1f, 1.0f));
                
                //ImGui.SetNextWindowFocus();
                ImGui.SetNextWindowSize(new Vector2(200, 200));
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha,0.1f);
                ImGui.Begin("TextBox", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove);
                ImGui.PopStyleVar();
                ImGui.PopStyleColor();

                ImGui.PushStyleVar(ImGuiStyleVar.Alpha,1.0f);
                ImGui.BeginChild("Text");
                ImGui.Text("Ari Aris Aris");
                ImGui.EndChild();
                ImGui.PopStyleVar();
                
                ImGui.End();
            }

            {
                ImGui.Text("Hello, world!"); // Display some text (you can use a format string too)
                ImGui.SliderFloat("float", ref _f, 0, 1, _f.ToString("0.000")); // Edit 1 float using a slider from 0.0f to 1.0f    
                //ImGui.ColorEdit3("clear color", ref _clearColor);                   // Edit 3 floats representing a color

                ImGui.Text($"Mouse position: {ImGui.GetMousePos()}");

                ImGui.Checkbox("Demo Window", ref _showDemoWindow); // Edit bools storing our windows open/close state
                ImGui.Checkbox("Another Window", ref _showAnotherWindow);
                ImGui.Checkbox("Memory Editor", ref _showMemoryEditor);
                if (ImGui.Button("Button")) // Buttons return true when clicked (NB: most widgets return true when edited/activated)
                    _counter++;
                ImGui.SameLine(0, -1);
                ImGui.Text($"counter = {_counter}");

                ImGui.DragInt("Draggable Int", ref _dragInt);

                var framerate = ImGui.GetIO().Framerate;
                ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
            }

            // 2. Show another simple window. In most cases you will use an explicit Begin/End pair to name your windows.
            if (_showAnotherWindow)
            {
                ImGui.Begin("Another Window", ref _showAnotherWindow);
                ImGui.Text("Hello from another window!");
                if (ImGui.Button("Close Me"))
                    _showAnotherWindow = false;
                ImGui.End();
            }

            // 3. Show the ImGui demo window. Most of the sample code is in ImGui.ShowDemoWindow(). Read its code to learn more about Dear ImGui!
            if (_showDemoWindow)
            {
                // Normally user code doesn't need/want to call this because positions are saved in .ini file anyway.
                // Here we just want to make the demo initial state a bit more friendly!
                ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref _showDemoWindow);
            }

            if (ImGui.TreeNode("Tabs"))
            {
                if (ImGui.TreeNode("Basic"))
                {
                    var tab_bar_flags = ImGuiTabBarFlags.None;
                    if (ImGui.BeginTabBar("MyTabBar", tab_bar_flags))
                    {
                        if (ImGui.BeginTabItem("Avocado"))
                        {
                            ImGui.Text("This is the Avocado tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("Broccoli"))
                        {
                            ImGui.Text("This is the Broccoli tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("Cucumber"))
                        {
                            ImGui.Text("This is the Cucumber tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }

                    ImGui.Separator();
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Advanced & Close Button"))
                {
                    // Expose a couple of the available flags. In most cases you may just call BeginTabBar() with no flags (0).
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_Reorderable", ref s_tab_bar_flags, (uint) ImGuiTabBarFlags.Reorderable);
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_AutoSelectNewTabs", ref s_tab_bar_flags, (uint) ImGuiTabBarFlags.AutoSelectNewTabs);
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_NoCloseWithMiddleMouseButton", ref s_tab_bar_flags, (uint) ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);
                    if ((s_tab_bar_flags & (uint) ImGuiTabBarFlags.FittingPolicyMask) == 0)
                        s_tab_bar_flags |= (uint) ImGuiTabBarFlags.FittingPolicyDefault;
                    if (ImGui.CheckboxFlags("ImGuiTabBarFlags_FittingPolicyResizeDown", ref s_tab_bar_flags, (uint) ImGuiTabBarFlags.FittingPolicyResizeDown))
                        s_tab_bar_flags &= ~((uint) ImGuiTabBarFlags.FittingPolicyMask ^ (uint) ImGuiTabBarFlags.FittingPolicyResizeDown);
                    if (ImGui.CheckboxFlags("ImGuiTabBarFlags_FittingPolicyScroll", ref s_tab_bar_flags, (uint) ImGuiTabBarFlags.FittingPolicyScroll))
                        s_tab_bar_flags &= ~((uint) ImGuiTabBarFlags.FittingPolicyMask ^ (uint) ImGuiTabBarFlags.FittingPolicyScroll);

                    // Tab Bar
                    string[] names = {"Artichoke", "Beetroot", "Celery", "Daikon"};

                    for (var n = 0; n < s_opened.Length; n++)
                    {
                        if (n > 0)
                        {
                            ImGui.SameLine();
                        }

                        ImGui.Checkbox(names[n], ref s_opened[n]);
                    }

                    // Passing a bool* to BeginTabItem() is similar to passing one to Begin(): the underlying bool will be set to false when the tab is closed.
                    if (ImGui.BeginTabBar("MyTabBar", (ImGuiTabBarFlags) s_tab_bar_flags))
                    {
                        for (var n = 0; n < s_opened.Length; n++)
                            if (s_opened[n] && ImGui.BeginTabItem(names[n], ref s_opened[n]))
                            {
                                ImGui.Text($"This is the {names[n]} tab!");
                                if ((n & 1) != 0)
                                    ImGui.Text("I am an odd tab.");
                                ImGui.EndTabItem();
                            }

                        ImGui.EndTabBar();
                    }

                    ImGui.Separator();
                    ImGui.TreePop();
                }

                ImGui.TreePop();
            }

            var io = ImGui.GetIO();
            SetThing(out io.DeltaTime, 2f);

            if (_showMemoryEditor)
            {
                _memoryEditor.Draw("Memory Editor", _memoryEditorData, _memoryEditorData.Length);
            }
        }

        public void Dispose()
        {
            _gd?.WaitForIdle();
            _controller?.Dispose();
            _cl?.Dispose();
            _gd?.Dispose();
        }
    }
}