using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using Lab_1.MatrixAlgorithms;
using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Tests
{
    class TestManager : ITestManager<int>
    {


        public async IAsyncEnumerable<Point> TestArrayAlgorithm(IArrayAlgorithm<int> algorithm, IArrayGenerator<int> generator, int count)
        {
            IAsyncEnumerable<Point> points = TestAlgorithm(algorithm, generator, count);

            await foreach (var item in points)
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<Point> TestMatrixAlgorithm(IMatrixAlgorithm<int> algorithm, IMatrixGenerator<int> generator, int count)
        {
            await foreach (var item in TestAlgorithm(algorithm, generator, count))
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<Point> TestAlgorithm<InputType>(IAlgorithm<InputType> algorithm, Generators.IGenerator<InputType> generator, int count)
        {
            Test<IAlgorithm<InputType>, InputType> test = new();
            for (int i = 0; i < count; i++)
            {
                Point point = new(i,
                    test.RunTest(algorithm, generator.Generate(i)).Result
                    );

                yield return point;
            }
        }

    }
}
