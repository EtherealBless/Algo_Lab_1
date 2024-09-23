using Lab_1.ArrayAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1
{
    interface ITestManager<T>
    {
        IAsyncEnumerable<Point> TestArrayAlgorithm(IArrayAlgorithm<T> algorithm, IArrayGenerator<T> generator, int count);
        // IAsyncEnumerable<Point> TestMatrixAlgorithm(IMatrixAlgorithm<T> algorithm, IMatrixGenerator<T> generator, int count);
    }
}
