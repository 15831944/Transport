using System;
using System.Collections.Generic;
using System.Linq;

namespace Transport.Aca3.Models
{
    public class Path
    {
        public int Origin => Nodes.First();
        public int Destination => Nodes.Last();

        // Узлы
        private List<int> _nodes = new List<int>();
        public IEnumerable<int> Nodes => _nodes ?? (_nodes = new List<int>());

        public void Add(int value)
        {
            _nodes.Add(value);
        }

        public int RemoveLast()
        {
            var c = _nodes.Count - 1;
            if (c < 0) throw new InvalidOperationException(@"В пути нет элементов для удаления");
            var item = _nodes.ElementAt(c);
            _nodes.RemoveAt(c);

            return item;
        }
    }
}
