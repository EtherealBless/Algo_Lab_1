using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.PowAlgorithms
{
    internal class QuickPow<T> : IPow<T> where T : IMultiplyOperators<T, T, T>, IMultiplicativeIdentity<T, T>
    {


        public Task<int> Execute(Pair<T, uint> data)
        {
            int count = 0;

            T x = data.First;
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
                k = k / 2;
                c = c * c;

                count += 3;

                if (k % 2 != 0)
                {
                    result = result * c;
                    count++;
                }
                count++; // k%2==0
            }

            return Task.FromResult(count);
        }
    }
}
