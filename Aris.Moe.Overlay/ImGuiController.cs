using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using Veldrid;

namespace Aris.Moe.Overlay
{
    /// <summary>
    ///     A modified version of Veldrid.ImGui's ImGuiRenderer.
    ///     Manages input for ImGui and handles rendering ImGui's DrawLists with Veldrid.
    /// </summary>
    public class ImGuiController : IDisposable
    {
        private readonly List<IDisposable> _ownedResources = new List<IDisposable>();

        // Image trackers
        private readonly Dictionary<TextureView, ResourceSetInfo> _setsByView
            = new Dictionary<TextureView, ResourceSetInfo>();

        private readonly Dictionary<IntPtr, ResourceSetInfo> _viewsById = new Dictionary<IntPtr, ResourceSetInfo>();
        private bool _altDown;
        private bool _controlDown;

        private readonly IntPtr _fontAtlasId = (IntPtr) 1;
        private Texture _fontTexture;
        private ResourceSet _fontTextureResourceSet;
        private TextureView _fontTextureView;
        private Shader _fragmentShader;
        private bool _frameBegun;
        private GraphicsDevice _gd;
        private DeviceBuffer _indexBuffer;
        private int _lastAssignedId = 100;
        private ResourceLayout _layout;
        private ResourceSet _mainResourceSet;
        private Pipeline _pipeline;
        private DeviceBuffer _projMatrixBuffer;
        private readonly Vector2 _scaleFactor = Vector2.One;
        private bool _shiftDown;
        private ResourceLayout _textureLayout;

        // Veldrid objects
        private DeviceBuffer _vertexBuffer;
        private Shader _vertexShader;
        private int _windowHeight;

        private int _windowWidth;
        private bool _winKeyDown;

        /// <summary>
        ///     Constructs a new ImGuiController.
        /// </summary>
        public ImGuiController(GraphicsDevice gd, OutputDescription outputDescription, int width, int height)
        {
            _gd = gd;
            _windowWidth = width;
            _windowHeight = height;

            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            //TODO config
            const float sizePixels = 32f;
            ImGui.GetIO().Fonts.AddFontFromFileTTF(GetTextFilePath(), sizePixels, null, ImGui.GetIO().Fonts.GetGlyphRangesJapanese());

            CreateDeviceResources(gd, outputDescription);
            SetKeyMappings();

            SetPerFrameImGuiData(1f / 60f);

            ImGui.NewFrame();
            _frameBegun = true;
        }

        /// <summary>
        ///     Frees all graphics resources used by the renderer.
        /// </summary>
        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _projMatrixBuffer.Dispose();
            _fontTexture.Dispose();
            _fontTextureView.Dispose();
            _vertexShader.Dispose();
            _fragmentShader.Dispose();
            _layout.Dispose();
            _textureLayout.Dispose();
            _pipeline.Dispose();
            _mainResourceSet.Dispose();

            foreach (var resource in _ownedResources) resource.Dispose();
        }

        private string GetTextFilePath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Fonts", "NotoSansJP-Medium.otf");

