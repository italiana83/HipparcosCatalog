using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosStarProcessor
{
    public class StarDataCompact
    {
        #region Идентификаторы звезды

        /// <summary>
        /// Номер звезды в каталоге HIPPARCOS
        /// </summary>
        public int? HIP { get; set; }

        #endregion

        #region Дополнительная информация

        /// <summary>
        /// Собственное название звезды, например «Звезда Барнарда» или «Сириус». Я взял эти названия в основном с веб-сайта проекта Hipparcos,
        /// где перечислены репрезентативные названия 150 самых ярких звезд и многих из 150 ближайших звезд. Я добавил несколько названий в этот список. Большинство добавлений — это
        /// обозначения из каталогов, которые сейчас в основном забыты (например, Лаланд, Грумбридж и Гулд [«G.»]), за исключением некоторых близких звезд, которые до сих пор лучше всего известны по
        /// этим обозначениям.
        /// </summary>
        public string? ProperName { get; set; }

        /// <summary>
        /// Период переменности (дни)
        /// </summary>
        public double? Period { get; set; }
        /// <summary>
        /// Тип переменности
        /// </summary>
        public string HvarType { get; set; }
        /// <summary>
        /// Флаг двойной/множественной системы
        /// </summary>
        public string MultFlag { get; set; }
        #endregion

        #region Координаты

        /// <summary>
        /// Прямое восхождение (Right Ascension) звезды на эпоху 2000.0. Звезды, присутствующие только в каталоге Gliese, который использует координаты 1950.0, имели
        /// эти координаты, прецессированные до 2000.
        /// </summary>
        public double? RA { get; set; }

        /// <summary>
        /// Правое склонение (Declination) звезды на эпоху 2000.0. Звезды, присутствующие только в каталоге Gliese, который использует координаты 1950.0, имели
        /// эти координаты, прецессированные до 2000.
        /// </summary>
        public double? Dec { get; set; }

        /// <summary>
        /// Trigonometric parallax
        /// </summary>
        public double? Plx { get; set; }

        /// <summary>
        /// Расстояние до звезды в парсеках, наиболее распространенная единица в астрометрии. Чтобы преобразовать парсеки в световые годы, умножьте на 3,262. Значение 10000000
        /// указывает на отсутствующие или сомнительные (например, отрицательные) данные о параллаксе в Hipparcos.
        /// </summary>
        public double? Distance { get; set; }

        #endregion

        #region Собственное движение
        /// <summary>
        /// Собственное движение по RA (массивная экранировка)
        /// </summary>
        public double? PMRA { get; set; }
        /// <summary>
        /// Собственное движение по Dec
        /// </summary>
        public double? PMDec { get; set; }
        /// <summary>
        /// Радиальная скорость
        /// </summary>
        public double? RV { get; set; }

        #endregion

        #region Яркость и спектральные данные
        /// <summary>
        /// Видимая звёздная величина
        /// </summary>
        public double? Mag { get; set; }
        /// <summary>
        /// Абсолютная звёздная величина (ее видимая величина с расстояния 10 парсеков).
        /// </summary>
        public double? AbsMag { get; set; }
        /// <summary>
        /// Спектральный класс (например, "G2V")
        /// </summary>
        public string? Spectrum { get; set; }
        /// <summary>
        /// Цветовой индекс B-V системы Джонсона
        /// </summary>
        public double? B_V { get; set; }
        /// <summary>
        /// ? Colour index in Cousins' system
        /// Индекс цвета в системе Казинс
        /// </summary>
        public double? V_I { get; set; }
        public Vector3 ColorRGB { get; set; }

        #endregion


        #region Позиция в пространстве
        // Декартовы координаты звезды в системе, основанной на экваториальных координатах, видимых с Земли. +X в направлении весеннего равноденствия
        // (в эпоху 2000 г.), +Z в направлении северного небесного полюса и +Y в направлении прямого восхождения 6 часов, склонение 0 градусов.

        /// <summary>The X component of the vector.</summary>
        public float X;

        /// <summary>The Y component of the vector.</summary>
        public float Y;

        /// <summary>The Z component of the vector.</summary>
        public float Z;

        #endregion

        #region Скорость в пространстве
        // Декартовы компоненты скорости звезды в той же системе координат, которая описана выше. Они определяются из собственного
        // движения и радиальной скорости (если она известна). Единица скорости — парсеки в год; это небольшие значения (около 10-5–10-6), но они чрезвычайно упрощают
        // вычисления, используя парсеки в качестве базовых единиц для небесной карты.
        /// <summary>The X component of the vector.</summary>
        public float VX;

        /// <summary>The Y component of the vector.</summary>
        public float VY;

        /// <summary>The Z component of the vector.</summary>
        public float VZ;

        #endregion

        public static Vector3 GetColorFromSpectrum(string spectrum)
        {
            if (string.IsNullOrEmpty(spectrum))
                return new Vector3(1.0f, 1.0f, 1.0f);

            Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);

            if (spectrum.Contains("O"))
            {
                color.X = 0.0546875F;
                color.Y = 0.9453125F;
                color.Z = 0.9921875F;
            }
            else if (spectrum.Contains("B"))
            {
                color.X = 0.75390625F;
                color.Y = 0.984375F;
                color.Z = 0.99609375F;
            }
            else if (spectrum.Contains("A"))
            {
                color.X = 1.0F;
                color.Y = 1.0F;
                color.Z = 1.0F;
            }
            else if (spectrum.Contains("F"))
            {
                color.X = 0.99609375F;
                color.Y = 0.99609375F;
                color.Z = 0.75390625F;
            }
            else if (spectrum.Contains("G"))
            {
                color.X = 0.9921875F;
                color.Y = 0.9921875F;
                color.Z = 0.2109375F;
            }
            else if (spectrum.Contains("K"))
            {
                color.X = 0.99609375F;
                color.Y = 0.6796875F;
                color.Z = 0.20703125F;
            }
            else if (spectrum.Contains("M"))
            {
                color.X = 1.0F;
                color.Y = 0.46484375F;
                color.Z = 0.46484375F;
            }

            return color;
        }

        public override string ToString()
        {
            return ProperName; ;
        }
    }

}
