using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class Constellations
    {
        private int lineVao, lineVbo, lineEbo; // Буферы для линий
        private int pointVao, pointVbo;       // Буферы для точек (звёзд)
        private int totalIndices; // Общее количество индексов для всех созвездий
        private int totalStars;               // Общее количество звёзд
        private Shader _lineShader;
        private Shader _starShader;

        uint starTextureHandle;
        OpenTK.Graphics.OpenGL.TextureTarget starTextureTarget;

        public bool IsComplete { get; set; } = false;
        public List<Constellation> ConstellationList { get; private set; } = new List<Constellation>();

        public Constellations()
        {
            GenerateConstellations();
            InitializeShaderConstellations();

            var textureParams = new TextureLoaderParameters();
            ImageGDI.LoadFromDisk("./Resources/1.jpg", textureParams, out starTextureHandle, out starTextureTarget);

        }

        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model, Vector3 cameraPosition, TextRenderer textRenderer)
        {
            _lineShader.Use();
            _lineShader.SetMatrix4("view", view);
            _lineShader.SetMatrix4("projection", projection);
            _lineShader.SetMatrix4("model", model);

            // Устанавливаем ширину линии
            GL.LineWidth(2.0f);
            GL.BindVertexArray(lineVao);
            GL.DrawElements(PrimitiveType.Lines, totalIndices, DrawElementsType.UnsignedInt, 0);

            //GL.Disable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);

            #region Textures

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, starTextureHandle);

            //_shader.SetFloat("pointSize", 1);

            #endregion Textures
            _starShader.Use();
            _starShader.SetMatrix4("view", view);
            _starShader.SetMatrix4("projection", projection);
            _starShader.SetMatrix4("model", model);
            _starShader.SetInt("textureSampler", 0);


            // Рисуем точки (звёзды)
            GL.PointSize(5.0f);
            GL.BindVertexArray(pointVao);
            GL.DrawArrays(PrimitiveType.Points, 0, totalStars);

            GL.BindVertexArray(0);
        }

        public void InitializeBuffersConstellations()
        {
            var allVertices = new List<float>();
            var allIndices = new List<uint>();
            var allStarVertices = new List<float>();
            int indexOffset = 0;

            foreach (var constellation in ConstellationList)
            {
                if (constellation.Visible)
                {
                    foreach (var star in constellation.Stars)
                    {
                        // Добавляем позицию и цвет для линий
                        allVertices.Add(star.Pos.X);
                        allVertices.Add(star.Pos.Y);
                        allVertices.Add(star.Pos.Z);

                        allVertices.Add(constellation.ColorRGB.X);
                        allVertices.Add(constellation.ColorRGB.Y);
                        allVertices.Add(constellation.ColorRGB.Z);

                        // Добавляем позицию и цвет звезды для точек
                        allStarVertices.Add(star.Pos.X);
                        allStarVertices.Add(star.Pos.Y);
                        allStarVertices.Add(star.Pos.Z);

                        allStarVertices.Add(star.ColorRGB.X);
                        allStarVertices.Add(star.ColorRGB.Y);
                        allStarVertices.Add(star.ColorRGB.Z);
                    }

                    var indices = constellation.Connections
                        .SelectMany(c => new[] { (uint)(c.Item1 + indexOffset), (uint)(c.Item2 + indexOffset) })
                        .ToList();

                    allIndices.AddRange(indices);
                    indexOffset += constellation.Stars.Count;
                }
            }

            // Данные для линий
            float[] vertexData = allVertices.ToArray();
            uint[] indexData = allIndices.ToArray();
            totalIndices = indexData.Length;

            // Данные для точек
            float[] starVertexData = allStarVertices.ToArray();
            totalStars = allStarVertices.Count / 6;

            // Инициализация буферов линий
            lineVao = GL.GenVertexArray();
            lineVbo = GL.GenBuffer();
            lineEbo = GL.GenBuffer();

            GL.BindVertexArray(lineVao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, lineVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

            // Указываем атрибут позиции (3 компонента)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Указываем атрибут цвета (3 компонента)
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, lineEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexData.Length * sizeof(uint), indexData, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            // Инициализация буферов точек
            pointVao = GL.GenVertexArray();
            pointVbo = GL.GenBuffer();

            GL.BindVertexArray(pointVao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, pointVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, starVertexData.Length * sizeof(float), starVertexData, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);

            IsComplete = true;
        }

        private void InitializeShaderConstellations()
        {
            string lineVertexShaderSource = @"
            #version 330 core

            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec3 aColor;

            out vec3 fragColor;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                fragColor = aColor;
                gl_Position = projection * view * model * vec4(aPosition, 1.0);
            }";

            string lineFragmentShaderSource = @"
            #version 330 core

            in vec3 fragColor;
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(fragColor, 1.0);
            }";

            _lineShader = new Shader(lineVertexShaderSource, lineFragmentShaderSource, null, ShaderSourceMode.Code);

            string starVertexShaderSource = @"
            #version 330 core

            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec3 aColor;

            out vec3 fragColor;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                fragColor = aColor;
                gl_Position = projection * view * model * vec4(aPosition, 1.0);

                gl_PointSize = 40;
            }";

            string starFragmentShaderSource = @"
            #version 330 core

            in vec3 fragColor;
            out vec4 FragColor;

            uniform sampler2D textureSampler;

            void main()
            {
                vec4 textureColor = texture(textureSampler, gl_PointCoord);
                FragColor = textureColor * vec4(fragColor, 1.0); 
            }";

            _starShader = new Shader(starVertexShaderSource, starFragmentShaderSource, null, ShaderSourceMode.Code);
        }

        public void FillStars(Star star)
        {
            foreach (var constellation in ConstellationList)
            {
                if (star.HIP.HasValue && constellation.Hips.ContainsKey(star.HIP.Value))
                    constellation.Stars.Add(star);
            }
        }

        private void GenerateConstellations()
        {
            #region Андромеда
            //Constellation constellation = new Constellation("Андромеда");

            //677,   // Alpheratz (shared with Pegasus)
            //3092,  // Delta Andromedae
            //5447,  // Mu Andromedae
            //9640,  // Nu Andromedae
            //116631, // Beta Andromedae (Mirach)
            //544,   // Pi Andromedae
            //3881,  // Gamma Andromedae (Almach)
            //3086,  // Iota Andromedae
            //116805, // Upsilon Andromedae
            //4436,  // Lambda Andromedae
            //116584, // Kappa Andromedae
            //116637, // Theta Andromedae
            //constellation.Hips = new Dictionary<int, string>
            //{
            //    {677, "Альферац {α Andromedae}"},
            //    {116631, "Мирах {β Andromedae}"},
            //    {9640, "Аламак {γ Andromedae}"},
            //    {10765, "Дельта Андромеды {δ Andromedae}"},
            //    {9280, "51 Андромеды"},
            //    {84012, "Эйпсилон Андромеды {ε Andromedae}"},
            //    {7513, "Уpsilon Andromedae"},
            //    {11652, "Пи Андромеды {π Andromedae}"},
            //    {14862, "Тета Андромеды {θ Andromedae}"},
            //    {9153, "Кси Андромеды {ξ Andromedae}"},
            //    {98298, "Ню Андромеды {ν Andromedae}"},
            //    {14228, "Омега Андромеды {ω Andromedae}"},
            //    {10294, "Ро Андромеды {ρ Andromedae}"},
            //    {10395, "Тау Андромеды {τ Andromedae}"},
            //    {16735, "Зета Андромеды {ζ Andromedae}"},
            //    {5434, "Фи Андромеды {φ Andromedae}"},
            //    {4463, "Эта Андромеды {η Andromedae}"},
            //    {116584, "Лямбда Андромеды {λ Andromedae}"},
            //    {113726, "Омикрон Андромеды {ο Andromedae}"},
            //    {4436, "Мю Андромеды {μ Andromedae}"}
            //};

            //constellation.Connections = new List<(int, int)>
            //{
            //    (0, 3), // Альферацем (α Andromedae) -> Дельтой Андромеды (δ Andromedae)
            //    (3, 5), // Дельтой Андромеды (δ Andromedae) -> Эйпсилоном Андромеды (ε Andromedae)
            //    (5, 14), // Эйпсилоном Андромеды (ε Andromedae) -> Зета Андромеды (ζ Andromedae)
            //    (14, 16), // Зета Андромеды (ζ Andromedae) -> Эта Андромеды (ŋ Andromedae)
            //    (3, 7), // Дельтой Андромеды (δ Andromedae) -> Пи Андромедой (π Andromedae)
            //    (7, 8), // Пи Андромедой (π Andromedae) -> Тетой Андромедой (θ Andromedae)
            //    (7, 17), // Тетой Андромедой (θ Andromedae) -> Лямбда Андромеды (λ Andromedae)
            //    (17, 18), // Лямбда Андромеды (λ Andromedae) -> "Омикрон Андромеды (ο Andromedae)
            //    (3, 1), // Дельтой Андромеды (δ Andromedae) -> Мирахом (β Andromedae)
            //    (1, 2), // Мирахом (β Andromedae) -> Аламаком (γ Andromedae)
            //    (1, 18), // Мирахом (β Andromedae) -> Мю Андромеды (μ Andromedae)
            //    (18, 10), //  Мю Андромеды (μ Andromedae) -> Ню Андромедой (ν Andromedae)
            //    (18, 15), //  Ню Андромедой (ν Andromedae) -> Фи Андромеды (φ Andromedae)
            //    (15, 4), //  Фи Андромеды (φ Andromedae) -> 51 Андромеды


            //};

            //ConstellationList.Add(constellation);
            #endregion

            #region Большая Медведица
            Constellation constellation = new Constellation("Большая Медведица", new Vector3(0.9f, 0.4f, 0.6f));

            constellation.Hips = new Dictionary<int, string>
            {
                {54061, "Дубхе {α UMa}"},
                {53910, "Мерак {β UMa}"},
                {58001, "Фекда {γ UMa}"},
                {59774, "Мегрец {δ UMa}"},
                {62956, "Алиот {ε UMa}"},
                {65378, "Мицар {ζ UMa}"},
                {67301, "Алькаид {ι UMa}/Бенетнаш {η UMa}"}
            };

            constellation.Connections = new List<(int, int)>
            {
                (6, 5), // Бенетнаш - Мицар
                (5, 4), // Мицар - Алиот
                (4, 3), // Алиот - Мегрец
                (3, 0), // Мегрец - Дубхе
                (0, 1), // Дубхе - Мерак
                (1, 2), // Мерак - Фекда
                (2, 3), // Фекда - Мегрец
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Малая Медведица
            constellation = new Constellation("Малая Медведица", new Vector3(0.4f, 0.8f, 0.8f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 11767, "Полярная звезда (α UMi) - жёлтый сверхгигант, двойная звезда, самая яркая звезда Малой Медведицы, расстояние ~323 световых года." },
                { 72607, "Кохаб (β UMi) - оранжевый гигант, вторая по яркости звезда, расстояние ~126 световых лет." },
                { 75097, "Феркад (γ UMi) - оранжевый гигант, расстояние ~487 световых лет." },
                { 85822, "δ UMi - белая звезда главной последовательности, расстояние ~172 световых года." },
                { 82080, "ε UMi - бело-голубая звезда, расстояние ~347 световых лет." },
                { 77055, "ζ UMi - белая звезда главной последовательности, расстояние ~361 световых лет." },
                { 79822, "η UMi - оранжевый гигант, расстояние ~97 световых лет." },
                { 76008, "θ UMi - белая звезда главной последовательности, расстояние ~860 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 3), // Полярная звезда - δ UMi
                (3, 4), // δ UMi - ε UMi
                (4, 5), // ε UMi - ζ UMi
                (5, 1),  // ζ UMi - Кохаб
                (1, 2), // Кохаб - Феркад
                (2, 6), // Феркад - η UMi
                (6, 5) // η UMi - ζ UMi
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Волосы Вероники
            constellation = new Constellation("Волосы Вероники", new Vector3(0.6f, 0.6f, 1.0f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 64241, "α Comae Berenices (Диадема) - двойная звезда, белые субгиганты, расстояние ~63 световых года." },
                { 64394, "β Comae Berenices - жёлтый карлик, похожий на Солнце, расстояние ~29.9 световых лет." },
                { 60742, "γ Comae Berenices - оранжевый гигант, расстояние ~170 световых лет." }
                //{ 64241, "4 Comae Berenices - бело-голубая звезда главной последовательности, расстояние ~320 световых лет." },
                //{ 63121, "37 Comae Berenices - жёлтый гигант, расстояние ~169 световых лет." },
                //{ 64394, "FK Comae Berenices - переменная звезда, вращающаяся с высокой скоростью, расстояние ~800 световых лет." },
                //{ 61658, "19 Comae Berenices - бело-голубая звезда главной последовательности, расстояние ~150 световых лет." },
                //{ 60746, "α² Comae Berenices - компонент системы Диадема, белая звезда, расстояние ~63 световых года." },
                //{ 63031, "31 Comae Berenices - оранжевый гигант, расстояние ~300 световых лет." },
                //{ 62325, "23 Comae Berenices - белая звезда главной последовательности, расстояние ~310 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 1), // α Comae Berenices - β Comae Berenices
                (1, 2)  // β Comae Berenices - γ Comae Berenices
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Цефей
            constellation = new Constellation("Цефей", new Vector3(1.0f, 1.0f, 0.2f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 105199, "Альдермин (α Cep) - бело-голубая звезда главной последовательности, расстояние ~49 световых лет." },
                { 106032, "Альфирк (β Cep) - бело-голубая звезда, пульсирующий переменный звёздный гигант, расстояние ~690 световых лет." },
                { 116727, "Гарнас (γ Cep) - оранжевый гигант, двойная звезда, расстояние ~45 световых лет." },
                { 110991, "δ Cep - жёлтый сверхгигант, классическая цефеида (переменная звезда), расстояние ~890 световых лет." },
                { 109857, "ε Cep - белая звезда главной последовательности, расстояние ~85 световых лет." },
                { 109492, "ζ Cep - красный гигант, расстояние ~730 световых лет." },
                { 102422, "η Cep - оранжевый гигант, расстояние ~46 световых лет." },
                { 101093, "θ Cep - белая звезда главной последовательности, расстояние ~139 световых лет." },
                { 112724, "ι Cep - бело-голубая звезда, расстояние ~1150 световых лет." },
                { 99255, "κ Cep - бело-голубая звезда, расстояние ~510 световых лет." },
                { 109556, "λ Cep - бело-голубая звезда главной последовательности, расстояние ~1120 световых лет." },
                { 107259, "μ Cep (Гранатовая звезда) - красный сверхгигант, одна из крупнейших известных звёзд, расстояние ~2850 световых лет." },
                { 107418, "ν Cep - бело-голубая звезда, расстояние ~630 световых лет." },
                { 115088, "ο Cep - жёлтый гигант, расстояние ~48 световых лет." },
                { 114222, "π Cep - бело-голубая звезда, расстояние ~326 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (6, 0), // η Cep - Альдермин
                (0, 5),  // Альдермин - ζ Cep
                (5, 3), // ζ Cep - δ Cep
                (3, 8), // δ Cep - ι Cep
                (8, 2), // ι Cep - Гарнас
                (2, 1), // Гарнас - Альфирк
                (1, 0) // Альфирк - Альдермин
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Персей
            constellation = new Constellation("Персей", new Vector3(1.0f, 0.6f, 1.0f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 15863, "Мирфак (α Per) - жёлтый сверхгигант, самая яркая звезда созвездия, входит в рассеянное скопление α Персея, расстояние ~510 световых лет." },
                { 14576, "Алголь (β Per) - знаменитая затменная двойная звезда, также известная как Демонская звезда, расстояние ~93 световых года." },
                { 14328, "γ Per - жёлтый гигант, расстояние ~243 световых года." },
                { 17358, "δ Per - бело-голубая звезда главной последовательности, расстояние ~520 световых лет." },
                { 18532, "ε Per - бело-голубая звезда главной последовательности, расстояние ~640 световых лет." },
                { 18246, "ζ Per - бело-голубая звезда главной последовательности, расстояние ~750 световых лет." },
                { 13268, "η Per - бело-голубая звезда, расстояние ~700 световых лет." },
                { 12777, "θ Per - бело-голубая звезда главной последовательности, расстояние ~37 световых лет." },
               
                //{ 16922, "κ Per - красный гигант, расстояние ~112 световых лет." },
                //{ 19587, "λ Per - бело-голубая звезда главной последовательности, расстояние ~530 световых лет." },
                //{ 16228, "μ Per - бело-голубая звезда главной последовательности, расстояние ~660 световых лет." },
                //{ 18532, "ν Per - бело-голубая звезда главной последовательности, расстояние ~550 световых лет." },
                //{ 17440, "ο Per - бело-голубая звезда, расстояние ~1000 световых лет." },
                //{ 114222, "π Per - бело-голубая звезда главной последовательности, расстояние ~300 световых лет." },
                //{ 17358, "ξ Per - бело-голубая звезда главной последовательности, расстояние ~1000 световых лет." },
                //{ 18907, "29 Per - бело-голубая звезда, расстояние ~730 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (2, 0), // γ Per - Мирфак
                (0, 1),  // Мирфак - Алголь
                (0, 3), // Мирфак - δ Per
                (3, 4), // δ Per - ε Per
                (4, 5) // ε Per - ζ Per
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Гончие Псы
            constellation = new Constellation("Гончие Псы", new Vector3(0.6f, 1.0f, 0.8f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 63121, "Сердце Карла (α CVn) - бело-жёлтый карлик спектрального класса F, одна из самых ярких звёзд в созвездии, расстояние ~110 световых лет." },
                { 61317, "β CVn - жёлтый карлик, похожий на Солнце, расстояние ~27 световых лет." }
               
                //{ 59504, "γ CVn - бело-голубая звезда главной последовательности, расстояние ~319 световых лет." },
                //{ 60965, "4 CVn - переменная звезда типа δ Щита, расстояние ~243 световых года." },
                //{ 63176, "5 CVn - белая звезда главной последовательности, расстояние ~264 световых года." },
                //{ 60646, "6 CVn - оранжевый гигант, расстояние ~513 световых лет." },
                //{ 61395, "ζ CVn - бело-жёлтый карлик, расстояние ~123 световых года." },
                //{ 59947, "12 CVn - жёлтый гигант, расстояние ~291 световых год." },
                //{ 60514, "13 CVn - бело-голубая звезда главной последовательности, расстояние ~316 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 1) // γ Per - Мирфак
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Тукан
            constellation = new Constellation("Тукан", new Vector3(0.4f, 0.8f, 0.4f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 110130, "α Tuc - оранжевый гигант, самая яркая звезда в созвездии, расстояние ~199 световых лет." },              
                { 2487, "β Tuc - бело-голубая звезда главной последовательности, расстояние ~140 световых лет." },
                { 114996, "γ Tuc - бело-голубая звезда главной последовательности, расстояние ~75 световых лет." },
                { 110838, "δ Tuc - двойная звезда, состоящая из бело-голубых звёзд, расстояние ~275 световых лет." },
                //{ 114341, "ε Tuc - белая звезда главной последовательности, расстояние ~370 световых лет." },
                //{ 114078, "ζ Tuc - белая звезда главной последовательности, расстояние ~220 световых лет." },
                //{ 112917, "η Tuc - бело-голубая звезда главной последовательности, расстояние ~46 световых лет." },
                //{ 112934, "θ Tuc - белая звезда главной последовательности, расстояние ~150 световых лет." },
                //{ 116853, "ι Tuc - бело-голубая звезда, расстояние ~500 световых лет." },
                //{ 112935, "κ Tuc - белая звезда главной последовательности, расстояние ~260 световых лет." },
                //{ 112374, "λ Tuc - жёлтый гигант, расстояние ~190 световых лет." },
                //{ 109268, "μ Tuc - оранжевый гигант, расстояние ~115 световых лет." }        
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 2), // γ Per - Мирфак
                (2, 1) // γ Per - Мирфак
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Кассиопея
            constellation = new Constellation("Кассиопея", new Vector3(1.0f, 0.4f, 0.4f));

            constellation.Hips = new Dictionary<int, string>
            {
                {3179, "Шедар (α Cas) — Ярчайшая звезда созвездия, оранжевый сверхгигант."},
                {746, "Каф (β Cas) — Вторая по яркости звезда в созвездии, бело-голубой гигант."},
                {4427, "Нави (γ Cas) — Переменная звезда типа Гаммы Кассиопеи, известный источник рентгеновского излучения." },
                {6686, "Рукбах (δ Cas) — Голубой гигант, часть астеризма W." },
                {8886, "Сегин (ε Cas) — Двойная звезда, состоит из оранжевого гиганта и белого карлика" }
                //HIP некорректные
                //{8102, "ζ Cas — Горячая белая звезда главной последовательности." },
                //{9874, "η Cas — Широко известная двойная звезда, одна из первых двойных систем, открытых Уильямом Гершелем." },
                //{10826, "θ Cas — Белая звезда главной последовательности, имеет слабое магнитное поле." },
                //{11767, "κ Cas — Переменная звезда типа Цефеида, меняет свою яркость каждые 2 дня." },
                //{11848, "ρ Cas — Гиперггант, известен своими мощными вспышками, увеличивающими его яркость до нескольких миллионов раз." },
                //{12169, "σ Cas — Двойная звезда, главная компонента — белый карлик." },
                //{12446, "τ Cas — Желтый гигант, примерно в 500 световых годах от Солнца." },
                //{12896, "υ² Cas — Переменная звезда типа RR Лиры, меняет свою яркость каждые 0,43 дня." }

            };

            constellation.Connections = new List<(int, int)>
            {
                (1, 0), // Каф - Шедар
                (0, 2), // Шедар - Нави
                (2, 3), // Нави - Рукбах
                (3, 4) // Рукбах - Сегин
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Лира
            constellation = new Constellation("Лира", new Vector3(0.4f, 0.6f, 1.0f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 91262, "Вега (α Lyr) — Самая яркая звезда в созвездии Лиры и пятая по яркости звезда на ночном небе. "},
                { 92420, "Шелиак (β Lyr) — Яркий представитель класса переменных звёзд типа β Лиры. "},
                { 93194, "Сульафат (γ Lyr) — Горячая белая звезда главной последовательности. "},
                { 91971, "Дзета Лиры (ζ Lyr) — Двойная звезда, состоящая из белого гиганта и белого карлика. "},
                { 91926, "Эпсилон Лиры (ε Lyr) — Знаменитая кратная звезда, известная как \"Двойная-двойная\". "},
                { 94481, "Эта Лиры (η Lyr) — Переменная звезда типа Цефеиды. "},
                { 94713, "Тета Лиры (θ Lyr) — Белый гигант, находящийся примерно в 779 световых годах от Солнца. "},
                { 92791, "Дельта² Лиры (δ² Lyr) — Оранжевый гигант, являющийся частью кратной звёздной системы. "},
                { 92862, "Р Лиры (R Lyr) — Долгопериодическая переменная звезда типа Мириды. "},
                { 93903, "Йота Лиры (ι Lyr) — Голубой гигант, входящий в состав рассеянного скопления IC 4756. "}
                //HIP некорректные
                //{ 91926, "Каппа Лиры (κ Lyr) — Белая звезда главной последовательности, находящаяся на расстоянии около 238 световых лет. "},
                //{ 91971, "Лямбда Лиры (λ Lyr) — Белая звезда главной последовательности, расположенная примерно в 1539 световых годах от Земли. "},
                //{ 92420, "Мю Лыры (μ Lyr) — Переменная звезда типа Алголя. "},
                //{ 93194, "Ню Лиры (ν Lyr) — Голубая звезда главной последовательности, находящаяся на расстоянии около 617 световых лет. "},
                //{ 92405, "Ксипси Лиры (ξ Lyr) — Кратная звезда, состоящая из трёх компонентов. "},
                //{ 93174, "Омега Лиры (ω Lyr) — Белая звезда главной последовательности, удалённая примерно на 1940 световых лет. "},
                //{ 93148, "Пи Лиры (π Lyr) — Белый сверхгигант, находящийся на расстоянии около 1100 световых лет. "},
                //{ 93163, "Ро Лиры (ρ Lyr) — Белая звезда главной последовательности, расположенная примерно в 401 световом году от Земли. "},
                //{ 93194, "Сигма Лиры (σ Lyr) — Голубая звезда главной последовательности, находящаяся на расстоянии около 898 световых лет. "},
                //{ 93174, "Тау Лиры (τ Lyr) — Переменная звезда типа δ Щита. "},
                //{ 93148, "Фи Лиры (φ Lyr) — Белый карлик, расположенный примерно в 215 световых годах от Солнца. "},
                //{ 93163, "Хи Лиры (χ Lyr) — Белая звезда главной последовательности, находящаяся на расстоянии около 836 световых лет. "},
                //{ 93194, "Пси Лиры (ψ Lyr) — Белая звезда главной последовательности, удалённая примерно на 1250 световых лет. "},
                //{ 93174, "Омега¹ Лиры (ω¹ Lyr) — Белая звезда главной последовательности, находящаяся на расстоянии около 1730 световых лет. "},
                //{ 93174, "Омега² Лиры (ω² Lyr) — Белая звезда главной последовательности, удалённая примерно на 1950 световых лет. "},
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 3), // Вега - Дзета Лиры
                (3, 1), // Дзета Лиры - Шелиак
                (1, 2), // Шелиак - Сульафат
                (2, 7), // Сульафат - Дельта² Лиры
                (7, 3)  // Дельта² Лиры - Дельта² Лиры
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Телец
            constellation = new Constellation("Телец", new Vector3(1.0f, 0.4f, 0.0f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 21421, "Альдебаран (α Tau) - красный гигант, самая яркая звезда Тельца, расстояние ~65 световых лет." },
                { 25428, "Эльнат (β Tau) - бело-голубой гигант, граница созвездий Телец и Возничий, расстояние ~131 световой год." },
                { 26451, "Tianguan (ζ Tau) - бело-голубая звезда главной последовательности, затменная двойная звезда, расстояние ~417 световых лет." },
                { 20885, "θ¹ Tau - жёлтый гигант, входит в рассеянное скопление Гиады, расстояние ~153 световых года." },
                { 20894, "Chamukuy (θ² Tau) - бело-голубая звезда главной последовательности, также часть Гиад, расстояние ~155 световых лет." },
                { 20205, "Hyadum I (γ Tau) - жёлтый гигант, также входит в рассеянное скопление Гиады, расстояние ~154 световых года." },
                { 20455, "Hyadum II (δ Tau) - жёлтый гигант в рассеянном скоплении Гиады, расстояние ~147 световых лет." },
                { 20889, "Ain (ε Tau) - оранжевый гигант, также из Гиад, расстояние ~147 световых лет." },
                { 18724, "λ Tau - бело-голубая затменная двойная звезда, расстояние ~370 световых лет." },
                { 15900, "ο Tau - двойная звёздная система. Она имеет жёлтый оттенок, расстояние ~270 световых лет." },
                { 17702, "Alcyone (η Tau) - самая яркая, бело-голубая звезда в Плеядах, расстояние ~440 световых лет." }
                //{ 25428, "27 Tau (Atlas) - бело-голубая звезда в Плеядах, расстояние ~440 световых лет." },
                //{ 25539, "28 Tau (Electra) - бело-голубая звезда в Плеядах, расстояние ~440 световых лет." },
                //{ 24845, "23 Tau (Merope) - бело-голубая звезда в Плеядах, расстояние ~440 световых лет." },
                //{ 25041, "24 Tau (Taygeta) - бело-голубая звезда в Плеядах, расстояние ~440 световых лет." },
                //{ 25499, "25 Tau (Maia) - бело-голубая звезда в Плеядах, расстояние ~440 световых лет." },
                //{ 25292, "26 Tau (Celaeno) - бело-голубая звезда в Плеядах, расстояние ~440 световых лет." },
                //{ 25695, "Pleione (28 Tau) - бело-голубая переменная звезда в Плеядах, расстояние ~440 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (2, 0), // Tianguan - Альдебаран
                (0, 4), // Альдебаран - Chamukuy
                (4, 5), // Chamukuy - Hyadum I
                (5, 6), // Hyadum I - Hyadum II
                (6, 7), // Hyadum II - Ain
                (7, 1), // Ain - Эльнат
                (5, 8), // Hyadum I - λ Tau
                (8, 9), // λ Tau - ο Tau
                (7, 10) // Ain - Alcyone
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Лебедь
            constellation = new Constellation("Лебедь", new Vector3(1.0f, 0.4f, 0.4f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 102098, "Денеб (α Cyg) - белый сверхгигант, одна из ярчайших звёзд неба, расстояние ~2600 световых лет." },
                { 95947, "Альбирео (β Cyg) - двойная звезда, золотисто-голубая пара, расстояние ~430 световых лет." },
                { 100453, "Садр (γ Cyg) - жёлтый яркий гигант, расстояние ~1800 световых лет." },
                { 97165, "δ Cyg - бело-голубая звезда главной последовательности, расстояние ~165 световых лет." },
                { 102488, "Дженах (ε Cyg) - оранжевый гигант, расстояние ~73 световых года." },
                { 104732, "ζ Cyg - жёлтый гигант, расстояние ~153 световых лет." },
                { 98110, "η Cyg - бело-голубая звезда главной последовательности, расстояние ~140 световых лет." },
                { 96441, "θ Cyg - белая звезда главной последовательности, расстояние ~59 световых лет." },
                { 95853, "ι² Cyg - бело-голубая звезда главной последовательности, расстояние ~140 световых лет." },
                { 94779, "κ Cyg - красный гигант, расстояние ~124 световых года." },
                { 102589, "λ Cyg - бело-голубая звезда, расстояние ~1100 световых лет." },
                { 107310, "μ Cyg - бело-голубая звезда главной последовательности, расстояние ~73 световых года." },
                { 104060, "ξ Cyg - жёлтый гигант, расстояние ~245 световых лет." },
                { 103413, "ν Cyg - бело-голубая звезда, расстояние ~280 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 2), // Денеб - Садр

                (2, 3), // Садр - δ Cyg
                (3, 8), // δ Cyg - ι² Cyg
                (8, 9),  // ι² Cyg - κ Cyg

                (2, 4), // Садр - Дженах
                (4, 5), // Дженах - ζ Cyg
                (5, 11), // ζ Cyg - μ Cyg

                (2, 6), // Садр - η Cyg
                (6, 1) // η Cyg - Альбирео
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Орел
            constellation = new Constellation("Орел", new Vector3(0.4f, 1.0f, 0.8f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 97649, "Альтаир (α Aql) - ярчайшая звезда созвездия Орла, бело-голубой звёздный гигант, расстояние ~16.7 световых лет." },
                { 97278, "Таразед (γ Aql) - оранжевый гигант, третья по яркости звезда в созвездии, расстояние ~460 световых лет." },
                { 98036, "Альшаин (β Aql) - жёлтый субгигант, двойная звезда, расстояние ~44.7 световых лет." },
                { 93747, "ζ Aql - бело-голубая звезда главной последовательности, расстояние ~83 световых лет." },
                { 95501, "δ Aql - бело-голубая звезда главной последовательности, расстояние ~50 световых лет." },
                { 97804, "η Aql - жёлтый сверхгигант, переменная звезда типа δ Цефея, расстояние ~1200 световых лет." },
                { 99473, "θ Aql - бело-голубая звезда, двойная система, расстояние ~288 световых лет." },
                { 93805, "λ Aql - жёлтый гигант, расстояние ~125 световых лет." },
                { 93717, "15 Aql - белая звезда главной последовательности, расстояние ~320 световых лет." },
                { 97473, "π Aql - бело-голубая звезда главной последовательности, расстояние ~620 световых лет." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (1, 0), // Таразед - Альтаир
                (0, 2), // Альтаир - Альшаин
                (2, 6), // Альшаин - θ Aql
                (6, 5),  // θ Aql - η Aql
                (5, 4), // η Aql - δ Aql
                (4, 3), // δ Aql - ζ Aql
                (3, 0), // ζ Aql - Альтаир
                (4, 7) // δ Aql - λ Aql
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Близнецы
            //constellation = new Constellation("Близнецы", new Vector3(0.2f, 0.6f, 1.0f));

            //constellation.Hips = new Dictionary<int, string>
            //{
            //    { 37826, "Кастор (α Gem) - система из шести звёзд, состоит из трёх двойных систем, расстояние ~51 световой год." },
            //    { 36850, "Поллукс (β Gem) - оранжевый гигант, самая яркая звезда в созвездии, расстояние ~34 световых года." },
            //    { 34088, "γ Gem (Альхена) - белый субгигант, расстояние ~105 световых лет." },
            //    { 31681, "δ Gem (Васат) - жёлтый субгигант, расстояние ~59 световых лет." },
            //    { 30343, "ε Gem (Мебсута) - жёлтый сверхгигант, расстояние ~840 световых лет." },
            //    { 28910, "ζ Gem - классическая цефеида, переменная звезда, расстояние ~1200 световых лет." },
            //    { 34693, "η Gem (Теуатастра) - красный гигант, двойная система, расстояние ~350 световых лет." },
            //    { 37819, "θ Gem - белая звезда главной последовательности, расстояние ~190 световых лет." },
            //    { 36942, "ι Gem - белая звезда главной последовательности, расстояние ~120 световых лет." },
            //    { 32246, "κ Gem - жёлтый гигант, расстояние ~140 световых лет." },
            //    { 31685, "λ Gem - бело-голубая звезда главной последовательности, расстояние ~94 световых года." },
            //    { 34670, "μ Gem (Теят) - красный гигант, расстояние ~230 световых лет." },
            //    { 29434, "ν Gem - бело-голубая звезда главной последовательности, расстояние ~450 световых лет." },
            //    { 29655, "ξ Gem - бело-голубая звезда, расстояние ~55 световых лет." },
            //    { 34909, "ο Gem - белая звезда главной последовательности, расстояние ~180 световых лет." }       
            //};

            //constellation.Connections = new List<(int, int)>
            //{
            //    (0, 3), // Вега - Дзета Лиры
            //    (3, 1), // Дзета Лиры - Шелиак
            //    (1, 2), // Шелиак - Сульафат
            //    (2, 7), // Сульафат - Дельта² Лиры
            //    (7, 3)  // Дельта² Лиры - Дельта² Лиры
            //};

            //ConstellationList.Add(constellation);
            #endregion

            #region Возничий
            //constellation = new Constellation("Возничий", new Vector3(0.8f, 1.0f, 0.6f));

            //constellation.Hips = new Dictionary<int, string>
            //{
            //    { 15207, "Капелла (α Aur)" },      // Капелла
            //    { 16846, "Менкалинан (β Aur)" },   // Менкалинан
            //    { 19037, "Эпсилон Возничего (ε Aur)" }, // Эпсилон Возничего
            //    { 20149, "Альфа Возничего (α Aur B)" }, // Альфа Возничего B
            //    { 17499, "Иота Возничего (ι Aur)" },  // Иота Возничего
            //    { 19373, "Тета Возничего (θ Aur)" },  // Тета Возничего
            //    { 20210, "Эта Возничего (η Aur)" },   // Эта Возничего
            //    { 21190, "Зета Возничего (ζ Aur)" },  // Зета Возничего
            //    { 21278, "Дельта Возничего (δ Aur)" },// Дельта Возничего
            //    { 23453, "Ню Возничего (ν Aur)" },    // Ню Возничего
            //    { 23302, "Кси Возничего (ξ Aur)" },   // Кси Возничего
            //    { 24608, "Пи Возничего (π Aur)" },    // Пи Возничего
            //    { 25500, "Каппа Возничего (κ Aur)" }, // Каппа Возничего
            //    { 27989, "Лямбда Возничего (λ Aur)" },// Лямбда Возничего
            //    { 30057, "Му Возничего (μ Aur)" },    // Му Возничего
            //    { 32196, "Сигма Возничего (σ Aur)" }, // Сигма Возничего
            //    { 34658, "Тау Возничего (τ Aur)" },   // Тау Возничего
            //    { 35575, "Упсилон Возничего (υ Aur)" }// Упсилон Возничего
            //};

            //constellation.Connections = new List<(int, int)>
            //{
            //    (0, 1), // Capella - Menkalinan
            //    (1, 2), // Menkalinan - Mahasim
            //    (0, 3), // Capella - Theta Aur
            //    (0, 4)  // Capella - Iota Aur
            //};

            //ConstellationList.Add(constellation);
            #endregion

            #region Волопас
            //constellation = new Constellation("Boötes", new Vector3(1.0f, 0.6f, 0.2f));
            //constellation.Hips = new List<(int, int)>
            //{
            //    69673, // Arcturus
            //    72105, // Izar
            //    74666, // Muphrid
            //    72125, // Nekkar
            //    74689, // Seginus
            //    67459, // Asellus Primus
            //    71075  // Asellus Secondus
            //};

            //constellation.Connections = new List<(int, int)>
            //{
            //    (0, 1), // Arcturus - Izar
            //    (0, 2), // Arcturus - Muphrid
            //    (0, 3), // Arcturus - Nekkar
            //    (3, 4), // Nekkar - Seginus
            //    (1, 4), // Izar - Seginus
            //    (5, 6), // Asellus Primus - Asellus Secondus
            //};

            //ConstellationList.Add(constellation);
            #endregion

            #region Геркулес
            constellation = new Constellation("Геркулес", new Vector3(0.8f, 0.8f, 0.0f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 84345, "Рас Альгети (α Her) - Третья по яркости звезда в созвездии, известный красный гигант." },
                { 80816, "Корнефорос (β Her) - Вторая по яркости звезда в созвездии, яркий голубой гигант." },
                { 80170, "Зенкер (γ Her) - Первая по яркости звезда в созвездии, переменная звезда типа RS Гончих Псов." },
                { 84379, "Дельта Геркулеса (δ Her) - Двойная звезда, одна из компонент которой - белый карлик." },
                { 83207, "Эпсилон Геркулеса (ε Her) - Переменная звезда, меняющая свою яркость." },
                { 81693, "Дзета Геркулеса (ζ Her) - Тройная звезда, интересный объект для наблюдений" },
                { 81833, "Эта Геркулеса (η Her) - Яркая звезда, привлекающая внимание астрономов." },
                { 87808, "Тета Геркулеса (θ Her) - Важная звезда в астеризме 'Голова Геркулеса;." },
                { 86414, "Йота Геркулеса (ι Her) - Ещё одна звезда, составляющая голову Геркулеса." },
                { 79043, "Каппа Геркулеса (κ Her) - Составляет левое плечо Геркулеса." },
                { 85693, "Лямбда Геркулеса (λ Her) - Интересная звезда, представляющая собой двойную систему." },
                { 86974, "Мю Геркулеса (μ Her) - Красивый бело-голубой гигант." },
                { 87998, "Ню Геркулеса (ν Her) - Часть астеризма 'Колено Геркулеса'." },
                { 87933, "Кси Геркулеса (ξ Her) - Светлый гигант, составляющий колено Геркулеса." },
                { 80463, "Омега Геркулеса (ω Her) - Переменая звезда, заслуживающая внимания." },
                { 84380, "Пи Геркулеса (π Her) - Одна из ярких звёзд в правой руке Геркулеса." },
                { 85112, "Ро Геркулеса (ρ Her) - Еще одна звезда, формирующая правую руку Геркулеса." },
                { 81126, "Сигма Геркулеса (σ Her) - Звезда, расположенная вблизи головы Геркулеса." },
                { 79992, "Тау Геркулеса (τ Her) - Представитель астеризма 'Правая рука Геркулеса'." },
                { 78592, "Ипсилон Геркулеса (υ Her) - Формация, напоминающая меч Геркулеса." },
                { 79101, "Фи Геркулеса (φ Her) - Слабая звезда, завершающая список основных звёзд Геркулеса." },
                { 77760, "Хи Геркулеса (χ Her) - Является звездой, похожей на Солнце." },
                { 88794, "Омикрон Геркулеса (o Her) -  тройная звезда в созвездии Геркулеса на расстоянии приблизительно 334 световых года от Солнца." }
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 1), // Рас Альгети - Корнефорос
                (1, 2), // Корнефорос - Зенкер
                (1, 5), // Корнефорос - ζ Her
                (5, 6), // ζ Her - η Her
                (6, 17), // η Her - σ Her
                (17, 18), // σ Her - τ Her
                (18, 20), // τ Her - φ Her
                (20, 21),  // φ Her - χ Her
                (6, 15), // η Her - π Her
                (15, 16), // π Her - ρ Her
                (16, 7), // ρ Her - θ Her
                (7, 8), // θ Her - ι Her
                (15, 4), // π Her - ε Her
                (4, 5), // ε Her - ζ Her
                (4, 3), // ε Her - δ Her
                (3, 10), // δ Her - λ Her
                (10, 11), // λ Her - μ Her
                (11, 13), // μ Her - ξ Her
                (13, 22) // ξ Her - o Her

            };

            ConstellationList.Add(constellation);
            #endregion

            #region Орион
            constellation = new Constellation("Орион", new Vector3(0.4f, 1.0f, 0.6f));
            constellation.Hips = new Dictionary<int, string>
            {
                {27989, "Бетельгейзе" },
                {25336, "Беллатрикс" },
                {26727, "Альнитак" },
                {26311, "Альнилам" },
                {25930, "Минтака" },
                {27366, "Саиф" },
                {24436, "Ригель" }
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 2), // Бетельгейзе -> Альнитак
                (2, 3), // Альнитак -> Альнилам
                (3, 4), // Альнилам -> Минтака
                (0, 1), // Бетельгейзе -> Беллатрикс
                (5, 2), // Саиф -> Альнитак
                (1, 4), // Беллатрикс -> Минтака
                (4, 6), // Минтака -> Ригель
                (6, 5)  // Ригель -> Саиф
            };

            ConstellationList.Add(constellation);
            #endregion

            #region Южный Крест
            constellation = new Constellation("Южный Крест", new Vector3(0.8f, 0.4f, 0.8f));

            constellation.Hips = new Dictionary<int, string>
            {
                { 60718, "Акрукс (α Cru) - Самая яркая звезда в созвездии, одна из четырёх звёзд, образующих форму креста." },
                { 62434, "Мимоза (β Cru) - Вторая по яркости звезда в созвездии, также известная как Бекрукс." },
                { 61084, "Гакрукс (γ Cru) - Третья по яркости звезда, образует верхний край креста." },
                { 59747, "Дельта Южного Креста (δ Cru) - Четвёртая звезда, формирующая крест." }
                //{ 64858, "Эпсилон Южного Креста (ε Cru) - Пятикратная звезда, одна из составляющих креста." },
                //{ 64484, "Дзета Южного Креста (ζ Cru) - Звезда, расположенная вне основного рисунка креста." },
                //{ 64398, "Эта Южного Креста (η Cru) - Яркая звезда, составляющая часть 'ножки' креста." },
                //{ 64575, "Тета Южного Креста (θ Cru) - Важная звезда, участвующая в формировании креста." },
                //{ 64146, "Йота Южного Креста (ι Cru) - Слабая звезда, но играет важную роль в общем рисунке созвездия." },
                //{ 64287, "Каппа Южного Креста (κ Cru) - Ещё одна звезда, дополняющая общую картину созвездия." },
                //{ 64499, "Лямбда Южного Креста (λ Cru) - Незначительная звезда, близкая к основной части креста." },
                //{ 63744, "Мю Южного Креста (μ Cru) - Немного удалённая звезда, относящаяся к созвездию." },
                //{ 63992, "Ню Южного Креста (ν Cru) - Частично скрытая звезда, видимая в тёмное время суток." },
                //{ 63874, "Кси Южного Креста (ξ Cru) - Интересная звезда, достойная внимания." },
                //{ 63512, "Омега Южного Креста (ω Cru) - Одна из самых маленьких звёзд в созвездии." },
                //{ 63487, "Пи Южного Креста (π Cru) - Окончательный штрих в картине Южного Креста." },
                //{ 63112, "Роу Южного Креста (ρ Cru) - Небольшая звезда, играющая второстепенную роль в созвездии." },
                //{ 62667, "Сигма Южного Креста (σ Cru) - Заключает список звёзд Южного Креста, являясь одной из последних звёзд." } 
            };

            constellation.Connections = new List<(int, int)>
            {
                (0, 2), // Акрукс - Гакрукс
                (1, 3)  // Мимоза - δ Cru
            };

            ConstellationList.Add(constellation);
            #endregion

            //#region Pleiades
            //constellation = new Constellation("Pleiades");
            //constellation.Hips = new List<int>()
            //{
            //    17702, // Alcyone
            //    17531, // Atlas
            //    17499, // Electra
            //    17573, // Maia
            //    17608, // Merope
            //    17579, // Taygeta
            //    17692  // Pleione
            //};

            //constellation.Connections = new List<(int, int)>
            //{
            //    (1, 6), // Atlas -> Pleione
            //    (6, 0), // Pleione -> Alcyone
            //    (0, 4), // Alcyone -> Merope
            //    (4, 2), // Merope -> Electra
            //    (2, 3), // Electra -> Maia
            //    (3, 0), // Maia -> Alcyone
            //    (3, 5) // Maia -> Taygeta
            //};

            //ConstellationList.Add(constellation);
            //#endregion
        }

        public void OrderByStars()
        {
            foreach (var constellation in ConstellationList)
            {
                // Сортируем список звезд в порядке Hips
                constellation.Stars = constellation.Stars
                    .OrderBy(star => constellation.Hips.Keys.ToList().IndexOf((int)star.HIP)) // Сортируем по индексу в Hips
                    .ToList();
            }
        }

        public List<string> GetConstellationsName()
        {
            return ConstellationList.Select(x => x.Name).ToList();
        }

    }
}
