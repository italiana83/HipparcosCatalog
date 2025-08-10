using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HipparcosCatalog
{
    public class BoundingBoxRenderer: BoundingBox
    {
        private int _vao;
        private int _vbo;
        private Shader _shader;
        private Color4 _color;

        public BoundingBoxRenderer(Color4 color) 
        {
            _color = color;
        }

        public BoundingBoxRenderer(Vector3 min, Vector3 max, Color4 color) :
            base(min, max)
        {
            _color = color;
        }

        public void CreateBoundingBox()
        {


            // Вершины параллелепипеда
            float[] vertices = {
            // Нижняя грань
            Min.X, Min.Y, Min.Z, Max.X, Min.Y, Min.Z,
            Max.X, Min.Y, Min.Z, Max.X, Max.Y, Min.Z,
            Max.X, Max.Y, Min.Z, Min.X, Max.Y, Min.Z,
            Min.X, Max.Y, Min.Z, Min.X, Min.Y, Min.Z,
            
            // Верхняя грань
            Min.X, Min.Y, Max.Z, Max.X, Min.Y, Max.Z,
            Max.X, Min.Y, Max.Z, Max.X, Max.Y, Max.Z,
            Max.X, Max.Y, Max.Z, Min.X, Max.Y, Max.Z,
            Min.X, Max.Y, Max.Z, Min.X, Min.Y, Max.Z,
            
            // Связь верхней и нижней граней
            Min.X, Min.Y, Min.Z, Min.X, Min.Y, Max.Z,
            Max.X, Min.Y, Min.Z, Max.X, Min.Y, Max.Z,
            Max.X, Max.Y, Min.Z, Max.X, Max.Y, Max.Z,
            Min.X, Max.Y, Min.Z, Min.X, Max.Y, Max.Z
        };

            // Генерация VAO/VBO
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Настройка атрибутов вершин
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;

                uniform mat4 view;
                uniform mat4 projection;

                void main()
                {
                    gl_Position = projection * view * vec4(aPos, 1.0);
                }";

            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(0.0, 0.7, 1.0, 0.5);
                }";

            _shader = new Shader(vertexShaderSource, fragmentShaderSource, null, ShaderSourceMode.Code);

            //_shader.SetVector4("color", (Vector4)_color);

        }

        public void DrawBoundingBox(Matrix4 view, Matrix4 projection)
        {
            _shader.Use();

            // Передаем матрицы в шейдер
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Lines, 0, 24); // 12 линий (24 вершины)
            GL.BindVertexArray(0);

        }

        public void Dispose()
        {
            if (_vao != 0) GL.DeleteVertexArray(_vao);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
        }
    }

}
