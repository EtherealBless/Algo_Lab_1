using Lab_1;
using Lab_1.ArrayAlgorithms;
using Newtonsoft.Json.Bson;

namespace Lab_1_Tests
{
    [TestClass]
    public class TimSortTest
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += currentDomain_UnhandledException;
        }

        static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }
        [TestMethod]
        public void TestTimSort()
        {
            var arraySize = 555555;
            var timSort = new Lab_1.ArrayAlgorithms.TimSort<int>();
            var generator = new IntArrayGenerator(arraySize);
            var array = generator.GenerateArray(arraySize);
            timSort.Execute(array);

            Assert.IsTrue(Util.CheckArraySorted(array));
        }

        [TestMethod]
        public void TestBinarySort()
        {
            var array = new int[] { 1, 2, 23, 4, 7, 9, 10, 55, 56, 1};

            TimSort<int>.BinarySort(array, 0, array.Length, 3);
            Assert.IsTrue(Util.CheckArraySorted(array));
        }
    }
}