using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GaiaToHipparcosConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            RemoveByClusterName();
            args = new string[2];
            args[0] = "1754751399064O-result.csv";
            args[1] = "maint_cat_new.csv";

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: GaiaToHipparcosConverter <inputFile> <outputFile>");
                return;
            }

            string inputFile = args[0];
            string outputFile = args[1];

            try
            {
                ConvertCsv(inputFile, outputFile);
                Console.WriteLine($"Conversion completed successfully. Output saved to {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static List<Vector3> stars = new List<Vector3>();

        static void ConvertCsv(string inputPath, string outputPath)
        {
            var lines = File.ReadAllLines(inputPath);
            if (lines.Length == 0) return;

            var outputLines = new System.Collections.Generic.List<string>
            {
                "HIP;HD;HR;Gliese;BayerFlamsteed;GaiaDR3;ProperName;RAhms;DEdms;RA;Dec;Distance;PMRA;PMDec;RV;Mag;AbsMag;Spectrum;B_V;X;Y;Z;VX;VY;VZ;Cluster;ClusterName"
            };

            foreach (var line in lines.Skip(1))
            
            {
                var fields = line.Split(',');

                // Обработка полей с дефолтными значениями как у Солнца
                var source_id = SafeGetField(fields, 0);
                var hip = SafeGetField(fields, 1, "0");
                var hd = SafeGetField(fields, 2, "0");
                var ra_hip_epoch = SafeGetField(fields, 3, "0");
                var dec_hip_epoch = SafeGetField(fields, 4, "0");
                var parallax = SafeGetField(fields, 5, "0");
                var distance_pc = SafeGetField(fields, 6, "4.848E-06");
                var x_pc = SafeGetField(fields, 7, "0");
                var y_pc = SafeGetField(fields, 8, "0");
                var z_pc = SafeGetField(fields, 9, "0");
                var absMag = SafeGetField(fields, 10, "0");
                var pmra = SafeGetField(fields, 11, "0");
                var pmdec = SafeGetField(fields, 12, "0");
                var phot_g_mean_mag = SafeGetField(fields, 13, "-26.73");
                var teff_gspphot = SafeGetField(fields, 14);
                var bp_rp = SafeGetField(fields, 15);
                var spectrum = SafeGetField(fields, 16);
                var radius_gspphot = SafeGetField(fields, 17);
                var radius_flame = SafeGetField(fields, 18);

                double distance = 0;
                double.TryParse(distance_pc, NumberStyles.Any, CultureInfo.InvariantCulture, out distance);

                distance = distance - 7;

                double x = 0;
                double y = 0;
                double z = 0;

                double.TryParse(x_pc, NumberStyles.Any, CultureInfo.InvariantCulture, out x);
                double.TryParse(y_pc, NumberStyles.Any, CultureInfo.InvariantCulture, out y);
                double.TryParse(z_pc, NumberStyles.Any, CultureInfo.InvariantCulture, out z);

                x = x * 0.9459;
                y = y * 0.9512;
                z = z * 0.9358;

                var star = new Vector3((float)x, (float)y, (float)z);


                stars.Add(star);

                //var center = new Vector3(-6059.7573f, -17132.518f, 13220.686f);
                //var direction = new Vector3(-0.26964754f, -0.7623641f, 0.5882951f);

                //var newPos = CompressByMaxDistance(star, center, 44, (float)4.747672E+07);
                //x = newPos.X;
                //y = newPos.Y;
                //z = newPos.Z;


                if (string.IsNullOrEmpty(spectrum))
                {
                    // Спектральный класс (по умолчанию G2V как у Солнца)
                    spectrum = "B";
                    if (!string.IsNullOrEmpty(teff_gspphot) &&
                        double.TryParse(teff_gspphot, NumberStyles.Any, CultureInfo.InvariantCulture, out double teff))
                    {
                        spectrum = GetSpectralTypeFromTemperature(teff);
                    }
                }
                //if (!string.IsNullOrEmpty(bp_rp) &&
                //    double.TryParse(bp_rp, NumberStyles.Any, CultureInfo.InvariantCulture, out double bpRp))
                //{
                //    spectrum = GetSpectralTypeFromColorIndex(bpRp);
                //}

                // Преобразование координат
                string rahms = "00:00:00.000", dedms = "+00:00:00.000";
                if (!string.IsNullOrEmpty(ra_hip_epoch) && ra_hip_epoch != "0" &&
                    double.TryParse(ra_hip_epoch, NumberStyles.Any, CultureInfo.InvariantCulture, out double ra))
                {
                    rahms = DegreesToHms(ra);
                }
                if (!string.IsNullOrEmpty(dec_hip_epoch) && dec_hip_epoch != "0" &&
                    double.TryParse(dec_hip_epoch, NumberStyles.Any, CultureInfo.InvariantCulture, out double dec))
                {
                    dedms = DegreesToDms(dec);
                }


                // Формирование выходной строки
                var outputFields = new string[]
                {
                hip, hd,
                "0", "", "", source_id, "", // HR=0, остальные пустые
                rahms, dedms,
                ra_hip_epoch, dec_hip_epoch,
                distance.ToString(),
                pmra, pmdec,
                "0", // RV=0
                phot_g_mean_mag, absMag,
                spectrum,
                "0.656", // B_V=0.656
                x.ToString("G10", System.Globalization.CultureInfo.InvariantCulture),
                y.ToString("G10", System.Globalization.CultureInfo.InvariantCulture),
                z.ToString("G10", System.Globalization.CultureInfo.InvariantCulture),
                "0", "0", "0", // VX,VY,VZ=0
                "1","Hercules Cluster (M13)"
                };

                outputLines.Add(string.Join(";", outputFields));
            }

            //var center = new Vector3(-6059.7573f, -17132.518f, 13220.686f);
            //var direction = new Vector3(-0.26964754f, -0.7623641f, 0.5882951f);



            //Vector3 center = new Vector3(
            //    stars.Average(s => s.X),
            //    stars.Average(s => s.Y),
            //    stars.Average(s => s.Z)
            //);

            //Vector3 direction = Vector3.Normalize(center);

            var ddd = File.AppendText("3.txt");
            foreach (var ee in stars)
            {
                string strX = ee.X.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
                string strY = ee.Y.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
                string strZ = ee.Z.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
                ddd.WriteLine($"{strX}  {strY}  {strZ}");
            }
            File.AppendAllLines(outputPath, outputLines.Skip(1));

        }
        static int counteee = 0;
        static Vector3 CompressByMaxDistance(Vector3 star, Vector3 center, float maxRadius, float maxFoundProj)
        {
            Vector3 relative = star - center;
            float dist = relative.Length();

            if (dist <= maxRadius)
                return star;

            // Пропорциональное сжатие: maxFoundProj → maxRadius
            float scale = maxRadius / maxFoundProj;
            return center + relative * scale;
        }

        //static Vector3 CompressByMaxDistance(Vector3 star, Vector3 center, Vector3 direction, float maxDistance, float maxFoundProj)
        //{
        //    // Нормализуем направление
        //    direction = Vector3.Normalize(direction);

        //    // Вектор от центра до звезды
        //    Vector3 relative = star - center;

        //    // Проекция на направление
        //    float projLength = Vector3.Dot(relative, direction);

        //    // Масштабируем проекцию так, чтобы maxFoundProj стал maxDistance
        //    float scale = maxDistance / maxFoundProj;
        //    float scaledProj = projLength * scale;

        //    // Параллельная часть после масштабирования
        //    Vector3 parallel = scaledProj * direction;

        //    // Перпендикулярная часть без изменений
        //    Vector3 perpendicular = relative - (projLength * direction);

        //    return center + perpendicular + parallel;
        //}



        //static Vector3 CompressByMaxDistance(Vector3 star, Vector3 center, Vector3 direction, float maxDistance, float maxFoundProj)
        //{
        //    // нормализуем направление
        //    direction = Vector3.Normalize(direction);

        //    // относительный вектор от центра
        //    Vector3 relative = star - center;

        //    // проекция на direction (скалярная длина вдоль направления)
        //    float projLength = Vector3.Dot(relative, direction);

        //    // масштабируем только вдоль direction
        //    float scale = maxDistance / maxFoundProj;
        //    float newProjLength = projLength * scale;

        //    // параллельная и перпендикулярная части
        //    Vector3 parallel = newProjLength * direction;
        //    Vector3 perpendicular = relative - projLength * direction;

        //    // новое положение
        //    return center + perpendicular + parallel;
        //}

        static float CalculateMaxFoundProj(Vector3[] stars, Vector3 center)
        {
            float maxDist = 0f;
            foreach (var star in stars)
            {
                float dist = Vector3.Distance(star, center);
                if (dist > maxDist)
                    maxDist = dist;
            }
            return maxDist;
        }
        static Vector3 CalculateCenter(Vector3[] stars)
        {
            if (stars.Length == 0)
                throw new ArgumentException("Нет точек для вычисления центра");

            Vector3 sum = Vector3.Zero;
            foreach (var star in stars)
            {
                sum += star;
            }
            return sum / stars.Length;
        }
        //static Vector3 CompressByMaxDistance(Vector3 star, Vector3 center, Vector3 direction, float maxDistance)
        //{
        //    Vector3 relative = star - center;

        //    // проекция на направление (скалярная длина вдоль direction)
        //    float projLength = Vector3.Dot(relative, direction);

        //    // ограничиваем projLength максимальным расстоянием (по модулю)
        //    float clampedProjLength = Math.Clamp(projLength, -maxDistance, maxDistance);

        //    Vector3 parallel = clampedProjLength * direction;

        //    // перпендикулярная часть остаётся без изменений
        //    Vector3 perpendicular = relative - (projLength * direction);

        //    Vector3 newRelative = perpendicular + parallel;

        //    Vector3 newPos = center + newRelative;
        //    return newPos;
        //}


        static string SafeGetField(string[] fields, int index, string defaultValue = "")
        {
            return (fields.Length > index && !string.IsNullOrWhiteSpace(fields[index]))
                ? fields[index]
                : defaultValue;
        }

        static string GetSpectralTypeFromTemperature(double teff)
        {
            return teff switch
            {
                >= 30000 => "O",
                >= 10000 => "B",
                >= 7500 => "A",
                >= 6000 => "F",
                >= 5200 => "G",
                >= 3700 => "K",
                _ => "M"
            };
        }

        public static string GetSpectralTypeFromRvTemplateTeff(double? rvTemplateTeff)
        {
            // Проверка на null и некорректные значения
            if (rvTemplateTeff == null || rvTemplateTeff <= 0)
                return "Unknown";

            double teff = rvTemplateTeff.Value;

            // Определение спектрального класса по температуре
            if (teff >= 30000)
                return "O";
            else if (teff >= 10000)
                return "B";
            else if (teff >= 7500)
                return "A";
            else if (teff >= 6000)
                return "F";
            else if (teff >= 5200)
                return "G";
            else if (teff >= 3700)
                return "K";
            else if (teff >= 2400)
                return "M";
            else if (teff >= 1300)
                return "L";
            else if (teff >= 500)
                return "T";
            else
                return "Y";
        }

        public static string GetSpectralTypeFromColorIndex(double bpRp)
        {
            if (bpRp < -0.25) return "O";
            else if (bpRp < -0.05) return "B";
            else if (bpRp < 0.25) return "A";
            else if (bpRp < 0.50) return "F";
            else if (bpRp < 0.80) return "G";
            else if (bpRp < 1.40) return "K";
            else if (bpRp < 2.20) return "M";
            else if (bpRp < 3.50) return "L";
            else if (bpRp < 5.00) return "T";
            else return "Unknown"; // Для экстремальных или некорректных значений
        }

        static string DegreesToHms(double degrees)
        {
            degrees = degrees / 15; // Переводим в часы
            int hours = (int)degrees;
            double remaining = (degrees - hours) * 60;
            int minutes = (int)remaining;
            double seconds = (remaining - minutes) * 60;

            return $"{hours:D2}:{minutes:D2}:{seconds:00.000}";
        }

        static string DegreesToDms(double degrees)
        {
            int sign = Math.Sign(degrees);
            degrees = Math.Abs(degrees);
            int deg = (int)degrees;
            double remaining = (degrees - deg) * 60;
            int minutes = (int)remaining;
            double seconds = (remaining - minutes) * 60;

            return $"{(sign < 0 ? "-" : "+")}{deg:D2}:{minutes:D2}:{seconds:00.000}";
        }

        static void CreateCsvNewVersion()
        {
            string inputFilePath = "maint_cat.csv";
            string outputFilePath = "maint_cat_new.csv";

            try
            {
                // Чтение всех строк из исходного файла
                var lines = File.ReadAllLines(inputFilePath);

                if (lines.Length == 0)
                {
                    Console.WriteLine("Исходный файл пуст.");
                    return;
                }

                // Заголовок исходного файла
                string headerLine = lines[0];

                // Проверяем структуру заголовка
                var expectedHeader = "HIP;HD;HR;Gliese;BayerFlamsteed;ProperName;RAhms;DEdms;RA;Dec;Distance;PMRA;PMDec;RV;Mag;AbsMag;Spectrum;B_V;X;Y;Z;VX;VY;VZ";
                if (headerLine != expectedHeader)
                {
                    Console.WriteLine("Структура исходного файла не соответствует ожидаемой.");
                    return;
                }

                // Создаем новый заголовок с добавленными полями
                string newHeader = "HIP;HD;HR;Gliese;BayerFlamsteed;GaiaDR3;ProperName;RAhms;DEdms;RA;Dec;Distance;PMRA;PMDec;RV;Mag;AbsMag;Spectrum;B_V;X;Y;Z;VX;VY;VZ;Cluster;ClusterName";

                // Обрабатываем данные
                var newLines = new string[lines.Length];
                newLines[0] = newHeader;

                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(';');

                    // Вставляем пустые поля GaiaDR3 после BayerFlamsteed (поле 4) и Cluster, ClusterName в конец
                    var newFields = fields.Take(5) // HIP;HD;HR;Gliese;BayerFlamsteed
                        .Concat(new[] { "" })     // GaiaDR3 (пустое поле)
                        .Concat(fields.Skip(5))   // Остальные поля из исходной строки
                        .Concat(new[] { "0", "" }) // Cluster;ClusterName (пустые поля)
                        .ToArray();

                    newLines[i] = string.Join(";", newFields);
                }

                // Запись в новый файл
                File.WriteAllLines(outputFilePath, newLines);

                Console.WriteLine($"Файл успешно преобразован и сохранен как {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        static void RemoveByClusterName()
        {
            string inputFile = "maint_cat_new.csv";    // Исходный файл
            string outputFile = "maint_cat_new_new.csv";  // Файл после удаления
            string clusterNameToRemove = "Hercules Cluster (M13)";   // Удалить все записи с этим ClusterName

            try
            {
                // Читаем все строки из файла
                var lines = File.ReadAllLines(inputFile).ToList();

                if (lines.Count == 0)
                {
                    Console.WriteLine("Файл пуст!");
                    return;
                }

                // Получаем заголовок и индекс столбца ClusterName
                string header = lines[0];
                var columnNames = header.Split(';');
                int clusterNameIndex = Array.IndexOf(columnNames, "ClusterName");

                if (clusterNameIndex == -1)
                {
                    Console.WriteLine("Столбец 'ClusterName' не найден!");
                    return;
                }

                // Фильтруем строки (оставляем только те, где ClusterName НЕ равен заданному)
                var filteredLines = lines
                    .Where(line =>
                    {
                        if (line == header) return true;  // Всегда оставляем заголовок
                        var values = line.Split(';');
                        return values.Length > clusterNameIndex &&
                               values[clusterNameIndex] != clusterNameToRemove;
                    })
                    .ToList();

                // Записываем результат в новый файл
                File.WriteAllLines(outputFile, filteredLines);

                Console.WriteLine($"Удалено {lines.Count - filteredLines.Count} записей. Результат сохранен в {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}