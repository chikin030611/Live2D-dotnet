using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Animations;
using ColorMC.Gui.UI.Model.Skin;
using SkiaSharp;

namespace ColorMC.Gui.UI.Controls.Skin;

public class SkinRender : OpenGlControlBase, ICustomHitTest
{
    private bool _haveCape = false;
    private bool _switchModel = false;
    private bool _switchSkin = false;

    private float _dis = 1;
    private float _xdiff = 0;
    private float _ydiff = 0;
    private Vector2 _rotXY;
    private Vector2 _diffXY;

    private Vector2 _xy;
    private Vector2 _saveXY;
    private Vector2 _lastXY;

    private Matrix4x4 _last;

    private int _texture;
    private int _texture1;
    private int _steveModelDrawOrder;

    private int _vertexShader;
    private int _fragmentShader;
    private int _shaderProgram;

    private int _colorRenderBuffer;
    private int _depthRenderBuffer;
    private int _frameBufferObj;

    private int _width, _height;

    private readonly ModelVAO _normalVAO = new();
    private readonly ModelVAO _topVAO = new();

    private readonly SkinAnimation _skina;

    private delegate void GlFunc1(int v1, int v2);
    private delegate void GlFunc2(int v1);
    private delegate void GlFunc3(float v1);
    private delegate void GlFunc4(bool v1);
    private delegate void GlFunc6(int v1, float v2, float v3, float v4);
    private delegate void GlFunc7(int v1, int v2, int v3, int v4, int v5);

    private GlFunc2 glDepthFunc;
    private GlFunc1 glBlendFunc;
    private GlFunc4 glDepthMask;
    private GlFunc2 glDisable;
    private GlFunc2 glDisableVertexAttribArray;
    private GlFunc6 glUniform3f;
    private GlFunc2 glCullFace;
    private GlFunc7 glRenderbufferStorageMultisample;

    public SkinRender()
    {
        _skina = new(this);
        _last = Matrix4x4.Identity;

        DataContextChanged += SkinRender_DataContextChanged;

        PointerMoved += SkinTop_PointerMoved;
        PointerPressed += SkinTop_PointerPressed;
        PointerWheelChanged += SkinTop_PointerWheelChanged;
    }

