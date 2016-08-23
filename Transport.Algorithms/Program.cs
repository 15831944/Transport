using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Transport.DataAccessLayer;
using static System.Int32;

namespace Transport.Algorithms
{
    class Program
    {
        static void Main(string[] args)
        {

            var origin = 0;
            var dest = 20;
            var matrix = GetAdjacencyMatrix();

            var sw = Stopwatch.StartNew();
            BreadthFirstSearch(matrix, origin, dest);
            Console.WriteLine($"Поиск в ширину выполнен за {sw.Elapsed.TotalSeconds}");

            sw.Restart();
            DijkstraSearch(matrix, origin);
            Console.WriteLine($"Поиск кратчайших расстояниц алг. Дейкстры выполнен за {sw.Elapsed.TotalSeconds}");

            sw.Restart();
            int[,] history;
            int[,] result;
            FloydWarshallSearch(matrix, out result, out history);
            Console.WriteLine($"Поиск кратчайших расстояниц алг. Флойда-Уоршолла выполнен за {sw.Elapsed.TotalSeconds}");
            sw.Stop();

            var dirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Results");
            var filePath = Path.Combine(dirPath, @"path_fw.txt");
            Directory.CreateDirectory(dirPath);
            var fileStream = new FileStream(filePath, FileMode.Create);
            using (var writer = new StreamWriter(fileStream))
            {
                for (var i = 0; i < result.GetLength(0); i++)
                {
                    writer.Write($"{i} ({result[origin, i]}): ");
                    var p = origin;
                    do
                    {
                        writer.Write($"{p}->");
                        p = history[p, i];
                    } while (p != MaxValue);
                    writer.WriteLine();
                }
            }

            Console.ReadKey(true);
        }

        private static int[,] GetRandomAdjMatrix(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "Размерность матрицы должна быть положительной!");

            var rand = new Random();

            var matrix = new int[length, length];
            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    matrix[i, j] = i == j ? MaxValue : rand.Next(0, 10000);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Получение матрицы смежности
        /// </summary>
        /// <returns></returns>
        private static int[,] GetAdjacencyMatrix()
        {
            using (var db = new TransportContext())
            {
                var dim = db.Areas.Count();
                var matrix = new int[dim, dim];

                for (var i = 0; i < dim; i++)
                {
                    for (var j = 0; j < dim; j++)
                    {
                        matrix[i, j] = MaxValue;
                    }
                }

                foreach (var areaRoute in db.AreaRoutes)
                {
                    matrix[areaRoute.OriginId, areaRoute.DestinationId] = areaRoute.Distance;
                }

                return matrix;
            }
        }

        /// <summary>
        /// Поиск в ширину
        /// Поиск в ширину работает путём последовательного просмотра отдельных уровней графа,
        /// начиная с узла-источника
        /// </summary>
        private static void BreadthFirstSearch(int[,] map, int origin, int dest)
        {
            // число строк
            var n = map.GetLength(0);
            // число столбцов
            var m = map.GetLength(1);
            if (n != m) throw new ArgumentException($"Размерность матрицы смежности некорректна: {n} != {m}");

            var visited = new bool[n];
            var queue = new Queue<int>();

            queue.Enqueue(origin);
            visited[origin] = true;

            while (queue.Count != 0)
            {
                var curr = queue.Dequeue();

                if (curr == dest)
                {
                    Console.WriteLine("Путь найден!");
                    return; // succcess
                }

                for (var j = 0; j < n; j++)
                {
                    if (map[curr, j] == 0 || visited[j]) continue;
                    queue.Enqueue(j);
                    visited[j] = true;
                }
            }

            Console.WriteLine("Путь не найден!");
        }


        /// <summary>
        /// Алгоритм Дейкстры для поиска кратчайшего расстояния от одной вершины до всех остальных
        /// </summary>
        /// <param name="map">матрица весов</param>
        /// <param name="origin">исходный узел</param>
        private static void DijkstraSearch(int[,] map, int origin)
        {
            // число строк
            var n = map.GetLength(0);
            // число столбцов
            var m = map.GetLength(1);
            if (n != m) throw new ArgumentException($"Размерность матрицы смежности некорректна: {n} != {m}");

            // инициализация массива посещенных вершин
            var visited = new bool[n];

            // инициализация массива расстояний
            var distance = new int[n];
            // инициализация массива списков вершин
            var paths = new List<int>[n];
            for (var i = 0; i < n; i++)
            {
                distance[i] = MaxValue;
                paths[i] = new List<int>();
            }
            distance[origin] = 0;

            while (true)
            {
                // поиск минимальной метки вершины
                var minIndx = -1;
                var min = MaxValue;
                for (var i = 0; i < n; i++)
                {
                    if (visited[i] == false && distance[i] < min)
                    {
                        minIndx = i;
                        min = distance[i];
                    }
                }

                // если все вершины посещенные, то выходим
                if (minIndx == -1) break;

                // отмечаем найденную вершину как посещенную
                visited[minIndx] = true;

                // попытка уменьшить путь до соседей минимальной вершины
                for (var j = 0; j < n; j++)
                {
                    // если вершина уже посещена или пути нет - то пропускаем вершину
                    if (visited[j] || map[minIndx, j] == MaxValue) continue;
                    var currDist = distance[minIndx] + map[minIndx, j];
                    // если текущий путь короче найденного ранее - заносим его длину в вершину
                    if (currDist < distance[j])
                    {
                        distance[j] = currDist;
                        paths[j] = new List<int>(paths[minIndx]) { j };
                    }
                }
            }

            var dirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Results");
            var filePath = Path.Combine(dirPath, @"paths.txt");
            Directory.CreateDirectory(dirPath);
            var fileStream = new FileStream(filePath, FileMode.Create);
            using (var writer = new StreamWriter(fileStream))
            {
                for (var i = 0; i < n; i++)
                {
                    writer.Write($"{i} ({distance[i]}): ");
                    foreach (var path in paths[i])
                    {
                        writer.Write($"{path}->");
                    }
                    writer.WriteLine();
                }
            }
        }

        private static void FloydWarshallSearch(int[,] map, out int[,] result, out int[,] history)
        {
            var n = map.GetLength(0);
            var m = map.GetLength(1);
            if (n != m) throw new ArgumentException($"Размерность матрицы смежности некорректна: {n} != {m}");

            result = new int[n, n];
            history = new int[n, n];

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    result[i, j] = map[i, j];
                    history[i, j] = map[i, j] == MaxValue ? MaxValue : j;
                }
            }

            for (var k = 0; k < n; k++)
            {
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i == j || result[i, k] == MaxValue || result[k, j] == MaxValue) continue;
                        // длина пути из i в j через k
                        var pathLen = result[i, k] + result[k, j];
                        if (pathLen < result[i, j])
                        {
                            result[i, j] = pathLen;
                            history[i, j] = k;
                        }
                    }
                }
            }
        }

        
    }
}
