using Lab_1.Generators;
using Lab_1.PowAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1
{
    public class PowTestConfig
    {
        public int X { get; set; } = 3;
        public int MaxN { get; set; } = 1000;

        public int Iterations { get; set; } = 100;

        public IPow<int> SelectedAlgorithm { get; set; } = new Pow<int>();
        public IPowGenerator PowGenerator { get; set; } = new IntPowGenerator(0);
        public Dictionary<string, IPow<int>> Algorithms { get; set; } = new Dictionary<string, IPow<int>>() { 
            { "Полный алгоритм", new Pow<int>() },
            { "Рекурсивный алгоритм", new RecPow<int>() },
            { "Быстрый алгоритм", new QuickPow<int>() },
        };

    }
}
