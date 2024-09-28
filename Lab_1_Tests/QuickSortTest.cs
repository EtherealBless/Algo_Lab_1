using Lab_1;
using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1_Tests
{
    [TestClass]
    public class QuickSortTest
    {
        [TestMethod]
        public void TestQuickSort()
        {
            var arraySize = 555555;
            var quickSort = new QuickSort<int>();
            var generator = new IntArrayGenerator(arraySize);
            var array = generator.Generate(arraySize);

            quickSort.Execute(array);

            Assert.IsTrue(Util.CheckArraySorted(array));
        }
    }
}
