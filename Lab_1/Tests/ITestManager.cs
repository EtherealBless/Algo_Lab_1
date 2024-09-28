using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using Lab_1.MatrixAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Tests
{
    interface ITestManager<NumberType>
    {
        IAsyncEnumerable<Point> TestAlgorithm<AlgorithmInputType>(IAlgorithm<AlgorithmInputType> algorithm, IGenerator<AlgorithmInputType> generator, int count);
        IAsyncEnumerable<Point> TestArrayAlgorithm(IArrayAlgorithm<NumberType> algorithm, IArrayGenerator<NumberType> generator, int count);
        IAsyncEnumerable<Point> TestMatrixAlgorithm(IMatrixAlgorithm<NumberType> algorithm, IMatrixGenerator<NumberType> generator, int count);
        //IAsyncEnumerable<Point> TestPowAlgorithm(IPowAlgorithm<T> algorithm, IPowGenerator<T> generator, int count);
    }
}
