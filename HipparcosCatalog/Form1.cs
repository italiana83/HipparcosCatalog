using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Linq;
using Microsoft.VisualBasic.Devices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace HipparcosCatalog
{
    public partial class Form1 : Form
    {
        bool loaded = false;
        Fps fps = new Fps();

        Camera _camera;
        Matrix4 _projection = Matrix4.Identity;
        private Matrix4 _model = Matrix4.Identity; // Текущая матрица модели 
        Catalog _catalog;
        Selector _bracket;


        float scaleStep = 10;

        private bool mouseDown = false;
        Vector2 lastMousePos = new Vector2();
        private float _rotationX = 0.0f; // Угол вращения вокруг оси X
        private float _rotationY = 0.0f; // Угол вращения вокруг оси Y

        float pointSize = 4.5F;
        TextRenderer _textRenderer;
        TextRenderer _textRenderer2;

        Star _selectedStar = null;
        Vector3 modelCenter = new Vector3(0, 0, 0);

        bool check = false;

        #region Scroll с постепенно увеличивающимся множителем
        private DateTime _lastScrollTime;
        private int _scrollCount;
        private const int AccelerationThreshold = 3; // кол-во скроллов до ускорения
        private const float InitialMultiplier = 1.0f;
        private float _currentMultiplier = InitialMultiplier;
        private const float MaxMultiplier = 50.0f;
        private const float MultiplierIncrement = 0.5f;
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loaded = true;

            GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
            GL.ClearDepth(1.0f);										// Depth Buffer Setup
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);									// Enable Depth Testing
            GL.Enable(EnableCap.VertexProgramPointSize);
            //GL.ShadeModel(ShadingModel.Smooth);									// Select Smooth Shading
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.DontCare);			// Set Perspective Calculations To Most Accurate
            //GL.Hint(HintTarget.PolygonSmoothHint, HintMode.DontCare);			// Set Perspective Calculations To Most Accurate
            //GL.Hint(HintTarget.LineSmoothHint, HintMode.DontCare);          // Set Perspective Calculations To Most Accurate
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);


            _catalog = new Catalog();
            _catalog.Load();
            var stars = _catalog.PrepareVertices();
            _catalog.InitializeBuffers();


            stars = stars.OrderBy(x => x.ProperName).ToList();
            foreach (var star in stars)
            {
                comboBox1.Items.Add(star);
            }

            if(comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            comboBox2.Items.Add("Собственное имя");
            comboBox2.Items.Add("Номер HIP");
            comboBox2.Items.Add("Номер в каталоге Gliese");
            comboBox2.Items.Add("Номер в каталоге BayerFlamsteed\r\n");
            comboBox2.Items.Add("Номер в каталоге HD");
            comboBox2.SelectedIndex = 0;

            var constellations = _catalog.Constellations.GetConstellationsName();
            checkedListBox1.Items.Add("Все");
            foreach (var constellation in _catalog.Constellations.ConstellationList)
                checkedListBox1.Items.Add(constellation);

            _camera = new Camera(0.001f, 0.02f);
            _camera.Position = new Vector3(0, 0, -1);
            _camera.Orientation = new Vector3(0, 0, 0);

            _bracket = new Selector();
            _textRenderer = new TextRenderer(@"C:\\Windows\Fonts\arial.ttf", 22, glControl1.Width, glControl1.Height);
            _textRenderer2 = new TextRenderer(@"C:\\Windows\Fonts\times.ttf", 22, glControl1.Width, glControl1.Height);

            _bracket.TextRenderer = _textRenderer2;

            _catalog.WindowWidth = glControl1.Width;
            _catalog.WindowHeight = glControl1.Height;


            _projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                glControl1.Width / (float)glControl1.Height,
                0.1f,
                20000000.0f);

            SetupViewport();

            timer2.Start();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) //Пока контекст не создан
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.LoadIdentity();

            Matrix4 view = _camera.GetViewMatrix();
            UpdateModelMatrix();
            //Matrix4 rotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90));
            //model = model * rotation;

            //GL.Enable(EnableCap.PointSprite);

            _catalog.Draw(view, _projection, _model, _camera.Position, _textRenderer);

            if (_selectedStar != null)
            {
                Vector2 screenSize = new Vector2(glControl1.Width, glControl1.Height);
                // Преобразование мировых координат в экранные
                Vector2 screenPosition = WorldToScreen(
                    _selectedStar.Pos,
                    _model,
                    view,
                    _projection,
                    screenSize
                );

                double? distance = _selectedStar.Distance * 3.26156f;
                string text = $"Name: {_selectedStar.ProperName}\n" +
                    $"Gliese: {_selectedStar.Gliese}\n" +
                    $"HD: {_selectedStar.HD}\n" +
                    $"HIP: {_selectedStar.HIP}\n" +
                    $"Bayer: {_selectedStar.BayerFlamsteed}\n" +
                    $"Distance LY: {distance:F2}\n" +
                    $"Spectrum: {_selectedStar.Spectrum}";

                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                _bracket.RenderStarBracket(text, screenPosition, screenSize, toolStripMenuItem2.Checked);
                GL.Disable(EnableCap.PointSprite);
                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
            }

            GL.Flush();
            glControl1.SwapBuffers();

            fps.Update();
            this.Text = "Fps: " + fps.GetFps().ToString() + "    Pos: " + _camera.Position.ToString() + "     Ori: " + _camera.Orientation.ToString();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            SetupViewport();
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);

            mouseDown = true;
            lastMousePos = new Vector2(e.X, e.Y);

            #region Выделение

            //if (e.Button == MouseButtons.Left)
            //{
            // Координаты курсора
            Vector2 mousePosition = new Vector2(e.X, e.Y);
            Vector2 screenSize = new Vector2(glControl1.Width, glControl1.Height);

            // Обработка выделения объекта
            Vector3 rayDirection = Selector.CalculateRayFromMouse(mousePosition, _camera, _projection, screenSize);
            if (!toolStripMenuItem2.Checked)
                _selectedStar = Selector.SelectStar(_camera.Position, rayDirection, _model, _catalog.VisibleStars);
            if (_selectedStar != null)
            {
                toolStripMenuItem2.Enabled = true;
                double? distance = _selectedStar.Distance * 3.26156f;
                string text = $"Name: {_selectedStar.ProperName}\n" +
                    $"Gliese: {_selectedStar.Gliese}\n" +
                    $"HD: {_selectedStar.HD}\n" +
                    $"HIP: {_selectedStar.HIP}\n" +
                    $"Bayer: {_selectedStar.BayerFlamsteed}\n" +
                    $"Distance LY: {distance:F2}\n" +
                    $"Spectrum: {_selectedStar.Spectrum}";

                richTextBox1.Text = text;
            }
            else
            {
                toolStripMenuItem2.Enabled = false;
                richTextBox1.Text = "";
            }

            glControl1.Invalidate();
            //}

            #endregion
        }

        private Vector2 WorldToScreen(Vector3 worldPosition, Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector2 screenSize)
        {
            // Преобразуем мировую позицию в Clip-Space
            Vector4 worldPos = new Vector4(worldPosition, 1.0f);
            Vector4 clipSpacePos = worldPos * modelMatrix * viewMatrix * projectionMatrix;

            // Если W = 0, объект за пределами видимости
            if (clipSpacePos.W == 0)
                return Vector2.Zero;

            // Преобразуем в NDC
            clipSpacePos.X /= clipSpacePos.W;
            clipSpacePos.Y /= clipSpacePos.W;

            // Преобразуем в экранные координаты
            float screenX = (clipSpacePos.X + 1.0f) * 0.5f * screenSize.X;
            float screenY = (1.0f - clipSpacePos.Y) * 0.5f * screenSize.Y;

            return new Vector2(screenX, screenY);
        }

        //private Vector2 WorldToScreen(Vector3 worldPosition, Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector2 screenSize)
        //{
        //    Vector4 worldPos = new Vector4(worldPosition, 1.0f);
        //    //Vector4 clipSpacePos = worldPos.Multiply(viewMatrix).Multiply(projectionMatrix);
        //    //Matrix4 temp = projectionMatrix * viewMatrix * modelMatrix;
        //    Vector4 clipSpacePos = worldPos * modelMatrix * viewMatrix * projectionMatrix;

        //    if (clipSpacePos.W != 0)
        //    {
        //        clipSpacePos.X /= clipSpacePos.W;
        //        clipSpacePos.Y /= clipSpacePos.W;
        //    }

        //    float screenX = (clipSpacePos.X + 1.0f) * 0.5f * screenSize.X;
        //    float screenY = (1.0f - clipSpacePos.Y) * 0.5f * screenSize.Y;

        //    return new Vector2(screenX, screenY);
        //}
        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseDown = false;

            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(glControl1, new Point(e.X, e.Y));
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (mouseDown)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Vector2 delta = lastMousePos - new Vector2(e.X, e.Y);

                    if (checkBox2.Checked)
                    {
                        // Обновляем углы вращения, изменяя их на основе движения мыши
                        RotationModel(delta);
                    }
                    else
                    {
                        _camera.AddRotation(delta.X / 10, delta.Y / 10);
                    }

                    lastMousePos = new Vector2(e.X, e.Y);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (e.Y > lastMousePos.Y)
                        _camera.Move(0f, 0f, 0.1f);
                    else
                        _camera.Move(0f, 0f, -0.1f);

                    lastMousePos = new Vector2(e.X, e.Y);
                }

                glControl1.Invalidate();
            }
        }

        /// <summary>
        /// Обновляем углы вращения
        /// </summary>
        /// <param name="delta"></param>
        private void RotationModel(Vector2 delta)
        {
            _rotationX += delta.Y * 0.1f; // Масштаб для контроля чувствительности
            _rotationY += delta.X * 0.1f;

            // Ограничиваем вращение по оси X (наклон) между -90 и +90 градусов
            _rotationX = Math.Clamp(_rotationX, -90.0f, 90.0f);
        }

        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            // Перемещение камеры вдоль оси Z при скроллинге мыши
            var now = DateTime.Now;
            var timeSinceLastScroll = now - _lastScrollTime;

            // Если прошло больше 500мс - сбрасываем счетчик
            if (timeSinceLastScroll.TotalMilliseconds > 500)
            {
                _scrollCount = 0;
                _currentMultiplier = InitialMultiplier;
            }

            _scrollCount++;
            _lastScrollTime = now;

            // Увеличиваем множитель после определенного количества скроллов
            if (_scrollCount >= AccelerationThreshold && _currentMultiplier < MaxMultiplier)
            {
                _currentMultiplier += MultiplierIncrement;
                _currentMultiplier = Math.Min(_currentMultiplier, MaxMultiplier);
            }

            // Применяем скролл с текущим множителем
            int scrollAmount = (int)(e.Delta * _currentMultiplier);

            _camera.Zoom(scrollAmount);
            //var data = e.Delta * scaleStep / 10;

            //if (e.Delta > 0)
            //{
            //    _camera.Move(0f, data, 0f); // Прокрутка вверх — движение вперёд
            //}
            //else if (e.Delta < 0)
            //{
            //    _camera.Move(0f, -data, 0f); // Прокрутка вниз — движение назад
            //}

            glControl1.Invalidate();
        }

        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            _catalog.WindowWidth = w;
            _catalog.WindowHeight = h;
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, w, 0, h, -1, 1); // Верхний левый угол имеет кооординаты(0, 0)
            //GL.Viewport(0, 0, w, h); // Использовать всю поверхность GLControl под рисование

            //camera.Width = w;
            //camera.Height = h;
            _textRenderer.WindowHeight = h;
            _textRenderer.WindowWidth = w;
            _textRenderer.UpdateProjection();
            _textRenderer2.WindowHeight = h;
            _textRenderer2.WindowWidth = w;
            _textRenderer2.UpdateProjection();

            GL.Viewport(0, 0, w, h);

            _projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                glControl1.Width / (float)glControl1.Height,
                0.1f,
                20000000.0f);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            ////Glu.Perspective(45.0, w / (double)h, 0.1, 20000000.0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _camera.MoveSpeed = trackBar1.Value * 0.001f;
            //scaleStep = trackBar1.Value;
            label2.Text = trackBar1.Value.ToString();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBox1.Checked)
            //{
            //    timer1.Start();
            //}
            //else
            //{
            //    timer1.Stop();
            //}
        }

        // Метод для генерации модельной матрицы
        public void UpdateModelMatrix()
        {
            if (checkBox1.Checked || checkBox2.Checked) // Вращение относительно центра модели
            {
                // Матрицы вращения вокруг осей X и Y
                Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotationX));
                Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotationY));
                Matrix4 rotation = rotationX * rotationY;

                // Перемещаем центр модели к началу координат
                Matrix4 translateToOrigin = Matrix4.CreateTranslation(-modelCenter);
                Matrix4 translateBack = Matrix4.CreateTranslation(modelCenter);

                // Обновляем матрицу вращения относительно центра модели
                _model = translateToOrigin * rotation * translateBack;
            }

            //return _model;
        }

        Matrix4 ChangeRotationCenter(Matrix4 modelMatrix, Vector3 newCenter)
        {
            // Перемещаем центр к началу координат
            Matrix4 translateToOrigin = Matrix4.CreateTranslation(-newCenter);

            // Возвращаем центр на место
            Matrix4 translateBack = Matrix4.CreateTranslation(newCenter);

            // Итоговая матрица
            Matrix4 result = translateToOrigin * translateBack;

            // Применяем изменения к текущей матрице модели
            return modelMatrix * result;
        }

        //private float _rotationAngle = 0.0f;
        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    _rotationAngle += 10.0F * _camera.MoveSpeed;

        //    //// deltaTime - время, прошедшее с последнего кадра
        //    //const float rotationSpeed = 50.0f; // Скорость вращения в градусах в секунду
        //    //_rotationAngle += rotationSpeed * deltaTime;

        //    // Ограничиваем угол поворота от 0 до 360 градусов
        //    if (_rotationAngle > 360.0f)
        //        _rotationAngle -= 360.0f;

        //    glControl1.Invalidate();

        //}

        private void button1_Click(object sender, EventArgs e)
        {
            _camera.Position = new Vector3(0.0f, 0.325f, 10.0f);
            _camera.Orientation = new Vector3(-3.0f, 0.0f, 0.0f);
            glControl1.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_camera != null)
            {
                Star star = (Star)comboBox1.SelectedItem;
                _selectedStar = star;

                // Параметр для управления расстоянием от звезды (чем больше, тем ближе к звезде)
                float alpha = 0.8f; // Значение от 0 до 1, например, 0.8 для позиции ближе к звезде
                // Рассчитываем точку на линии между началом координат и звездой
                Vector3 cameraPosition = star.Pos + new Vector3(0, 0, 2);
                // Устанавливаем позицию камеры
                _camera.Position = cameraPosition;

                // Рассчитываем направление от камеры к звезде
                Vector3 directionToStar = Vector3.Normalize(star.Pos - _camera.Position);

                // Рассчитываем ориентацию камеры
                _camera.Orientation.X = (float)Math.Atan2(directionToStar.X, directionToStar.Z); // Вращение вокруг вертикальной оси (горизонтальная ориентация)
                _camera.Orientation.Y = (float)Math.Asin(directionToStar.Y); // Вращение вверх/вниз
                _camera.Orientation.Z = 0; // Нет вращения вокруг оси Z

                if(checkBox4.Checked)
                {
                    _catalog.HighlightCenter = star.Pos;
                }

                glControl1.Invalidate();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (_catalog != null)
            {
                _catalog.VisibleRadius = (int)numericUpDown1.Value;
                _catalog.VisibleRadius = _catalog.VisibleRadius * 0.306601f; // Коэффициент пересчета, 1 световой год = 0.306601 парсека

                _catalog.PrepareVertices();
                _catalog.InitializeBuffers();

                glControl1.Invalidate();
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            _catalog.DistanceCameraLablesVisible = (float)numericUpDown2.Value;

            glControl1.Invalidate();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    _catalog.DisplayProperNames = true;
                    _catalog.DisplayStarHip = false;
                    _catalog.DisplayGlieseNames = false;
                    _catalog.DisplayBayerFlamsteedNames = false;
                    _catalog.DisplayHDNames = false;
                    break;
                case 1:
                    _catalog.DisplayProperNames = false;
                    _catalog.DisplayStarHip = true;
                    _catalog.DisplayGlieseNames = false;
                    _catalog.DisplayBayerFlamsteedNames = false;
                    _catalog.DisplayHDNames = false;
                    break;
                case 2:
                    _catalog.DisplayProperNames = false;
                    _catalog.DisplayStarHip = false;
                    _catalog.DisplayGlieseNames = true;
                    _catalog.DisplayBayerFlamsteedNames = false;
                    _catalog.DisplayHDNames = false;
                    break;
                case 3:
                    _catalog.DisplayProperNames = false;
                    _catalog.DisplayStarHip = false;
                    _catalog.DisplayGlieseNames = false;
                    _catalog.DisplayBayerFlamsteedNames = true;
                    _catalog.DisplayHDNames = false;
                    break;
                case 4:
                    _catalog.DisplayProperNames = false;
                    _catalog.DisplayStarHip = false;
                    _catalog.DisplayGlieseNames = false;
                    _catalog.DisplayBayerFlamsteedNames = false;
                    _catalog.DisplayHDNames = true;
                    break;
            }

            glControl1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                int moveSpeed = trackBar1.Value;

                Vector2 _deltaForAutoRotation = new Vector2(1.0f * moveSpeed, 0);

                // Обновляем углы для авто вращения
                RotationModel(_deltaForAutoRotation);
            }

            glControl1.Invalidate();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_selectedStar != null)
                modelCenter = _selectedStar.Pos;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == 0 && e.NewValue == CheckState.Checked)
            {
                for (int i = 1; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                    ((Constellation)checkedListBox1.Items[i]).Visible = true;
                }
            }
            else if (e.Index == 0 && e.NewValue == CheckState.Unchecked)
            {
                for (int i = 1; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                    ((Constellation)checkedListBox1.Items[i]).Visible = false;
                }
            }
            else if (e.Index > 0)
            {
                checkedListBox1.SetItemCheckState(0, CheckState.Indeterminate);
                ((Constellation)checkedListBox1.Items[e.Index]).Visible = e.NewValue == CheckState.Checked ? true : false;
            }

            _catalog.Constellations.InitializeBuffersConstellations();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //if
            //_catalog.DisplayStarNames = radioButton1.Checked;
            //glControl1.Invalidate();

            //comboBox2.Enabled = radioButton1.Checked;
            //numericUpDown2.Enabled = radioButton1.Checked;
            comboBox2.Enabled = !radioButton1.Checked;

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            _catalog.DisplayStarNames = radioButton2.Checked;
            numericUpDown2.Enabled = radioButton2.Checked;

            glControl1.Invalidate();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            _catalog.DisplayConstellationsNames = radioButton3.Checked;

            glControl1.Invalidate();

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _catalog.DisplayAxis = checkBox3.Checked;
            glControl1.Invalidate();

        }

        private void toolStripMenuItem3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedStar != null)
            {

            }
            else
                toolStripMenuItem2.Checked = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            _catalog.HighlightEnabled = checkBox4.Checked;

            if (_catalog.HighlightEnabled)
            {
                var starsCount = _catalog.CountStarsInRadius(_catalog.HighlightCenter, _catalog.HighlightRadius);
                label6.Text = $"В радиус попало {starsCount} звезд";
            }
            else
            {
                label6.Text = $"В радиус попало 0 звезд";
            }

            glControl1.Invalidate();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            _catalog.HighlightRadius = (float)numericUpDown3.Value;

            var starsCount = _catalog.CountStarsInRadius(_catalog.HighlightCenter, _catalog.HighlightRadius);
            label6.Text = $"В радиус попало {starsCount} звезд";

            glControl1.Invalidate();
        }
    }
}
