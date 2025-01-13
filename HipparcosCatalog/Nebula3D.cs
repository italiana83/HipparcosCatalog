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
        private int _vbo;
        private int _ebo;
        private Shader _shader;

        public Nebula3D()
        {
            InitializeShader();
        }

        Vector3 baseColor = new Vector3(1.1f, 1.0f, 1.0f); // Темно-синий
        Vector3 highlightColor = new Vector3(1.0f, 0.4f, 0.0f); // Оранжевый
        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model, Vector3 cameraPosition, TextRenderer textRenderer)
        {
            _shader.Use();
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);

            float time = (float)GLFW.GetTime();
            _shader.SetFloat("uTime", time);// Time (for animation)
            _shader.SetFloat("uNebulaScale", 5.0f);// Nebula size
            _shader.SetVector3("uCameraPosition", cameraPosition);
            _shader.SetVector3("uNebulaPosition", new Vector3(1,1,0));

            //baseColor = new Vector3(0.1f + 0.1f * (float)Math.Sin(time), 0.2f, 0.5f); // Анимация цвета
            _shader.SetVector3("uBaseColor", baseColor);
            _shader.SetVector3("uHighlightColor", highlightColor);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

        }

        //public void InitializeBuffers()
        //{
        //    _vao = GL.GenVertexArray();
        //    GL.BindVertexArray(_vao);

        //    float[] vertices =
        //    {
        //    -1f, -1f,
        //     1f, -1f,
        //     1f,  1f,
        //    -1f,  1f
        //};
        //    int[] indices = { 0, 1, 2, 0, 2, 3 };

        //    int vbo = GL.GenBuffer();
        //    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        //    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        //    int ebo = GL.GenBuffer();
        //    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        //    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

        //    GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        //    GL.EnableVertexAttribArray(0);
        //}

        //    public void InitializeBuffers()
        //    {
        //        // Куб, ограничивающий туманность (в локальных координатах [-1, 1])
        //        float[] vertices = {
        //    // Позиции         // Нормали
        //    -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, // Задняя грань
        //     1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f,
        //     1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,
        //    -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,

        //    -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, // Передняя грань
        //     1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,
        //     1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,
        //    -1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,
        //};

        //        // Индексы для куба (упрощают отрисовку)
        //        uint[] indices = {
        //    // Задняя грань
        //    0, 1, 2,
        //    2, 3, 0,
        //    // Передняя грань
        //    4, 5, 6,
        //    6, 7, 4,
        //    // Левая грань
        //    0, 4, 7,
        //    7, 3, 0,
        //    // Правая грань
        //    1, 5, 6,
        //    6, 2, 1,
        //    // Нижняя грань
        //    0, 1, 5,
        //    5, 4, 0,
        //    // Верхняя грань
        //    3, 2, 6,
        //    6, 7, 3
        //};

        //        // Генерируем VAO
        //        _vao = GL.GenVertexArray();
        //        GL.BindVertexArray(_vao);

        //        // Генерируем VBO
        //        _vbo = GL.GenBuffer();
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        //        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        //        // Генерируем EBO
        //        _ebo = GL.GenBuffer();
        //        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        //        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        //        // Настройка атрибутов вершин
        //        // Позиции (location = 0)
        //        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        //        GL.EnableVertexAttribArray(0);

        //        // Нормали (location = 1)
        //        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        //        GL.EnableVertexAttribArray(1);

        //        // Отвязываем VAO
        //        GL.BindVertexArray(0);
        //    }

        public void InitializeBuffers()
        {
            // Вершины куба (без нормалей)
            float[] vertices = {
        // Позиции
        -1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
    };

            // Индексы для куба
            uint[] indices = {
        // Задняя грань
        0, 1, 2,
        2, 3, 0,
        // Передняя грань
        4, 5, 6,
        6, 7, 4,
        // Левая грань
        0, 4, 7,
        7, 3, 0,
        // Правая грань
        1, 5, 6,
        6, 2, 1,
        // Нижняя грань
        0, 1, 5,
        5, 4, 0,
        // Верхняя грань
        3, 2, 6,
        6, 7, 3
    };

            // Генерация VAO
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            // Генерация VBO
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Генерация EBO
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Настройка атрибутов вершин
            // Позиции (location = 0)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Отвязываем VAO
            GL.BindVertexArray(0);
        }

        private void InitializeShader()
        {
            string vertexShaderSource = @"
               #version 330 core

                layout(location = 0) in vec3 aPos; // Local space position of the vertex
                layout(location = 1) in vec3 aNormal; // Vertex normal (if needed for lighting)

                out vec3 fragPosition; // World space position (for Ray Marching)
                out vec3 fragNormal; // Vertex normal (optional)

                uniform mat4 model; // Model matrix (translates from local to world space)
                uniform mat4 view; // View matrix (camera)
                uniform mat4 projection; // Projection matrix

                void main() {
                    // Vertex position in world space
                    vec4 worldPosition = projection * view * model * vec4(aPos, 1.0);
                    fragPosition = worldPosition.xyz;

                    // Transform the normal to world space
                    fragNormal = mat3(transpose(inverse(model))) * aNormal;

                    // Translate the vertex position to screen space
                    gl_Position = worldPosition;
                }";

            string fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;

            in vec3 fragPosition; // Fragment position in world space

            uniform vec3 uCameraPosition; // Camera position
            uniform vec3 uNebulaPosition; // Nebula center
            uniform float uNebulaScale; // Nebula size
            uniform float uTime; // Time (for animation)
            uniform vec3 uBaseColor; // Nebula base color
            uniform vec3 uHighlightColor; // Accent color

            // Hash function for noise generation
            float hash(vec3 p) {
            return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453123);
            }

            // 3D noise
            float noise(vec3 p) {
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

            // Fractal noise (fbm)
            float fbm(vec3 p) {
             float value = 0.0;
             float amplitude = 0.5;
             float frequency = 1.0;
             for (int i = 0; i < 5; i++) {
             value += amplitude * noise(p * frequency);
             frequency *= 2.0;
             amplitude *= 0.5;
             }
             return value;
            }

            // Ray Marching for volume
            float raymarch(vec3 origin, vec3 direction) {
             float totalDensity = 0.0;
             float t = 0.0;
             for (int i = 0; i < 100; i++) { // 100 steps Ray Marching
             vec3 pos = origin + direction * t;

             // Check if we are inside the cube
            vec3 localPos = (pos - uNebulaPosition) / uNebulaScale; // Local position in the cube
            if (any(lessThan(localPos, vec3(-1.0))) || any(greaterThan(localPos, vec3(1.0)))) {
            break; // If outside the cube, stop
            }

            float density = fbm(localPos * 3.0 + uTime * 0.1);
            totalDensity += density * 0.1;
            if (totalDensity > 1.0) break; // Clamp the density
            t += 0.05; // Step
            }
            return clamp(totalDensity, 0.0, 1.0);
            }

            void main() {
            vec3 rayDir = normalize(fragPosition - uCameraPosition); // Ray direction
            float density = raymarch(uCameraPosition, rayDir);

            // Nebula color, mix of base and accent colors
            vec3 nebulaColor = mix(uBaseColor, uHighlightColor, density);
            FragColor = vec4(nebulaColor, density); // Transparency depends on density
            }";

            _shader = new Shader(vertexShaderSource, fragmentShaderSource, null, ShaderSourceMode.Code);
        }
    }
}