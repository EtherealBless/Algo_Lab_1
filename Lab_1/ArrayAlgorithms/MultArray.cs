using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    class MultArray<T> : IArrayAlgorithm<T> where T : IMultiplyOperators<T, T, T>, IMultiplicativeIdentity<T, T>
    {
        public Task Execute(T[] array)
        {
            T res = T.MultiplicativeIdentity;
            foreach (T i in array)
            {
                res *= i;
            }
            return Task.CompletedTask;
        }
    }
}
