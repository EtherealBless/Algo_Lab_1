using Lab_1.ArrayAlgorithms;
using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.PowAlgorithms
{
    public interface IPow<T> : IAlgorithm<Pair<T, uint>, Task<int>>
    {
    }
}
