using CsvHelper;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using OpenTK.Graphics.GL;
using System.Drawing;
using System.Xml.Linq;
using CsvHelper.Configuration;
using static System.Windows.Forms.LinkLabel;

namespace HipparcosCatalog
{
    public class Catalog
    {
        private float _time; // Время, используемое для анимации
        public List<Star> AllStars { get; set; } = new List<Star>();
        public List<Star> VisibleStars { get; set; } = new List<Star>();
        List<Line> lines = new List<Line>();

        public List<Cluster> Clusters { get; set; } = new List<Cluster>();

        private int _vao;
        private int _vbo;
        private Shader _shader;
        private Shader _lineShader;
        private float[] _vertices;

        private int _lineVao, _lineVbo;
        private float[] _lineVertices;
        public Constellations Constellations { get; private set; } = new Constellations();

        float _pointSize = 1.0f;
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public bool DisplayStarNames { get; set; } = false;

        public bool DisplayAxis { get; set; } = true;
        public bool DisplayConstellationsNames { get; set; } = false;

        public bool DisplayStarHip { get; set; } = false;
        public bool DisplayProperNames { get; set; } = false;
        public bool DisplayGlieseNames { get; set; } = false;
        public bool DisplayBayerFlamsteedNames { get; set; } = false;
        public bool DisplayHDNames { get; set; } = false;

        AxisCircularRender axisCircularRender;
        AxisRender axisRender;

        uint starTextureHandle;
        OpenTK.Graphics.OpenGL.TextureTarget starTextureTarget;

        /// <summary>
        /// Дистанция от Солнца на которой видны звезды
        /// </summary>
        public float VisibleRadius { get; set; } = 10;

        /// <summary>
        /// Дистанция от камеры на которой видны подписи
        /// </summary>
        public float DistanceCameraLablesVisible { get; set; } = 10;

        public bool DisplayStarLines { get; set; } = false;

        public bool HighlightEnabled = false;
        public Vector3 HighlightCenter = new Vector3(0, 0, 0);
        public float HighlightRadius = 5;

        public Catalog()
        {
            axisRender = new AxisRender();
            axisCircularRender = new AxisCircularRender();
            InitializeShaderStars();            
        }

        public void Load()
        {
            axisRender.Axis.Add(new Axis(1000.0f, 0.5f, 1.0f, new Vector3(0.0f, 0.0f, 0.0f)));

            int rowCounter = 1;

            var filePath = "./Resources/maint_cat_new.csv"; // Путь к вашему файлу
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var star = new Star
                        {
                            Id = rowCounter,
                            HIP = csv.GetField<int?>("HIP"),
                            HD = csv.GetField<string?>("HD"),
                            HR = csv.GetField<int?>("HR"),
                            Gliese = csv.GetField<string?>("Gliese"),
                            BayerFlamsteed = csv.GetField<string?>("BayerFlamsteed"),
                            GaiaDR3 = csv.GetField<long?>("GaiaDR3"),
                            ProperName = csv.GetField<string?>("ProperName"),
                            RA = csv.GetField<double?>("RA"),
                            Dec = csv.GetField<double?>("Dec"),
                            Distance = csv.GetField<double?>("Distance"),
                            PMRA = csv.GetField<double?>("PMRA"),
                            PMDec = csv.GetField<double?>("PMDec"),
                            RV = csv.GetField<double?>("RV"),
                            Mag = csv.GetField<double?>("Mag"),
                            AbsMag = csv.GetField<double?>("AbsMag"),
                            Spectrum = csv.GetField<string?>("Spectrum"),
                            B_V = csv.GetField<double?>("B_V"),
                            Pos = new Vector3()
                            {
                                X = csv.GetField<float>("X"),
                                Y = csv.GetField<float>("Y"),
                                Z = csv.GetField<float>("Z")
                            },
                            VPos = new Vector3()
                            {
                                X = csv.GetField<float>("VX"),
                                Y = csv.GetField<float>("VY"),
                                Z = csv.GetField<float>("VZ")
                            },
                            Cluster = csv.GetField<int?>("Cluster"),
                            ClusterName = csv.GetField<string?>("ClusterName"),
                        };

                        star.ColorRGB = Star.GetColorFromSpectrum(star.Spectrum);

                        //if (star.ClusterName != "Hercules Cluster (M13)")
                        //    continue;


                        AllStars.Add(star);

                     

                        if(star.Cluster == 1)
                        {
                            var cluster = Clusters.SingleOrDefault(x => x.Name == star.ClusterName);
                            if (cluster == null)
                            {
                                cluster = new Cluster(star.ClusterName);
                                cluster.CalcBoundingBox(star.Pos);
                                cluster.StarsId.Add(star.Id);
                                Clusters.Add(cluster);
                            }
                            else
                            {
                                cluster.CalcBoundingBox(star.Pos);
                                cluster.StarsId.Add(star.Id);
                            }
                        }

                        rowCounter++;
                        //if (star.ProperName == "Aldebaran" || star.ProperName == "Proxima Centauri")
                        //    axisRender.Axis.Add(new Axis(5.0f, 0.0f, 0.0f, star.Pos));
                    }
                }

