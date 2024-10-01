using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using Lab_1.MatrixAlgorithms;
using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Tests
{
    interface ITestManager<NumberType>
    {
        IAsyncEnumerable<Point> TestAlgorithm<AlgorithmInputType>(IAlgorithm<AlgorithmInputType> algorithm, IGenerator<AlgorithmInputType> generator, int count, CancellationToken? token);
        IAsyncEnumerable<Point> TestArrayAlgorithm(IArrayAlgorithm<NumberType> algorithm, IArrayGenerator<NumberType> generator, int count, CancellationToken? token);
        IAsyncEnumerable<Point> TestMatrixAlgorithm(IMatrixAlgorithm<NumberType> algorithm, IMatrixGenerator<NumberType> generator, int count, CancellationToken? token);
        //IAsyncEnumerable<Point> TestPowAlgorithm(IPowAlgorithm<T> algorithm, IPowGenerator<T> generator, int count);
    }
}
