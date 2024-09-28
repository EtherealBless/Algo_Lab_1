using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Generators
{
    public class IntPowGenerator : IPowGenerator
    {

        public int X { get; set; }

        public IntPowGenerator(int x)
        {
            X = x;
        }

        Pair<int, uint> IGenerator<Pair<int, uint>>.Generate(int seed)
        {
            return new Pair<int, uint>(X, (uint)seed);
        }
    }
}
