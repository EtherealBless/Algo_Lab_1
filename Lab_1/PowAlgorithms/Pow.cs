using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.PowAlgorithms
{
    internal class Pow<T> : IPow<T> where T : IMultiplyOperators<T, T, T>, IMultiplicativeIdentity<T, T>
    {
        public Task<int> Execute(Pair<T, uint> data)
        {
            T x = data.First;
            uint n = data.Second;

            int count = 0;
            T result = T.MultiplicativeIdentity;
            count += 2; // result=1, k=0

            for (uint k = 0; k < n; k++)
            {
                result = result * x;
                count += 2; // result*=x, k++
            }

            return Task.FromResult(count);
        }
    }
}