    private void SkinTop_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.Delta.Y > 0)
        {
            AddDis(0.05f);
        }
        else
        {
            AddDis(-0.05f);
        }
    }

    private void SkinTop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var pro = e.GetCurrentPoint(this);
        _xdiff = (float)pro.Position.X;
        _ydiff = -(float)pro.Position.Y;
    }

    private void SkinTop_PointerMoved(object? sender, PointerEventArgs e)
    {
        var pro = e.GetCurrentPoint(this);
        if (pro.Properties.IsLeftButtonPressed)
        {
            float y = (float)pro.Position.X - _xdiff;
            float x = (float)pro.Position.Y + _ydiff;

            _xdiff = (float)pro.Position.X;
            _ydiff = -(float)pro.Position.Y;

            Rot(x, y);
        }
        else if (pro.Properties.IsRightButtonPressed)
        {
            float x = (float)pro.Position.X - _xdiff;
            float y = (float)pro.Position.Y + _ydiff;

            _xdiff = (float)pro.Position.X;
            _ydiff = -(float)pro.Position.Y;

            Pos(x / ((float)Bounds.Width / 8), -y / ((float)Bounds.Height / 8));
        }
    }

    private void SkinRender_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is SkinModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
            _skina.Run = true;
        }
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var model = (sender as SkinModel)!;
        if (e.PropertyName == "SteveModelType")
        {
            Dispatcher.UIThread.Post(ChangeType);
        }
        else if (e.PropertyName == "EnableAnimation")
        {
            Dispatcher.UIThread.Post(SetAnimation);
        }
        else if (e.PropertyName == "EnableCape")
        {
            Dispatcher.UIThread.Post(RequestNextFrameRendering);
        }
        else if (e.PropertyName == "EnableTop")
        {
            Dispatcher.UIThread.Post(RequestNextFrameRendering);
        }
        else if (e.PropertyName == "Rotate")
        {
            Dispatcher.UIThread.Post(RequestNextFrameRendering);
        }
        else if (e.PropertyName == "Pos")
        {
            _xy.X += model.X;
            _xy.Y += model.Y;
        }
        else if (e.PropertyName == "Dis")
        {
            _dis += model.X;
        }
        else if (e.PropertyName == "Rot")
        {
            _rotXY.X += model.X;
            _rotXY.Y += model.Y;
        }
    }

    public void Rot(float x, float y)
    {
        _rotXY.X += x;
        _rotXY.Y += y;
    }

    public void Pos(float x, float y)
    {
        _xy.X += x;
        _xy.Y += y;
    }

    public void AddDis(float x)
    {
        _dis += x;
    }

    private bool _init = false;

    protected override unsafe void OnOpenGlInit(GlInterface gl)
    {
        if (_init)
            return;

        _init = true;

        CheckError(gl);

        glDepthFunc = Marshal.GetDelegateForFunctionPointer<GlFunc2>(gl.GetProcAddress("glDepthFunc"));
        glBlendFunc = Marshal.GetDelegateForFunctionPointer<GlFunc1>(gl.GetProcAddress("glBlendFunc"));
        glDepthMask = Marshal.GetDelegateForFunctionPointer<GlFunc4>(gl.GetProcAddress("glDepthMask"));
        glCullFace = Marshal.GetDelegateForFunctionPointer<GlFunc2>(gl.GetProcAddress("glCullFace"));
        glDisable = Marshal.GetDelegateForFunctionPointer<GlFunc2>(gl.GetProcAddress("glDisable"));
        glDisableVertexAttribArray = Marshal.GetDelegateForFunctionPointer<GlFunc2>(gl.GetProcAddress("glDisableVertexAttribArray"));
        glUniform3f = Marshal.GetDelegateForFunctionPointer<GlFunc6>(gl.GetProcAddress("glUniform3f"));
        glRenderbufferStorageMultisample = Marshal.GetDelegateForFunctionPointer<GlFunc7>(gl.GetProcAddress("glRenderbufferStorageMultisample"));

        gl.ClearColor(0, 0, 0, 1);
        //GL_BLEND
        gl.Enable(0x0BE2);
        //GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA
        glBlendFunc(0x0302, 0x0303);
        //GL_BACK
        glCullFace(0x0405);

        CheckError(gl);

        var model = (DataContext as SkinModel)!;
        model.Info = $"Renderer: {gl.GetString(GlConsts.GL_RENDERER)} Version: {gl.GetString(GlConsts.GL_VERSION)}";

        _vertexShader = gl.CreateShader(GlConsts.GL_VERTEX_SHADER);
        var smg = gl.CompileShaderAndGetError(_vertexShader, SkinShader.VertexShader(GlVersion, false));
        if (smg != null)
        {
            WindowManager.ShowError(App.Lang("SkinWindow.Error2"),
                    new Exception($"GlConsts.GL_VERTEX_SHADER.\n{smg}"));
        }

        _fragmentShader = gl.CreateShader(GlConsts.GL_FRAGMENT_SHADER);
        smg = gl.CompileShaderAndGetError(_fragmentShader, SkinShader.VertexShader(GlVersion, true));
        if (smg != null)
        {
            WindowManager.ShowError(App.Lang("SkinWindow.Error2"),
                    new Exception($"GlConsts.GL_FRAGMENT_SHADER.\n{smg}"));
        }

        _shaderProgram = gl.CreateProgram();
        gl.AttachShader(_shaderProgram, _vertexShader);
        gl.AttachShader(_shaderProgram, _fragmentShader);

        smg = gl.LinkProgramAndGetError(_shaderProgram);
        if (smg != null)
        {
            WindowManager.ShowError(App.Lang("SkinWindow.Error1"), new Exception(smg));
        }

        InitVAO(gl, _normalVAO);
        InitVAO(gl, _topVAO);

        _texture = gl.GenTexture();
        _texture1 = gl.GenTexture();

        CheckError(gl);

        PointerWheelChanged += OpenGlPageControl_PointerWheelChanged;
        PointerPressed += OpenGlPageControl_PointerPressed;
        PointerReleased += OpenGlPageControl_PointerReleased;
        PointerMoved += OpenGlPageControl_PointerMoved;

        model.SkinLoadDone();

        model.IsLoad = true;
    }

    private void InitFrameBuffer(GlInterface gl)
    {
        _colorRenderBuffer = gl.GenRenderbuffer();
        gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, _colorRenderBuffer);
        glRenderbufferStorageMultisample(GlConsts.GL_RENDERBUFFER, 8, GlConsts.GL_RGBA8, _width, _height);

        _depthRenderBuffer = gl.GenRenderbuffer();
        gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, _depthRenderBuffer);
        //GlConsts.GL_DEPTH_COMPONENT24
        glRenderbufferStorageMultisample(GlConsts.GL_RENDERBUFFER, 8, 0x81A6, _width, _height);

        _frameBufferObj = gl.GenFramebuffer();
        gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, _frameBufferObj);

        gl.FramebufferRenderbuffer(GlConsts.GL_FRAMEBUFFER,
            GlConsts.GL_COLOR_ATTACHMENT0, GlConsts.GL_RENDERBUFFER, _colorRenderBuffer);

        gl.FramebufferRenderbuffer(GlConsts.GL_FRAMEBUFFER,
            GlConsts.GL_DEPTH_ATTACHMENT, GlConsts.GL_RENDERBUFFER, _depthRenderBuffer);

        if (gl.CheckFramebufferStatus(GlConsts.GL_FRAMEBUFFER) != GlConsts.GL_FRAMEBUFFER_COMPLETE)
        {
            throw new Exception("glCheckFramebufferStatus status != GL_FRAMEBUFFER_COMPLETE");
        }
        gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, 0);
        gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, 0);
    }

    private void DeleteFrameBuffer(GlInterface gl)
    {
        if (_frameBufferObj != 0)
        {
            gl.DeleteFramebuffer(_frameBufferObj);
            _frameBufferObj = 0;
        }

        if (_colorRenderBuffer != 0)
        {
            gl.DeleteRenderbuffer(_colorRenderBuffer);
            _colorRenderBuffer = 0;
        }

        if (_depthRenderBuffer != 0)
        {
            gl.DeleteRenderbuffer(_depthRenderBuffer);
            _depthRenderBuffer = 0;
        }
    }

    private static void InitVAOItem(GlInterface gl, VAOItem item)
    {
        item.VertexBufferObject = gl.GenBuffer();
        item.IndexBufferObject = gl.GenBuffer();
    }

    private static void InitVAO(GlInterface gl, ModelVAO vao)
    {
        vao.Head.VertexArrayObject = gl.GenVertexArray();
        vao.Body.VertexArrayObject = gl.GenVertexArray();
        vao.LeftArm.VertexArrayObject = gl.GenVertexArray();
        vao.RightArm.VertexArrayObject = gl.GenVertexArray();
        vao.LeftLeg.VertexArrayObject = gl.GenVertexArray();
        vao.RightLeg.VertexArrayObject = gl.GenVertexArray();
        vao.Cape.VertexArrayObject = gl.GenVertexArray();

        InitVAOItem(gl, vao.Head);
        InitVAOItem(gl, vao.Body);
        InitVAOItem(gl, vao.LeftArm);
        InitVAOItem(gl, vao.RightArm);
        InitVAOItem(gl, vao.LeftLeg);
        InitVAOItem(gl, vao.RightLeg);
        InitVAOItem(gl, vao.Cape);
    }

    public void ChangeType()
    {
        _switchModel = true;

        RequestNextFrameRendering();
    }

    public void ChangeSkin()
    {
        _switchSkin = true;

        RequestNextFrameRendering();
    }

    public void Reset()
    {
        _dis = 1;
        _diffXY.X = 0;
        _diffXY.Y = 0;
        _xy.X = 0;
        _xy.Y = 0;
        _saveXY.X = 0;
        _saveXY.Y = 0;
        _lastXY.X = 0;
        _lastXY.Y = 0;
        _last = Matrix4x4.Identity;

        RequestNextFrameRendering();
    }

    private static void LoadTex(GlInterface gl, SKBitmap image, int tex)
    {
        gl.ActiveTexture(GlConsts.GL_TEXTURE0);
        gl.BindTexture(GlConsts.GL_TEXTURE_2D, tex);

        gl.TexParameteri(
            GlConsts.GL_TEXTURE_2D,
            GlConsts.GL_TEXTURE_MIN_FILTER, GlConsts.GL_LINEAR
        );
        gl.TexParameteri(
            GlConsts.GL_TEXTURE_2D,
            GlConsts.GL_TEXTURE_MAG_FILTER, GlConsts.GL_NEAREST
        );
        gl.TexParameteri(
            GlConsts.GL_TEXTURE_2D,
            //GlConsts.GL_TEXTURE_WRAP_S GL_CLAMP_TO_EDGE
            0x2802, 0x812F
        );
        gl.TexParameteri(
            GlConsts.GL_TEXTURE_2D,
            //GlConsts.GL_TEXTURE_WRAP_T, GlConsts.GL_CLAMP_TO_EDGE
            0x2803, 0x812F
        );

        using var image1 = image.Copy(SKColorType.Rgba8888);

        gl.TexImage2D(GlConsts.GL_TEXTURE_2D, 0, GlConsts.GL_RGBA, image.Width,
               image.Height, 0, GlConsts.GL_RGBA, GlConsts.GL_UNSIGNED_BYTE, image1.GetPixels());
        gl.BindTexture(GlConsts.GL_TEXTURE_2D, 0);
    }

    private unsafe void PutVAO(GlInterface gl, VAOItem vao, CubeModelItemObj model, float[] uv)
    {
        float[] vertices =
        [
            0.0f,  0.0f, -1.0f,
            0.0f,  0.0f, -1.0f,
            0.0f,  0.0f, -1.0f,
            0.0f,  0.0f, -1.0f,

            0.0f,  0.0f,  1.0f,
            0.0f,  0.0f,  1.0f,
            0.0f,  0.0f,  1.0f,
            0.0f,  0.0f,  1.0f,

            -1.0f,  0.0f,  0.0f,
            -1.0f,  0.0f,  0.0f,
            -1.0f,  0.0f,  0.0f,
            -1.0f,  0.0f,  0.0f,

            1.0f,  0.0f,  0.0f,
            1.0f,  0.0f,  0.0f,
            1.0f,  0.0f,  0.0f,
            1.0f,  0.0f,  0.0f,

            0.0f,  1.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  1.0f,  0.0f,

            0.0f, -1.0f,  0.0f,
            0.0f, -1.0f,  0.0f,
            0.0f, -1.0f,  0.0f,
            0.0f, -1.0f,  0.0f,
        ];

        gl.UseProgram(_shaderProgram);
        gl.BindVertexArray(vao.VertexArrayObject);

        int a_Position = gl.GetAttribLocationString(_shaderProgram, "a_position");
        int a_texCoord = gl.GetAttribLocationString(_shaderProgram, "a_texCoord");
        int a_normal = gl.GetAttribLocationString(_shaderProgram, "a_normal");

        glDisableVertexAttribArray(a_Position);
        glDisableVertexAttribArray(a_texCoord);
        glDisableVertexAttribArray(a_normal);

        int size = model.Model.Length / 3;

        var points = new Vertex[size];

        for (var primitive = 0; primitive < size; primitive++)
        {
            var srci = primitive * 3;
            var srci1 = primitive * 2;
            points[primitive] = new Vertex
            {
                Position = new(model.Model[srci], model.Model[srci + 1], model.Model[srci + 2]),
                UV = new(uv[srci1], uv[srci1 + 1]),
                Normal = new(vertices[srci], vertices[srci + 1], vertices[srci + 2])
            };
        }

        gl.BindBuffer(GlConsts.GL_ARRAY_BUFFER, vao.VertexBufferObject);
        var vertexSize = Marshal.SizeOf<Vertex>();
        fixed (void* pdata = points)
        {
            gl.BufferData(GlConsts.GL_ARRAY_BUFFER, points.Length * vertexSize,
                    new IntPtr(pdata), GlConsts.GL_STATIC_DRAW);
        }

        gl.BindBuffer(GlConsts.GL_ELEMENT_ARRAY_BUFFER, vao.IndexBufferObject);
        fixed (void* pdata = model.Point)
        {
            gl.BufferData(GlConsts.GL_ELEMENT_ARRAY_BUFFER,
                model.Point.Length * sizeof(ushort), new IntPtr(pdata), GlConsts.GL_STATIC_DRAW);
        }

        gl.VertexAttribPointer(a_Position, 3, GlConsts.GL_FLOAT, 0, 8 * sizeof(float), 0);
        gl.VertexAttribPointer(a_texCoord, 2, GlConsts.GL_FLOAT, 0, 8 * sizeof(float), 3 * sizeof(float));
        gl.VertexAttribPointer(a_normal, 3, GlConsts.GL_FLOAT, 0, 8 * sizeof(float), 5 * sizeof(float));

        gl.EnableVertexAttribArray(a_Position);
        gl.EnableVertexAttribArray(a_texCoord);
        gl.EnableVertexAttribArray(a_normal);

        gl.BindVertexArray(0);

        CheckError(gl);
    }
    private void OpenGlPageControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var model = (DataContext as SkinModel)!;
        if (!model.HaveSkin)
            return;

        var po = e.GetCurrentPoint(this);
        var pos = e.GetPosition(this);

        if (po.Properties.IsLeftButtonPressed)
        {
            _diffXY.X = (float)pos.X;
            _diffXY.Y = -(float)pos.Y;
        }
        else if (po.Properties.IsRightButtonPressed)
        {
            _lastXY.X = (float)pos.X;
            _lastXY.Y = (float)pos.Y;
        }

        RequestNextFrameRendering();
    }

    private void OpenGlPageControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var model = (DataContext as SkinModel)!;
        if (!model.HaveSkin)
            return;

        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            _saveXY.X = _xy.X;
            _saveXY.Y = _xy.Y;
        }

        RequestNextFrameRendering();
    }

    private void OpenGlPageControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        var model = (DataContext as SkinModel)!;
        if (!model.HaveSkin)
            return;

        var po = e.GetCurrentPoint(this);

        if (po.Properties.IsLeftButtonPressed)
        {
            var point = e.GetPosition(this);
            _rotXY.Y = (float)point.X - _diffXY.X;
            _rotXY.X = (float)point.Y + _diffXY.Y;
            _rotXY.Y *= 2;
            _rotXY.X *= 2;
            _diffXY.X = (float)point.X;
            _diffXY.Y = -(float)point.Y;
        }
        else if (po.Properties.IsRightButtonPressed)
        {
            var point = e.GetPosition(this);
            _xy.X = (-(_lastXY.X - (float)point.X) / 100) + _saveXY.X;
            _xy.Y = ((_lastXY.Y - (float)point.Y) / 100) + _saveXY.Y;
        }

        RequestNextFrameRendering();
    }

    private void OpenGlPageControl_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var model = (DataContext as SkinModel)!;
        if (!model.HaveSkin)
            return;

        if (e.Delta.Y > 0)
        {
            _dis += 0.1f;
        }
        else if (e.Delta.Y < 0)
        {
            _dis -= 0.1f;
        }

        RequestNextFrameRendering();
    }

    private static void CheckError(GlInterface gl)
    {
        int err;
        while ((err = gl.GetError()) != GlConsts.GL_NO_ERROR)
            Console.WriteLine(err);
    }

    private static void DeleteVAOItem(GlInterface gl, VAOItem item)
    {
        gl.DeleteBuffer(item.VertexBufferObject);
        gl.DeleteBuffer(item.IndexBufferObject);
    }

    private static void DeleteVAO(GlInterface gl, ModelVAO vao)
    {
        gl.DeleteVertexArray(vao.Head.VertexArrayObject);
        gl.DeleteVertexArray(vao.Body.VertexArrayObject);
        gl.DeleteVertexArray(vao.LeftArm.VertexArrayObject);
        gl.DeleteVertexArray(vao.RightArm.VertexArrayObject);
        gl.DeleteVertexArray(vao.LeftLeg.VertexArrayObject);
        gl.DeleteVertexArray(vao.RightLeg.VertexArrayObject);

        DeleteVAOItem(gl, vao.Head);
        DeleteVAOItem(gl, vao.Body);
        DeleteVAOItem(gl, vao.LeftArm);
        DeleteVAOItem(gl, vao.RightArm);
        DeleteVAOItem(gl, vao.LeftLeg);
        DeleteVAOItem(gl, vao.RightLeg);
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        _skina.Close();

        // Unbind everything
        gl.BindBuffer(GlConsts.GL_ARRAY_BUFFER, 0);
        gl.BindBuffer(GlConsts.GL_ELEMENT_ARRAY_BUFFER, 0);
        gl.BindVertexArray(0);
        gl.UseProgram(0);

        // Delete all resources.
        DeleteVAO(gl, _normalVAO);
        DeleteVAO(gl, _topVAO);

        gl.DeleteProgram(_shaderProgram);
        gl.DeleteShader(_fragmentShader);
        gl.DeleteShader(_vertexShader);

        DeleteFrameBuffer(gl);
    }

    public void SetAnimation()
    {
        var model = (DataContext as SkinModel)!;
        if (model.EnableAnimation)
        {
            _skina.Run = true;
        }
        else
        {
            _skina.Run = false;
        }

        RequestNextFrameRendering();
    }

    public bool HitTest(Point point)
    {
        return IsVisible;
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        throw new NotImplementedException();
    }
}