using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HipparcosStarProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var xyz2 = Utils.GetCartesianCoordinates(0.00025315, -19.49883745, 45.662100456621);
            int counter1 = 0;
            int counter2 = 0;

            var stars1 = ReadCatalog1("hygxyz.csv", ",");
            var stars2 = ReadCatalog2("hip_main_new_new.csv", ";");
            foreach (var kvp in stars1)
            {
                if (kvp.HIP > 0)
                {
                    if (stars2.ContainsKey((int)kvp.HIP))
                    {
                        var tempStar = stars2[(int)kvp.HIP];
                        kvp.ProperName = tempStar.ProperName;

                        if(string.IsNullOrEmpty(kvp.HD) && !string.IsNullOrEmpty(tempStar.HD))
                        {
                            kvp.HD = tempStar.HD.Replace("HD ", "");
                            counter2++;
                        }
                        counter1++;
                    }
                }
            }


            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };
            using (var writer = new StreamWriter("hygxyz33333333.csv"))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteHeader<Star>();
                csv.NextRecord();
                foreach (var record in stars1)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }


            Console.ReadLine();

            ////// Проверка существования файла
            ////if (!File.Exists(inputFilePath))
            ////{
            ////    Console.WriteLine($"Файл {inputFilePath} не найден.");
            ////    return;
            ////}

            //var client = new HttpClient();

            //try
            //{
            //    var linesHIP = File.ReadAllLines("names.csv"); // Читаем все строки файла
            //    {
            //        for (int i = 107023; i < linesHIP.Length; i++)
            //        {
            //            using (FileStream fs = new FileStream("names2.csv", FileMode.Append, FileAccess.Write, FileShare.Read))
            //            using (StreamWriter sw = new StreamWriter(fs))
            //            {
            //                var line = linesHIP[i];
            //                string[] fields = line.Split(';');

            //                if (i == 0)
            //                {
            //                    sw.WriteLine($"{line};HR;StarName");
            //                }
            //                else
            //                {
            //                    // Получаем значение HIP из строки
            //                    string hipNumber = fields[2]; // Индексы корректны для формата данных

                               
            //                    string resp = await GetStarNameAsync(client, hipNumber);

            //                    var lines = resp.Split('\n');
            //                    string starName = ParseName(lines);
            //                    string hr = ParseHR(lines);

            //                    sw.WriteLine($"{line};{hr};{starName}");
            //                }
            //            }
            //            //    List<StarDataCompact> compacts = new List<StarDataCompact>();   

            //            //    var stars = Load();
            //            //    bool f = false;

            //            //    Dictionary<int ,StarDataFull> starsHIP = new Dictionary<int, StarDataFull>();

            //            //        var linesHIP = File.ReadAllLines(inputFilePath); // Читаем все строки файла
            //            //        {
            //            //            for (int i = 0; i < linesHIP.Length; i++)
            //            //            {
            //            //            using (FileStream fs = new FileStream("names.csv", FileMode.Append, FileAccess.Write, FileShare.Read))
            //            //            using (StreamWriter sw = new StreamWriter(fs))
            //            //            {
            //            //                var line = linesHIP[i];
            //            //                // Получаем значение HIP из строки
            //            //                string hipNumber = line.Substring(8, 6).Trim(); // Индексы корректны для формата данных

            //            //                string starName = await GetStarNameAsync(client, hipNumber);

            //            //                string[] fields = line.Split('|');
            //            //                StarDataFull? data = ParseStarData(fields);
            //            //                data.StarName = starName;
            //            //                if (data != null)
            //            //                    starsHIP.Add(data.HIP, data);

            //            //                // Получить все поля объекта
            //            //                var fileds = GetAllFields(data.GetType()).ToList();

            //            //                if (f == false)
            //            //                {
            //            //                    for (int j = 0; j < fileds.Count; j++)
            //            //                    {
            //            //                        FieldInfo field = fileds[j];
            //            //                        var name = field.Name;
            //            //                        name = name.Replace("k__BackingField", "");
            //            //                        name = name.Trim('<', '>');
            //            //                        // Записать значение в файл через запятую
            //            //                        if(j < fileds.Count-1)
            //            //                            sw.Write(name + ";");
            //            //                        else
            //            //                            sw.Write(name);
            //            //                    }
            //            //                    sw.WriteLine();

            //            //                    f = true;
            //            //                }

            //            //                for (int j = 0; j < fileds.Count; j++)
            //            //                {
            //            //                    FieldInfo field = fileds[j];
            //            //                    var name = field.Name;
            //            //                    // Получить значение поля
            //            //                    var value = field.GetValue(data);

            //            //                    // Привести значение к строке (обработка null)
            //            //                    string stringValue = value != null ? value.ToString() : "";

            //            //                    // Записать значение в файл через запятую

            //            //                    if (j < fileds.Count - 1)
            //            //                        sw.Write(stringValue + ";");
            //            //                    else
            //            //                        sw.Write(stringValue);
            //            //                }
            //            //                sw.WriteLine();


            //            //                Console.WriteLine($"Обработано: {i} HIP {hipNumber}, Star Name: {starName}");
            //            //            }
            //            //        }
            //            //    }

            //            //    using (FileStream fs = new FileStream("names.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            //            //    using (StreamWriter sw = new StreamWriter(fs))
            //            //    {
            //            //        foreach (var kvp in starsHIP.Values)
            //            //        {
            //            //            sw.WriteLine($"{kvp.HIP}|{kvp.StarName}");
            //            //            //StarDataCompact compact = new StarDataCompact();

            //            //        }
            //            //    }
            //            //SaveAllFieldsToCsv(stars, outputFilePath);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Ошибка: {ex.Message}");
            //}
            //finally
            //{
            //    client.Dispose();
            //}
        }

        /// <summary>
        /// Переводит угловое значение из формата градусов, минут и секунд в дробные градусы.
        /// </summary>
        /// <param name="degrees">Целое число градусов.</param>
        /// <param name="minutes">Целое число минут.</param>
        /// <param name="seconds">Число секунд (может быть дробным).</param>
        /// <param name="isNegative">Флаг, указывающий, отрицательное ли значение (по умолчанию false).</param>
        /// <returns>Угловое значение в дробных градусах.</returns>
        private static List<Star> ReadCatalog1(string fileName, string delimiter)
        {
            List<Star> stars = new List<Star>();
            //stars.Add(sol);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = delimiter };
            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var star = new Star
                    {
                        HIP = csv.GetField<int?>("HIP"),
                        HD = csv.GetField<string?>("HD"),
                        HR = csv.GetField<int?>("HR"),
                        Gliese = csv.GetField<string?>("Gliese"),
                        BayerFlamsteed = csv.GetField<string?>("BayerFlamsteed"),
                        ProperName = csv.GetField<string?>("ProperName"),
                        //RAhms = csv.GetField<string?>("RAhms"),
                        //DEdms = csv.GetField<string?>("DEdms"),
                        RA = csv.GetField<double?>("RA"),
                        Dec = csv.GetField<double?>("Dec"),
                        Distance = csv.GetField<double?>("Distance"),
                        PMRA = csv.GetField<double?>("PMRA"),
                        PMDec = csv.GetField<double?>("PMDec"),
                        Mag = csv.GetField<double?>("Mag"),
                        Spectrum = csv.GetField<string?>("Spectrum"),
                        B_V = csv.GetField<double?>("ColorIndex"),
                        X = csv.GetField<float>("X"),
                        Y = csv.GetField<float>("Y"),
                        Z = csv.GetField<float>("Z"),
                        VX = csv.GetField<float>("VX"),
                        VY = csv.GetField<float>("VY"),
                        VZ = csv.GetField<float>("VZ"),
                        //Pos = new Vector3() { X = csv.GetField<float>("X"), Y = csv.GetField<float>("Y"), Z = csv.GetField<float>("Z") },
                        //VPos = new Vector3() { X = csv.GetField<float>("VX"), Y = csv.GetField<float>("VY"), Z = csv.GetField<float>("VZ") }
                    };

                    //star.Spectrum = star.Spectrum.TrimEnd(' ');
                    //try
                    //{
                    //    star.HR = csv.GetField<int?>("HR");
                    //}
                    //catch
                    //{ 
                    //}

                    //// Переменные для результата
                    //int degrees;
                    //int minutes;
                    //double seconds;


                    //ParseDMS(star.RAhms, out degrees, out minutes, out seconds);
                    //star.RA = ConvertToDecimalDegrees(degrees, minutes, seconds, isNegative: false);

                    ////ParseDMS(star.DEdms, out degrees, out minutes, out seconds);
                    ////star.Dec = ConvertToDecimalDegrees(degrees, minutes, seconds, isNegative: false);

                    //double? plx = csv.GetField<double?>("Plx");
                    //if(plx.HasValue && plx != 0)
                    //    star.Distance = 1000 / plx;

                    //if (star.Distance != null)
                    //{
                    //    var xyz = GetCartesianCoordinates((double)star.RA, (double)star.Dec, (double)star.Distance);
                    //    star.X = (float)xyz.X;
                    //    star.Y = (float)xyz.Y;
                    //    star.Z = (float)xyz.Z;
                    //}
                    //star.ColorRGB = Star.GetColorFromSpectrum(star.Spectrum); public float X { get; set; }


                    stars.Add(star);
                }
            }

            return stars;
        }

        private static Dictionary<int, Star> ReadCatalog2(string fileName, string delimiter)
        {
            Dictionary<int, Star> stars = new Dictionary<int, Star>();
            //stars.Add(sol);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = delimiter };
            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var star = new Star
                    {
                        HIP = csv.GetField<int?>("HIP"),
                        HD = csv.GetField<string?>("HD"),
                        HR = csv.GetField<int?>("HR"),
                        Gliese = csv.GetField<string?>("Gliese"),
                        BayerFlamsteed = csv.GetField<string?>("BayerFlamsteed"),
                        ProperName = csv.GetField<string?>("ProperName"),
                        RAhms = csv.GetField<string?>("RAhms"),
                        DEdms = csv.GetField<string?>("DEdms"),
                        RA = csv.GetField<double?>("RA"),
                        Dec = csv.GetField<double?>("Dec"),
                        Distance = csv.GetField<double?>("Distance"),
                        PMRA = csv.GetField<double?>("PMRA"),
                        PMDec = csv.GetField<double?>("PMDec"),
                        Mag = csv.GetField<double?>("Mag"),
                        Spectrum = csv.GetField<string?>("Spectrum"),
                        B_V = csv.GetField<double?>("B_V"),
                        X = csv.GetField<float>("X"),
                        Y = csv.GetField<float>("Y"),
                        Z = csv.GetField<float>("Z")
                        //Pos = new Vector3() { X = csv.GetField<float>("X"), Y = csv.GetField<float>("Y"), Z = csv.GetField<float>("Z") },
                        //VPos = new Vector3() { X = csv.GetField<float>("VX"), Y = csv.GetField<float>("VY"), Z = csv.GetField<float>("VZ") }
                    };

                    //star.Spectrum = star.Spectrum.TrimEnd(' ');
                    //try
                    //{
                    //    star.HR = csv.GetField<int?>("HR");
                    //}
                    //catch
                    //{ 
                    //}

                    //// Переменные для результата
                    //int degrees;
                    //int minutes;
                    //double seconds;


                    //ParseDMS(star.RAhms, out degrees, out minutes, out seconds);
                    //star.RA = ConvertToDecimalDegrees(degrees, minutes, seconds, isNegative: false);

                    ////ParseDMS(star.DEdms, out degrees, out minutes, out seconds);
                    ////star.Dec = ConvertToDecimalDegrees(degrees, minutes, seconds, isNegative: false);

                    //double? plx = csv.GetField<double?>("Plx");
                    //if(plx.HasValue && plx != 0)
                    //    star.Distance = 1000 / plx;

                    //if (star.Distance != null)
                    //{
                    //    var xyz = GetCartesianCoordinates((double)star.RA, (double)star.Dec, (double)star.Distance);
                    //    star.X = (float)xyz.X;
                    //    star.Y = (float)xyz.Y;
                    //    star.Z = (float)xyz.Z;
                    //}
                    //star.ColorRGB = Star.GetColorFromSpectrum(star.Spectrum); public float X { get; set; }

                    if (!star.Distance.HasValue || star.Distance == 0)
                    {
                        Console.WriteLine(star.HIP);
                    }
                    if (star.HIP > 0 || (star.HIP == 0 && star.ProperName == "Sol"))
                        stars.Add((int)star.HIP, star);
                }
            }

            return stars;
        }



    }
}