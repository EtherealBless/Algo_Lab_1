using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Tests
{
    class TestManager : ITestManager<int>
    {
        public async IAsyncEnumerable<Point> TestArrayAlgorithm(IArrayAlgorithm<int> algorithm, IArrayGenerator<int> generator, int count)
        {
            ArrayTest<int> test = new();
            for (int i = 0; i < count; i++)
            {
                Point point = new(i,
                    test.Test(algorithm, generator.GenerateArray(i)).Result
                    );

                yield return point;
            }
        }
    }
}
