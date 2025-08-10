using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class AxisCircularRender
    {
        private int _vao;
        private int _vbo;
        private Shader _shader;
        private int _planeVao;
        private int _planeVbo;
        private Shader _planeShader;


        private List<float> circleVertices = new List<float>();
        private List<float> planeVertices = new List<float>();
        public AxisCircularRender()
        {
            // Шейдер для круга
            string circleVertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;

        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;

        void main()
        {
            gl_Position = projection * view * model * vec4(aPosition, 1.0);
        }";

            string circleFragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(0.0, 0.0, 1.0, 1.0);
        }";

            _shader = new Shader(circleVertexShaderSource, circleFragmentShaderSource, null, ShaderSourceMode.Code);

            // Шейдер для плоскости
            string planeFragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(0.0, 0.0, 1.0, 0.05);
        }";

            _planeShader = new Shader(circleVertexShaderSource, planeFragmentShaderSource, null, ShaderSourceMode.Code);

        }

        public void GenerateCircles(float step, int circleCount, Vector3 center, int segments, string plane = "XY")
        {
            circleVertices.Clear();

            // Генерация кругов
            for (int j = 1; j <= circleCount; j++)
            {
                float currentRadius = j * step;

                for (int i = 0; i <= segments; i++)
                {
                    float angle = MathF.PI * 2 * i / segments;
                    float x = currentRadius * MathF.Cos(angle);
                    float y = currentRadius * MathF.Sin(angle);

                    // Координаты для различных плоскостей
                    if (plane == "XY")
                        circleVertices.AddRange(new float[] { x + center.X, y + center.Y, center.Z });
                    else if (plane == "XZ")
                        circleVertices.AddRange(new float[] { x + center.X, center.Y, y + center.Z });
                    else if (plane == "YZ")
                        circleVertices.AddRange(new float[] { center.X, x + center.Y, y + center.Z });
                }
            }

            // Создание VAO и VBO для кругов
            if (_vao == 0) _vao = GL.GenVertexArray();
            if (_vbo == 0) _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, circleVertices.Count * sizeof(float), circleVertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
        public void DrawCircle(Matrix4 view, Matrix4 projection, Matrix4 model)
        {
            //var error = GL.GetError();

            // Рисуем круг
            _shader.Use();
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, circleVertices.Count / 3);

            // Рисуем плоскость
            _planeShader.Use();
            _planeShader.SetMatrix4("view", view);
            _planeShader.SetMatrix4("projection", projection);
            _planeShader.SetMatrix4("model", model);
            GL.BindVertexArray(_planeVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, planeVertices.Count / 3);

            GL.BindVertexArray(0);

        }

        public void GenerateCirclesAndLinesAndPlane(float step, int circleCount, Vector3 center, int segments, string plane = "XY")
        {
            //step = step * 0.306601f;

            circleVertices.Clear();
            planeVertices.Clear();

            float maxRadius = circleCount * step;

            // Генерация кругов
            for (int j = 1; j <= circleCount; j++)
            {
                float currentRadius = j * step;

                for (int i = 0; i <= segments; i++)
                {
                    float angle = MathF.PI * 2 * i / segments;
                    float x = currentRadius * MathF.Cos(angle);
                    float y = currentRadius * MathF.Sin(angle);

                    if (plane == "XY")
                        circleVertices.AddRange(new float[] { x + center.X, y + center.Y, center.Z });
                    else if (plane == "XZ")
                        circleVertices.AddRange(new float[] { x + center.X, center.Y, y + center.Z });
                    else if (plane == "YZ")
                        circleVertices.AddRange(new float[] { center.X, x + center.Y, y + center.Z });
                }
            }

            // Генерация плоскости
            for (int i = 0; i < segments; i++)
            {
                float angle1 = MathF.PI * 2 * i / segments;
                float angle2 = MathF.PI * 2 * (i + 1) / segments;

                float x1 = maxRadius * MathF.Cos(angle1);
                float y1 = maxRadius * MathF.Sin(angle1);
                float x2 = maxRadius * MathF.Cos(angle2);
                float y2 = maxRadius * MathF.Sin(angle2);

                if (plane == "XY")
                {
                    planeVertices.AddRange(new float[]
                    {
                    center.X, center.Y, center.Z, // Центр круга
                    x1 + center.X, y1 + center.Y, center.Z, // Точка 1
                    x2 + center.X, y2 + center.Y, center.Z  // Точка 2
                    });
                }
                else if (plane == "XZ")
                {
                    planeVertices.AddRange(new float[]
                    {
                    center.X, center.Y, center.Z,
                    x1 + center.X, center.Y, y1 + center.Z,
                    x2 + center.X, center.Y, y2 + center.Z
                    });
                }
                else if (plane == "YZ")
                {
                    planeVertices.AddRange(new float[]
                    {
                    center.X, center.Y, center.Z,
                    center.X, x1 + center.Y, y1 + center.Z,
                    center.X, x2 + center.Y, y2 + center.Z
                    });
                }
            }

            // VAO и VBO для кругов, линий и плоскости
            UpdateBuffers();
        }

        private void UpdateBuffers()
        {
            // Круги
            if (_vao == 0) _vao = GL.GenVertexArray();
            if (_vbo == 0) _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, circleVertices.Count * sizeof(float), circleVertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Плоскость
            if (_planeVao == 0) _planeVao = GL.GenVertexArray();
            if (_planeVbo == 0) _planeVbo = GL.GenBuffer();

            GL.BindVertexArray(_planeVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _planeVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, planeVertices.Count * sizeof(float), planeVertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
        public void Dispose()
        {
            if (_vao != 0) GL.DeleteVertexArray(_vao);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
        }
    }

}
