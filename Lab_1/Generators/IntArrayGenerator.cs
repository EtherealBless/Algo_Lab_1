using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Generators
{
    public class IntArrayGenerator : IArrayGenerator<int>
    {
        private int[] array;

        private void GenerateNewArray(int maxPossibleSize)
        {
            Random random = new(0);

            array = new int[maxPossibleSize];

            for (int i = 0; i < maxPossibleSize; i++)
            {
                array[i] = random.Next();
            }
        }

        public IntArrayGenerator(int maxPossibleSize = 10000)
        {
            array = [];
            GenerateNewArray(maxPossibleSize);
        }

        /**
         * Generates array of given size
         * @param size
         * @return array of given size
         * @throws Exception if size is too big
         */
        public int[] GenerateArray(int size)
        {
            if (size > array.Length)
            {
                GenerateNewArray(size);
            }
            return array[..size];
        }
    }
}
