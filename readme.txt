Просто мое хобби))
Визуализация звездного каталога Hipparcos.
Для визуализации используется OpenTK.
Библиотека Open Toolkit — это быстрая низкоуровневая привязка C# к OpenGL.

Взвездном каталоге представлены следующие данные по более чем 118 тыс. звезд:
d — расстояние до объекта в парсеках (вычисляется из параллакса);
α и δ — прямое восхождение и склонение в радианах;
собственные движения;
звёздные величины в фотометрических системах B и V.
и т.д.

По этим данным можно получить Декартовы координаты (X,Y,Z) в галактической системе (в данном случае Солнце находится в центре).

Используем следующие формулы:

X=d*cos(δ)*cos(α)
Y=d*cos(δ)*sin(α)
Z=d*sin(δ)
Где:
d — расстояние до объекта в парсеках
α и δ — прямое восхождение и склонение в радианах.

-------------------------------------------------------------------------------------------------------------

Just my hobby))
Visualization of the Hipparcos star catalog.
OpenTK is used for visualization.
The Open Toolkit library is a fast, low-level C# binding for OpenGL.

The star catalog contains the following data on more than 118 thousand stars:
d — distance to the object in parsecs (calculated from parallax);
α and δ — right ascension and declination in radians;
proper motions;
stellar magnitudes in the photometric systems B and V.
etc.

From this data, you can get Cartesian coordinates (X,Y,Z) in the galactic system (in this case, the Sun is in the center).

We use the following formulas:

X=d*cos(δ)*cos(α)
Y=d*cos(δ)*sin(α)
Z=d*sin(δ)
Where:
d — distance to the object in parsecs
α and δ — right ascension and declination in radians.