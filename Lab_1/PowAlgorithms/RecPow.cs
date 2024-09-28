using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.PowAlgorithms
{
    public class RecPow<T> : IPow<T> where T : IMultiplyOperators<T, T, T>, IMultiplicativeIdentity<T, T>
    {
        private int count = 0;
        private T RecursivePow(T x, uint n)
        {
            if (n == 0)
            {
                count += 2; // if n==0, result=1
                return T.MultiplicativeIdentity;

            }

            T result = RecursivePow(x, n / 2);

            count += 3; // if n!=0, n/2, result=RecursivePow(x, n/2)

            if (n % 2 == 0)
            {
                count += 1; // if n%2==0
                result = result * result;
                count += 1; // result*=result
            }
            else
            {
                count += 1; // if n%2!=0
                result = result * result * x;
                count += 2; // result*=result*x
            }

            return result;
        }

        public Task<int> Execute(Pair<T, uint> data)
        {
            count = 0;
            RecursivePow(data.First, data.Second);

            return Task.FromResult(count);
        }
    }
}
