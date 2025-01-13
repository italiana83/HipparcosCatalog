using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Compute.OpenCL;
using SkiaSharp;

namespace HipparcosCatalog
{
    public class Nebula3D
    {
        private int _vao;
        private Shader _shader;

        public Nebula3D()
        {
            InitializeShader();
        }

        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model, Vector3 cameraPosition, TextRenderer textRenderer)
        {
            _shader.Use();
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);

            float time = (float)GLFW.GetTime();
            _shader.SetFloat("uTime", time);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

        }

        public void InitializeBuffers()
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            float[] vertices =
            {
            -1f, -1f,
             1f, -1f,
             1f,  1f,
            -1f,  1f
        };
            int[] indices = { 0, 1, 2, 0, 2, 3 };

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader.SetVector2("uResolution", new Vector2(10, 10));
        }

        private void InitializeShader()
        {
            string xertexShaderSource = @"
            #version 330 core
            layout (location = 0) in vec2 aPosition;
            out vec3 fragPosition;
            void main()
            {
                fragPosition = vec3(aPosition, 0.0);
                gl_Position = vec4(aPosition, 0.0, 1.0);
            }";

            string fragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        in vec3 fragPosition;

        uniform float uTime;
        uniform vec2 uResolution;

        float hash(vec3 p)
        {
            return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453123);
        }

        float noise(vec3 p)
        {
            vec3 i = floor(p);
            vec3 f = fract(p);
            vec3 u = f * f * (3.0 - 2.0 * f);

            return mix(
                mix(mix(hash(i + vec3(0.0, 0.0, 0.0)), hash(i + vec3(1.0, 0.0, 0.0)), u.x),
                    mix(hash(i + vec3(0.0, 1.0, 0.0)), hash(i + vec3(1.0, 1.0, 0.0)), u.x), u.y),
                mix(mix(hash(i + vec3(0.0, 0.0, 1.0)), hash(i + vec3(1.0, 0.0, 1.0)), u.x),
                    mix(hash(i + vec3(0.0, 1.0, 1.0)), hash(i + vec3(1.0, 1.0, 1.0)), u.x), u.y), u.z
            );
        }

        float fbm(vec3 p)
        {
            float value = 0.0;
            float amplitude = 0.5;
            float frequency = 1.0;
            for (int i = 0; i < 5; i++)
            {
                value += amplitude * noise(p * frequency);
                frequency *= 2.0;
                amplitude *= 0.5;
            }
            return value;
        }

        void main()
        {
            vec2 uv = fragPosition.xy * 0.5 + 0.5;
            vec3 p = vec3(uv, uTime * 0.1);

            float n = fbm(p * 3.0);
            vec3 color = vec3(0.1, 0.2, 0.5) * n + vec3(0.8, 0.4, 0.2) * (1.0 - n);

            FragColor = vec4(color, 1.0);
        }";

            _shader = new Shader(xertexShaderSource, fragmentShaderSource, null, ShaderSourceMode.Code);
        }
    }
}