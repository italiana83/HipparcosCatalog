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
        int texture3D;
        int _indexCount;

        public Nebula3D()
        {
            InitializeShader();
            InitializeBuffers();
            texture3D = Create3DTexture(64); // 64x64x64 текстура шума
        }

        private float _edgeSharpness = 10.0f; // Резкость краев туманности (чем больше, тем четче края)
        private float _opacity = 0.3f; // Прозрачность туманности (от 0.0 до 1.0)
        private int steps = 64;         // Количество шагов трассировки
        private float densityScale = 0.9f; // Масштаб плотности
        private Vector3 _position = new Vector3(2.0f, 0.0f, 0.0f);
        private Vector3 _size = new Vector3(5.0f, 5.0f, 5.0f);
        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model, Vector3 cameraPosition, TextRenderer textRenderer)
        {
            _shader.Use();
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);


            // Передаём цвета
            _shader.SetVector3("nebulaColor", new Vector3(0.8f, 0.1f, 0.2f));
            _shader.SetVector3("lightColor", new Vector3(0.1f, 0.1f, 0.8f));

            // Передаём текстуру
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture3D, texture3D);
            _shader.SetInt("noiseTexture", 0);

            // Передаём положение и размер
            _shader.SetVector3("position", _position);
            _shader.SetVector3("size", _size);

            // Передача прозрачности
            _shader.SetFloat("opacity", _opacity);

            _shader.SetFloat("edgeSharpness", _edgeSharpness);

            // Рендерим куб
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            GL.UseProgram(0);

        }

        public void InitializeBuffers()
        {
            const int latitudeSegments = 32; // Количество сегментов по широте
            const int longitudeSegments = 32; // Количество сегментов по долготе
            const float radius = 10.0f; // Радиус сферы

            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();

            // Генерация вершин сферы
            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = lat * MathF.PI / latitudeSegments;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2.0f * MathF.PI / longitudeSegments;
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    // Позиции
                    vertices.Add(x * radius);
                    vertices.Add(y * radius);
                    vertices.Add(z * radius);

                    // Нормали
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);
                }
            }

            // Генерация индексов для треугольников
            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    uint first = (uint)(lat * (longitudeSegments + 1) + lon);
                    uint second = first + (uint)longitudeSegments + 1;

                    indices.Add(first);
                    indices.Add(second);
                    indices.Add(first + 1);

                    indices.Add(second);
                    indices.Add(second + 1);
                    indices.Add(first + 1);
                }
            }

            // Генерация VAO, VBO и EBO
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            // Настройка атрибутов вершин
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        private void InitializeShader()
        {
            string vertexShaderSource = @"
            #version 330 core

            layout(location = 0) in vec3 aPosition;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            out vec3 fragPosition;

            void main()
            {
                vec4 worldPosition = model * vec4(aPosition, 1.0);
                fragPosition = worldPosition.xyz;
                gl_Position = projection * view * worldPosition;
            }";

            string fragmentShaderSource = @"
            #version 330 core

            out vec4 FragColor;

            in vec3 fragPosition;

            uniform vec3 nebulaColor; // Primary nebula color
            uniform vec3 lightColor; // Secondary color
            uniform vec3 position; // Nebula position
            uniform vec3 size; // Nebula size
            uniform sampler3D noiseTexture; // 3D noise texture
            uniform float opacity; // Transparency coefficient
            uniform float edgeSharpness; // Edge sharpness control

            // Function for jagged edges
            float edgeFalloff(vec3 localPos)
            {
                // Distance from the center of the sphere
                float distance = length(localPos - 0.5);

                // Reduce density at sphere edge
                return exp(-edgeSharpness * pow(max(0.0, distance - 0.5), 2.0));
            }

            void main()
            {
                vec3 localPos = (fragPosition - position) / size + 0.5; // Convert to local coordinates

                // Check if point is inside sphere
                if (length(localPos - 0.5) > 0.5)
                {
                    discard; // Trim all points outside sphere
                }

                // Sample density from noise texture
                float density = texture(noiseTexture, localPos).r;

                // Modify density based on edges
                density *= edgeFalloff(localPos);

                // Final color
                vec3 color = nebulaColor * density * lightColor;
                FragColor = vec4(color, density * opacity);
            }";

            _shader = new Shader(vertexShaderSource, fragmentShaderSource, null, ShaderSourceMode.Code);
        }

        private int Create3DTexture(int size)
        {
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture3D, textureId);

            int width = size;
            int height = size;
            int depth = size;

            // Генерация данных для текстуры (например, шум)
            float[] textureData = new float[width * height * depth];
            Random random = new Random();
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float value = (float)random.NextDouble(); // Или процедурный шум
                        textureData[x + y * width + z * width * height] = value;
                    }
                }
            }

            // Загрузка данных в OpenGL
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.R8, width, height, depth, 0,
                          PixelFormat.Red, PixelType.Float, textureData);

            // Настройки текстуры
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture3D, 0);
            return textureId;
        }

    }
}