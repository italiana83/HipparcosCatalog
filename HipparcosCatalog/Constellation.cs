using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class Constellation
    {
        public string Name { get; }
        public List<Star> Stars { get; set; } = new List<Star>();
        public List<(int, int)> Connections { get; set; } = new List<(int, int)>(); // Индексы звёзд для соединений
        public Dictionary<int, string> Hips { get; set; }

        public int[] Indices
        {
            get
            {
                // Преобразуем List<(int, int)> в uint[]
                int[] indices = Connections.SelectMany(c => new[] { (int)c.Item1, (int)c.Item2 }).ToArray();
                return indices;
            }
        }

        public Vector3 ColorRGB { get; set; }

        public bool Visible { get; set; }

        public Constellation(string name, Vector3 colorRGB) 
        {
            Name = name;
            ColorRGB = colorRGB;
        }

        public override string ToString()
        {
            return Name;
        }
        //Первая группа
        //Андромеда: new Vector3(0.6f, 0.8f, 1.0f) (нежно-голубой)
        //Большая Медведица: new Vector3(0.9f, 0.4f, 0.6f) (ярко-розовый)
        //Возничий: new Vector3(0.8f, 1.0f, 0.6f) (светло-зелёный)
        //Волопас: new Vector3(1.0f, 0.6f, 0.2f) (оранжевый)
        //Волосы Вероники: new Vector3(0.6f, 0.6f, 1.0f) (фиолетово-синий)
        //Геркулес: new Vector3(0.8f, 0.8f, 0.0f) (жёлто-зелёный)
        //Гончие Псы: new Vector3(0.6f, 1.0f, 0.8f) (бирюзовый)
        //Дева: new Vector3(1.0f, 0.6f, 0.6f) (персиковый)
        //Дельфин: new Vector3(0.4f, 0.8f, 1.0f) (ярко-голубой)
        //Дракон: new Vector3(0.8f, 0.4f, 1.0f) (фиолетовый)
        //Жираф: new Vector3(1.0f, 0.8f, 0.4f) (жёлто-оранжевый)
        //Змееносец: new Vector3(0.8f, 1.0f, 0.8f) (пастельно-зелёный)
        //Змея: new Vector3(0.8f, 0.4f, 0.4f) (бордовый)
        //Кассиопея: new Vector3(1.0f, 0.4f, 0.8f) (ярко-розовый)
        //Лебедь: new Vector3(1.0f, 0.4f, 0.4f) (ярко-красный)
        //Лев: new Vector3(1.0f, 0.8f, 0.0f) (золотистый)
        //Лира: new Vector3(0.4f, 0.6f, 1.0f) (индиго)
        //Лисичка: new Vector3(0.8f, 0.6f, 0.2f) (коричневато-золотой)
        //Малая Медведица: new Vector3(0.4f, 0.8f, 0.8f) (аквамарин)
        //Малый Конь: new Vector3(0.6f, 0.4f, 1.0f) (фиолетово-синий)
        //Малый Лев: new Vector3(1.0f, 0.4f, 0.4f) (алый)
        //Овен: new Vector3(1.0f, 0.6f, 0.0f) (ярко-оранжевый)
        //Орел: new Vector3(0.4f, 1.0f, 0.8f) (бирюзово-зелёный)
        //Пегас: new Vector3(0.8f, 0.8f, 1.0f) (светло-сиреневый)
        //Рыбы: new Vector3(0.6f, 1.0f, 1.0f) (пастельно-голубой)
        //Рысь: new Vector3(0.6f, 0.6f, 0.2f) (оливковый)
        //Северная Корона: new Vector3(0.6f, 1.0f, 0.4f) (неоново-зелёный)
        //Стрела: new Vector3(0.4f, 0.4f, 1.0f) (синий)
        //Треугольник: new Vector3(1.0f, 1.0f, 0.4f) (светло-жёлтый)
        //Цефей: new Vector3(1.0f, 1.0f, 0.2f) (ярко-жёлтый)
        //Щит: new Vector3(0.8f, 1.0f, 0.2f) (зелёно-жёлтый)
        //Ящерица: new Vector3(0.4f, 0.8f, 0.4f) (зеленовато-голубой)
        //Персей: new Vector3(1.0f, 0.6f, 1.0f) (розово-фиолетовый)
        //Вторая группа
        //Близнецы: new Vector3(0.2f, 0.6f, 1.0f) (ярко-голубой)
        //Большой Пес: new Vector3(1.0f, 0.2f, 0.2f) (ярко-красный)
        //Весы: new Vector3(0.6f, 1.0f, 0.6f) (пастельно-зелёный)
        //Водолей: new Vector3(0.2f, 1.0f, 1.0f) (ярко-бирюзовый)
        //Волк: new Vector3(0.6f, 0.4f, 0.2f) (коричневый)
        //Ворон: new Vector3(0.4f, 0.4f, 0.6f) (тёмно-синий)
        //Гидра: new Vector3(0.8f, 0.8f, 0.6f) (пастельный песочный)
        //Голубь: new Vector3(0.6f, 0.6f, 1.0f) (светло-фиолетовый)
        //Единорог: new Vector3(0.8f, 0.4f, 0.2f) (золотисто-коричневый)
        //Жертвенник: new Vector3(1.0f, 0.8f, 0.6f) (персиковый)
        //Живописец: new Vector3(0.6f, 0.6f, 0.4f) (оливковый)
        //Журавль: new Vector3(0.4f, 0.8f, 0.8f) (нежно-бирюзовый)
        //Заяц: new Vector3(1.0f, 0.6f, 0.8f) (розовый)
        //Золотая Рыба: new Vector3(1.0f, 1.0f, 0.0f) (ярко-жёлтый)
        //Индеец: new Vector3(0.8f, 0.2f, 0.6f) (фуксия)
        //Киль: new Vector3(0.2f, 0.6f, 0.8f) (голубовато-зелёный)
        //Кит: new Vector3(0.6f, 0.2f, 0.8f) (фиолетовый)
        //Козерог: new Vector3(0.8f, 1.0f, 0.4f) (неоново-зелёный)
        //Компас: new Vector3(0.4f, 0.6f, 0.8f) (индиго)
        //Корма: new Vector3(0.8f, 0.6f, 1.0f) (лиловый)
        //Летучая Рыба: new Vector3(0.2f, 1.0f, 0.6f) (бирюзово-зелёный)
        //Малый Пес: new Vector3(0.6f, 0.8f, 1.0f) (светло-голубой)
        //Микроскоп: new Vector3(0.4f, 0.2f, 1.0f) (тёмно-фиолетовый)
        //Муха: new Vector3(0.6f, 0.8f, 0.6f) (светло-зелёный)
        //Насос: new Vector3(0.8f, 0.4f, 0.6f) (сиреневый)
        //Наугольник: new Vector3(0.4f, 1.0f, 0.4f) (салатовый)
        //Октант: new Vector3(0.6f, 0.6f, 0.6f) (серебристый)
        //Орион: new Vector3(0.4f, 1.0f, 0.6f) (салатовый)
        //Павлин: new Vector3(0.2f, 0.4f, 1.0f) (тёмно-голубой)
        //Паруса: new Vector3(0.4f, 0.8f, 1.0f) (светло-синий)
        //Печь: new Vector3(0.8f, 0.6f, 0.2f) (золотисто-коричневый)
        //Райская Птица: new Vector3(1.0f, 0.2f, 0.8f) (розово-фуксия)
        //Рак: new Vector3(1.0f, 0.6f, 0.4f) (персиково-оранжевый)
        //Резец: new Vector3(0.6f, 0.6f, 0.8f) (серо-синий)
        //Секстант: new Vector3(0.4f, 1.0f, 0.2f) (салатовый)
        //Сетка: new Vector3(0.8f, 0.2f, 0.4f) (тёмно-розовый)
        //Скорпион: new Vector3(0.8f, 0.0f, 0.6f) (тёмно-фиолетовый)
        //Скульптор: new Vector3(0.6f, 0.6f, 0.4f) (оливковый)
        //Столовая Гора: new Vector3(0.4f, 0.6f, 0.2f) (тёмно-зелёный)
        //Стрелец: new Vector3(0.8f, 0.2f, 0.2f) (бордовый)
        //Телескоп: new Vector3(0.4f, 0.4f, 0.8f) (синий)
        //Телец: new Vector3(1.0f, 0.4f, 0.0f) (оранжевый)
        //Тукан: new Vector3(0.4f, 0.8f, 0.4f) (зеленовато-голубой)
        //Феникс: new Vector3(1.0f, 0.2f, 0.2f) (алый)
        //Хамелеон: new Vector3(0.6f, 1.0f, 0.2f) (неоново-зелёный)
        //Центавр: new Vector3(0.4f, 1.0f, 0.8f) (ярко-бирюзовый)
        //Циркуль: new Vector3(0.8f, 0.8f, 0.4f) (жёлто-зелёный)
        //Часы: new Vector3(0.8f, 0.4f, 0.2f) (коричневато-золотой)
        //Чаша: new Vector3(0.6f, 0.2f, 0.2f) (бордовый)
        //Эридан: new Vector3(0.2f, 0.6f, 1.0f) (ярко-голубой)
        //Южная Корона: new Vector3(1.0f, 0.6f, 1.0f) (розово-фиолетовый)
        //Южная Рыба: new Vector3(0.2f, 0.8f, 0.6f) (бирюзово-зелёный)
        //Южный Змей: new Vector3(0.6f, 0.4f, 1.0f) (фиолетово-синий)
        //Южный Крест: new Vector3(0.8f, 0.4f, 0.8f) (сиреневый)
        //Южный Треугольник: new Vector3(0.8f, 0.4f, 0.8f) (сиреневый)
    }
}
