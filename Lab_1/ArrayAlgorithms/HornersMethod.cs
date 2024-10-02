using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    internal class HornersMethod<T> : IArrayAlgorithm<int>
    {
        public Task Execute(int[] data)
        {
            float x = 1.5f;
            float res = 0;
            for (int i = data.Length - 1; i >= 1; i-=2)
            {
                
                res += x*(data[i] * x + data[i - 1]);
            }
            if (data.Length % 2 != 0)
            {
                res += data[0];
            }

            return Task.CompletedTask;
        }
    }
}
