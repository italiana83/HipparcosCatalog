using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HipparcosStarProcessor
{
    public static class Utils
    {
        /// <summary>
        /// переводит видимую звёздную величину в абсолютную, принимая видимую величину (𝑚) и расстояние(𝑑) в парсеках
        /// </summary>
        /// <param name="apparentMagnitude"></param>
        /// <param name="distanceInParsecs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double CalculateAbsoluteMagnitude(double apparentMagnitude, double distanceInParsecs)
        {
            if (distanceInParsecs <= 0)
            {
                throw new ArgumentException("Расстояние должно быть больше нуля.", nameof(distanceInParsecs));
            }

            // Формула для абсолютной звёздной величины
            double absoluteMagnitude = apparentMagnitude - 5 * (Math.Log10(distanceInParsecs) - 1);

            return absoluteMagnitude;
        }

        /// <summary>
        /// Переводит угловое значение из формата градусов, минут и секунд в дробные градусы.
        /// </summary>
        /// <param name="degrees">Целое число градусов.</param>
        /// <param name="minutes">Целое число минут.</param>
        /// <param name="seconds">Число секунд (может быть дробным).</param>
        /// <param name="isNegative">Флаг, указывающий, отрицательное ли значение (по умолчанию false).</param>
        /// <returns>Угловое значение в дробных градусах.</returns>
        public static double ConvertToDecimalDegrees(int degrees, int minutes, double seconds, bool isNegative = false)
        {
            double decimalDegrees = degrees + minutes / 60.0 + seconds / 3600.0;
            return isNegative ? -decimalDegrees : decimalDegrees;
        }

        public static async Task<string> GetStarNameAsync(HttpClient client, string hipNumber)
        {
            string url = $"http://simbad.u-strasbg.fr/simbad/sim-id?Ident=HIP+{hipNumber}&output.format=ASCII";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    return responseData; // Если имя не найдено
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static string ParseName(string[] lines)
        {
            foreach (var line in lines)
            {
                if (line.Contains("NAME")) // Примерный маркер для строки с именем
                {
                    // Регулярное выражение для извлечения всей строки после "NAME"
                    string pattern = @"NAME\s+(.+?)\s{2,}";
                    Match match = Regex.Match(line, pattern);
                    string result = "";

                    if (match.Success)
                    {
                        result = match.Groups[1].Value.Trim();
                    }

                    return result; // Извлекаем имя
                }
            }

            return "";
        }

        public static string ParseHR(string[] lines)
        {
            foreach (var line in lines)
            {
                if (line.Contains("HR ")) // Примерный маркер для строки с именем
                {
                    // Регулярное выражение для извлечения всей строки после "NAME"
                    string pattern = @"HR\s+(.+?)\s{2,}";
                    Match match = Regex.Match(line, pattern);
                    string result = "";

                    if (match.Success)
                    {
                        result = match.Groups[1].Value.Trim();
                    }

                    return result; // Извлекаем имя
                }
            }

            return "";
        }

        /// <summary>
        /// Получение названия звезды по номеру HIP
        /// </summary>
        /// <param name="client">HttpClient для выполнения запроса</param>
        /// <param name="hipNumber">Номер HIP</param>
        /// <returns>Название звезды</returns>
        //public static async Task<string> GetStarNameAsync(HttpClient client, string hipNumber)
        //{
        //    string url = $"http://simbad.u-strasbg.fr/simbad/sim-id?Ident=HIP+{hipNumber}&output.format=ASCII";
        //    try
        //    {
        //        HttpResponseMessage response = await client.GetAsync(url);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string responseData = await response.Content.ReadAsStringAsync();

        //            // Извлечение имени звезды из ответа
        //            var lines = responseData.Split('\n');
        //            foreach (var line in lines)
        //            {
        //                if (line.StartsWith("Object")) // Примерный маркер для строки с именем
        //                {
        //                    // Найти начальную позицию подстроки "Object"
        //                    int startIndex = line.IndexOf("Object") + "Object".Length;

        //                    // Найти позицию первого "---" после "Object"
        //                    int endIndex = line.IndexOf("---", startIndex);

        //                    // Извлечь подстроку между "Object" и "---"
        //                    string result = line.Substring(startIndex, endIndex - startIndex).Trim();

        //                    return result; // Извлекаем имя
        //                }
        //            }

        //            return "Name Not Found"; // Если имя не найдено
        //        }
        //        else
        //        {
        //            return "Request Failed";
        //        }
        //    }
        //    catch
        //    {
        //        return "Error";
        //    }
        //}

        public static StarDataFull ParseStarData(string[] fields)
        {
            if (fields.Length >= 77) // Минимальное количество полей для корректного парсинга
            {
                StarDataFull starData = new StarDataFull();

                // Заполнение свойств объекта данными из массива
                starData.Catalog = fields[0].Trim();                           // 1
                starData.HIP = Int32.Parse(fields[1]);                         // 9-14
                starData.Proxy = fields[2];                                    // 16
                starData.RAhms = fields[3].Trim();                             // 18-28
                starData.DEdms = fields[4].Trim();                             // 30-40
                starData.Vmag = ParseNullableDouble(fields[5]);                     // 42-46
                starData.VarFlag = ParseNullableInt(fields[6]);                     // 48
                starData.r_Vmag = fields[7];                                   // 50
                starData.RAdeg = ParseNullableDouble(fields[8]);                    // 52-63
                starData.DEdeg = ParseNullableDouble(fields[9]);                    // 65-76
                starData.AstroRef = fields[10];                                // 78
                starData.Plx = ParseNullableDouble(fields[11]);                     // 80-86
                starData.pmRA = ParseNullableDouble(fields[12]);                    // 88-95
                starData.pmDE = ParseNullableDouble(fields[13]);                    // 97-104
                starData.e_RAdeg = ParseNullableDouble(fields[14]);                 // 106-111
                starData.e_DEdeg = ParseNullableDouble(fields[15]);                 // 113-118
                starData.e_Plx = ParseNullableDouble(fields[16]);                   // 120-125
                starData.e_pmRA = ParseNullableDouble(fields[17]);                  // 127-132
                starData.e_pmDE = ParseNullableDouble(fields[18]);                  // 134-139
                starData.DE_RA = ParseNullableDouble(fields[19]);                   // 141-145
                starData.Plx_RA = ParseNullableDouble(fields[20]);                  // 147-151
                starData.Plx_DE = ParseNullableDouble(fields[21]);                  // 153-157
                starData.pmRA_RA = ParseNullableDouble(fields[22]);                 // 159-163
                starData.pmRA_DE = ParseNullableDouble(fields[23]);                 // 165-169
                starData.pmRA_Plx = ParseNullableDouble(fields[24]);                // 171-175
                starData.pmDE_RA = ParseNullableDouble(fields[25]);                 // 177-181
                starData.pmDE_DE = ParseNullableDouble(fields[26]);                 // 183-187
                starData.pmDE_Plx = ParseNullableDouble(fields[27]);                // 189-193
                starData.pmDE_pmRA = ParseNullableDouble(fields[28]);               // 195-199
                starData.F1 = ParseNullableInt(fields[29]);                         // 201-203
                starData.F2 = ParseNullableDouble(fields[30]);                      // 205-209
                starData.BTmag = ParseNullableDouble(fields[31]);                   // 218-223
                starData.e_BTmag = ParseNullableDouble(fields[32]);                 // 225-229
                starData.VTmag = ParseNullableDouble(fields[33]);                   // 231-236
                starData.e_VTmag = ParseNullableDouble(fields[34]);                 // 238-242
                starData.m_BTmag = fields[35];                                 // 244
                starData.B_V = ParseNullableDouble(fields[36]);                     // 246-251
                starData.e_B_V = ParseNullableDouble(fields[37]);                   // 253-257
                starData.r_B_V = fields[38];                                   // 259
                starData.V_I = ParseNullableDouble(fields[39]);                     // 261-264
                starData.e_V_I = ParseNullableDouble(fields[40]);                   // 266-269
                starData.r_V_I = fields[41];                                   // 271
                starData.CombMag = fields[42];                                 // 273
                starData.Hpmag = ParseNullableDouble(fields[43]);                   // 275-281
                starData.E_Hpmag = ParseNullableDouble(fields[44]);                 // 283-288
                starData.Hpscat = ParseNullableDouble(fields[45]);                  // 290-294
                starData.O_Hpmag = ParseNullableInt(fields[46]);                    // 296-298
                starData.M_Hpmag = fields[47];                                 // 300
                starData.Hpmax = ParseNullableDouble(fields[48]);                   // 302-306
                starData.HPmin = ParseNullableDouble(fields[49]);                   // 308-312
                starData.Period = ParseNullableDouble(fields[50]);                  // 314-320
                starData.HvarType = fields[51];                                // 322
                starData.MoreVar = fields[52];                                 // 324
                starData.MorePhoto = fields[53];                               // 326
                starData.CCDM = fields[54].Trim();                             // 328-337
                starData.N_CCDM = fields[55];                                  // 339
                starData.Nsys = ParseNullableInt(fields[56]);                       // 341-342
                starData.Ncomp = ParseNullableInt(fields[57]);                      // 344-345
                starData.MultFlag = fields[58];                                // 347
                starData.Source = fields[59];                                  // 349
                starData.Qual = fields[60];                                    // 351
                starData.M_HIP = fields[61].Trim();                            // 353-354
                starData.Theta = ParseNullableInt(fields[62]);                      // 356-358
                starData.Rho = ParseNullableDouble(fields[63]);                     // 360-366
                starData.E_Rho = ParseNullableDouble(fields[64]);                   // 368-372
                starData.DHp = ParseNullableDouble(fields[65]);                     // 374-378
                starData.E_DHp = ParseNullableDouble(fields[66]);                   // 380-383
                starData.Survey = fields[67];                                  // 385
                starData.Chart = fields[68];                                   // 387
                starData.Notes = fields[69];                                   // 389
                starData.HD = ParseNullableInt(fields[70]);                         // 391-396
                starData.BD = fields[71].Trim();                               // 398-407
                starData.CoD = fields[72].Trim();                              // 409-418
                starData.CPD = fields[73].Trim();                              // 420-429
                starData.VIred = ParseNullableDouble(fields[74]);                   // 431-434
                starData.SpType = fields[75];                                  // 436-447
                starData.r_SpType = fields[76];                                // 449

                return starData;
            }
            else
            {
                return null;
            }
        }

        public static async void SaveAllFieldsToCsv(List<StarDataFull> starDataList, string filePath)
        {
            var client = new HttpClient();

            // Создание CSV-файла
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Заголовки колонок (если необходимо)
                // writer.WriteLine("Catalog|HIP|Proxy|..."); // до 77 колонок

                foreach (var starData in starDataList)
                {
                    if (!starData.Plx.HasValue || starData.Plx <= 0)
                    {
                        throw new InvalidOperationException("Параллакс должен быть положительным для расчета расстояния.");
                    }
                    if (!starData.RAdeg.HasValue)
                    {
                        throw new InvalidOperationException("RAdeg не должен быть null для расчета расстояния.");
                    }
                    if (!starData.DEdeg.HasValue)
                    {
                        throw new InvalidOperationException("DEdeg не должен быть null для расчета расстояния.");
                    }

                    // Перевод параллакса в парсеки
                    double distance = 1000.0 / (double)starData.Plx;

                    var pos = GetCartesianCoordinates((double)starData.RAdeg, (double)starData.DEdeg, (double)starData.Plx);

                    string starName = await GetStarNameAsync(client, starData.HIP.ToString());

                    // Формирование строки для записи в файл
                    var line = $"{starData.HIP}|{starName}|" +
                              $"{starData.RAhms}|{starData.DEdms}|{starData.Vmag}|{starData.VarFlag}|" +
                              $"{starData.RAdeg}|{starData.DEdeg}|" +
                              $"{starData.Plx}|{distance}|{starData.pmRA}|{starData.pmDE}|" +
                              $"{starData.e_DEdeg}|{starData.e_Plx}|{starData.e_pmRA}|{starData.e_pmDE}|" +
                              $"{starData.DE_RA}|{starData.Plx_RA}|{starData.Plx_DE}|{starData.pmRA_RA}|" +
                              $"{starData.pmRA_DE}|{starData.pmRA_Plx}|{starData.pmDE_RA}|{starData.pmDE_DE}|" +
                              $"{starData.pmDE_Plx}|{starData.pmDE_pmRA}|{starData.F1}|{starData.F2}|" +
                              $"{starData.BTmag}|{starData.e_BTmag}|{starData.VTmag}|{starData.e_VTmag}|" +
                              $"{starData.m_BTmag}|{starData.B_V}|{starData.e_B_V}|{starData.r_B_V}|" +
                              $"{starData.V_I}|{starData.e_V_I}|{starData.r_V_I}|{starData.CombMag}|" +
                              $"{starData.Hpmag}|{starData.E_Hpmag}|{starData.Hpscat}|{starData.O_Hpmag}|" +
                              $"{starData.M_Hpmag}|{starData.Hpmax}|{starData.HPmin}|{starData.Period}|" +
                              $"{starData.HvarType}|{starData.MoreVar}|{starData.MorePhoto}|{starData.CCDM}|" +
                              $"{starData.N_CCDM}|{starData.Nsys}|{starData.Ncomp}|{starData.MultFlag}|" +
                              $"{starData.Source}|{starData.Qual}|{starData.M_HIP}|{starData.Theta}|" +
                              $"{starData.Rho}|{starData.E_Rho}|{starData.DHp}|{starData.E_DHp}|" +
                              $"{starData.Survey}|{starData.Chart}|{starData.Notes}|{starData.HD}|" +
                              $"{starData.BD}|{starData.CoD}|{starData.CPD}|{starData.VIred}|" +
                              $"{starData.SpType}|{starData.r_SpType}";

                    // Запись строки в файл
                    writer.WriteLine(line);
                }
            }

            Console.WriteLine($"Файл успешно сохранён: {filePath}");
        }

        public static (double X, double Y, double Z) GetCartesianCoordinates(double RAdeg, double DEdeg, double distance)
        {
            // Перевод в радианы
            double raRad = RAdeg * Math.PI / 180.0;
            double deRad = DEdeg * Math.PI / 180.0;

            // Расчёт декартовых координат
            double x = distance * Math.Cos(deRad) * Math.Cos(raRad);
            double y = distance * Math.Cos(deRad) * Math.Sin(raRad);
            double z = distance * Math.Sin(deRad);
            //57.67077	86.2089	46.42553

            return (x, y, z);
        }

        public static double? ParseNullableDouble(string value) =>
     double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : (double?)null;

        public static int? ParseNullableInt(string value) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : (int?)null;

        public static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }

        /// <summary>
        /// Извлекает градусы, минуты и секунды из строки в формате "-00 00 06.55".
        /// </summary>
        /// <param name="input">Строка в формате "±DD MM SS.ss".</param>
        /// <param name="degrees">Выходной параметр для градусов.</param>
        /// <param name="minutes">Выходной параметр для минут.</param>
        /// <param name="seconds">Выходной параметр для секунд.</param>
        public static void ParseDMS(string input, out int degrees, out int minutes, out double seconds)
        {
            // Убираем возможные лишние пробелы и разбиваем строку
            var parts = input.Trim().Split(' ');

            // Парсим градусы, минуты и секунды
            degrees = int.Parse(parts[0]);
            minutes = int.Parse(parts[1]);
            seconds = double.Parse(parts[2].Replace(".", ","));
        }


        //public static Dictionary<int, Star> Load()
        //{
        //    Dictionary<int, Star> stars = new Dictionary<int, Star>();

        //    var filePath = "hygxyz.csv"; // Путь к вашему файлу

        //    using (var reader = new StreamReader(filePath))
        //    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //    {
        //        csv.Read();
        //        csv.ReadHeader();
        //        while (csv.Read())
        //        {
        //            var star = new Star
        //            {
        //                StarID = csv.GetField<int?>("StarID"),
        //                HIP = csv.GetField<int?>("HIP"),
        //                HD = csv.GetField<int?>("HD"),
        //                HR = csv.GetField<int?>("HR"),
        //                Gliese = csv.GetField<string>("Gliese"),
        //                BayerFlamsteed = csv.GetField<string>("BayerFlamsteed"),
        //                ProperName = csv.GetField<string>("ProperName"),
        //                RA = csv.GetField<double?>("RA"),
        //                Dec = csv.GetField<double?>("Dec"),
        //                Distance = csv.GetField<double?>("Distance"),
        //                PMRA = csv.GetField<double?>("PMRA"),
        //                PMDec = csv.GetField<double?>("PMDec"),
        //                RV = csv.GetField<double?>("RV"),
        //                Mag = csv.GetField<double?>("Mag"),
        //                AbsMag = csv.GetField<double?>("AbsMag"),
        //                Spectrum = csv.GetField<string>("Spectrum"),
        //                B_V = csv.GetField<double?>("ColorIndex"),
        //                Pos = new Vector3() { X = csv.GetField<float>("X"), Y = csv.GetField<float>("Y"), Z = csv.GetField<float>("Z") },
        //                VPos = new Vector3() { X = csv.GetField<float>("VX"), Y = csv.GetField<float>("VY"), Z = csv.GetField<float>("VZ") }
        //            };

        //            int hip = star.HIP ?? 0;
        //            if (hip == 0 && star.ProperName != "Sol")
        //                continue;

        //            stars.Add(hip, star);

        //        }
        //    }

        //    return stars;
        //}
    }
}
