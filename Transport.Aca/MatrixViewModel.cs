using System.Data;
using GalaSoft.MvvmLight;

namespace Transport.Aca
{
    public abstract class MatrixViewModel : ViewModelBase
    {
        protected void BuildMatrix()
        {
            var table = new DataTable();

            // размер таблицы
            var n = Matrix.GetLength(0);
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
                    row[j] = Matrix[i, j];
                }
                table.Rows.Add(row);
            }

            MatrixDataTable = table;
        }

        protected abstract double[,] Matrix { get; }

        private DataTable _matrixDataTable;
        public DataTable MatrixDataTable
        {
            get { return _matrixDataTable; }
            set { Set(() => MatrixDataTable, ref _matrixDataTable, value); }
        }
    }
}
