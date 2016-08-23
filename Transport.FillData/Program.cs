using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Transport.DataAccessLayer;
using System.Windows;
using Transport.Common;
using Transport.Domain.Entities;

namespace Transport.FillData
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CountRoutesOnEachArea();
            Console.WriteLine("Выполнено!");
            Console.ReadKey(true);
        }

        /// <summary>
        /// расчет количества маршрутов в каждой зоне
        /// </summary>
        private static void CountRoutesOnEachArea()
        {
            var routes = File.ReadLines(@"C:\Users\Ярослав Мартынов\Dropbox\Маршруты\Маршруты_существующие.txt");
            var dirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Results");
            Directory.CreateDirectory(dirPath);
            var writer = new StreamWriter(Path.Combine(dirPath, @"Маршрутов_в_зоне.txt"))
            {
                AutoFlush = true
            };

            List<Area> areas;
            using (var db = new TransportContext())
            {
                areas = db.Areas.ToList();
            }

            var routesInArea = new List<string>[areas.Count];
            for (var i = 0; i < routesInArea.Length; i++)
            {
                routesInArea[i] = new List<string>();
            }

            foreach (var route in routes)
            {
                var values = route.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in values.Skip(1).ToList())
                {
                    var areaId = int.Parse(value);
                    routesInArea[areaId].Add(values[0]);
                }
            }

            foreach (var area in areas)
            {
                writer.WriteLine($"{area.AreaId}. {area.Name}: Маршрутов: {routesInArea[area.AreaId].Count}. ({string.Join(";", routesInArea[area.AreaId])})");
            }
        }

        /// <summary>
        /// Расчет характеристик маршрутов в сети
        /// </summary>
        private static void CountRoutesCharacteristicsInNetwork()
        {
            var adjacencyMatrix = GetAdjacencyMatrix(0);
            var demandMatrix = GetDemandMatrixFromFile(@"C:\Users\Ярослав Мартынов\Documents\Visual Studio 2015\Projects\Transport\Transport.FillData\bin\Debug\Results\matrix.txt");
            var routes = File.ReadLines(@"C:\Users\Ярослав Мартынов\Dropbox\Маршруты\Маршруты_существующие.txt");
            var routesChar = new List<RouteCharacteristics>();

            var dirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Results");
            Directory.CreateDirectory(dirPath);
            var routeChrWriter = new StreamWriter(Path.Combine(dirPath, @"routesChr.txt"))
            {
                AutoFlush = true
            };

            foreach (var route in routes)
            {
                var values = route.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                var rc = RouteCharacteristicsCounter(values.Skip(1).Select(int.Parse).ToArray(), adjacencyMatrix, demandMatrix);
                routesChar.Add(rc);

                routeChrWriter.WriteLine($"{values[0]}: Длина: {rc.Length}; Зон: {rc.AreasCount}; Пассажиров: {rc.DirectTravalers}; Плотность: {rc.DirectTravalersDensity}");
            }

        }

        /// <summary>
        /// Получение матрицы корреспонденций из файла
        /// </summary>
        /// <param name="filePath">путь к файлу</param>
        /// <returns></returns>
        private static double[,] GetDemandMatrixFromFile(string filePath)
        {
            var lines = File.ReadLines(filePath).Skip(1).ToArray(); // пропуск 1 строки, там заголовки
            var size = lines.Length;
            var demandMatrix = new double[size, size];

            for (var i = 0; i < size; i++)
            {
                var line = lines[i];
                var values = line.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

                for (var j = 1; j < values.Length; j++)
                {
                    demandMatrix[i, j] = Double.Parse(values[j]);
                }
            }

            return demandMatrix;
        }

        private class RouteCharacteristics
        {
            public double Length { get; set; }
            public int AreasCount { get; set; }
            public double DirectTravalers { get; set; }
            public double DirectTravalersDensity => Math.Abs(Length) < 1e-5 ? 0.0 : DirectTravalers / Length;
        }

        /// <summary>
        /// Метод расчитывает и выводит характеристики маршрута:
        /// длина маршрута, количество зон в маршруте, плотность пассажиров прямого сообщения,
        /// </summary>
        /// <param name="route">список остановок маршрута</param>
        /// <param name="adjacentMatrix">матрица смежности</param>
        /// <param name="demandMatrix">матрица корреспонденций</param>
        private static RouteCharacteristics RouteCharacteristicsCounter(int[] route, int[,] adjacentMatrix, double[,] demandMatrix)
        {
            var routeChar = new RouteCharacteristics();

            // количество остановок
            routeChar.AreasCount = route.Length;

            for (var i = 0; i < route.Length - 1; i++)
            {
                // длина маршрута
                routeChar.Length += adjacentMatrix[route[i], route[i + 1]];
                for (var j = i + 1; j < route.Length; j++)
                {
                    // количество пассажиров прямого сообщения
                    routeChar.DirectTravalers += demandMatrix[route[i], route[j]];
                }
            }

            return routeChar;
        }


        /// <summary>
        /// Метод выполняет проверку, что зоны внутри марщрутов связаны 
        /// попарно между собой, т.е. нет явной ошибки при создании маршрутов
        /// </summary>
        /// <param name="routesFilePath">Путь к файлу с маршрутами</param>
        private static void CheckAllAreasInRoutesConnected(string routesFilePath)
        {
            var routes = File.ReadLines(routesFilePath);

            using (var db = new TransportContext())
            {
                foreach (var route in routes)
                {
                    var routeAreasIds = route.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (routeAreasIds.Length < 1)
                    {
                        throw new InvalidOperationException($"В маршруте {routeAreasIds[0]} нет остановок");
                    }

                    for (var i = 1; i < routeAreasIds.Length - 1; i++)
                    {
                        var originId = int.Parse(routeAreasIds[i]);
                        var destId = int.Parse(routeAreasIds[i + 1]);
                        if (!db.AreaRoutes.Any(ar => ar.OriginId == originId && ar.DestinationId == destId))
                        {
                            Console.WriteLine($"Нет пути в маршруте {routeAreasIds[0]} из {routeAreasIds[i]} в {routeAreasIds[i + 1]}");
                            Console.ReadKey(true);
                        }
                    }

                    Console.WriteLine($"{routeAreasIds[0]} проверен");
                }
            }
        }

        /// <summary>
        /// Метод выполняет проверку, что все зоны используются в маршрутах
        /// ВЫводит не используемые зоны
        /// </summary>
        /// <param name="routesFilePath">Путь к файлу с маршрутами</param>
        private static void CheckAllAreasIsUsedInRoutes(string routesFilePath)
        {
            var routes = File.ReadLines(routesFilePath);

            using (var db = new TransportContext())
            {
                var areas = db.Areas.ToList();
                foreach (var route in routes)
                {
                    var routeAreasIds = route.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (routeAreasIds.Length < 1)
                    {
                        throw new InvalidOperationException($"В маршруте {routeAreasIds[0]} нет остановок");
                    }

                    for (var i = 1; i < routeAreasIds.Length; i++)
                    {
                        var id = int.Parse(routeAreasIds[i]);
                        var area = areas.Find(a => a.AreaId == id);
                        if (area != null)
                        {
                            areas.Remove(area);
                        }
                    }

                    if (!areas.Any()) break;
                }

                if (areas.Any())
                {
                    Console.WriteLine("Список неиспользуемых в маршрутах зон:");
                    areas.ForEach(a => Console.WriteLine($"{a.AreaId} - {a.Name}"));
                }
                else
                {
                    Console.WriteLine("Все зоны задействованы в маршрутах");
                }
            }
        }

        private static void DeleteReturnWay()
        {
            var noroute = new[,]
            {
                { 87, 11}, { 246, 16}, { 195, 21}, { 249, 27}, { 104, 32}, { 212, 60}, { 102, 60}, { 110, 62}, { 204, 87},
                { 258, 87}, { 150, 87}, { 195, 90}, { 27, 104}, { 56, 110}, { 119, 123}, { 262, 119}, { 110, 119}, { 229, 127},
                { 130, 129}, { 129, 144}, { 258, 150}, { 194, 264}, { 123, 195}, { 261, 242}, { 245, 258}, { 260, 253}
            };

            using (var db = new TransportContext())
            {
                for (var i = 0; i < noroute.GetLength(0); i++)
                {
                    var origin = noroute[i, 0];
                    var dest = noroute[i, 1];
                    var route = db.AreaRoutes.FirstOrDefault(ar => ar.OriginId == origin && ar.DestinationId == dest);
                    if (route == null) continue;

                    db.AreaRoutes.Remove(route);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Создаем обратный путь между остановками
        /// </summary>
        private static void AddReturnWay()
        {
            var noroute = new[,]
            {
                { 87, 11}, { 246, 16}, { 195, 21}, { 249, 27}, { 104, 32}, { 212, 60}, { 102, 60}, { 110, 62}, { 204, 87},
                { 258, 87}, { 150, 87}, { 195, 90}, { 27, 104}, { 56, 110}, { 119, 123}, { 262, 119}, { 110, 119}, { 229, 127},
                { 130, 129}, { 129, 144}, { 258, 150}, { 194, 264}, { 123, 195}, { 261, 242}, { 245, 258}, { 260, 253}
            };

            using (var db = new TransportContext())
            {
                foreach (var areaRoute in db.AreaRoutes.ToList())
                {
                    // если обратного пути нет
                    if (!db.AreaRoutes.Any(ar => ar.OriginId == areaRoute.DestinationId
                                                && ar.DestinationId == areaRoute.OriginId))
                    {
                        // если путь в списке к удалению, то пропускаем
                        var skipRoute = false;
                        for (var i = 0; i < noroute.GetLength(0); i++)
                        {
                            if (noroute[i, 0] == areaRoute.OriginId && noroute[i, 1] == areaRoute.DestinationId)
                            {
                                skipRoute = true;
                                break;
                            }
                        }

                        if (skipRoute) continue;

                        // добавляем новый обратный маршрут
                        db.AreaRoutes.Add(new AreaRoutes
                        {
                            OriginId = areaRoute.DestinationId,
                            DestinationId = areaRoute.OriginId,
                            Distance = areaRoute.Distance
                        });
                    }
                }

                db.SaveChanges();
            }
        }

        private static void OrderAreaIds()
        {
            using (var db = new TransportContext())
            {
                var i = 0;
                foreach (var area in db.Areas.ToList())
                {
                    if (area.AreaId != i)
                    {
                        // сдвигаем
                        var areaRoutes =
                            db.AreaRoutes.Where(ar => ar.OriginId == area.AreaId || ar.DestinationId == area.AreaId)
                                .ToList();

                        db.AreaRoutes.RemoveRange(areaRoutes);
                        foreach (var building in db.Buildings.Where(b => b.AreaId == area.AreaId))
                        {
                            building.AreaId = null;
                        }

                        foreach (var busstop in db.Busstops.Where(bs => bs.AreaId == area.AreaId))
                        {
                            busstop.AreaId = null;
                        }

                        db.Areas.Remove(area);
                        db.SaveChanges();

                        foreach (var areaRoute in areaRoutes)
                        {
                            if (areaRoute.OriginId == area.AreaId) areaRoute.OriginId = i;
                            if (areaRoute.DestinationId == area.AreaId) areaRoute.DestinationId = i;
                        }

                        area.AreaId = i;
                        db.Areas.Add(area);
                        db.AreaRoutes.AddRange(areaRoutes);

                        foreach (var building in db.Buildings.Where(b => b.AreaId == null))
                        {
                            building.AreaId = i;
                        }

                        foreach (var busstop in db.Busstops.Where(bs => bs.AreaId == null))
                        {
                            busstop.AreaId = i;
                        }

                        db.SaveChanges();
                    }

                    i++;
                }
            }
        }

        /// <summary>
        /// Метод вычисляем матрицу корреспонденций
        /// </summary>
        private static void FindCorrespMatrix()
        {
            // население города
            Console.Write("Введите населения для расчета: ");
            var input = Console.ReadLine();
            int population;
            if (!int.TryParse(input, out population) || population < 0)
            {
                Console.WriteLine("Некорректное значение населения");
                return;
            }

            Area[] areas;

            using (var db = new TransportContext())
            {
                areas = db.Areas.ToArray();
            }

            var dirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Results");
            Directory.CreateDirectory(dirPath);
            var matrixWriter = new StreamWriter(Path.Combine(dirPath, @"matrix.txt"));
            var deptWriter = new StreamWriter(Path.Combine(dirPath, @"departure.txt"));
            var arrWriter = new StreamWriter(Path.Combine(dirPath, @"arrival.txt"));
            var costWriter = new StreamWriter(Path.Combine(dirPath, @"cost.txt"));


            var n = areas.Length;
            const double gamma = 0.065;
            const double tolerance = 1e-10;
            // аппроксимирующая функция
            var f = new Func<double, double>(a => Math.Exp(-gamma * a));
            // массив значений расстояний между остановками с примененой аппроксимирующей функцией
            var fc = new double[n, n];

            // Заполнение заголовочной строки для матрицы корреспонденций и матрицы затрат
            matrixWriter.Write("\t;");
            costWriter.Write("\t;");
            for (var i = 0; i < n; i++)
            {
                matrixWriter.Write($"{areas[i].Name};");
                costWriter.Write($"{areas[i].Name};");
            }
            matrixWriter.WriteLine();
            costWriter.WriteLine();

            // считываем матрицу достижимости с весами
            var map = GetAdjacencyMatrix(int.MaxValue);
            // вычисляем расстояния между всеми парами вершин
            FloydWarshallSearch(ref map);
            // применение функции f к массиву расстояний
            for (var i = 0; i < n; i++)
            {
                costWriter.Write($"{areas[i].Name};");
                for (var j = 0; j < n; j++)
                {
                    if (map[i, j] == int.MaxValue) map[i, j] = 0;
                    var distInKm = map[i, j] / 1000.0;
                    costWriter.Write($"{distInKm};");
                    if (i == j || map[i, j] == 0) continue;
                    fc[i, j] = f(distInKm);
                }
                costWriter.WriteLine();
                costWriter.Flush();
            }


            // заполнение массивов отправления и прибытия
            var s = areas.Select(a => a.DeparturePct * population).ToArray();
            var d = areas.Select(a => a.ArrivalPct * population).ToArray();

            for (var i = 0; i < n; i++)
            {
                deptWriter.Write($"{areas[i].Name};{s[i]};{Environment.NewLine}");
                arrWriter.Write($"{areas[i].Name};{d[i]};{Environment.NewLine}");
            }

            // инициализация матрицы корреспонденций начальными значениями
            var p = new double[n, n];
            for (var i = 0; i < n; i++)
            {
                // расчет знаменателя
                var sum = 0.0;
                for (var l = 0; l < n; l++)
                {
                    sum += d[l] * fc[i, l];
                }

                for (var j = 0; j < n; j++)
                {
                    p[i, j] = s[i] * d[j] * fc[i, j] * Math.Pow(sum, -1);
                }
            }

            var g = new double[n, n];
            var q = new double[n];
            var r = new double[n];
            bool next; // определяет необходимость выполнения новой итерации
            var k = 0; // счетчик итераций
            while (true)
            {
                next = false;

                for (var j = 0; j < n; j++)
                {
                    var sum = 0.0;
                    for (var l = 0; l < n; l++)
                    {
                        sum += p[l, j];
                    }

                    for (var i = 0; i < n; i++)
                    {
                        var val = sum - d[j];
                        if (Math.Abs(val) < tolerance || val < 0)
                        {
                            g[i, j] = p[i, j];
                        }
                        else
                        {
                            g[i, j] = p[i, j] * d[j] * Math.Pow(sum, -1);
                            next = true;
                        }
                    }
                }

                if (!next) break;

                for (var i = 0; i < n; i++)
                {
                    var sumD = 0.0;
                    var sumS = 0.0;

                    for (var j = 0; j < n; j++)
                    {
                        sumS += g[i, j];
                        sumD += g[j, i];
                    }
                    q[i] = s[i] - sumS;
                    r[i] = d[i] - sumD;
                }

                for (var i = 0; i < n; i++)
                {
                    var sum = 0.0;

                    for (var l = 0; l < n; l++)
                    {
                        sum += r[l] * fc[i, l];
                    }

                    for (var j = 0; j < n; j++)
                    {
                        p[i, j] = g[i, j] + q[i] * r[j] * fc[i, j] * Math.Pow(sum, -1);
                    }
                }
                k++;
            }

            var popSum = 0.0;
            for (var i = 0; i < n; i++)
            {
                matrixWriter.Write($"{areas[i].Name};");
                for (var j = 0; j < n; j++)
                {
                    popSum += p[i, j];
                    matrixWriter.Write($"{p[i, j]};");
                }
                matrixWriter.WriteLine();
                matrixWriter.Flush();
            }

            Console.WriteLine($"Итераций: {k}, Проверочная сумма: {popSum}");
        }

        private static void FloydWarshallSearch(ref int[,] map)
        {
            var n = map.GetLength(0);
            var m = map.GetLength(1);
            if (n != m) throw new ArgumentException($"Размерность матрицы смежности некорректна: {n} != {m}");

            for (var k = 0; k < n; k++)
            {
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i == j || map[i, k] == int.MaxValue || map[k, j] == int.MaxValue) continue;
                        // длина пути из i в j через k
                        var pathLen = map[i, k] + map[k, j];
                        if (pathLen < map[i, j])
                        {
                            map[i, j] = pathLen;
                        }
                    }
                }
            }
        }

        private static int[,] GetAdjacencyMatrix(int seed)
        {
            using (var db = new TransportContext())
            {
                var dim = db.Areas.Count();
                var matrix = new int[dim, dim];

                for (var i = 0; i < dim; i++)
                {
                    for (var j = 0; j < dim; j++)
                    {
                        matrix[i, j] = seed;
                    }
                }

                foreach (var areaRoute in db.AreaRoutes)
                {
                    matrix[areaRoute.OriginId, areaRoute.DestinationId] = areaRoute.Distance;
                }

                return matrix;
            }
        }

        private static void CountDirectDistanceBetweenAreas()
        {
            Console.WriteLine("Расчет расстояния между остановками по прямой");
            var dirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Results");
            var filePath = Path.Combine(dirPath, @"distances.txt");
            Directory.CreateDirectory(dirPath);
            var fileStream = new FileStream(filePath, FileMode.Create);
            using (var writer = new StreamWriter(fileStream))
            {
                using (var db = new TransportContext())
                {
                    var areas = db.Areas.Select(a => new { a.AreaId, a.Location }).ToArray();

                    for (var i = 0; i < areas.Length; i++)
                    {
                        for (var j = 0; j < areas.Length; j++)
                        {
                            var dist = areas[i].Location.Distance(areas[j].Location);
                            var val = dist.HasValue ? (long)dist.Value : 0;
                            writer.Write($"{areas[i].AreaId},{areas[j].AreaId},{val};");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        private static void CountDepArrPcs()
        {
            Console.WriteLine("Расчет количества людей прибывающих и отправляющихся из зоны");
            using (var db = new TransportContext())
            {
                var sumPeople = db.Areas.Sum(a => a.People);
                var sumOrgs = db.OrgFils.Count();

                foreach (var area in db.Areas.Include(a => a.Buildings.Select(b => b.OrgFils)))
                {
                    area.DeparturePct = (double)area.People / sumPeople;
                    var orgsInArea = area.Buildings.SelectMany(b => b.OrgFils).Count();
                    area.ArrivalPct = (double)orgsInArea / sumOrgs;
                }

                db.SaveChanges();
            }
        }

        delegate void WriteTo(string line = null);

        delegate void ProgressCallback(int current, int max);

        private static void PrintLine(char symbol, int count, WriteTo write)
        {
            for (var i = 0; i < count; i++)
            {
                write(symbol.ToString());
            }
            write(Environment.NewLine);
        }

        private static void PrintOrgs(WriteTo write, ProgressCallback progress = null)
        {
            using (var db = new TransportContext())
            {

                var p = 0;
                var max = db.Areas.Count();
                // для каждой зоны записываем свой блок в файл
                foreach (var area in db.Areas.Include(a => a.Buildings
                    .Select(b => b.OrgFils
                        .Select(of => of.Org)
                        .Select(o => o.OrgRubs))))
                {
                    const int lineLenght = 60;
                    PrintLine('*', lineLenght, write);
                    var buildingsCount = area.Buildings.Count;
                    var orgFilsCount = area.Buildings.SelectMany(b => b.OrgFils).Count();
                    write($"{area.AreaId}. {area.Name} " + Environment.NewLine
                                      + $"Зданий: {buildingsCount} Жителей: {area.People} Организаций: {orgFilsCount}" + Environment.NewLine);
                    PrintLine('-', lineLenght, write);

                    var n = 1;
                    foreach (var orgFil in area.Buildings.SelectMany(b => b.OrgFils))
                    {
                        write($"{n++}. {orgFil.Org.Name}" + Environment.NewLine);
                    }
                    write(Environment.NewLine);
                    progress?.Invoke(++p, max);
                }
                write(Environment.NewLine);
            }
        }

        private static void PrintOrgsToFile()
        {
            var dirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Results");
            var filePath = Path.Combine(dirPath, @"organizations.txt");
            Directory.CreateDirectory(dirPath);
            var orgsFs = new FileStream(filePath, FileMode.Create);
            using (var writer = new StreamWriter(orgsFs))
            {
                var pb = new ProgressBar(title: "Cписок организаций по зонам... ");
                PrintOrgs(writer.Write, pb.Report);
                pb.Stop();
                Console.WriteLine();
            }
        }

        private static void PrintOrgsToConsole()
        {
            PrintOrgs(Console.Write);
        }

        /// <summary>
        /// Добавление расстояний между зонами
        /// </summary>
        private static void AddRoutes()
        {
            using (var db = new TransportContext())
            {
                while (true)
                {
                    Console.Write("***Введите идентификатор начальной зоны: ");
                    var input = Console.ReadLine();
                    int originId;
                    if (Int32.TryParse(input, out originId) && originId != 0)
                    {
                        var origin = db.Areas.Find(originId);
                        if (origin == null) continue;
                        Console.WriteLine($"Введите построчно идентификатор зоны и расстояние до нее от {origin.Name} (enter - закончить)");
                        do
                        {
                            input = Console.ReadLine();
                            var destIdDist = input.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                            if (destIdDist.Length == 2)
                            {
                                int destId;
                                int distance;
                                if (Int32.TryParse(destIdDist[0], out destId)
                                    && Int32.TryParse(destIdDist[1], out distance)
                                    && originId != destId)
                                {
                                    var dest = db.Areas.Find(destId);
                                    var areaRoute = new AreaRoutes
                                    {
                                        OriginId = originId,
                                        DestinationId = destId,
                                        Distance = distance
                                    };
                                    db.AreaRoutes.Add(areaRoute);
                                    try
                                    {
                                        db.SaveChanges();
                                        Console.WriteLine($"Путь от {origin.Name} до {dest.Name} равный {distance} сохранен");
                                    }
                                    catch (DbUpdateException e)
                                    {
                                        db.AreaRoutes.Remove(areaRoute);
                                        Console.WriteLine("Дублирование записи, повторите ввод");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Ошибка ввода");
                                }
                            }
                        } while (input != String.Empty);

                    }
                    else break;
                }
            }
        }

        /// <summary>
        /// Группировка зданий по зонам
        /// </summary>
        private static void GroupBuildingToAreas()
        {
            using (var db = new TransportContext())
            {
                // ищем ближайшие зоны для каждого здания
                var sw = Stopwatch.StartNew();
                var buildings = db.Buildings.Where(b => b.AreaId == null);
                var pb = new ProgressBar(buildings.Count() - 1, "Группировка зданий по зонам... ");
                var p = 0;
                foreach (var building in buildings)
                {
                    var minDistance = double.MaxValue;
                    long minAreaId = 0;

                    sw = Stopwatch.StartNew();
                    foreach (var area in db.Areas)
                    {
                        var curDistance = area.Location.Distance(building.Location) ?? 0;
                        if (curDistance < minDistance)
                        {
                            minAreaId = area.AreaId;
                            minDistance = curDistance;
                        }
                    }

                    if (minAreaId != 0)
                    {
                        building.AreaId = minAreaId;
                    }

                    pb.Report(++p);
                }
                pb.Pause();
                db.SaveChanges();
                Console.WriteLine(Environment.NewLine + $"Группировка зданий заняла {sw.Elapsed.TotalSeconds} сек.");
            }
        }

        /// <summary>
        /// Ручное разделение остановок на две
        /// </summary>
        private static void DivideBusstop()
        {
            using (var db = new TransportContext())
            {
                int idForDiv;
                Console.Write(Environment.NewLine + "Введите Id зоны для разделения: ");
                var input = Console.ReadLine();
                if (Int32.TryParse(input, out idForDiv) && idForDiv != 0)
                {
                    var area = db.Areas.Include(a => a.Busstops).FirstOrDefault(a => a.AreaId == idForDiv);
                    if (area == null) return;

                    foreach (var busstop in area.Busstops)
                    {
                        Console.WriteLine($"{busstop.BusstopId}. {busstop.Name}");
                    }

                    Console.WriteLine("Введите идентификаторы остановок для новой зоны: ");
                    input = Console.ReadLine();
                    var bsIds = input.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    var newAreaBs = new List<Busstop>();
                    foreach (var bsId in bsIds)
                    {
                        int id;
                        if (!Int32.TryParse(bsId, out id)) continue;
                        var bs = area.Busstops.FirstOrDefault(b => b.BusstopId == id);
                        if (bs == null) continue;
                        newAreaBs.Add(bs);
                    }

                    Console.WriteLine("Создать новую зону со списком остановок:");
                    newAreaBs.ForEach(b => Console.WriteLine($"{b.BusstopId}. {b.Name}"));
                    Console.Write("Y/N: ");

                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        // создание новой зоны
                        var newArea = new Area
                        {
                            AreaId = db.Areas.Max(a => a.AreaId) + 1,
                            Name = GetAreaName(newAreaBs)
                        };
                        // добавление/исключение остановок
                        newAreaBs.ForEach(b =>
                        {
                            newArea.Busstops.Add(b);
                            area.Busstops.Remove(b);
                        });
                        // расчет центра
                        newArea.Location = CalculateAreaCentre(newAreaBs);

                        // обработка предыдущей зоны
                        // изменение имени
                        area.Name = GetAreaName(area.Busstops.ToList());
                        // пересчет центра
                        area.Location = CalculateAreaCentre(area.Busstops.ToList());
                        db.Areas.Add(newArea);
                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Ручное объединение двух остановок
        /// </summary>
        private static void UniteBusstops()
        {
            using (var db = new TransportContext())
            {
                int id1, id2;
                Console.Write(Environment.NewLine + "Введите Id зон для объединения: ");
                var input = Console.ReadLine();
                var idsArr = input?.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (idsArr != null && idsArr.Length == 2
                    && Int32.TryParse(idsArr[0], out id1)
                    && Int32.TryParse(idsArr[1], out id2))
                {
                    var area1 = db.Areas.Include(a => a.Busstops).FirstOrDefault(a => a.AreaId == id1);
                    var area2 = db.Areas.Include(a => a.Busstops).FirstOrDefault(a => a.AreaId == id2);
                    if (area1 == null || area2 == null) return;

                    var newAreaBs = area1.Busstops.ToList();
                    newAreaBs.AddRange(area2.Busstops);
                    Console.WriteLine("Создать новую зону со списком остановок:");
                    newAreaBs.ForEach(b => Console.WriteLine($"{b.BusstopId}. {b.Name}"));
                    Console.Write("Y/N: ");

                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        var uniteArea = area1;
                        uniteArea.Name = GetAreaName(newAreaBs);
                        // добавление остановок
                        newAreaBs.ForEach(b => uniteArea.Busstops.Add(b));
                        // расчет центра
                        uniteArea.Location = CalculateAreaCentre(newAreaBs);
                        db.Areas.Remove(area2);
                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Меню ручного разделение и объединение остановок
        /// </summary>
        private static void DivideUniteBusstopsMenu()
        {
            while (true)
            {
                Console.WriteLine(Environment.NewLine + "*** Разделение (1) и объединение (2)");
                var key = Console.ReadKey().KeyChar;
                // раздление
                if (key == '1')
                {
                    DivideBusstop();
                }
                // объединение
                else if (key == '2')
                {
                    UniteBusstops();
                }
                else break;
            }
        }

        private static string GetAreaName(IEnumerable<Busstop> busstops)
        {
            var sb = new StringBuilder();
            var bsNames = busstops.Select(b => b.Name).Distinct().ToList();
            foreach (var bsName in bsNames)
            {
                sb.Append(bsName);

                if (bsName != bsNames.Last())
                {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Автоматическое объединение остановок
        /// </summary>
        private static void AggregateBusstops()
        {
            using (var db = new TransportContext())
            {
                Console.Write("Очистка зон.. ");
                foreach (var busstop in db.Busstops)
                {
                    busstop.AreaId = null;
                }

                foreach (var building in db.Buildings)
                {
                    building.AreaId = null;
                }

                db.Areas.RemoveRange(db.Areas);
                db.SaveChanges();
                Console.WriteLine("завершено");

                Console.Write("Группировка остановок в остановочные пункты... ");
                // Этап 1. Группируем остановки с одинаковыми названиями
                var busstopGroups = db.Busstops.GroupBy(busstop => busstop.Name);
                var areas = new List<Area>();

                foreach (var group in busstopGroups)
                {
                    var area = new Area();
                    area.Name = @group.Key;
                    @group.ToList().ForEach(busstop => area.Busstops.Add(busstop));
                    area.Location = CalculateAreaCentre(area.Busstops.ToList());
                    areas.Add(area);
                }

                // Этап 2. Объединяем остановочные пункты, которые находятся близко друг к другу
                const double maxDistance = 250.0;
                var delAreas = new List<Area>();

                foreach (var area in areas)
                {
                    if (delAreas.Contains(area)) continue;

                    var nearestAreas = areas.Where(bs => area != bs && !delAreas.Contains(bs) &&
                                                                     area.Location.Distance(bs.Location) < maxDistance).ToList();

                    foreach (var nearestArea in nearestAreas)
                    {
                        area.Name += $"/{nearestArea.Name}";
                        nearestArea.Busstops.ToList().ForEach(busstop => area.Busstops.Add(busstop));
                        area.Location = CalculateAreaCentre(area.Busstops.ToList());
                    }

                    delAreas.AddRange(nearestAreas);
                }

                delAreas.ForEach(busstation => areas.Remove(busstation));

                var i = 1;
                foreach (var area in areas)
                {
                    area.AreaId = i++;
                }

                db.Areas.AddRange(areas);
                db.SaveChanges();
            }

            Console.WriteLine("завершено");
            Console.ReadKey();
        }

        private static DbGeography CalculateAreaCentre(IReadOnlyCollection<Busstop> busstops)
        {
            double lat = 0.0, lon = 0.0;
            double busstopsCount = busstops.Count;
            foreach (var busstop in busstops)
            {
                if (busstop.Location.Latitude == null || busstop.Location.Longitude == null)
                {
                    throw new ArgumentNullException($"Неверное значение Location для остановки {busstop.BusstopId}. {busstop.Name}");
                }

                lat += (double)busstop.Location.Latitude;
                lon += (double)busstop.Location.Longitude;
            }

            return GeoUtils.CreatePoint(lat / busstopsCount, lon / busstopsCount);
        }

        /// <summary>
        /// Удаление остановок по идентификаторам
        /// </summary>
        private static void DeleteBusStopsById()
        {
            Console.WriteLine("Удаление остановок по идентификаторам");
            using (var db = new TransportContext())
            {
                int idForDel;
                do
                {
                    Console.Write(Environment.NewLine + "Введите Id остановки для удаления: ");
                    var input = Console.ReadLine();
                    if (Int32.TryParse(input, out idForDel) && idForDel != 0)
                    {
                        var bs = db.Busstops.Find(idForDel);
                        if (bs == null) continue;
                        Console.Write($"Удалить остановку: \"{bs.BusstopId}. {bs.Name}\" (Y/N)? ");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            db.Busstops.Remove(bs);
                            db.SaveChanges();
                        }
                    }
                } while (idForDel != 0);
            }
        }

        /// <summary>
        /// расчет количества людей в зоне
        /// </summary>
        private static void CountPeopleInAreas()
        {
            const double squarePerPerson = 22.8; // площадь на одного человека
            const double livingSpacePercentage = 0.875;

            var purposes = new[] { "Жилой дом", "Жилой дом с административными помещениями", "Частный дом", "Таунхаус", "Коттедж" };

            using (var db = new TransportContext())
            {
                var pb = new ProgressBar(db.Areas.Count() - 1, "Расчет людей, проживающих в зонах");
                var p = 0;
                foreach (var area in db.Areas.Include(a => a.Buildings))
                {
                    var sum = 0;
                    foreach (var b in area.Buildings)
                    {
                        if (purposes.Contains(b.Purpose) && b.Levels > 0 && b.Square > 0)
                        {
                            int levels = b.Purpose == "Жилой дом с административными помещениями" ? b.Levels - 1 : b.Levels;
                            sum += (int)(b.Square * levels * livingSpacePercentage / squarePerPerson);
                        }
                    }
                    area.People = sum;
                    pb.Report(++p);
                }
                pb.Pause();
                Console.WriteLine(Environment.NewLine + "Сохраняем в БД...");
                db.SaveChanges();
                Console.WriteLine($"Суммарное количетсов жителей: {db.Areas.Sum(a => a.People)}");
            }
        }

        /// <summary>
        /// Поменять lat и lon у полигонов
        /// </summary>
        private static void SwitchLatLon()
        {
            using (var db = new TransportContext())
            {
                var buildingsWithPolygon = db.Buildings.Where(b => b.Polygon != null);
                var progress = new ProgressBar(buildingsWithPolygon.Count(), "Перерасчет площадей... ");
                var indx = 0;
                foreach (var building in buildingsWithPolygon)
                {
                    // для каждого полигона внутри building.Polygon
                    var polyTexts = building.Polygon.AsText()
                        .Split(new[] { ")), ((", "), (" }, StringSplitOptions.RemoveEmptyEntries);

                    DbGeography polygon = null;
                    foreach (var polyText in polyTexts)
                    {
                        var matches = Regex.Matches(polyText, @"(?:\d*\.)?\d+");
                        polygon = polygon == null
                            ? GeoUtils.CreatePolygon(matches.Cast<Match>().Select(m => m.Value))
                            : polygon.Union(GeoUtils.CreatePolygon(matches.Cast<Match>().Select(m => m.Value)));
                    }

                    building.Polygon = polygon;
                    building.Square = polygon?.Area ?? 0;
                    progress.Report(indx++);
                }
                progress.Pause();
                Console.WriteLine("Сохраняем в БД...");
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Заполняем площади пропущенных домов
        /// </summary>
        private static void SetAreaToBuildings()
        {
            using (var db = new TransportContext())
            {
                var purpose = "Жилой дом";
                var buildingsWithoutArea = db.Buildings.Include(b => b.Addresses)
                    .Where(b => b.Addresses.Any() && Math.Abs(b.Square) < 0.1 && b.Purpose.StartsWith(purpose) && b.Levels > 2)
                    .OrderBy(b => b.AreaId);
                var totalBWA = buildingsWithoutArea.Count();
                var currentBWA = 0;
                foreach (var building in buildingsWithoutArea.ToList())
                {
                    currentBWA++;
                    var firstAddr = building.Addresses.First();
                    Clipboard.SetText($"{firstAddr.Street} {firstAddr.Number}");

                    double area;
                    string areaStr;
                    do
                    {
                        Console.Write($"{currentBWA}/{totalBWA}\t{firstAddr.Street}, {firstAddr.Number}. Площадь: ");
                        areaStr = Console.ReadLine();
                    } while (!Double.TryParse(areaStr, out area));

                    building.Square = area;
                    db.SaveChanges();
                }
                Console.WriteLine("Сохраняем в БД...");
            }
        }

        /// <summary>
        /// Заполняем площади частных домов средним значением
        /// </summary>
        private static void SetAvgPrivateHousesArea()
        {
            using (var db = new TransportContext())
            {
                var types = new[] { "Частный дом", "Таунхаус", "Коттедж" };
                var avgArea =
                    db.Buildings.Where(b => Math.Abs(b.Square) > 0.1 && types.Contains(b.Purpose))
                        .Average(b => b.Square);
                Console.WriteLine(
                    $"Выполнить заполнение площадей частный домов средней площадью = {avgArea.ToString("F3")}? (Y/N): ");
                var key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Y) return;

                foreach (var building
                    in db.Buildings.Where(b => Math.Abs(b.Square) < 0.1 && types.Contains(b.Purpose)))
                {
                    building.Square = avgArea;
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Заполняем площади киосков средним зачением
        /// </summary>
        private static void SetAvgKioskArea()
        {
            using (var db = new TransportContext())
            {
                var types = new[] { "Киоск" };
                var avgArea = db.Buildings.Where(b => Math.Abs(b.Square) > 0.1 && types.Contains(b.Purpose)).Average(b => b.Square);
                Console.WriteLine($"Выполнить заполнение площадей киосков средней площадью = {avgArea.ToString("F3")}? (Y/N): ");
                var key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Y) return;

                foreach (var building
                    in db.Buildings.Where(b => Math.Abs(b.Square) < 0.1 && types.Contains(b.Purpose)))
                {
                    building.Square = avgArea;
                }

                db.SaveChanges();
            }
        }
    }
}