                axisRender.Generate();

                foreach (Cluster cluster in Clusters)
                {
                    cluster.BoundingBoxRenderer.CreateBoundingBox();
                }

                var textureParams = new TextureLoaderParameters();
                ImageGDI.LoadFromDisk("./Resources/1.jpg", textureParams, out starTextureHandle, out starTextureTarget);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }

        public List<Star> PrepareVertices()
        {
            VisibleStars.Clear();
            lines.Clear();
            List<float> lineVertices = new List<float>();
            var starWithName = new List<Star>();

            var vertices = new List<float>();
            foreach (var star in AllStars)
            {
                float distance = Vector3.Distance(star.Pos, new Vector3(0,0,0));
                if (distance < VisibleRadius) 
                {
                    VisibleStars.Add(star);
                    // Добавляем позицию, цвет и размер
                    vertices.Add(star.Pos.X);
                    vertices.Add(star.Pos.Y);
                    vertices.Add(star.Pos.Z);

                    vertices.Add(star.ColorRGB.X);
                    vertices.Add(star.ColorRGB.Y);
                    vertices.Add(star.ColorRGB.Z);

                    double absMag = star.AbsMag ?? 0;
                    vertices.Add((float)absMag);

                    
                    if (DisplayStarLines)
                    {
                        #region Рендеринг линий от звезды до плоскости проекции

                        if (!string.IsNullOrEmpty(star.ProperName))
                        {
                            Line line1 = new Line()
                            {
                                Start = star.Pos,
                                End = new Vector3(star.Pos.X, 0, star.Pos.Z), // Плоскость XZ (Y = 0)
                                Color = star.Pos.Y > 0 ? Color.FromArgb(143, 154, 255) : Color.FromArgb(255, 95, 91)// Определим цвет линии
                            };
                            Line line2 = new Line()
                            {
                                Start = line1.End,
                                End = new Vector3(0, 0, 0), // Плоскость XZ (Y = 0)
                                Color = line1.Color// Определим цвет линии
                            };

                            // Добавим данные для линии: координаты и цвет
                            lineVertices.Add(line1.Start.X);
                            lineVertices.Add(line1.Start.Y);
                            lineVertices.Add(line1.Start.Z);  // Начало
                            lineVertices.Add(line1.Color.R / 255f);
                            lineVertices.Add(line1.Color.G / 255f);
                            lineVertices.Add(line1.Color.B / 255f); // Цвет

                            lineVertices.Add(line1.End.X);
                            lineVertices.Add(line1.End.Y);
                            lineVertices.Add(line1.End.Z);  // Конец
                            lineVertices.Add(line1.Color.R / 255f);
                            lineVertices.Add(line1.Color.G / 255f);
                            lineVertices.Add(line1.Color.B / 255f); // Цвет


                            // Добавим данные для линии: координаты и цвет
                            lineVertices.Add(line2.Start.X);
                            lineVertices.Add(line2.Start.Y);
                            lineVertices.Add(line2.Start.Z);  // Начало
                            lineVertices.Add(line2.Color.R / 255f);
                            lineVertices.Add(line2.Color.G / 255f);
                            lineVertices.Add(line2.Color.B / 255f); // Цвет

                            lineVertices.Add(line2.End.X);
                            lineVertices.Add(line2.End.Y);
                            lineVertices.Add(line2.End.Z);  // Конец
                            lineVertices.Add(line2.Color.R / 255f);
                            lineVertices.Add(line2.Color.G / 255f);
                            lineVertices.Add(line2.Color.B / 255f); // Цвет
                        }
                        #endregion
                    }
                }

                if (!string.IsNullOrEmpty(star.ProperName))
                    starWithName.Add(star);
                else if (!string.IsNullOrEmpty(star.Gliese))
                    starWithName.Add(star);
                //else if (!string.IsNullOrEmpty(star.BayerFlamsteed))
                //    starWithName.Add(star);
                //else if (!string.IsNullOrEmpty(star.HD))
                //    starWithName.Add(star);


                if(!Constellations.IsComplete)
                    Constellations.FillStars(star);
            }

            if (!Constellations.IsComplete)
                Constellations.OrderByStars();

            _lineVertices = lineVertices.ToArray();
            _vertices = vertices.ToArray();

            axisCircularRender.GenerateCirclesAndLinesAndPlane(VisibleRadius, 1, new Vector3(0, 0, 0), 100, "XZ");

            return starWithName;
        }

        public void InitializeBuffers()
        {
            //  Точки
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            // Указание атрибутов: позиция, цвет и размер
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);               // Позиция
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float)); // Цвет
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 7 * sizeof(float), 6 * sizeof(float)); // Размер
            GL.EnableVertexAttribArray(2);

            Constellations.InitializeBuffersConstellations();

            // Создаём VAO и VBO для линий
            _lineVao = GL.GenVertexArray();
            _lineVbo = GL.GenBuffer();
            GL.BindVertexArray(_lineVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _lineVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _lineVertices.Length * sizeof(float), _lineVertices, BufferUsageHint.StaticDraw);

            // Указываем атрибуты: позиция и цвет
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);  // Позиция
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));  // Цвет
            GL.EnableVertexAttribArray(1);
        }

        private void InitializeShaderStars()
        {
            // Вершинный шейдер
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                layout(location = 1) in vec3 aColorRGB;
                layout(location = 2) in float aMag;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                uniform float time;

                uniform bool uHighlightEnabled;
                uniform vec3 uHighlightCenter;
                uniform float uHighlightRadius;

                out vec3 vColor;
                out float alpha;

                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);

                    vec4 mod = view * model * vec4(aPosition, 1.0);
                    float pulse = 1.0 + 0.01 * sin(time * 5.0);

                    float baseSize = 20.0;
                    float scale = 1.5;
                    float eyeDepth = -mod.z;

                    float brightness = 1.0;
                    float pointSize;

                    if (uHighlightEnabled && distance(aPosition, uHighlightCenter) <= uHighlightRadius) {
                        brightness = 1.5;
                        pointSize = baseSize * pulse;
                    } else {
                        brightness = 1.2;
                        float effectiveDepth = max(eyeDepth, 0.1);
                        pointSize = (0.01 + baseSize / effectiveDepth * scale) * pulse;
                    }

                    gl_PointSize = clamp(pointSize, 0.5, baseSize * 1.5);
                    vColor = aColorRGB * brightness;
                    alpha = brightness;
                }";

            // Фрагментный шейдер
            string fragmentShaderSource = @"
                #version 330 core
                in vec3 vColor;
                in float alpha;

                out vec4 FragColor;
                uniform sampler2D textureSampler;

                void main()
                {
                    vec4 textureColor = texture(textureSampler, gl_PointCoord);
                    FragColor = textureColor * vec4(vColor, alpha);
                }";

            _shader = new Shader(vertexShaderSource, fragmentShaderSource, null, ShaderSourceMode.Code);


            // Вершинный шейдер
            string vertexLineShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec3 aColorRGB;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            out vec3 vColor;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPosition, 1.0);
                vColor = aColorRGB;
            }";

            // Фрагментный Lineшейдер
            string fragmentLineShaderSource = @"
            #version 330 core
            in vec3 vColor;
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(vColor, 1.0);
            }";

             _lineShader = new Shader(vertexLineShaderSource, fragmentLineShaderSource, null, ShaderSourceMode.Code);
        }

        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model, Vector3 cameraPosition, TextRenderer textRenderer)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);

            _time = (float)DateTime.Now.TimeOfDay.TotalSeconds; // Обновляем время

            #region Textures

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, starTextureHandle);

            _shader.SetInt("textureSampler", 0);


            //_shader.SetFloat("pointSize", 1);

            #endregion Textures

            // Рендер звёзд
            _shader.Use();
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);
            _shader.SetFloat("time", _time); // Передаем время в шейдер

            _shader.SetInt("uHighlightEnabled", HighlightEnabled ? 1 : 0);
            _shader.SetVector3("uHighlightCenter", HighlightCenter);
            _shader.SetFloat("uHighlightRadius", HighlightRadius);

            //_shader.SetVector3("cameraPosition", cameraPosition);
            //_shader.SetFloat("time", (float)DateTime.Now.TimeOfDay.TotalSeconds);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Points, 0, VisibleStars.Count);


            if (DisplayConstellationsNames)
            {
                #region DisplayConstellationsNames
                foreach (var co in Constellations.ConstellationList)
                    foreach (var star in co.Stars)
                    {
                        // Проверяем видимость звезды
                        if (!IsPointInFrustum(star.Pos, model, view, projection))
                            continue; // Звезда не видна, пропускаем её

                        Vector2 screenCoords = TransformWorldToScreen(star.Pos, model, view, projection);

                        if (screenCoords.X < 0 || screenCoords.X > WindowWidth ||
                            screenCoords.Y < 0 || screenCoords.Y > WindowHeight)
                        {
                            Console.WriteLine("Координаты текста вне видимой области экрана.");
                            continue;
                        }

                        float distanceSol = Vector3.Distance(new Vector3(0, 0, 0), star.Pos);
                        distanceSol = distanceSol * 3.26156f;// перевод парсеков в световые года

                        string name = null;
                        Color textColor = Color.LightGray;
                        Color lineColor = Color.LightCyan;

                        if (DisplayProperNames && !string.IsNullOrEmpty(star.ProperName))
                            name = $"{star.ProperName} ({distanceSol})";
                        else if (DisplayStarHip && star.HIP.HasValue)
                            name = $"{star.HIP.Value} ({distanceSol})";
                        else if (DisplayGlieseNames && !string.IsNullOrEmpty(star.Gliese))
                            name = $"{star.Gliese} ({distanceSol})";
                        else if (DisplayGlieseNames && !string.IsNullOrEmpty(star.BayerFlamsteed))
                            name = $"{star.BayerFlamsteed} ({distanceSol})";
                        else if (DisplayHDNames && !string.IsNullOrEmpty(star.HD))
                            name = $"{star.HD} ({distanceSol})";


                        textRenderer.RenderTextWithLines(name, screenCoords.X, screenCoords.Y, 1, textColor, lineColor);
                        //textRenderer.RenderText(name, screenCoords.X, screenCoords.Y, 1, Color.Yellow);
                    }
                #endregion
            }
            else if (DisplayStarNames)
            {
                #region DisplayStarNames
                foreach (var star in VisibleStars)
                {
                    float distance = Vector3.Distance(cameraPosition, star.Pos);
                    if (distance < DistanceCameraLablesVisible)
                    {
                        // Проверяем видимость звезды
                        if (!IsPointInFrustum(star.Pos, model, view, projection))
                            continue; // Звезда не видна, пропускаем её

                        Vector2 screenCoords = TransformWorldToScreen(star.Pos, model, view, projection);

                        if (screenCoords.X < 0 || screenCoords.X > WindowWidth ||
                            screenCoords.Y < 0 || screenCoords.Y > WindowHeight)
                        {
                            Console.WriteLine("Координаты текста вне видимой области экрана.");
                            continue;
                        }

                        float distanceSol = Vector3.Distance(new Vector3(0, 0, 0), star.Pos);
                        distanceSol = distanceSol * 3.26156f;// перевод парсеков в световые года

                        string name = null;
                        Color textColor = Color.LightGray;
                        Color lineColor = Color.LightCyan;

                        if (DisplayProperNames && !string.IsNullOrEmpty(star.ProperName))
                            name = $"{star.ProperName} ({distanceSol})";
                        else if (DisplayStarHip && star.HIP.HasValue)
                            name = $"{star.HIP.Value} ({distanceSol})";
                        else if (DisplayGlieseNames && !string.IsNullOrEmpty(star.Gliese))
                            name = $"{star.Gliese} ({distanceSol})";
                        else if (DisplayGlieseNames && !string.IsNullOrEmpty(star.BayerFlamsteed))
                            name = $"{star.BayerFlamsteed} ({distanceSol})";
                        else if (DisplayHDNames && !string.IsNullOrEmpty(star.HD))
                            name = $"{star.HD} ({distanceSol})";


                        textRenderer.RenderTextWithLines(name, screenCoords.X, screenCoords.Y, 1, textColor, lineColor);
                        //textRenderer.RenderText(name, screenCoords.X, screenCoords.Y, 1, Color.Yellow);
                    }
                }
                #endregion
            }

            Constellations.Draw(view, projection, model, cameraPosition, textRenderer);

            if (DisplayAxis)
            {
                axisCircularRender.DrawCircle(view, projection, model);
            }


            GL.Disable(EnableCap.PointSprite);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);


            // Рендер линий (если они включены)
            if (DisplayStarLines)
            {
                _lineShader.Use();
                _lineShader.SetMatrix4("view", view);
                _lineShader.SetMatrix4("projection", projection);
                _lineShader.SetMatrix4("model", model);

                GL.BindVertexArray(_lineVao);
                GL.DrawArrays(PrimitiveType.Lines, 0, _lineVertices.Length / 6); // 6 — количество данных для каждой вершины (позиция + цвет)
            }

            if (DisplayAxis)
            {
                axisRender.DrawAxis(view, projection, model);
            }

            foreach(Cluster cluster in Clusters)
            {
                cluster.BoundingBoxRenderer.DrawBoundingBox(view, projection);
            }
        }

        private bool IsPointInFrustum(Vector3 worldPosition, Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            Matrix4 normalizedModel = model;
            normalizedModel.Row0.Normalize();
            normalizedModel.Row1.Normalize();
            normalizedModel.Row2.Normalize();

            Matrix4 normalizedView = view;
            normalizedView.Row0.Normalize();
            normalizedView.Row1.Normalize();
            normalizedView.Row2.Normalize();

            // Преобразуем мировую позицию в координаты устройства (NDC)
            //Vector4 clipSpacePosition = new Vector4(worldPosition, 1.0f) * normalizedModel;
            //clipSpacePosition = clipSpacePosition * normalizedView;
            //clipSpacePosition = clipSpacePosition * projection;
            // Преобразуем мировую позицию в Clip-Space
            Vector4 worldPos = new Vector4(worldPosition, 1.0f);
            Vector4 clipSpacePosition = worldPos * normalizedModel * normalizedView * projection;

            // Перевод в нормализованные координаты устройства (NDC)
            if (clipSpacePosition.W != 0.0f)
            {
                clipSpacePosition /= clipSpacePosition.W;
            }

            // Проверяем, находится ли точка внутри границ NDC (-1, 1)
            return clipSpacePosition.X >= -1.0f && clipSpacePosition.X <= 1.0f &&
                   clipSpacePosition.Y >= -1.0f && clipSpacePosition.Y <= 1.0f &&
                   clipSpacePosition.Z >= -1.0f && clipSpacePosition.Z <= 1.0f;
        }

        private Vector2 TransformWorldToScreen(Vector3 worldPosition, Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            // Применяем трансформации модели
            Vector4 transformedPosition = new Vector4(worldPosition, 1.0f) * model * view * projection;

            // Преобразуем в NDC (координаты устройства)
            if (transformedPosition.W != 0.0f)
            {
                transformedPosition /= transformedPosition.W;
            }

            // Перевод в экранные координаты
            float screenX = (transformedPosition.X * 0.5f + 0.5f) * WindowWidth;
            float screenY = (1.0f - (transformedPosition.Y * 0.5f + 0.5f)) * WindowHeight;

            return new Vector2(screenX, screenY);
        }

        /// <summary>
        /// Возвращает количество звёзд, попадающих в заданный радиус:
        /// </summary>
        /// <param name="highlightCenter"></param>
        /// <param name="highlightRadius"></param>
        /// <returns></returns>
        public int CountStarsInRadius(Vector3 highlightCenter, float highlightRadius)
        {
            int count = 0;
            float radiusSquared = highlightRadius * highlightRadius;

            foreach (var star in AllStars)
            {
                if (Vector3.DistanceSquared(star.Pos, highlightCenter) <= radiusSquared)
                    count++;
            }

            return count;
        }

        public void Dispose()
        {
            if (_vao != 0)
                GL.DeleteVertexArray(_vao);
            if (_vbo != 0)
                GL.DeleteBuffer(_vbo);
            //if (_vaoSphere != 0)
            //    GL.DeleteVertexArray(_vaoSphere);
            //if (_vboSphere != 0)
            //    GL.DeleteBuffer(_vboSphere);
        }

    }
}
