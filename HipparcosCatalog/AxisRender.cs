using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace HipparcosCatalog
{
    public class Axis
    {
        private const float LightYearToUnits = 0.306601f; // Коэффициент пересчета, 1 световой год = 0.306601 парсека

        /// <summary>
        /// Описыват оси XYZ
        /// </summary>
        /// <param name="length">Длина осей</param>
        /// <param name="tickSize">Длина тика</param>
        /// <param name="tickSpacing">Расстояние между тиками</param>
        /// <param name="position">Позиция в пространстве</param>
        /// <param name="convertToParsecs">Признак необходимости конвертации в парсеки</param>
        public Axis(float length, float tickSize, float tickSpacing, Vector3 position, bool convertToParsecs = true)
        {
            float factor = 1.0f;

            if(convertToParsecs)
                factor = LightYearToUnits;

            Length = length * factor;
            TickSize = tickSize * factor;
            TickSpacing = tickSpacing * factor;
            Position = position;

            XYPlaneColor = new Vector3(0.0f, 0.5f, 1.0f);
            XZPlaneColor = new Vector3(0.0f, 1.0f, 0.5f);
            YZPlaneColor = new Vector3(1.0f, 0.5f, 0.0f);
            PlaneAlpha = 0.3f;
        }

        /// <summary>
        /// Длина осей
        /// </summary>
        public float Length { get; }
        /// <summary>
        /// Длина тика
        /// </summary>
        public float TickSize { get; }
        /// <summary>
        /// Расстояние между тиками
        /// </summary>
        public float TickSpacing { get; }
        /// <summary>
        /// Позиция в пространстве
        /// </summary>
        public Vector3 Position { get; }
        public bool ShowXYPlane { get; set; } = false;
        public bool ShowXZPlane { get; set; } = false;
        public bool ShowYZPlane { get; set; } = false;

        // Индивидуальные настройки прозрачности и цвета
        public float PlaneAlpha { get; set; }
        public Vector3 XYPlaneColor { get; set; }
        public Vector3 XZPlaneColor { get; set; }
        public Vector3 YZPlaneColor { get; set; }
    }

    public class AxisRender
    {
        private int _vao;
        private int _vbo;
        private int _planeVao;
        private int _planeVbo;
        private Shader _shader;
        private Shader _planeShader;
        float[] axisVertices;
        float[] planeVertices;
        public List<Axis> Axis { get; set; } = new List<Axis>();

        private float _planeAlpha = 0.1f;

        public AxisRender()
        {
            // Создаем отдельные шейдеры для осей
            string axisVertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;

        out vec3 ourColor;

        void main()
        {
            gl_Position = projection * view * model * vec4(aPosition, 1.0);
            ourColor = aColor;
        }";

            string axisFragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        uniform float alpha;
        in vec3 ourColor;

        void main()
        {
            FragColor = vec4(ourColor, alpha);
        }";

            _shader = new Shader(axisVertexShaderSource, axisFragmentShaderSource, null, ShaderSourceMode.Code);

            // Создаем шейдер для плоскостей
            string planeVertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;

        out vec3 ourColor;

        void main()
        {
            gl_Position = projection * view * model * vec4(aPosition, 1.0);
            ourColor = aColor;
        }";

            string planeFragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        in vec3 ourColor;

        uniform float alpha;

        void main()
        {
            FragColor = vec4(ourColor, alpha);
        }
        ";

            _planeShader = new Shader(planeVertexShaderSource, planeFragmentShaderSource, null, ShaderSourceMode.Code);
        }

        public void Generate()
        {
            List<float> vertices = new List<float>();
            List<float> planes = new List<float>();

            foreach (Axis axis in Axis)
            {
                // Ось X
                vertices.AddRange(new float[] {
                            -axis.Length / 2 + axis.Position.X, axis.Position.Y, axis.Position.Z, 1.0f, 0.0f, 0.0f,
                             axis.Length / 2 + axis.Position.X, axis.Position.Y, axis.Position.Z, 1.0f, 0.0f, 0.0f
                        });

                // Ось Y
                vertices.AddRange(new float[] {
                            axis.Position.X, -axis.Length / 2 + axis.Position.Y, axis.Position.Z, 0.0f, 1.0f, 0.0f,
                            axis.Position.X,  axis.Length / 2 + axis.Position.Y, axis.Position.Z, 0.0f, 1.0f, 0.0f
                        });

                // Ось Z
                vertices.AddRange(new float[] {
                            axis.Position.X, axis.Position.Y, -axis.Length / 2 + axis.Position.Z, 0.0f, 0.0f, 1.0f,
                            axis.Position.X, axis.Position.Y,  axis.Length / 2 + axis.Position.Z, 0.0f, 0.0f, 1.0f
                        });

                if (axis.TickSize > 0 && axis.TickSpacing > 0)
                {
                    // Тики по оси X
                    for (float i = -axis.Length / 2; i <= axis.Length / 2; i += axis.TickSpacing)
                    {
                        vertices.AddRange(new float[] {
                                i + axis.Position.X, axis.Position.Y - axis.TickSize / 2, axis.Position.Z, 1.0f, 0.0f, 0.0f,
                                i + axis.Position.X, axis.Position.Y + axis.TickSize / 2, axis.Position.Z, 1.0f, 0.0f, 0.0f
                            });
                    }

                    // Тики по оси Y
                    for (float i = -axis.Length / 2; i <= axis.Length / 2; i += axis.TickSpacing)
                    {
                        vertices.AddRange(new float[] {
                                axis.Position.X - axis.TickSize / 2, i + axis.Position.Y, axis.Position.Z, 0.0f, 1.0f, 0.0f,
                                axis.Position.X + axis.TickSize / 2, i + axis.Position.Y, axis.Position.Z, 0.0f, 1.0f, 0.0f
                            });
                    }

                    // Тики по оси Z
                    for (float i = -axis.Length / 2; i <= axis.Length / 2; i += axis.TickSpacing)
                    {
                        vertices.AddRange(new float[] {
                                axis.Position.X - axis.TickSize / 2, axis.Position.Y, i + axis.Position.Z, 0.0f, 0.0f, 1.0f,
                                axis.Position.X + axis.TickSize / 2, axis.Position.Y, i + axis.Position.Z, 0.0f, 0.0f, 1.0f
                            });
                    }
                }


                // Добавление плоскостей
                if (axis.ShowXYPlane)
                {
                    planes.AddRange(new float[] {
                    -axis.Length / 2 + axis.Position.X, -axis.Length / 2 + axis.Position.Y, axis.Position.Z, axis.XYPlaneColor.X, axis.XYPlaneColor.Y, axis.XYPlaneColor.Z,
                    axis.Length / 2 + axis.Position.X, -axis.Length / 2 + axis.Position.Y, axis.Position.Z, axis.XYPlaneColor.X, axis.XYPlaneColor.Y, axis.XYPlaneColor.Z,
                    axis.Length / 2 + axis.Position.X, axis.Length / 2 + axis.Position.Y, axis.Position.Z, axis.XYPlaneColor.X, axis.XYPlaneColor.Y, axis.XYPlaneColor.Z,

                    -axis.Length / 2 + axis.Position.X, -axis.Length / 2 + axis.Position.Y, axis.Position.Z, axis.XYPlaneColor.X, axis.XYPlaneColor.Y, axis.XYPlaneColor.Z,
                    axis.Length / 2 + axis.Position.X, axis.Length / 2 + axis.Position.Y, axis.Position.Z, axis.XYPlaneColor.X, axis.XYPlaneColor.Y, axis.XYPlaneColor.Z,
                    -axis.Length / 2 + axis.Position.X, axis.Length / 2 + axis.Position.Y, axis.Position.Z, axis.XYPlaneColor.X, axis.XYPlaneColor.Y, axis.XYPlaneColor.Z,
                });
                }

                if (axis.ShowXZPlane)
                {
                    planes.AddRange(new float[] {
                    -axis.Length / 2 + axis.Position.X, axis.Position.Y, -axis.Length / 2 + axis.Position.Z, axis.XZPlaneColor.X, axis.XZPlaneColor.Y, axis.XZPlaneColor.Z,
                    axis.Length / 2 + axis.Position.X, axis.Position.Y, -axis.Length / 2 + axis.Position.Z, axis.XZPlaneColor.X, axis.XZPlaneColor.Y, axis.XZPlaneColor.Z,
                    axis.Length / 2 + axis.Position.X, axis.Position.Y, axis.Length / 2 + axis.Position.Z, axis.XZPlaneColor.X, axis.XZPlaneColor.Y, axis.XZPlaneColor.Z,

                    -axis.Length / 2 + axis.Position.X, axis.Position.Y, -axis.Length / 2 + axis.Position.Z, axis.XZPlaneColor.X, axis.XZPlaneColor.Y, axis.XZPlaneColor.Z,
                    axis.Length / 2 + axis.Position.X, axis.Position.Y, axis.Length / 2 + axis.Position.Z, axis.XZPlaneColor.X, axis.XZPlaneColor.Y, axis.XZPlaneColor.Z,
                    -axis.Length / 2 + axis.Position.X, axis.Position.Y, axis.Length / 2 + axis.Position.Z, axis.XZPlaneColor.X, axis.XZPlaneColor.Y, axis.XZPlaneColor.Z,
                });
                }

                if (axis.ShowYZPlane)
                {
                    planes.AddRange(new float[] {
                    axis.Position.X, -axis.Length / 2 + axis.Position.Y, -axis.Length / 2 + axis.Position.Z, axis.YZPlaneColor.X, axis.YZPlaneColor.Y, axis.YZPlaneColor.Z,
                    axis.Position.X, axis.Length / 2 + axis.Position.Y, -axis.Length / 2 + axis.Position.Z, axis.YZPlaneColor.X, axis.YZPlaneColor.Y, axis.YZPlaneColor.Z,
                    axis.Position.X, axis.Length / 2 + axis.Position.Y, axis.Length / 2 + axis.Position.Z, axis.YZPlaneColor.X, axis.YZPlaneColor.Y, axis.YZPlaneColor.Z,

                    axis.Position.X, -axis.Length / 2 + axis.Position.Y, -axis.Length / 2 + axis.Position.Z, axis.YZPlaneColor.X, axis.YZPlaneColor.Y, axis.YZPlaneColor.Z,
                    axis.Position.X, axis.Length / 2 + axis.Position.Y, axis.Length / 2 + axis.Position.Z, axis.YZPlaneColor.X, axis.YZPlaneColor.Y, axis.YZPlaneColor.Z,
                    axis.Position.X, -axis.Length / 2 + axis.Position.Y, axis.Length / 2 + axis.Position.Z, axis.YZPlaneColor.X, axis.YZPlaneColor.Y, axis.YZPlaneColor.Z,
                });
                }
            }

            // Генерация данных для осей
            axisVertices = vertices.ToArray();
            planeVertices = planes.ToArray();

            // Создание VAO и VBO для осей
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, axisVertices.Length * sizeof(float), axisVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Создание VAO и VBO для плоскостей
            _planeVao = GL.GenVertexArray();
            _planeVbo = GL.GenBuffer();

            GL.BindVertexArray(_planeVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _planeVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, planeVertices.Length * sizeof(float), planeVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public void DrawAxis(Matrix4 view, Matrix4 projection, Matrix4 model)
        {
            // Рисуем оси
            _shader.Use();
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Lines, 0, axisVertices.Length / 6);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);



            // Отключение теста глубины для рендеринга прозрачных объектов
            GL.Disable(EnableCap.DepthTest);

            //// Рисуем плоскости
            //if (ShowXYPlane || ShowXZPlane || ShowYZPlane)
            //{
                _planeShader.Use();
                _planeShader.SetMatrix4("view", view);
                _planeShader.SetMatrix4("projection", projection);
                _planeShader.SetMatrix4("model", model);

                // Установка цвета плоскости с учетом прозрачности
                _planeShader.SetFloat("alpha", 0.1f);

                GL.BindVertexArray(_planeVao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, planeVertices.Length / 6);
                //GL.BindVertexArray(0);
            //}

            // Включение теста глубины обратно
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);

        }

        public void SetPlaneTransparency(float alpha)
        {
            _planeAlpha = Math.Clamp(alpha, 0.0f, 1.0f); // Ограничиваем значение от 0 до 1
        }
        public void Dispose()
        {
            if (_vao != 0) GL.DeleteVertexArray(_vao);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
            if (_planeVao != 0) GL.DeleteVertexArray(_planeVao);
            if (_planeVbo != 0) GL.DeleteBuffer(_planeVbo);
        }
    }
}
