using System;
using System.Collections.ObjectModel;

namespace Transport.Aca2
{
    public class Graph<T> where T : new()
    {
        public ObservableCollection<ObservableCollection<T>> Rows { get; } =
            new ObservableCollection<ObservableCollection<T>>();

        public Graph(int nodes = 1)
        {
            Rows.Add(new ObservableCollection<T> { default(T) });
        }

        public void AddNode()
        {
            // Adding new row
            Rows.Add(new ObservableCollection<T>(new T[NodesCount]));

            // Adding new column
            foreach (var row in Rows)
            {
                row.Add(default(T));
            }
        }

        public void RemoveNode(int indx)
        {
            if (indx < 0 || indx >= Rows.Count) throw new ArgumentException(nameof(indx));

            // Remove column by indx
            foreach (var row in Rows)
            {
                row.RemoveAt(indx);
            }

            // Remove row
            Rows[indx].Clear();
            Rows.RemoveAt(indx);
        }

        public T this[int r, int c]
        {
            get { return Rows[r][c]; }
            set { Rows[r][c] = value; }
        }

        public int NodesCount => Rows.Count;
    }
}
