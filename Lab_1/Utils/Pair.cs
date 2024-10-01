using Lab_1.MatrixAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Utils
{
    public class Pair<T, K>
    {
        public Pair(T first, K second)
        {
            First = first;
            Second = second;
        }

        public Pair() { }

        public T? First { get; set; }
        public K? Second { get; set; }

        internal void Deconstruct(out T x, out K y)
        {
            x = First;
            y = Second;
        }
    }
}
