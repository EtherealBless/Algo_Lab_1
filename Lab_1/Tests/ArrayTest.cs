using Lab_1.ArrayAlgorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Tests
{
    class ArrayTest<T> : IArrayTest<T>
    {
        public Task<double> Test(IArrayAlgorithm<T> algorithm, T[] array)
        {
            double sum = 0;
            return Task.Run(() =>
            {
                sum += ArrayTest<T>.TestOnce(algorithm, array);
                //Trace.WriteLine(array.Length);
                return sum;
            });
        }
        private static double TestOnce(IArrayAlgorithm<T> algorithm, T[] array)
        {
            var watch = new Stopwatch();
            watch.Start();
            algorithm.Execute(array).Wait();
            watch.Stop();
            return watch.ElapsedTicks;
        }
    }
}
