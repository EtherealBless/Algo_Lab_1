using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    class Polynomial<T> : IArrayAlgorithm<int> 
    {
        public Task Execute(int[] array)
        {
            double res = 0;
            double x = 1.5;

            for (int i = 0; i < array.Length; i++)
            {
                res += Math.Pow(x, i) * array[i];
            }

            return Task.CompletedTask;
        }
    }
}
