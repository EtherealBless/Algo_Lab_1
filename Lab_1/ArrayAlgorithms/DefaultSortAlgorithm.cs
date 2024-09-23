using Lab_1.ArrayAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1
{
    class DefaultSortAlgorithm<T> : IArrayAlgorithm<T> where T : IComparable

    {
        public Task Execute(T[] array)
        {
            Array.Sort(array);
            return Task.CompletedTask;
        }
    }
}
