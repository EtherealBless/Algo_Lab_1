using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Generators
{
    public class TestArrayGenerator : IArrayGenerator<int>
    {
        public int[] GenerateArray(int size)
        {
            return new int[]{
                1, 2,
                1, 2,
                1, 2, 3, 4,
                1, 2, 3, 4, 5, 6, 7, 8,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16

            };
        }
    }
}
