using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System;
using OpenTK.Mathematics;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using SkiaSharp;
using OpenTK.Compute.OpenCL;


namespace HipparcosCatalog
{
    public class TextRenderer
    {
        private int _vao;
        private int _vbo;
        private Shader _shader;
        private Shader _lineShader;
        private int _lineVao;
        private int _lineVbo;
        private Dictionary<char, Character> _characters;
        private Matrix4 _projection;

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        // Структура для хранения информации о символах
        private struct Character
        {
            public int TextureID;    // Идентификатор текстуры символа
            public int SizeX;        // Ширина символа
            public int SizeY;        // Высота символа
            public int BearingX;     // Смещение от базовой линии по X
            public int BearingY;     // Смещение от базовой линии по Y
            public int Advance;      // Смещение до следующего символа
        }

        public TextRenderer(string fontPath, int fontSize, int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            UpdateProjection();

            _characters = new Dictionary<char, Character>();
            LoadFontCharacters(fontPath, fontSize);
            InitializeOpenGLResources();
            InitializeLineResources();
        }

        public void UpdateProjection()
        {
            _projection = Matrix4.CreateOrthographicOffCenter(0, WindowWidth, 0, WindowHeight, -1, 1);
        }

        private void LoadFontCharacters(string fontPath, int fontSize)
        {
            var bbb = File.Exists(fontPath);
            var typeface = SKTypeface.FromFile(fontPath);
            var font = new SKFont(typeface, fontSize);

            for (char c = (char)0; c < (char)128; c++)
            {
                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Color = SKColors.White
                };

                string character = c.ToString();
                var bounds = new SKRect();
                font.MeasureText(character, out bounds);

                int width = (int)Math.Ceiling(bounds.Width);
                int height = (int)Math.Ceiling(bounds.Height);

                if (width == 0 || height == 0) continue;

                using var surface = SKSurface.Create(new SKImageInfo(width, height));
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.Transparent);
                canvas.DrawText(character, -bounds.Left, -bounds.Top, font, paint);

                using var image = surface.Snapshot();
                using var pixels = image.PeekPixels();

                int texture = CreateTextureFromPixels(pixels, width, height);

                float[] glyphWidths = font.GetGlyphWidths(font.GetGlyphs(character));
                int advance = glyphWidths.Length > 0 ? (int)Math.Ceiling(glyphWidths[0]) : width;
                int bearingY = (int)Math.Ceiling(-bounds.Top);

