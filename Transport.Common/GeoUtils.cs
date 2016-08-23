using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;

namespace Transport.Common
{
    public static class GeoUtils
    {
        static GeoUtils()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
        }

        private static int CoordinateSystem { get; } = 4326;

        /// <summary>
        /// Create a GeoLocation point based on latitude and longitude
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static DbGeography CreatePoint(double latitude, double longitude)
        {
            // в русском языке по умолчанию разделители  десятичных знаков - запятые
            // POINT (lon lat)
            var text = $"POINT({longitude.ToString(CultureInfo.InvariantCulture)} " +
                       $"{latitude.ToString(CultureInfo.InvariantCulture)})";
            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeography.PointFromText(text, CoordinateSystem);
        }

        /// <summary>
        /// Create a GeoLocation point based on latitude and longitude
        /// </summary>
        /// <param name="latitudeLongitude">
        /// String should be two values either single comma or space delimited
        /// 45.710030,-121.516153
        /// 45.710030 -121.516153
        /// </param>
        /// <returns></returns>
        public static DbGeography CreatePoint(string latitudeLongitude)
        {
            var tokens = latitudeLongitude.Split(',', ' ');
            if (tokens.Length != 2)
                throw new ArgumentException("Invalid location string passed");
            var text = $"POINT({tokens[1]} {tokens[0]})";
            return DbGeography.PointFromText(text, CoordinateSystem);
        }

        public static DbGeography CreatePolygon(string latLonPoints)
        {
            var tokens = latLonPoints.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length < 3 || tokens.Length % 2 == 1)
                throw new ArgumentException("Invalid coords count");
            if (tokens[0] != tokens[tokens.Length - 2] || tokens[1] != tokens[tokens.Length - 1])
                throw new ArgumentException("Polygon start and end points are different");
            var text = new StringBuilder("POLYGON((");

            for (var i = 0; i <= tokens.Length - 2; i += 2)
            {
                text.Append($"{tokens[i + 1]} {tokens[i]}");
                if (i != tokens.Length - 2)
                {
                    text.Append(",");
                }
            }
            text.Append("))");

            return MakeValidPolygon(text.ToString());
        }

        public static DbGeography CreatePolygon(IEnumerable<string> latLonPoints)
        {
            var points = latLonPoints as string[] ?? latLonPoints.ToArray();
            var pointsCount = points.Count();
            if (pointsCount % 2 == 1)
                throw new ArgumentException("Invalid coords count");

            var text = new StringBuilder("POLYGON((");

            for (var i = 0; i <= pointsCount - 2; i += 2)
            {
                text.Append($"{points[i + 1]} {points[i]}");
                if (i != pointsCount - 2)
                {
                    text.Append(",");
                }
            }
            text.Append("))");

            return MakeValidPolygon(text.ToString());
        }

        public static DbGeography CreatePolygon(IEnumerable<double> latLonPoints)
        {
            var points = latLonPoints as double[] ?? latLonPoints.ToArray();
            var pointsCount = points.Count();
            if (pointsCount % 2 == 1)
                throw new ArgumentException("Invalid coords count");

            var text = new StringBuilder("POLYGON((");

            for (var i = 0; i <= pointsCount - 2; i += 2)
            {
                text.Append($"{points[i + 1].ToString(CultureInfo.InvariantCulture)} {points[i].ToString(CultureInfo.InvariantCulture)}");
                if (i != pointsCount - 2)
                {
                    text.Append(",");
                }
            }
            text.Append("))");

            return MakeValidPolygon(text.ToString());
        }

        private static DbGeography MakeValidPolygon(string wkt)
        {
            var sqlGeography = SqlGeography.STPolyFromText(new SqlChars(wkt), CoordinateSystem).MakeValid();
            var invertedSqlGeography = sqlGeography.ReorientObject();

            // возвращает полигон, который обладает меньшей площадью
            // требуется чтобы исключить подсчет внешнего полигона (площадь Земли)
            return
                DbSpatialServices.Default.GeographyFromProviderValue(
                    sqlGeography.STArea() > invertedSqlGeography.STArea()
                    ? invertedSqlGeography
                    : sqlGeography);
        }
    }
}
