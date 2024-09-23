using Lab_1;
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
            var arraySize = 5555;
            var timSort = new Lab_1.ArrayAlgorithms.TimSort();
            var generator = new IntArrayGenerator(arraySize);
            var array = generator.GenerateArray(arraySize);
            timSort.Execute(array);

            Assert.IsTrue(Util.CheckArraySorted(array));
        }

    }
}