using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Transport.Aca3.Services
{
    public static class DataLoaderService
    {
        public static double[,] LoadAdjacencyMatrix(string path)
        {
            return ReadDoubleMatrix(path);
        }

        public static double[] LoadArrivals(string path)
        {
            return ReadDoubleArray(path);
        }

        public static double[] LoadDepartures(string path)
        {
            return ReadDoubleArray(path);
        }

        public static double[,] LoadDirectTravelersMatrix(string path)
        {
            return ReadDoubleMatrix(path);
        }

        public static List<Point> LoadNodesCoords(string path)
        {
            var reader = File.OpenText(path);
            var points = new List<Point>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = SplitStringFunc(line);
                points.Add(new Point(double.Parse(values[0]), double.Parse(values[1])));
            }

            return points;
        }

        public static string[] LoadNodesAtrributes(string path)
        {
            var nodesAttr = new List<string>();
            var reader = File.OpenText(path);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                nodesAttr.Add(line);
            }

            return nodesAttr.ToArray();
        }

        private static readonly char[] SplitSymbols = { ';' };

        private static readonly Func<string, string[]> SplitStringFunc = s => s.Split(SplitSymbols, StringSplitOptions.RemoveEmptyEntries);

        private static double[] ReadDoubleArray(string path)
        {
            var reader = File.OpenText(path);

            string line;
            if ((line = reader.ReadLine()) == null) return null;

            var values = SplitStringFunc(line);
            var result = new double[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                result[i] = Double.Parse(values[i]);
            }

            return result;
        }

        private static double[,] ReadDoubleMatrix(string path)
        {
            var reader = File.OpenText(path);

            double[,] result = null;

            string line;
            var i = 0;
            while ((line = reader.ReadLine()) != null)
            {
                var values = SplitStringFunc(line);

                // задание размерности
                if (result == null)
                {
                    var n = values.Length;
                    result = new double[n, n];
                }

                for (var j = 0; j < values.Length; j++)
                {
                    result[i, j] = Double.Parse(values[j]);
                }

                i++;
            }

            return result;
        }

    }
}
