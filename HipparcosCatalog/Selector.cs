using Microsoft.VisualBasic.Devices;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HipparcosCatalog
{
    public class Selector
    {
        int vao, vbo;
        private Shader _shader;
        public TextRenderer TextRenderer { get; set; }
        float[] vertexData;

        bool _pinSelection = false;

        bool isOutsideTop = false;
        bool isOutsideRight = false;

        float squareSize = 30.0f;
        float rectWidth = 150.0f;
        float rectHeight = 300.0f;
        float rectOffsetX;
        float rectOffsetY;
        
        private void GenerateVertices(Vector2 screenPosition, Vector2 screenSize)
        {
            // Координаты квадрата
            float squareTop = screenPosition.Y - squareSize / 2;
            float squareBottom = screenPosition.Y + squareSize / 2;
            float squareLeft = screenPosition.X - squareSize / 2;
            float squareRight = screenPosition.X + squareSize / 2;

            // Проверка верхней и нижней границы экрана
            if (squareTop - rectHeight < 0) // Верхняя граница экрана
            {
                rectOffsetY = squareSize / 2 + 10.0f;
                isOutsideTop = true;
            }
            else if (squareBottom + rectHeight > screenSize.Y) // Нижняя граница экрана
            {
                rectOffsetY = -squareSize / 2 - rectHeight - 10.0f;
                isOutsideTop = false;
            }
            else
            {
                rectOffsetY = -squareSize / 2 - rectHeight - 10.0f;
                isOutsideTop = false;
            }

            // Проверка выхода за правую границу экрана
            if (squareRight + rectWidth > screenSize.X) // Пересечение правой границы
            {
                rectOffsetX = -rectWidth - (squareSize / 2 + 10.0f);
                isOutsideRight = true;
            }
            else
            {
                rectOffsetX = squareSize / 2 + 10.0f;
                isOutsideRight = false;
            }

            Vector3 squareColor = new Vector3(0.717f, 0.717f, 0.717f);
            if(_pinSelection)
                squareColor = new Vector3(0.0f, 1.0f, 0.0f);
            // Вершины квадрата
            float[] squareVertices = new float[]
            {
                -squareSize / 2, -squareSize / 2, squareColor.X, squareColor.Y, squareColor.Z, 1.0f,
                 squareSize / 2, -squareSize / 2, squareColor.X, squareColor.Y, squareColor.Z, 1.0f,
                 squareSize / 2,  squareSize / 2, squareColor.X, squareColor.Y, squareColor.Z, 1.0f,
                -squareSize / 2,  squareSize / 2, squareColor.X, squareColor.Y, squareColor.Z, 1.0f
            };

            // Вершины прямоугольника
            float[] rectVertices = new float[]
            {
                rectOffsetX, rectOffsetY, 0.717f, 0.717f, 0.717f, 0.5f,
                rectOffsetX + rectWidth, rectOffsetY, 0.717f, 0.717f, 0.717f, 0.1f,
                rectOffsetX + rectWidth, rectOffsetY + rectHeight, 0.717f, 0.717f, 0.717f, 0.5f,
                rectOffsetX, rectOffsetY + rectHeight, 0.717f, 0.717f, 0.717f, 0.1f
            };

            // Обработка линий в зависимости от положения прямоугольника
            // Вершины линий
            float[] lineVertices;
            if (isOutsideTop && isOutsideRight)
            {
                // Прямоугольник ниже и левее квадрата
                lineVertices = new float[]
                {
                    // Линия 1
                    squareVertices[0], squareVertices[1], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[0], rectVertices[1], 0.717f, 0.717f, 0.717f, 0.0f,

                    // Линия 2
                    squareVertices[12], squareVertices[13], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[12], rectVertices[13], 0.717f, 0.717f, 0.717f, 0.0f
                };
            }
            else if (!isOutsideTop && !isOutsideRight)
            {
                // Прямоугольник выше и правее квадрата
                lineVertices = new float[]
                {
                    // Линия 1
                    squareVertices[0], squareVertices[1], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[0], rectVertices[1], 0.717f, 0.717f, 0.717f, 0.0f,

                    // Линия 2
                    squareVertices[12], squareVertices[13], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[12], rectVertices[13], 0.717f, 0.717f, 0.717f, 0.0f
                };
            }
            else if (isOutsideTop && !isOutsideRight)
            {
                // Прямоугольник ниже и правее квадрата
                lineVertices = new float[]
                {
                    // Линия 1
                    squareVertices[6], squareVertices[7], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[6], rectVertices[7], 0.717f, 0.717f, 0.717f, 0.0f,

                    // Линия 2
                    squareVertices[18], squareVertices[19], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[18], rectVertices[19], 0.717f, 0.717f, 0.717f, 0.0f
                };
            }
            else if (!isOutsideTop && isOutsideRight)
            {
                // Прямоугольник выше и левее квадрата
                lineVertices = new float[]
                {
                    // Линия 1
                    squareVertices[6], squareVertices[7], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[6], rectVertices[7], 0.717f, 0.717f, 0.717f, 0.0f,

                    // Линия 2
                    squareVertices[18], squareVertices[19], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[18], rectVertices[19], 0.717f, 0.717f, 0.717f, 0.0f
                };
            }
            else
            {
                // Прямоугольник выше и правее квадрата
                lineVertices = new float[]
                {
                    // Линия 1
                    squareVertices[0], squareVertices[1], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[0], rectVertices[1], 0.717f, 0.717f, 0.717f, 0.0f,

                    // Линия 2
                    squareVertices[12], squareVertices[13], 0.717f, 0.717f, 0.717f, 1.0f,
                    rectVertices[12], rectVertices[13], 0.717f, 0.717f, 0.717f, 0.0f
                };
            }

            // Объединяем все вершины
            vertexData = new float[squareVertices.Length + rectVertices.Length + lineVertices.Length];
            Array.Copy(squareVertices, 0, vertexData, 0, squareVertices.Length);
            Array.Copy(rectVertices, 0, vertexData, squareVertices.Length, rectVertices.Length);
            Array.Copy(lineVertices, 0, vertexData, squareVertices.Length + rectVertices.Length, lineVertices.Length);
        }

        public Selector()
        {
            // Вершины квадрата, прямоугольника и линий в относительных единицах
            //GenerateVertices();

            // Создаем VAO и VBO
          

            string vertexShaderSource = @"
            #version 330 core

            layout(location = 0) in vec2 aPos; // Position in local coordinates
            layout(location = 1) in vec4 aColor; // Alpha channel
            uniform vec2 screenPosition; // Position of the square on the screen
            uniform vec2 screenSize; // Screen size

            out vec4 vColor; // Pass the alpha channel to the fragment shader

            void main()
            {
                // Convert coordinates to NDC
                vec2 ndcPosition = (screenPosition + aPos) / screenSize * 2.0 - 1.0;

                // Invert Y for OpenGL
                ndcPosition.y = -ndcPosition.y;

                // Pass the position and alpha channel
                gl_Position = vec4(ndcPosition, 0.0, 1.0);
                vColor = aColor;
            }";

            string fragmentShaderSource = @"
            #version 330 core

            in vec4 vColor; // Get alpha channel from vertex shader
            out vec4 FragColor;

            void main()
            {
                FragColor = vColor; // Use alpha channel for transparency
            }";

            _shader = new Shader(vertexShaderSource, fragmentShaderSource, null, ShaderSourceMode.Code);
        }

        public void InitBuffer()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            // Загружаем данные вершин в VBO
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0); // Позиция
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float)); // Цвет (RGB) + альфа
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }
        public void RenderStarBracket(string text, Vector2 screenPosition, Vector2 screenSize, bool pinSelection)
        {
            _pinSelection = pinSelection;
            GenerateVertices(screenPosition, screenSize);
            InitBuffer();
            // Используем шейдер
            _shader.Use();

            // Передача униформ
            _shader.SetVector2("screenPosition", screenPosition);
            _shader.SetVector2("screenSize", screenSize);

            GL.BindVertexArray(vao);

            // Рисуем квадрат
            GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);

            // Рисуем прямоугольник
            GL.DrawArrays(PrimitiveType.LineLoop, 4, 4);

            // Рисуем линии
            GL.DrawArrays(PrimitiveType.Lines, 8, 4);

            GL.BindVertexArray(0);

            float rectLeftTopX = screenPosition.X + vertexData[36];
            float rectLeftTopY = screenPosition.Y + rectOffsetY;// vertexData[37];

            // Координаты для текста
            //float textOffsetX = 30.0f; // Отступ вправо
            //float textOffsetY = 50.0f; // Отступ вниз

            // Вычисляем координаты прямоугольника относительно мышки
            float rectLeft = screenPosition.X;             // Левый край прямоугольника
            float rectTop = screenPosition.Y - rectHeight + squareSize; // Верхний край прямоугольника

            //float textScreenX;
            //float textScreenY;

            //if (isOutsideRight)
            //    textScreenX = screenPosition.X - rectWidth - squareSize/2;//   прямоугольник слева от квадрата
            //else
            //    textScreenX = screenPosition.X + rectOffsetX + 10;//   прямоугольник справа от квадрата

            //if (isOutsideTop)               
            //    textScreenY = screenSize.Y - screenPosition.Y - squareSize;//   прямоугольник снизу от квадрата
            //else
            //    textScreenY = screenSize.Y - screenPosition.Y + rectHeight + 10;//   прямоугольник сверху от квадрата
            // Координаты для текста
            // Вычисляем координаты для текста (левый верхний угол прямоугольника)
            float textScreenX = screenPosition.X + rectOffsetX + 10; // Левый край прямоугольника
            float textScreenY = screenSize.Y - (screenPosition.Y + rectOffsetY) - 25; // Верхний край прямоугольника


            TextRenderer.RenderText(text, textScreenX, textScreenY, 1, Color.FromArgb(183,183,183), new RectangleF(0,0, rectWidth, rectHeight));
        }

        public static Vector3 CalculateRayFromMouse(Vector2 mousePosition, Camera camera, Matrix4 projectionMatrix, Vector2 screenSize)
        {
            // 1. Преобразуем координаты мыши из экранных в нормализованные устройства (NDC)
            float ndcX = (2.0f * mousePosition.X) / screenSize.X - 1.0f;
            float ndcY = 1.0f - (2.0f * mousePosition.Y) / screenSize.Y; // Инверсия Y, т.к. экранные координаты Y идут сверху вниз

            // 2. Создаём вектор в clip space
            Vector4 clipCoords = new Vector4(ndcX, ndcY, -1.0f, 1.0f); // Z = -1 для ближней плоскости

            // 3. Преобразуем из clip space в eye space, используя обратную матрицу проекции
            Matrix4 inverseProjection;
            Matrix4.Invert(projectionMatrix, out inverseProjection);


            Vector4 eyeCoords = clipCoords.Multiply(inverseProjection);
            eyeCoords = new Vector4(eyeCoords.X, eyeCoords.Y, -1.0f, 0.0f); // Z = -1, W = 0 для направления

            // 4. Преобразуем из eye space в world space, используя обратную матрицу вида
            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 inverseView;
            Matrix4.Invert(viewMatrix, out inverseView);


            Vector4 worldCoords = eyeCoords.Multiply(inverseView);

            // 5. Получаем направление луча в мировом пространстве
            Vector3 rayDirection = new Vector3(worldCoords.X, worldCoords.Y, worldCoords.Z);
            rayDirection.Normalize(); // Нормализуем направление

            return rayDirection;
        }

        // Метод для выбора звезды
        public static Star SelectStar(Vector3 rayOrigin, Vector3 rayDirection, Matrix4 modelMatrix, List<Star> stars)
        {
            Star nearestStar = null;
            float nearestDistance = float.MaxValue;

            foreach (Star star in stars)
            {
                // Преобразуем позицию звезды из локальных в мировые координаты через матрицу модели
                Vector4 localPosition = new Vector4(star.Pos, 1.0f);
                Vector4 worldPosition4 = localPosition.Multiply(modelMatrix);
                Vector3 worldPosition = new Vector3(worldPosition4.X, worldPosition4.Y, worldPosition4.Z);

                // Рассчитываем масштаб объекта
                Vector3 scale;
                Quaternion rotation;
                Vector3 translation;
                DecomposeModelMatrix(modelMatrix, out scale, out rotation, out translation);

                // Масштабируем радиус звезды
                float worldRadius = star.Radius * Math.Max(scale.X, Math.Max(scale.Y, scale.Z));

                // Проверяем пересечение луча со сферой, представляющей звезду
                if (RayIntersectsSphere(rayOrigin, rayDirection, worldPosition, worldRadius))
                {
                    float distance = (worldPosition - rayOrigin).LengthSquared;
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestStar = star;
                    }
                }
            }

            return nearestStar;
        }

        public static void DecomposeModelMatrix(Matrix4 model, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            // Используем System.Numerics.Matrix4x4 для декомпозиции
            var mat = new System.Numerics.Matrix4x4(
                model.M11, model.M12, model.M13, model.M14,
                model.M21, model.M22, model.M23, model.M24,
                model.M31, model.M32, model.M33, model.M34,
                model.M41, model.M42, model.M43, model.M44
            );

            System.Numerics.Matrix4x4.Decompose(mat, out System.Numerics.Vector3 sysScale, out System.Numerics.Quaternion sysRotation, out System.Numerics.Vector3 sysTranslation);
            scale = new Vector3(sysScale.X, sysScale.Y, sysScale.Z);
            rotation = new Quaternion(sysRotation.X, sysRotation.Y, sysRotation.Z, sysRotation.W);
            translation = new Vector3(sysTranslation.X, sysTranslation.Y, sysTranslation.Z);
        }

        // Метод для проверки пересечения луча с сферой
        private static bool RayIntersectsSphere(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius)
        {
            Vector3 oc = rayOrigin - sphereCenter;
            float a = Vector3.Dot(rayDirection, rayDirection);
            float b = 2.0f * Vector3.Dot(oc, rayDirection);
            float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;

            float discriminant = b * b - 4 * a * c;
            return discriminant > 0;
        }
    }

    public static class Vector4Extensions
    {
        // Ручное умножение матрицы на вектор
        public static Vector4 Multiply(this Vector4 vector, Matrix4 matrix)
        {
            return new Vector4(
                vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31 + vector.W * matrix.M41,
                vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32 + vector.W * matrix.M42,
                vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33 + vector.W * matrix.M43,
                vector.X * matrix.M14 + vector.Y * matrix.M24 + vector.Z * matrix.M34 + vector.W * matrix.M44
            );
        }
    }
}
