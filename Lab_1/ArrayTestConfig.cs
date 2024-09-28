using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1
{
    public class ArrayTestConfig
    {
        public int Iterations { get; set; } = 100;
        public int ArraySize { get; set; } = 10000;
        public IArrayGenerator<int> ArrayGenerator { get; set; } = new IntArrayGenerator();
        public IArrayAlgorithm<int> SelectedAlgorithm { get; set; } = new ConstFunction<int>();

        public Dictionary<string, IArrayAlgorithm<int>> Algorithms { get; set; } = new Dictionary<string, IArrayAlgorithm<int>>() {
            { "ConstFunction", new ConstFunction<int>() },
            { "SumArray", new SumArray<int>() },
            { "Polynomial", new Polynomial<int>() },
            { "QuickSort", new QuickSort<int>() },
            { "BubbleSort", new BubbleSort<int>() },
            { "TimSort", new TimSort<int>() },
            //{ "QuickPow", new QuickPow<int>() } // TODO 
        };
    }
}
