using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    public class QuickSort<T> : IArrayAlgorithm<T> where T : IComparisonOperators<T, T, bool>
    {
        private static T Partition(T[] array, int left, int right)
        {
            var random = new Random();

            var pivot = array[left + random.Next(right - left)];
            return pivot;
        }

        private static T[] Sort(T[] array, int left, int right)
        {
            if (right <= left) return array;
            var pivot = QuickSort<T>.Partition(array, left, right);
            int i = left, j = right;

            while (i <= j)
            {
                while (array[i] < pivot)
                {
                    i++;
                }

                while (array[j] > pivot)
                {
                    j--;
                }
                if (i <= j)
                {
                    (array[j], array[i]) = (array[i], array[j]);
                    i++;
                    j--;
                }
            }
            if (left < j) QuickSort<T>.Sort(array, left, j);
            if (i < right) QuickSort<T>.Sort(array, i, right);
            return array;
        }

        public Task Execute(T[] array)
        {
            QuickSort<T>.Sort(array, 0, array.Length-1);

            return Task.CompletedTask;
        }
    }
}
