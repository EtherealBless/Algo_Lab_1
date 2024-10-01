using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    internal class BogoSort<T> : IArrayAlgorithm<T> where T : IComparisonOperators<T, T, bool>
    {
        static bool IsSorted(T[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] > array[i + 1])
                    return false;
            }
            return true;
        }

        static void Shuffle(T[] array)
        {
            Random rand = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                int randomIndex = rand.Next(array.Length);
                T temp = array[i];
                array[i] = array[randomIndex];
                array[randomIndex] = temp;
            }
        }

        static void BogoSortMethod(T[] array)
        {
            while (!IsSorted(array))
            {
                Shuffle(array);
            }
        }

        public Task Execute(T[] data)
        {
            BogoSortMethod(data);

            return Task.CompletedTask;
        }
    }
}
