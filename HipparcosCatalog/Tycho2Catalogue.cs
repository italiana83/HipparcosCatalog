using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class Tycho2Catalogue
    {
        public List<Tycho2CatalogueItem> LoadTycho2Catalog(string filePath)
        {
            var catalog = new List<Tycho2CatalogueItem>();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    var item = new Tycho2CatalogueItem
                    {
                        TYC1 = int.Parse(line.Substring(0, 4).Trim()),
                        TYC2 = int.Parse(line.Substring(5, 5).Trim()),
                        TYC3 = int.Parse(line.Substring(11, 1).Trim()),
                        PFlag = line.Substring(13, 1).Trim(),
                        RAMdeg = ParseNullableDouble(line.Substring(15, 12).Trim()),
                        DEMdeg = ParseNullableDouble(line.Substring(28, 12).Trim()),
                        PmRA = ParseNullableDouble(line.Substring(41, 7).Trim()),
                        PmDE = ParseNullableDouble(line.Substring(49, 7).Trim()),
                        E_RAMdeg = ParseNullableInt(line.Substring(57, 3).Trim()),
                        E_DEMdeg = ParseNullableInt(line.Substring(61, 3).Trim()),
                        E_PmRA = ParseNullableDouble(line.Substring(65, 4).Trim()),
                        E_PmDE = ParseNullableDouble(line.Substring(70, 4).Trim()),
                        EpRAM = ParseNullableDouble(line.Substring(75, 7).Trim()),
                        EpDEM = ParseNullableDouble(line.Substring(83, 7).Trim()),
                        Num = ParseNullableInt(line.Substring(91, 2).Trim()),
                        Q_RAMdeg = ParseNullableDouble(line.Substring(94, 3).Trim()),
                        Q_DEMdeg = ParseNullableDouble(line.Substring(98, 3).Trim()),
                        Q_PmRA = ParseNullableDouble(line.Substring(102, 3).Trim()),
                        Q_PmDE = ParseNullableDouble(line.Substring(106, 3).Trim()),
                        BTMag = ParseNullableDouble(line.Substring(110, 6).Trim()),
                        E_BTMag = ParseNullableDouble(line.Substring(117, 5).Trim()),
                        VTMag = ParseNullableDouble(line.Substring(123, 6).Trim()),
                        E_VTMag = ParseNullableDouble(line.Substring(130, 5).Trim()),
                        Prox = ParseNullableInt(line.Substring(136, 3).Trim()),
                        TYC = line.Substring(140, 1).Trim(),
                        HIP = ParseNullableInt(line.Substring(142, 6).Trim()),
                        CCDM = line.Substring(148, 3).Trim(),
                        RADeg = ParseNullableDouble(line.Substring(152, 12).Trim()),
                        DEDeg = ParseNullableDouble(line.Substring(165, 12).Trim()),
                        EpRA1990 = ParseNullableDouble(line.Substring(178, 4).Trim()),
                        EpDE1990 = ParseNullableDouble(line.Substring(183, 4).Trim()),
                        E_RADeg = ParseNullableDouble(line.Substring(188, 5).Trim()),
                        E_DEDeg = ParseNullableDouble(line.Substring(194, 5).Trim()),
                        PosFlg = line.Substring(200, 1).Trim(),
                        Corr = ParseNullableDouble(line.Substring(202, 4).Trim())
                    };

                    catalog.Add(item);
                }
            }

            return catalog;
        }

        private double? ParseNullableDouble(string input)
        {
            if (double.TryParse(input, out double result))
                return result;
            return null;
        }

        private int? ParseNullableInt(string input)
        {
            if (int.TryParse(input, out int result))
                return result;
            return null;
        }
    }

    public class Tycho2CatalogueItem
    {
        /// <summary>
        /// [1,9537]+= TYC1 from TYC or GSC (1) 
        /// </summary>
        public int TYC1 { get; set; }
        /// <summary>
        /// [1,12121]  TYC2 from TYC or GSC (1) 
        /// </summary>
        public int TYC2 { get; set; }
        /// <summary>
        /// [1,3]      TYC3 from TYC (1) 
        /// </summary>
        public int TYC3 { get; set; }
        /// <summary>
        /// [ PX] mean position flag (2) 
        /// </summary>
        public string PFlag { get; set; }
        /// <summary>
        /// []? Mean Right Asc, ICRS, epoch=J2000 (3) 
        /// </summary>
        public double? RAMdeg { get; set; }
        /// <summary>
        /// []? Mean Decl, ICRS, at epoch=J2000 (3) 
        /// </summary>
        public double? DEMdeg { get; set; }
        /// <summary>
        /// ? Proper motion in RA*cos(dec) (12) 
        /// </summary>
        public double? PmRA { get; set; }
        /// <summary>
        /// ? Proper motion in Dec (12)
        /// </summary>
        public double? PmDE { get; set; }
        /// <summary>
        /// [3,183]? s.e. RA*cos(dec),at mean epoch (5) 
        /// </summary>
        public int? E_RAMdeg { get; set; }
        /// <summary>
        /// [1,184]? s.e. of Dec at mean epoch (5) 
        /// </summary>
        public int? E_DEMdeg { get; set; }
        /// <summary>
        /// [0.2,11.5]? s.e. prop mot in RA*cos(dec)(5) 
        /// </summary>
        public double? E_PmRA { get; set; }
        /// <summary>
        /// [0.2,10.3]? s.e. of proper motion in Dec(5) 
        /// </summary>
        public double? E_PmDE { get; set; }
        /// <summary>
        /// [1915.95,1992.53]? mean epoch of RA (4) 
        /// </summary>
        public double? EpRAM { get; set; }
        /// <summary>
        /// [1911.94,1992.01]? mean epoch of Dec (4) 
        /// </summary>
        public double? EpDEM { get; set; }
        /// <summary>
        /// [2,36]? Number of positions used 
        /// </summary>
        public int? Num { get; set; }
        /// <summary>
        /// [0.0,9.9]? Goodness of fit for mean RA (6) 
        /// </summary>
        public double? Q_RAMdeg { get; set; }
        /// <summary>
        /// [0.0,9.9]? Goodness of fit for mean Dec (6) 
        /// </summary>
        public double? Q_DEMdeg { get; set; }
        /// <summary>
        /// [0.0,9.9]? Goodness of fit for pmRA (6) 
        /// </summary>
        public double? Q_PmRA { get; set; }
        /// <summary>
        /// [0.0,9.9]? Goodness of fit for pmDE (6) 
        /// </summary>
        public double? Q_PmDE { get; set; }
        /// <summary>
        /// [2.183,16.581]? Tycho-2 BT magnitude (7) 
        /// </summary>
        public double? BTMag { get; set; }
        /// <summary>
        /// [0.014,1.977]? s.e. of BT (7) 
        /// </summary>
        public double? E_BTMag { get; set; }
        /// <summary>
        /// [1.905,15.193]? Tycho-2 VT magnitude (7) 
        /// </summary>
        public double? VTMag { get; set; }
        /// <summary>
        /// [0.009,1.468]? s.e. of VT (7) 
        /// </summary>
        public double? E_VTMag { get; set; }
        /// <summary>
        /// [3,999] proximity indicator (8) 
        /// </summary>
        public int? Prox { get; set; }
        /// <summary>
        /// [T] Tycho-1 star (9) 
        /// </summary>
        public string TYC { get; set; }
        /// <summary>
        /// [1,120404]? Hipparcos number
        /// </summary>
        public int? HIP { get; set; }
        /// <summary>
        /// CCDM component identifier for HIP stars(10) 
        /// </summary>
        public string CCDM { get; set; }
        /// <summary>
        /// Observed Tycho-2 Right Ascension, ICRS 
        /// </summary>
        public double? RADeg { get; set; }
        /// <summary>
        /// Observed Tycho-2 Declination, ICRS 
        /// </summary>
        public double? DEDeg { get; set; }
        /// <summary>
        /// [0.81,2.13]  epoch-1990 of RAdeg 
        /// </summary>
        public double? EpRA1990 { get; set; }
        /// <summary>
        /// [0.72,2.36]  epoch-1990 of DEdeg 
        /// </summary>
        public double? EpDE1990 { get; set; }
        /// <summary>
        /// s.e.RA*cos(dec), of observed Tycho-2 RA (5)
        /// </summary>
        public double? E_RADeg { get; set; }
        /// <summary>
        /// s.e. of observed Tycho-2 Dec (5) 
        /// </summary>
        public double? E_DEDeg { get; set; }
        /// <summary>
        /// [ DP] type of Tycho-2 solution (11) 
        /// </summary>
        public string PosFlg { get; set; }
        /// <summary>
        /// [-1,1] correlation (RAdeg,DEdeg) 
        /// </summary>
        public double? Corr { get; set; }

        //Примечание(1) :
        //Идентификатор TYC формируется из номера региона GSC
        //(TYC1), текущего номера в регионе(TYC2) и идентификатора компонента
        //(TYC3), который обычно равен 1. Некоторые текущие номера, не относящиеся к GSC
        //, были сформированы для первого каталога Tycho и для Tycho-2.
        //Рекомендуемое обозначение звезды содержит дефис между
        //номерами TYC, например, TYC 1-13-1.

        //Примечание(2) :
        //' ' = нормальное среднее положение и собственное движение.
        //'P' = среднее положение, собственное движение и т. д.относятся к
        //фотоцентру двух записей Tycho-2, где величины BT
        //использовались для взвешивания положений.
        //'X' = нет среднего положения, нет собственного движения.

        //Примечание (3):
        //Среднее положение является взвешенным средним для каталогов,
        //участвующих в определении собственного движения.Затем это среднее значение было приведено к
        //эпохе 2000.0 с помощью вычисленного собственного движения.Подробнее см.в примечании (2) выше.Tycho-2 — один из нескольких каталогов, используемых для определения
        //среднего положения и собственного движения.Наблюдаемое положение Tycho-2
        //приведено в полях RAdeg и DEdeg.

        //Примечание(4):
        //Средние эпохи приведены в юлианских годах.

        //Примечание(5):
        //Ошибки основаны на моделях ошибок.

        //Примечание(6):
        //Эта степень соответствия — это отношение ошибки, основанной на рассеянии, к ошибке, основанной на модели.Она определяется только при Num > 2. Значения,
        //превышающие 9,9, усекаются до 9,9.

        //Примечание (7):
        //Пусто, если нет доступной величины.Всегда указывается либо BTmag, либо VTmag. Приблизительная фотометрия Джонсона может быть получена как:
        //V = VT - 0,090*(BT-VT)
        //B-V = 0,850*(BT-VT)
        //Подробности см.в разделе 1.3 тома 1 "The Hipparcos and Tycho Catalogues",
        //ESA SP-1200, 1997.

        //Примечание (8):
        //Расстояние в единицах 100 мсд до ближайшей записи в основном каталоге Tycho-2
        //или дополнении.Расстояние вычисляется для
        //эпохи 1991.25. Значение 999 (т.е. 99,9 угловых секунд) дается, если
        //расстояние превышает 99,9 угловых секунд.

        //Примечание(9):
        //' ' = звезда Tycho-1 не найдена в пределах 0,8 угловых секунд(качество 1-8)
        //или 2,4 угловых секунд(качество 9).
        //'T' = это звезда Tycho-1. Идентификатор Tycho-1 указан в
        //начале записи.Для звезд Tycho-1, разрешенных в
        //Tycho-2 как близкая пара, оба компонента помечены как
        //звезда Tycho-1, а Tycho-1 TYC3 назначен
        //самому яркому (VT) компоненту.
        //Звезды, только HIP, указанные в Tycho-1, не помечены как звезды Tycho-1.

        //Примечание (10):
        //Идентификаторы компонентов CCDM для двойных или множественных звезд Hipparcos,
        //вносящих вклад в эту запись Tycho-2. Для решений фотоцентра все
        //компоненты в пределах 0,8 угловых секунд вносят вклад.Для решений двойной звезды любой
        //неразрешенный компонент в пределах 0,8 угловых секунд вносит вклад. Для решений одиночной звезды
        //предсказанный сигнал от близких звезд обычно
        //вычитался при анализе количества фотонов, и такие звезды
        //поэтому не вносят вклад в решение.Компоненты
        //приведены в лексическом порядке.

        //Примечание (11):
        //' ' = нормальная обработка, близкие звезды вычитались, когда это было возможно.
        //'D' = обработка двойной звезды.Было обнаружено две звезды.Спутник обычно включается как отдельная запись Tycho-2, но, возможно, был
        //отклонен.
        //'P' = обработка фотоцентра, близкие звезды не вычитались. Эта
        //специальная обработка применялась к известным или предполагаемым двойным
        //которые не были успешно (или надежно) разрешены при обработке
        //двойной звезды Tycho-2.

        //Примечание (12): Некоторые звезды Hipparcos(имеющие положительное число в столбце HIP)
        //не имеют собственных движений; они практически все находятся в нескольких системах.
    }
}
