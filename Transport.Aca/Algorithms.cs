using System;
using System.Linq;

namespace Transport.Aca
{
    public static class Algorithms
    {
        /// <summary>
        /// Метод нахождения матрицы паасажироаских корреспонденций на основе гравитационной модели
        /// </summary>
        /// <param name="adjacencyMatrix">матрица смежности</param>
        /// <param name="arrivals">массив прибытий</param>
        /// <param name="departures">массив отправлений</param>
        /// <param name="tolerance">точность</param>
        public static double[,] FindCorrespMatrix(double[,] adjacencyMatrix, double[] arrivals, double[] departures, double tolerance = 1e-10)
        {
            var n = adjacencyMatrix.GetLength(0);

            if (adjacencyMatrix.GetLength(1) != n || arrivals.Length != n || departures.Length != n)
            {
                throw new ArgumentException("Входные массивы имеют различные размеры");
            }

            // проверка массива прибытий и отправлений (равная сумма элементов)
            if (Math.Abs(arrivals.Sum() - departures.Sum()) > tolerance)
            {
                throw new ArgumentException("Суммы элементов массивов отправлений и прибытий не равны");
            }

            // матрица пассажирских корреспонденций
            var correspMatrix = new double[n, n];

            const double gamma = 0.065;
            // аппроксимирующая функция
            var f = new Func<double, double>(a => Math.Exp(-gamma * a));
            // массив значений расстояний между остановками с примененой аппроксимирующей функцией
            var fc = new double[n, n];

            // массив затрат
            var distMatrix = FloydWarshallSearch(adjacencyMatrix);

            // применение функции f к массиву расстояний
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (i == j || Math.Abs(distMatrix[i, j]) < tolerance) continue;
                    fc[i, j] = f(distMatrix[i, j]);
                }
            }

            // инициализация матрицы корреспонденций начальными значениями
            for (var i = 0; i < n; i++)
            {
                // расчет знаменателя
                var sum = 0.0;
                for (var l = 0; l < n; l++)
                {
                    sum += arrivals[l] * fc[i, l];
                }

                for (var j = 0; j < n; j++)
                {
                    correspMatrix[i, j] = departures[i] * arrivals[j] * fc[i, j] * Math.Pow(sum, -1);
                }
            }

            var g = new double[n, n];
            var q = new double[n];
            var r = new double[n];
            while (true)
            {
                var next = false; // определяет необходимость выполнения новой итерации

                for (var j = 0; j < n; j++)
                {
                    var sum = 0.0;
                    for (var l = 0; l < n; l++)
                    {
                        sum += correspMatrix[l, j];
                    }

                    for (var i = 0; i < n; i++)
                    {
                        var val = sum - arrivals[j];
                        if (Math.Abs(val) < tolerance || val < 0)
                        {
                            g[i, j] = correspMatrix[i, j];
                        }
                        else
                        {
                            g[i, j] = correspMatrix[i, j] * arrivals[j] * Math.Pow(sum, -1);
                            next = true;
                        }
                    }
                }

                if (!next) break;

                for (var i = 0; i < n; i++)
                {
                    var sumDeparture = 0.0;
                    var sumArrivals = 0.0;

                    for (var j = 0; j < n; j++)
                    {
                        sumDeparture += g[i, j];
                        sumArrivals += g[j, i];
                    }
                    q[i] = departures[i] - sumDeparture;
                    r[i] = arrivals[i] - sumArrivals;
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
                        correspMatrix[i, j] = g[i, j] + q[i] * r[j] * fc[i, j] * Math.Pow(sum, -1);
                    }
                }
            }


            return correspMatrix;
        }

        /// <summary>
        /// Метод вычисляет крастчайшие расстояния между всеми парами вершин
        /// </summary>
        /// <param name="map">матрица смежности графа</param>
        /// <param name="tolerance">точность</param>
        public static double[,] FloydWarshallSearch(double[,] map, double tolerance = 1e-10)
        {
            var n = map.GetLength(0);
            if (n != map.GetLength(1))
                throw new ArgumentException($"Размерность матрицы смежности некорректна: {n} != {map.GetLength(1)}");


            var distMatrix = new double[n, n];

            // подготовка матрицы для алгоритма Флойда-Уоршолла
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (Math.Abs(map[i, j]) > tolerance)
                    {
                        distMatrix[i, j] = Double.PositiveInfinity;
                    }
                    else
                    {
                        distMatrix[i, j] = map[i, j];
                    }
                }
            }

            // алгоритм Флойда-Уоршолла
            for (var k = 0; k < n; k++)
            {
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i == j || double.IsPositiveInfinity(distMatrix[i, k]) || double.IsPositiveInfinity(distMatrix[k, j])) continue;
                        // длина пути из i в j через k
                        var pathLen = distMatrix[i, k] + distMatrix[k, j];
                        if (pathLen < distMatrix[i, j])
                        {
                            distMatrix[i, j] = pathLen;
                        }
                    }
                }
            }

            // если между двумя вершинами не существует пути,
            // это означает что граф несвязанный
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (double.IsPositiveInfinity(distMatrix[i, j]))
                        distMatrix[i, j] = 0;
                }
            }

            return distMatrix;
        }
    }
}
