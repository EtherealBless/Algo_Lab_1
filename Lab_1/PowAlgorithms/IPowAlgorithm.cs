using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.PowAlgorithms
{
    internal interface IPowAlgorithm<T, K, V>
    {
        V Pow(T a, K b);
    }
}