            return path;
        }

        public void WindowResized(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
        }

        public void CreateDeviceResources(GraphicsDevice gd, OutputDescription outputDescription)
        {
            _gd = gd;
            var factory = gd.ResourceFactory;
            _vertexBuffer = factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            _vertexBuffer.Name = "ImGui.NET Vertex Buffer";
            _indexBuffer = factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            _indexBuffer.Name = "ImGui.NET Index Buffer";
            RecreateFontDeviceTexture(gd);

            _projMatrixBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _projMatrixBuffer.Name = "ImGui.NET Projection Buffer";

            var vertexShaderBytes = LoadEmbeddedShaderCode(gd.ResourceFactory, "imgui-vertex");
            var fragmentShaderBytes = LoadEmbeddedShaderCode(gd.ResourceFactory, "imgui-frag");
            _vertexShader = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, "VS"));
            _fragmentShader = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, "FS"));

            var vertexLayouts = new[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("in_position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                    new VertexElementDescription("in_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                    new VertexElementDescription("in_color", VertexElementSemantic.Color, VertexElementFormat.Byte4_Norm))
            };

            _layout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjectionMatrixBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("MainSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
            _textureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)));

            var pd = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                new DepthStencilStateDescription(false, false, ComparisonKind.Always),
                new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false, true),
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(vertexLayouts, new[] {_vertexShader, _fragmentShader}),
                new[] {_layout, _textureLayout},
                outputDescription);
            _pipeline = factory.CreateGraphicsPipeline(ref pd);

            _mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(_layout,
                _projMatrixBuffer,
                gd.PointSampler));

            _fontTextureResourceSet = factory.CreateResourceSet(new ResourceSetDescription(_textureLayout, _fontTextureView));
        }

        /// <summary>
        ///     Gets or creates a handle for a texture to be drawn with ImGui.
        ///     Pass the returned handle to Image() or ImageButton().
        /// </summary>
        public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, TextureView textureView)
        {
            if (_setsByView.TryGetValue(textureView, out var rsi)) 
                return rsi.ImGuiBinding;
            
            var resourceSet = factory.CreateResourceSet(new ResourceSetDescription(_textureLayout, textureView));
            rsi = new ResourceSetInfo(GetNextImGuiBindingId(), resourceSet);

            _setsByView.Add(textureView, rsi);
            _viewsById.Add(rsi.ImGuiBinding, rsi);
            _ownedResources.Add(resourceSet);

            return rsi.ImGuiBinding;
        }

        private IntPtr GetNextImGuiBindingId()
        {
            var newId = _lastAssignedId++;
            return (IntPtr) newId;
        }

        /// <summary>
        ///     Retrieves the shader texture binding for the given helper handle.
        /// </summary>
        public ResourceSet GetImageResourceSet(IntPtr imGuiBinding)
        {
            if (!_viewsById.TryGetValue(imGuiBinding, out var tvi)) throw new InvalidOperationException("No registered ImGui binding with id " + imGuiBinding);

            return tvi.ResourceSet;
        }

        private static byte[] LoadEmbeddedShaderCode(ResourceFactory factory, string name)
        {
            switch (factory.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                {
                    var resourceName = name + ".hlsl.bytes";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                case GraphicsBackend.OpenGL:
                {
                    var resourceName = name + ".glsl";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                case GraphicsBackend.Vulkan:
                {
                    var resourceName = name + ".spv";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                case GraphicsBackend.Metal:
                {
                    var resourceName = name + ".metallib";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                default:
                    throw new NotImplementedException();
            }
        }

        private static byte[] GetEmbeddedResourceBytes(string resourceName)
        {
            var assembly = typeof(ImGuiController).Assembly;
            using (var s = assembly.GetManifestResourceStream(resourceName))
            {
                if (s == null)
                {
                    throw new Exception($"Could not load resource '{resourceName}'");
                }
                
                var ret = new byte[s.Length];
                s.Read(ret, 0, (int) s.Length);
                return ret;
            }
        }

        /// <summary>
        ///     Recreates the device texture used to render text.
        /// </summary>
        public void RecreateFontDeviceTexture(GraphicsDevice gd)
        {
            var io = ImGui.GetIO();
            // Build

            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bytesPerPixel);
            // Store our identifier
            io.Fonts.SetTexID(_fontAtlasId);

            _fontTexture = gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint) width,
                (uint) height,
                1,
                1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.Sampled));
            _fontTexture.Name = "ImGui.NET Font Texture";
            gd.UpdateTexture(
                _fontTexture,
                pixels,
                (uint) (bytesPerPixel * width * height),
                0,
                0,
                0,
                (uint) width,
                (uint) height,
                1,
                0,
                0);
            _fontTextureView = gd.ResourceFactory.CreateTextureView(_fontTexture);

            io.Fonts.ClearTexData();
        }

        /// <summary>
        ///     Renders the ImGui draw list data.
        ///     This method requires a <see cref="GraphicsDevice" /> because it may create new DeviceBuffers if the size of vertex
        ///     or index data has increased beyond the capacity of the existing buffers.
        ///     A <see cref="CommandList" /> is needed to submit drawing and resource update commands.
        /// </summary>
        public void Render(GraphicsDevice gd, CommandList cl)
        {
            if (_frameBegun)
            {
                _frameBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData(), gd, cl);
            }
        }

        /// <summary>
        ///     Updates ImGui input and IO configuration state.
        /// </summary>
        public void Update(float deltaSeconds, InputSnapshot snapshot)
        {
            if (_frameBegun) ImGui.Render();

            SetPerFrameImGuiData(deltaSeconds);
            UpdateImGuiInput(snapshot);

            _frameBegun = true;
            ImGui.NewFrame();
        }

        /// <summary>
        ///     Sets per-frame data based on the associated window.
        ///     This is called by Update(float).
        /// </summary>
        private void SetPerFrameImGuiData(float deltaSeconds)
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new Vector2(
                _windowWidth / _scaleFactor.X,
                _windowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = _scaleFactor;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        private void UpdateImGuiInput(InputSnapshot snapshot)
        {
            var io = ImGui.GetIO();

            var mousePosition = snapshot.MousePosition;

            // Determine if any of the mouse buttons were pressed during this snapshot period, even if they are no longer held.
            var leftPressed = false;
            var middlePressed = false;
            var rightPressed = false;
            foreach (var me in snapshot.MouseEvents)
                if (me.Down)
                    switch (me.MouseButton)
                    {
                        case MouseButton.Left:
                            leftPressed = true;
                            break;
                        case MouseButton.Middle:
                            middlePressed = true;
                            break;
                        case MouseButton.Right:
                            rightPressed = true;
                            break;
                    }

            io.MouseDown[0] = leftPressed || snapshot.IsMouseDown(MouseButton.Left);
            io.MouseDown[1] = rightPressed || snapshot.IsMouseDown(MouseButton.Right);
            io.MouseDown[2] = middlePressed || snapshot.IsMouseDown(MouseButton.Middle);
            io.MousePos = mousePosition;
            io.MouseWheel = snapshot.WheelDelta;

            var keyCharPresses = snapshot.KeyCharPresses;
            for (var i = 0; i < keyCharPresses.Count; i++)
            {
                var c = keyCharPresses[i];
                io.AddInputCharacter(c);
            }

            var keyEvents = snapshot.KeyEvents;
            for (var i = 0; i < keyEvents.Count; i++)
            {
                var keyEvent = keyEvents[i];
                io.KeysDown[(int) keyEvent.Key] = keyEvent.Down;
                if (keyEvent.Key == Key.ControlLeft) _controlDown = keyEvent.Down;

                if (keyEvent.Key == Key.ShiftLeft) _shiftDown = keyEvent.Down;

                if (keyEvent.Key == Key.AltLeft) _altDown = keyEvent.Down;

                if (keyEvent.Key == Key.WinLeft) _winKeyDown = keyEvent.Down;
            }

            io.KeyCtrl = _controlDown;
            io.KeyAlt = _altDown;
            io.KeyShift = _shiftDown;
            io.KeySuper = _winKeyDown;
        }

        private static void SetKeyMappings()
        {
            var io = ImGui.GetIO();
            io.KeyMap[(int) ImGuiKey.Tab] = (int) Key.Tab;
            io.KeyMap[(int) ImGuiKey.LeftArrow] = (int) Key.Left;
            io.KeyMap[(int) ImGuiKey.RightArrow] = (int) Key.Right;
            io.KeyMap[(int) ImGuiKey.UpArrow] = (int) Key.Up;
            io.KeyMap[(int) ImGuiKey.DownArrow] = (int) Key.Down;
            io.KeyMap[(int) ImGuiKey.PageUp] = (int) Key.PageUp;
            io.KeyMap[(int) ImGuiKey.PageDown] = (int) Key.PageDown;
            io.KeyMap[(int) ImGuiKey.Home] = (int) Key.Home;
            io.KeyMap[(int) ImGuiKey.End] = (int) Key.End;
            io.KeyMap[(int) ImGuiKey.Delete] = (int) Key.Delete;
            io.KeyMap[(int) ImGuiKey.Backspace] = (int) Key.BackSpace;
            io.KeyMap[(int) ImGuiKey.Enter] = (int) Key.Enter;
            io.KeyMap[(int) ImGuiKey.Escape] = (int) Key.Escape;
            io.KeyMap[(int) ImGuiKey.A] = (int) Key.A;
            io.KeyMap[(int) ImGuiKey.C] = (int) Key.C;
            io.KeyMap[(int) ImGuiKey.V] = (int) Key.V;
            io.KeyMap[(int) ImGuiKey.X] = (int) Key.X;
            io.KeyMap[(int) ImGuiKey.Y] = (int) Key.Y;
            io.KeyMap[(int) ImGuiKey.Z] = (int) Key.Z;
        }

        private void RenderImDrawData(ImDrawDataPtr drawData, GraphicsDevice gd, CommandList cl)
        {
            uint vertexOffsetInVertices = 0;
            uint indexOffsetInElements = 0;

            if (drawData.CmdListsCount == 0) return;

            var totalVbSize = (uint) (drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
            if (totalVbSize > _vertexBuffer.SizeInBytes)
            {
                gd.DisposeWhenIdle(_vertexBuffer);
                _vertexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint) (totalVbSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            }

            var totalIbSize = (uint) (drawData.TotalIdxCount * sizeof(ushort));
            if (totalIbSize > _indexBuffer.SizeInBytes)
            {
                gd.DisposeWhenIdle(_indexBuffer);
                _indexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint) (totalIbSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            }

            for (var i = 0; i < drawData.CmdListsCount; i++)
            {
                var cmdList = drawData.CmdListsRange[i];

                cl.UpdateBuffer(
                    _vertexBuffer,
                    vertexOffsetInVertices * (uint) Unsafe.SizeOf<ImDrawVert>(),
                    cmdList.VtxBuffer.Data,
                    (uint) (cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()));

                cl.UpdateBuffer(
                    _indexBuffer,
                    indexOffsetInElements * sizeof(ushort),
                    cmdList.IdxBuffer.Data,
                    (uint) (cmdList.IdxBuffer.Size * sizeof(ushort)));

                vertexOffsetInVertices += (uint) cmdList.VtxBuffer.Size;
                indexOffsetInElements += (uint) cmdList.IdxBuffer.Size;
            }

            // Setup orthographic projection matrix into our constant buffer
            var io = ImGui.GetIO();
            var mvp = Matrix4x4.CreateOrthographicOffCenter(
                0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            _gd.UpdateBuffer(_projMatrixBuffer, 0, ref mvp);

            cl.SetVertexBuffer(0, _vertexBuffer);
            cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, _mainResourceSet);

            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            // Render command lists
            var vtxOffset = 0;
            var idxOffset = 0;
            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdListsRange[n];
                for (var cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
                {
                    var pcmd = cmdList.CmdBuffer[cmdI];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }

                    if (pcmd.TextureId != IntPtr.Zero)
                    {
                        if (pcmd.TextureId == _fontAtlasId)
                            cl.SetGraphicsResourceSet(1, _fontTextureResourceSet);
                        else
                            cl.SetGraphicsResourceSet(1, GetImageResourceSet(pcmd.TextureId));
                    }

                    cl.SetScissorRect(
                        0,
                        (uint) pcmd.ClipRect.X,
                        (uint) pcmd.ClipRect.Y,
                        (uint) (pcmd.ClipRect.Z - pcmd.ClipRect.X),
                        (uint) (pcmd.ClipRect.W - pcmd.ClipRect.Y));

                    cl.DrawIndexed(pcmd.ElemCount, 1, (uint) idxOffset, vtxOffset, 0);

                    idxOffset += (int) pcmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
        }

        private readonly struct ResourceSetInfo
        {
            public readonly IntPtr ImGuiBinding;
            public readonly ResourceSet ResourceSet;

            public ResourceSetInfo(IntPtr imGuiBinding, ResourceSet resourceSet)
            {
                ImGuiBinding = imGuiBinding;
                ResourceSet = resourceSet;
            }
        }
    }
}