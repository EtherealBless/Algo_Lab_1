using Lab_1.ArrayAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Tests
{
    interface IArrayTest<T>
    {
        /**
         * @param algorithm
         * @param array
         * @return the time it takes to complete the algorithm on the array
         * */
        Task<double> Test(IArrayAlgorithm<T> algorithm, T[] array);
    }
}
