using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class Particles
    {
        private int _vao;
        private int _vbo;
        private int _ebo;
        private Shader _shader;

        List<float> particleData;
        int particleCount = 100;

        uint textureHandle;
        OpenTK.Graphics.OpenGL.TextureTarget textureTarget;


        public Particles()
        {
            Vector3 position = new Vector3(0.0f, 5.0f, -10.0f); // Центр туманности
            Vector3 volume = new Vector3(20.0f, 30.0f, 20.0f);    // Размеры объема (ширина, высота, глубина)

            InitializeParticles(particleCount, position, volume);

            InitializeShader();
            InitializeBuffers();


            var textureParams = new TextureLoaderParameters();
            ImageGDI.LoadFromDisk("1.jpg", textureParams, out textureHandle, out textureTarget);

        }

        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model, Vector3 cameraPosition, TextRenderer textRenderer)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            _shader.SetInt("uParticleTexture", 0);

            _shader.Use();
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);

            // Рендеринг частиц
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Points, 0, particleData.Count);
            _shader.Disable();

        }

        public void InitializeParticles(int particleCount, Vector3 position, Vector3 volume)
        {
            Random random = new Random();
            particleData = new List<float>();

            for (int i = 0; i < particleCount; i++)
            {
                // Случайное положение частицы внутри объема
                float x = (float)(random.NextDouble() * 2.0 - 1.0) * volume.X + position.X;
                float y = (float)(random.NextDouble() * 2.0 - 1.0) * volume.Y + position.Y;
                float z = (float)(random.NextDouble() * 2.0 - 1.0) * volume.Z + position.Z;

                // Случайный цвет
                float r = (float)random.NextDouble();
                float g = (float)random.NextDouble();
                float b = (float)random.NextDouble();
                float a = 0.1f + (float)random.NextDouble() * 0.5f; // Прозрачность

                // Случайный размер
                float size = 5.0f + (float)random.NextDouble() * 10.0f;

                // Добавляем данные частицы
                particleData.AddRange(new float[] { x, y, z, r, g, b, a, size });
            }

         
        }

        public void InitializeBuffers()
        {
            // Генерация VBO и VAO
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, particleData.Count * sizeof(float), particleData.ToArray(), BufferUsageHint.StaticDraw);

            // Настройка атрибутов
            // Позиция (location = 0)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Цвет (location = 1)
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Размер (location = 2)
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 8 * sizeof(float), 7 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        private void InitializeShader()
        {
            string vertexShaderSource = @"
            #version 330 core

            layout(location = 0) in vec3 aPos; // Particle position
            layout(location = 1) in vec4 aColor; // Particle color (rgba)
            layout(location = 2) in float aSize; // Particle size

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            out vec4 vColor; // Pass color to fragment shader

            void main() {
                vColor = aColor; // Pass color
                gl_PointSize = aSize; // Set particle size
                gl_Position = projection * view * model * vec4(aPos, 1.0); // Position transformation
            }";

            string fragmentShaderSource = @"
            #version 330 core

            in vec4 vColor; // Color from vertex shader
            out vec4 FragColor;

            uniform sampler2D uParticleTexture; // Particle texture

            void main() {
                // Get value from texture
                vec4 texColor = texture(uParticleTexture, gl_PointCoord);
                // Multiply particle color by texture for glow effect
                FragColor = vColor * texColor;
            }";

            _shader = new Shader(vertexShaderSource, fragmentShaderSource, null, ShaderSourceMode.Code);
        }
    }
}
