using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibNoise;
using LibNoise.Primitive;

namespace NoiseGenerator
{
    public partial class Form1 : Form
    {
        Random rnd = new Random();
        private Bitmap _NoiseBitmap;
        private Bitmap _3dNoiseBitmap;
        NoiseQuality _NoiseQuality = NoiseQuality.Standard;
        NoiseQuality _3dNoiseQuality = NoiseQuality.Standard;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
            numericUpDown2.Value = rnd.Next(1000);
            numericUpDown4.Value = rnd.Next(1000);

            GenerateNoise();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numericUpDown2.Value = rnd.Next(1000);

            GenerateNoise();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_NoiseBitmap != null)
            {
                e.Graphics.DrawImage(_NoiseBitmap, 0, 0, panel1.Width, panel1.Height);
            }
        }

        //private void GenerateNoise()
        //{
        //    int width = 256;
        //    int height = 256;
        //    noiseBitmap = new Bitmap(width, height);

        //    var perlin = new SimplexPerlin(42, NoiseQuality.Standard);

        //    for (int y = 0; y < height; y++)
        //        for (int x = 0; x < width; x++)
        //        {
        //            float nx = (float)x / width;
        //            float ny = (float)y / height;

        //            float value = perlin.GetValue(nx * 4, ny * 4, 0); // Z = 0 для 2D
        //            value = (value + 1.0f) / 2.0f; // [−1,1] → [0,1]
        //            int gray = (int)(value * 255);
        //            noiseBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
        //        }

        //    panel1.Invalidate();
        //}

        private void GenerateNoise()
        {
            int width = 512;
            int height = 512;
            _NoiseBitmap = new Bitmap(width, height);

            var noise = new SimplexPerlin((int)numericUpDown2.Value, _NoiseQuality);
            float scale = (float)numericUpDown1.Value; // Чем меньше, тем более растянутый шум

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float nx = x * scale;
                    float ny = y * scale;

                    // Получаем шумовое значение [-1, 1]
                    float value = noise.GetValue(nx, ny);
                    value = (value + 1.0f) / 2.0f; // нормализация в [0, 1]

                    int gray = (int)(value * 255);
                    _NoiseBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }

            panel1.Invalidate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GenerateNoise();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            GenerateNoise();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _NoiseQuality = (NoiseQuality)comboBox1.SelectedIndex;

            GenerateNoise();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Generate3DNoise();
        }

        private void Generate3DNoise()
        {
            int size = 64;
            _3dNoiseBitmap = new Bitmap(size, size);
            float[,,] volume = new float[size, size, size];
            var perlin = new ImprovedPerlin((int)numericUpDown4.Value, quality: _3dNoiseQuality);
            float scale = (float)numericUpDown3.Value;

            for (int z = 0; z < size; z++)
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        float nx = x * scale;
                        float ny = y * scale;
                        float nz = z * scale;
                        float value = perlin.GetValue(nx, ny, nz); // [-1, +1]
                        volume[x, y, z] = (value + 1f) * 0.5f;        // [0, 1]
                    }

            // Maximum Intensity Projection
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float max = 0;
                    for (int z = 0; z < size; z++)
                        max = Math.Max(max, volume[x, y, z]);

                    float normalized = Clamp(max, 0f, 1f);
                    int gray = (int)(normalized * 255);
                    _3dNoiseBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }

            panel2.Invalidate();
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (_3dNoiseBitmap != null)
            {
                e.Graphics.DrawImage(_3dNoiseBitmap, 0, 0, panel1.Width, panel1.Height);
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Generate3DNoise();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Generate3DNoise();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            _3dNoiseQuality = (NoiseQuality)comboBox2.SelectedIndex;

            Generate3DNoise();
        }
    }
}
