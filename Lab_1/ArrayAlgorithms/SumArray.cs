using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    class SumArray<T> : IArrayAlgorithm<T> where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        public Task Execute(T[] array)
        {
            T sum = T.AdditiveIdentity;

            foreach(T i in array)
            {
                sum += i;
            }
            return Task.CompletedTask;
        }
    }
}
