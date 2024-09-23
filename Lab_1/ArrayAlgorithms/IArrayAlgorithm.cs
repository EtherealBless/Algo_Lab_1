using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    public interface IArrayAlgorithm<T>
    {
        Task Execute(T[] array);
    }
}
