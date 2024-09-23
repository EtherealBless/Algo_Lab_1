using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    class ConstFunction<T> : IArrayAlgorithm<T>
    {
        public Task Execute(T[] array)
        {
            return Task.CompletedTask;
        }
    }
}
