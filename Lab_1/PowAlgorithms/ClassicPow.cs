using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.PowAlgorithms
{
    internal class ClassicPow<T> : IPow<T> where T : IMultiplicativeIdentity<T, T>, IMultiplyOperators<T, T, T>
    {
        public Task<int> Execute(Pair<T, uint> data)
        {
            int count = 0;

            T x = data.First!;
            uint n = data.Second;

            T c = x;
            uint k = n;
            count += 2;

            T result = T.MultiplicativeIdentity;

            if (k % 2 != 0)
            {
                result = c;
            }

            count += 2; // result=, k%2==0
            while (k != 0)
            {
                if (k % 2 == 0)
                {
                    c = c * c;
                    k = k / 2;
                    count += 2; //c=, k/=2
                }
                else
                {
                    result = result * c;
                    k = k - 1;
                    count += 2; // result=, k--
                }
                count++; // k%2==0
            }

            return Task.FromResult(count);
        }
    }
}
