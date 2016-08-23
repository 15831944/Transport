using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Transport.Aca2
{
    public class RowIndexToHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridRow row = value as DataGridRow;
            if (row == null)
                throw new InvalidOperationException("Конвертор работает только с элементами типа DataGridRow");

            return row.GetIndex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