                _characters[c] = new Character
                {
                    TextureID = texture,
                    SizeX = width,
                    SizeY = height,
                    BearingX = (int)bounds.Left,
                    BearingY = bearingY,
                    Advance = advance << 6
                };
            }
        }

        private int CreateTextureFromPixels(SKPixmap pixels, int width, int height)
        {
            int texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, pixels.GetPixelSpan().ToArray());

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            return texture;
        }

        private void InitializeOpenGLResources()
        {
            // VAO, VBO
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 6 * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Шейдер
            _shader = new Shader(LoadVertexShader(), LoadFragmentShader(), null, ShaderSourceMode.Code);
        }

        private string LoadVertexShader() => @"
                #version 330 core
                layout (location = 0) in vec4 vertex;
                uniform mat4 projection;
                out vec2 TexCoords;
                void main()
                {
                    gl_Position = projection * vec4(vertex.xy, 0.0, 1.0);
                    TexCoords = vertex.zw;
                }";

        private string LoadFragmentShader() => @"
                #version 330 core
                in vec2 TexCoords;
                out vec4 color;
                uniform sampler2D text;
                uniform vec3 textColor;
                void main()
                {
                    vec4 sampled = texture(text, TexCoords);
                    color = vec4(textColor, 1.0) * sampled;
                }";

        private void InitializeLineResources()
        {
            _lineShader = new Shader(LoadLineVertexShader(), LoadLineFragmentShader(), null, ShaderSourceMode.Code);
            _lineVao = GL.GenVertexArray();
            _lineVbo = GL.GenBuffer();

            GL.BindVertexArray(_lineVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _lineVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 2 * 3 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private string LoadLineVertexShader() => @"
    #version 330 core
    layout (location = 0) in vec3 aPos;
    uniform mat4 projection;
    void main()
    {
        gl_Position = projection * vec4(aPos, 1.0);
    }";

        private string LoadLineFragmentShader() => @"
    #version 330 core
    out vec4 FragColor;
    uniform vec3 color;
    void main()
    {
        FragColor = vec4(color, 1.0);
    }";

        public void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            _lineShader.Use();
            _lineShader.SetMatrix4("projection", _projection);
            _lineShader.SetVector3("color", new Vector3(color.R / 255f, color.G / 255f, color.B / 255f));

            float[] vertices = {
            start.X, start.Y, start.Z,
            end.X, end.Y, end.Z
        };

            GL.BindVertexArray(_lineVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _lineVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);

            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            GL.BindVertexArray(0);
        }

        public void RenderTextWithLines(string text, float starCenterX, float starCenterY, float scale, Color textColor, Color lineColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Центр звезды
            Vector3 starCenter = new Vector3(starCenterX, WindowHeight - starCenterY, 0);

            // Начало первой линии (центр звезды)
            Vector3 start = starCenter;

            // Конец первой линии под углом 45 градусов
            float angle = MathHelper.PiOver4; // Угол 45 градусов
            float firstLineLength = 100.0f; // Длина первой линии
            Vector3 firstLineEnd = new Vector3(
                start.X + firstLineLength * MathF.Cos(angle),
                start.Y + firstLineLength * MathF.Sin(angle),
                0);

            // Вычисление длины текста
            float textWidth = 0.0f;
            foreach (var c in text)
            {
                if (_characters.ContainsKey(c))
                {
                    var ch = _characters[c];
                    textWidth += (ch.Advance >> 6) * scale;
                }
            }

            // Конец второй линии (горизонтальная линия длиной по тексту)
            Vector3 secondLineEnd = new Vector3(
                firstLineEnd.X + textWidth,
                firstLineEnd.Y,
                0);

            // Рисуем первую линию
            DrawLine(start, firstLineEnd, lineColor);

            // Рисуем вторую линию
            DrawLine(firstLineEnd, secondLineEnd, lineColor);

            // Позиция текста (над второй линией)
            float textX = firstLineEnd.X; // Начало текста совпадает с началом второй линии
            float textY = firstLineEnd.Y + 15.0f; // Смещаем текст выше второй линии

            // Рендеринг текста
            RenderText(text, textX, textY, scale, textColor);
        }

        public void RenderText(string text, float x, float y, float scale, Color color, RectangleF? boundingBox = null)
        {
            //// Вычисление ширины и высоты текста
            //float textWidth = 0.0f;
            //float textHeight = 0.0f;

            //foreach (var c in text)
            //{
            //    if (!_characters.ContainsKey(c)) continue;

            //    var ch = _characters[c];
            //    textWidth += (ch.Advance >> 6) * scale; // Общая ширина строки
            //    float characterHeight = ch.SizeY * scale;
            //    if (characterHeight > textHeight) textHeight = characterHeight; // Максимальная высота
            //}

            //// Корректируем начальные координаты
            //x -= textWidth / 2.0f; // Центрируем по ширине
            //y += textHeight / 2.0f; // Центрируем по высоте

            // Установка шейдера
            _shader.Use();
            _shader.SetMatrix4("projection", _projection);
            _shader.SetVector3("textColor", new Vector3(color.R / 255f, color.G / 255f, color.B / 255f));
            GL.ActiveTexture(TextureUnit.Texture0);
            _shader.SetInt("text", 0);
            GL.BindVertexArray(_vao);

            if (boundingBox == null)
                boundingBox = new RectangleF(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue);

            float startX = x; // Начальная позиция по X
            float maxWidth = boundingBox.Value.Width; // Максимальная ширина строки
            float maxHeight = boundingBox.Value.Height; // Максимальная высота текста
            float lineHeight = 24 * scale; // Высота строки (настраиваемая)
            // Отрисовка текста
            foreach (var c in text)
            {
                float sizeSymbolX = 0;

                if(_characters.ContainsKey(c))
                    sizeSymbolX = _characters[c].SizeX;

                if (c == '\n' || (x + 6 * scale > startX + maxWidth - sizeSymbolX*2))
                {
                    // Перенос строки: сброс X и смещение Y
                    x = startX;
                    y -= lineHeight;

                    // Прерываем, если текст выходит за нижнюю границу
                    if (y - lineHeight < boundingBox.Value.Y)
                        break;

                    if (c == '\n')
                        continue;
                }
                if (c == '\n')
                {
                    // Перенос строки: сброс X и смещение Y
                    x = startX;
                    y -= 24 * scale; // Регулируйте 24 в зависимости от межстрочного интервала
                    continue;
                }

                if (!_characters.ContainsKey(c) && c != ' ')
                    continue;

                if(c == ' ')
                    x += 6 * scale; // Смещение для следующего символа
                else
                { 
                    var ch = _characters[c];

                    // Рассчитываем позицию символа
                    float xpos = x + ch.BearingX * scale;
                    float ypos = y - (ch.SizeY - ch.BearingY) * scale;
                    //float ypos = WindowHeight - y;

                    float w = ch.SizeX * scale;
                    float h = ch.SizeY * scale;

                    float[] vertices = {
                        xpos,     ypos + h,   0.0f, 0.0f,
                        xpos,     ypos,       0.0f, 1.0f,
                        xpos + w, ypos,       1.0f, 1.0f,

                        xpos,     ypos + h,   0.0f, 0.0f,
                        xpos + w, ypos,       1.0f, 1.0f,
                        xpos + w, ypos + h,   1.0f, 0.0f
                    };

                    GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                    x += (ch.Advance >> 6) * scale; // Смещение для следующего символа
                }
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
