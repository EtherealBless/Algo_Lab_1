using Lab_1;
using Lab_1.ArrayAlgorithms;
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
            var arraySize = 5555;
            var quickSort = new QuickSort();
            var generator = new IntArrayGenerator(arraySize);
            var array = generator.GenerateArray(arraySize);

            quickSort.Execute(array);

            Assert.IsTrue(Util.CheckArraySorted(array));
        }
    }
}
