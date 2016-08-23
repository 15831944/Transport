using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Transport.Aca2
{
    public class GraphToDataGridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var graph = value as Graph<double>;
            if (graph == null) return Binding.DoNothing;

            var table = new DataTable();

            // размер таблицы
            var n = graph.NodesCount;
            var cols = new DataColumn[n];

            for (var i = 0; i < n; i++)
            {
                cols[i] = new DataColumn(i.ToString());
            }
            table.Columns.AddRange(cols);

            for (var i = 0; i < n; i++)
            {
                var row = table.NewRow();
                for (var j = 0; j < n; j++)
                {
                    row[j] = graph[i, j];
                }
                table.Rows.Add(row);
            }

            return table;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
