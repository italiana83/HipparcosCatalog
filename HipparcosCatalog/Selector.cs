using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class Selector
    {
        int vao, vbo;
        private Shader _shader;

        float[] bracketVertices = {
            // Верхняя левая скобка
            -1.0f,  1.0f,  -0.8f,  1.0f,  // Горизонтальная линия
            -1.0f,  1.0f,  -1.0f,  0.8f,  // Вертикальная линия

            // Верхняя правая скобка
             0.8f,  1.0f,   1.0f,  1.0f,  // Горизонтальная линия
             1.0f,  1.0f,   1.0f,  0.8f,  // Вертикальная линия

            // Нижняя левая скобка
            -1.0f, -0.8f,  -1.0f, -1.0f,  // Вертикальная линия
            -1.0f, -1.0f,  -0.8f, -1.0f,  // Горизонтальная линия

            // Нижняя правая скобка
             1.0f, -0.8f,   1.0f, -1.0f,  // Вертикальная линия
             0.8f, -1.0f,   1.0f, -1.0f   // Горизонтальная линия
        };


        public Selector() 
        {
            // Генерация буферов
            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);

            // Заполнение данных
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, bracketVertices.Length * sizeof(float), bracketVertices, BufferUsageHint.StaticDraw);

            // Настройка атрибутов шейдера
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Отвязка
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            string vertexShaderSource = @"
            #version 330 core

            layout(location = 0) in vec2 aPos; // Line position in local coordinates
            uniform vec2 screenPosition;       // Screen coordinates of the star's center
            uniform vec2 screenSize;           // Screen size
            uniform float lineSize;            // Line size in pixels

            void main()
            {
                // Scale the position relative to the center
                vec2 scaledPos = aPos * lineSize;

                // Convert to normalized device coordinates (NDC)
                vec2 ndcPosition = (screenPosition + scaledPos) / screenSize * 2.0 - 1.0;

                // Flip the Y-axis for OpenGL
                ndcPosition.y = -ndcPosition.y;

                // Set the final position
                gl_Position = vec4(ndcPosition, 0.0, 1.0);
            }";

            string fragmentShaderSource = @"
                #version 330 core

                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(0.0, 1.0, 0.0, 1.0); // Green color
                }";

            string geometryShaderSource = @"
                #version 330 core

                layout(lines) in;
                layout(triangle_strip, max_vertices = 4) out;

                uniform float lineThickness;

                void main()
                {
                    vec2 direction = normalize(vec2(gl_in[1].gl_Position.xy - gl_in[0].gl_Position.xy));
    
                    vec2 perpendicular = vec2(-direction.y, direction.x) * lineThickness;

                    gl_Position = gl_in[0].gl_Position + vec4(perpendicular, 0.0, 0.0);
                    EmitVertex();
                    gl_Position = gl_in[0].gl_Position - vec4(perpendicular, 0.0, 0.0);
                    EmitVertex();
                    gl_Position = gl_in[1].gl_Position + vec4(perpendicular, 0.0, 0.0);
                    EmitVertex();
                    gl_Position = gl_in[1].gl_Position - vec4(perpendicular, 0.0, 0.0);
                    EmitVertex();

                    EndPrimitive();
                }";

            _shader = new Shader(vertexShaderSource, fragmentShaderSource, geometryShaderSource, ShaderSourceMode.Code);
        }

        public void RenderStarBracket(Vector2 screenPosition, Vector2 screenSize, float lineSize)
        {
            // Используем шейдер
            _shader.Use();

            // Передача униформ
            _shader.SetVector2("screenPosition", screenPosition);
            _shader.SetVector2("screenSize", screenSize);
            _shader.SetFloat("lineSize", lineSize);
            _shader.SetFloat("lineThickness", 0.002f); // Установите желаемую толщину

            // Отрисовка линий
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Lines, 0, 16); // 16 вершин для 8 линий
            GL.BindVertexArray(0);
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

        //public static Star SelectStar(Vector3 rayOrigin, Vector3 rayDirection, List<Star> stars)
        //{
        //    Star nearestStar = null;
        //    float nearestDistance = float.MaxValue;

        //    foreach (Star star in stars)
        //    {
        //        // Преобразуем позицию звезды через её матрицу модели
        //        Vector4 localPosition = new Vector4(star.Pos, 1.0f);
        //        Vector4 worldPosition4 = localPosition.Multiply(star.ModelMatrix);
        //        Vector3 worldPosition = new Vector3(worldPosition4.X, worldPosition4.Y, worldPosition4.Z);

        //        // Рассчитываем радиус в мировых координатах (если масштабирование присутствует)
        //        // Предполагаем, что матрица модели содержит только масштабирование, вращение и трансляцию
        //        // Если в матрице модели присутствует искажение, это нужно учитывать отдельно
        //        Vector3 scale;
        //        Quaternion rotation;
        //        Vector3 translation;
        //        DecomposeModelMatrix(star.ModelMatrix, out scale, out rotation, out translation);
        //        float worldRadius = star.Radius * Math.Max(scale.X, Math.Max(scale.Y, scale.Z));

        //        if (RayIntersectsSphere(rayOrigin, rayDirection, worldPosition, worldRadius))
        //        {
        //            float distance = (worldPosition - rayOrigin).LengthSquared;
        //            if (distance < nearestDistance)
        //            {
        //                nearestDistance = distance;
        //                nearestStar = star;
        //            }
        //        }
        //    }

        //    return nearestStar;
        //}

        // Метод для декомпозиции матрицы модели на масштаб, вращение и трансляцию
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
